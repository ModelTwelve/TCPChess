using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPChess {
    public class ChessServer {        

        public class PerClientGameData {

            public List<string> serverResponses;
            public string playersName, opponentsName, opponentsRemoteEndPoint;
            public Dictionary<string, ChessPiece> chessPieces = null;
            // ToPlayername and ColorRequested
            public Dictionary<string,string> dictPendingPlayRequests;
            public string serverTestAutoResponseOnPlayRequest = "";
            public bool quitGAME = false;

            public string status {
                get {
                    return chessPieces == null ? "Available In the Lobby" : "Playing "+ opponentsName;
                }
            }

            public bool available {
                get {
                    return chessPieces == null;
                }
            }

            public PerClientGameData() {
                init();
            }

            public string serializeBoard() {
                StringBuilder sb = new StringBuilder();
                sb.Append("Board");
                foreach(var kvp in chessPieces) {
                    sb.Append("," + kvp.Key + ":" + kvp.Value.Color + ":" + kvp.Value.KindOfPiece);
                }
                return sb.ToString();
            }

            private void init() {
                serverResponses = new List<string>();
                playersName = null;
                chessPieces = null;
                opponentsName = "";
                opponentsRemoteEndPoint = "";
                dictPendingPlayRequests = new Dictionary<string, string>();
            }

            public void initializeMatch(string opName, string opRemoteEndPoint) {
                dictPendingPlayRequests = new Dictionary<string, string>();

                opponentsName = opName;
                opponentsRemoteEndPoint = opRemoteEndPoint;

                chessPieces = new Dictionary<string, ChessPiece>();
                chessPieces.Add("0:0", new ROOK("B"));
                chessPieces.Add("1:0", new KNIGHT("B"));
                chessPieces.Add("2:0", new BISHOP("B"));
                chessPieces.Add("3:0", new KING("B"));
                chessPieces.Add("4:0", new QUEEN("B"));
                chessPieces.Add("5:0", new BISHOP("B"));
                chessPieces.Add("6:0", new KNIGHT("B"));
                chessPieces.Add("7:0", new ROOK("B"));
                chessPieces.Add("0:1", new PAWN("B"));
                chessPieces.Add("1:1", new PAWN("B"));
                chessPieces.Add("2:1", new PAWN("B"));
                chessPieces.Add("3:1", new PAWN("B"));
                chessPieces.Add("4:1", new PAWN("B"));
                chessPieces.Add("5:1", new PAWN("B"));
                chessPieces.Add("6:1", new PAWN("B"));
                chessPieces.Add("7:1", new PAWN("B"));
                chessPieces.Add("0:6", new PAWN("W"));
                chessPieces.Add("1:6", new PAWN("W"));
                chessPieces.Add("2:6", new PAWN("W"));
                chessPieces.Add("3:6", new PAWN("W"));
                chessPieces.Add("4:6", new PAWN("W"));
                chessPieces.Add("5:6", new PAWN("W"));
                chessPieces.Add("6:6", new PAWN("W"));
                chessPieces.Add("7:6", new PAWN("W"));
                chessPieces.Add("0:7", new ROOK("W"));
                chessPieces.Add("1:7", new KNIGHT("W"));
                chessPieces.Add("2:7", new BISHOP("W"));
                chessPieces.Add("3:7", new KING("W"));
                chessPieces.Add("4:7", new QUEEN("W"));
                chessPieces.Add("5:7", new BISHOP("W"));
                chessPieces.Add("6:7", new KNIGHT("W"));
                chessPieces.Add("7:7", new ROOK("W"));
            }

            public void destroyMatch() {
                dictPendingPlayRequests = new Dictionary<string, string>();
                opponentsName = "";
                opponentsRemoteEndPoint = "";
                chessPieces = null;
            }
        }
        private int _listeningPort;
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        
        private Dictionary<string, PerClientGameData> dictConnections = new Dictionary<string, PerClientGameData>();
        private object _lock = new object();

        public ChessServer(int port, CancellationToken cToken, IProgress<ReportingClass> progress) {
            _listeningPort = port;
            this.cToken = cToken;
            this.progress = progress;
        }
        public async Task Start() {
            IPAddress ipAddre = IPAddress.Any;
            TcpListener listener = new TcpListener(ipAddre, _listeningPort);
            listener.Start();
            reportingClass.addMessage("Server is running");
            reportingClass.addMessage("Listening on port " + _listeningPort);

            Task.Run(() => serverHouseKeepingTask());

            int connectionNumber = 0;
            while (true) {
                ++connectionNumber;
                reportingClass.addMessage(String.Format("Waiting for connection {0} ...",connectionNumber.ToString()));
                try {
                    progress.Report(reportingClass);
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    lock(_lock) {
                        dictConnections.Add(tcpClient.Client.RemoteEndPoint.ToString(), new PerClientGameData());
                    }
                    Task.Run(() => HandleConnectionAsync(tcpClient));
                }
                catch (Exception exp) {
                    reportingClass.addMessage(exp.ToString());
                }
            }
        }
 
        private async void HandleConnectionAsync(TcpClient tcpClient) {
            string clientInfo = tcpClient.Client.RemoteEndPoint.ToString();
            reportingClass.addMessage(string.Format("Got connection request from {0}", clientInfo));
            try {
                using (var networkStream = tcpClient.GetStream()) {
                    using (var writer = new StreamWriter(networkStream)) {
                        using (var reader = new StreamReader(networkStream)) {
                            Task.Run(() => readTask(reader, clientInfo));
                            Task.Run(() => writeTask(writer, clientInfo));
                            await connectionMonitor(writer, clientInfo);
                            lock (_lock) {
                                dictConnections.Remove(clientInfo);
                            }
                        }
                    }
                }                
            }
            catch (Exception exp) {
                reportingClass.addMessage(exp.Message);
            }
            finally {
                reportingClass.addMessage(string.Format("Closing the client connection - {0}", clientInfo));
                tcpClient.Close();
            }

            progress.Report(reportingClass);

        }
        private async Task connectionMonitor(StreamWriter writer, string clientInfo) {
            bool disco = false;
            while ( (!disco) & (!cToken.IsCancellationRequested) ) {
                // Give the reader one additional sec to read from the stream while data is still available
                try {
                    //writer.WriteLine("NOOP");
                    disco = dictConnections[clientInfo].quitGAME;
                    Task.Delay(1000).Wait();
                }
                catch(Exception e) {
                    // TCP Disconnect
                    disco = true;
                }                
            }
        }

        private async Task serverHouseKeepingTask() {
            while (!cToken.IsCancellationRequested) {
                foreach (var client in dictConnections) {
                    if (client.Key.StartsWith("_")) {
                        // This is a pretend client added to the server for testing
                        lock (_lock) {
                            if (client.Value.serverResponses.Count > 0) {                            
                                // We've got something to simulate!
                                string messageToSend = null;
                                messageToSend = client.Value.serverResponses[0];
                                reportingClass.addMessage("Simulate Sending: " + messageToSend);
                                progress.Report(reportingClass);
                                client.Value.serverResponses.RemoveAt(0);
                                if (messageToSend.ToUpper().StartsWith("REQUEST,")) {
                                    string[] split = messageToSend.Split(',');
                                    string simAction = client.Value.serverTestAutoResponseOnPlayRequest.ToUpper();
                                    switch(simAction) {
                                        case "ACCEPT":
                                            server_Command_handleWithAccept(client.Value, split);
                                            break;
                                        case "REQUEST":
                                            // Instead of accepting ... send them a request instead!
                                            handlePLAY(new string[] { "PLAY", split[1], "W" }, client.Value, true);
                                            break;
                                        default:
                                            // Do nothing
                                            break;
                                    }                                    
                                }
                            }
                        }
                    } else {
                        // Good spot for a NOOP request and check for alive players!
                    }
                }
                Task.Delay(100).Wait();
            }
        }
        private void server_Command_handleWithAccept(PerClientGameData clientGameData, string[] dataSplit) {
            // In this case we're simulating the server accepting the request so the opponent is the real player!
            // Tell bothsides the match is not underway!
            string playerName = dataSplit[1];
            string color = dataSplit[2];
            var opRemoteEdPoint = getRemoteEndPoint(playerName);
            if (opRemoteEdPoint != null) {
                var opClientGameData = dictConnections[opRemoteEdPoint];
                handleACCEPT(clientGameData, opClientGameData);
            }
            else {
                // A request from a player that no longer exists?
                // Just don't do anything about it!
            }
        }

        private async Task writeTask(StreamWriter writer, string remoteEndPoint) {
            writer.AutoFlush = true;
            while (true) {
                string messageToSend = null;
                lock (_lock) {
                    if (dictConnections[remoteEndPoint].serverResponses.Count > 0) {
                        messageToSend = dictConnections[remoteEndPoint].serverResponses[0];
                        reportingClass.addMessage("Sending: " + messageToSend);
                        progress.Report(reportingClass);
                        dictConnections[remoteEndPoint].serverResponses.RemoveAt(0);
                    }
                }
                if (messageToSend != null) {
                    await writer.WriteLineAsync(messageToSend);
                }
                Task.Delay(100).Wait();   
            }
        }

        private async Task readTask(StreamReader reader, string remoteEndPoint) {
            while (true) {
                var dataFromClient = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(dataFromClient)) {

                    reportingClass.addMessage(dataFromClient);
                    lock (_lock) {
                        processCommand(remoteEndPoint, dataFromClient);
                    }
                }
                progress.Report(reportingClass);
            }
        }        
        private bool processServerTestCommand(string remoteEndPoint, string dataFromClient) {
            string[] split = dataFromClient.ToUpper().Split(',');

            if (split[1].StartsWith("ADD")) {
                string playerName = split[2];
                
                if (getRemoteEndPoint(playerName)==null) {
                    // Setup a special remote endpoint for testing
                    remoteEndPoint = "_"+ playerName + "_"+ remoteEndPoint;
                    dictConnections.Add(remoteEndPoint,new PerClientGameData());
                    dictConnections[remoteEndPoint].playersName = playerName;
                    if (split.Length>=4) {
                        dictConnections[remoteEndPoint].serverTestAutoResponseOnPlayRequest = split[3];
                    }
                }
                return true;
            }

            if (split[1].StartsWith("MATCH")) {
                string playerName1 = split[2];
                string playerName2 = split[3];
                createMatchBetweenPlayers(playerName1, playerName2);
                return true;
            }
            return false;
        }

        public bool createMatchBetweenPlayers(string playerName1, string playerName2) {
            string remoteEndPoint1 = getRemoteEndPoint(playerName1);
            string remoteEndPoint2 = getRemoteEndPoint(playerName2);

            if ((remoteEndPoint1!=null)&&(remoteEndPoint2!= null)) {
                // Put these two in a match                
                var playerData1 = dictConnections[remoteEndPoint1];
                var playerData2 = dictConnections[remoteEndPoint2];

                playerData1.initializeMatch(playerData2.playersName, remoteEndPoint2);
                playerData2.initializeMatch(playerData1.playersName, remoteEndPoint2);  
                return true;
            }
            return false;
        }

        private string getRemoteEndPoint(string playerToFind) {
            foreach(var player in dictConnections) {
                if (player.Value.playersName==null) {
                    // This player is not yet initialized!
                    continue;
                }
                if (player.Value.playersName.ToUpper().ToString().Equals(playerToFind.ToUpper())) {
                    return player.Key;
                }
            }
            return null;
        }

        private void processCommand(string remoteEndPoint, string dataFromClient) {
            string upperData = dataFromClient.ToUpper();
            string[] dataSplit = dataFromClient.Split(',');
            var clientGameData = dictConnections[remoteEndPoint];

            if (upperData.StartsWith("SERVER_COMMAND,")) {
                // Special server command used for testing only!
                processServerTestCommand(remoteEndPoint, dataFromClient);
                return;
            }

            if (upperData.StartsWith("CONNECT,")) {                
                string playerName = dataSplit[1];
                if (getRemoteEndPoint(playerName) == null) {
                    clientGameData.playersName = playerName;
                    clientGameData.serverResponses.Add("OK");
                    //sendPlayers(clientGameData);
                }
                else {
                    clientGameData.serverResponses.Add("ERROR,Invalid Name");
                }
                return;
            }            

            if (upperData.StartsWith("PLAY,")) {
                handlePLAY(dataSplit, clientGameData);
                return;
            }

            if (upperData.StartsWith("ACCEPT,")) {
                string playerName = dataSplit[1];
                var opRemoteEdPoint = getRemoteEndPoint(playerName);
                if (opRemoteEdPoint != null) {
                    var opClientGameData = dictConnections[opRemoteEdPoint];
                    handleACCEPT(clientGameData, opClientGameData);
                }
                else {
                    // A request from a player that no longer exists?
                    // Just don't do anything about it!
                }
                return;
            }

            if (upperData.StartsWith("GET,BOARD")) {
                clientGameData.serverResponses.Add("BOARD," + clientGameData.serializeBoard());
                return;
            }

            if (upperData.StartsWith("GET,PLAYERS")) {
                sendPlayers(clientGameData);
                return;
            }

            if (upperData.StartsWith("MOVE,")) {
                string[] split = dataFromClient.Split(',');
                if (movePieceOnBoard(remoteEndPoint, split[1], split[2])) {
                    clientGameData.serverResponses.Add("OK");
                }
                else {
                    clientGameData.serverResponses.Add("ERROR,You cannot move there");
                }
                return;
            }

            if (upperData.StartsWith("QUIT,")) {
                if (upperData.EndsWith("MATCH")) {
                    quitMatch(clientGameData);
                }
                else if (upperData.EndsWith("GAME")) {
                    quitGame(clientGameData);
                }
                else {
                    clientGameData.serverResponses.Add("ERROR,Invalid QUIT Command");
                }
                return;
            }

            // WTH ... just say OK
            clientGameData.serverResponses.Add("OK");
        }

        private void handlePLAY(string[] dataSplit, PerClientGameData clientGameData, bool serverTest = false) {
            string playerName = dataSplit[1].ToUpper();
            string color = dataSplit[2].ToUpper();

            if (!clientGameData.dictPendingPlayRequests.ContainsKey(playerName)) {
                var opRemoteEdPoint = getRemoteEndPoint(playerName);
                if (opRemoteEdPoint != null) {
                    var opClientGameData = dictConnections[opRemoteEdPoint];
                    if (opClientGameData.available) {
                        clientGameData.dictPendingPlayRequests.Add(playerName, opRemoteEdPoint);
                        opClientGameData.serverResponses.Add("REQUEST," + clientGameData.playersName + "," + color);
                    }
                    else {
                        clientGameData.serverResponses.Add("ERROR," + playerName + " is not available");
                    }
                }
                else {
                    clientGameData.serverResponses.Add("ERROR," + playerName + " does not exist");
                }
            }
            else {
                clientGameData.serverResponses.Add("ERROR,Already have a pending request sent to " + playerName);
            }
        }   
        private void handleACCEPT(PerClientGameData clientGameData, PerClientGameData opClientGameData) {
            createMatchBetweenPlayers(opClientGameData.playersName, clientGameData.playersName);
            clientGameData.serverResponses.Add("ACCEPTED," + opClientGameData.playersName);            
            opClientGameData.serverResponses.Add("ACCEPTED," + clientGameData.playersName);
            sendPlayers(opClientGameData);
        }
        private void sendPlayers(PerClientGameData clientGameData) {
            string listOfPlayers = serializePlayers(clientGameData.playersName);
            // It's ok to send an empty list
            clientGameData.serverResponses.Add("PLAYERS" + listOfPlayers);
        }
        private string serializePlayers(string currentPlayerName) {
            StringBuilder sb = new StringBuilder();
            foreach(var client in dictConnections) {
                if (client.Value.playersName!=null)  {
                    if ( (currentPlayerName==null)||
                        (!currentPlayerName.ToUpper().Equals(client.Value.playersName.ToUpper()))) {
                        sb.Append("," + client.Value.playersName+":"+client.Value.status);
                    }
                }
            }
            return sb.ToString();
        }

        private bool movePieceOnBoard(string remoteEndPoint, string from, string to) {
            bool rv = false;
            // Almost always true for now ... server has only small amount of logic!
            // dict should already be locked
            var chessPieces = dictConnections[remoteEndPoint].chessPieces;
            if (chessPieces.ContainsKey(from)) {
                ChessPiece cp = chessPieces[from];
                if (!chessPieces.ContainsKey(to)) {
                    chessPieces.Add(to, cp);
                    // If we moved it then it's now gone from the other location aye?
                    chessPieces.Remove(from);
                    return true;
                }
            }

            return rv;
        }
        private void quitMatch(PerClientGameData clientGameData) {
            if (dictConnections.ContainsKey(clientGameData.opponentsRemoteEndPoint)) {
                var opClientGameData = dictConnections[clientGameData.opponentsRemoteEndPoint];
                opClientGameData.serverResponses.Add("WINNER," + clientGameData.opponentsName);
                clientGameData.serverResponses.Add("WINNER," + clientGameData.opponentsName);

                clientGameData.destroyMatch();
                opClientGameData.destroyMatch();

                sendPlayers(opClientGameData);                
            }
            // Regardless of the circumstances ... let's send a new player list
            sendPlayers(clientGameData);
        }

        private void quitGame(PerClientGameData clientGameData) {
            if (dictConnections.ContainsKey(clientGameData.opponentsRemoteEndPoint)) {
                var opClientGameData = dictConnections[clientGameData.opponentsRemoteEndPoint];
                opClientGameData.serverResponses.Add("WINNER," + clientGameData.opponentsName);
                clientGameData.serverResponses.Add("WINNER," + clientGameData.opponentsName);

                clientGameData.destroyMatch();
                opClientGameData.destroyMatch();

                sendPlayers(opClientGameData);                
            }
            clientGameData.serverResponses.Add("OK");
            clientGameData.quitGAME = true;
        }
    }
}
