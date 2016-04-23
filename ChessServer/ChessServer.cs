using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ChessHelpers;
using System.Threading;
using System.Threading.Tasks;

namespace ServerForm {
    public class ChessServer {
        private int _listeningPort;
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        private ServerConnections serverConnections = new ServerConnections();

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

                    serverConnections.AddClient(tcpClient.Client.RemoteEndPoint.ToString());
                    
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
                            serverConnections.Remove(clientInfo);
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
            int x = 0;
            while ( (!disco) & (!cToken.IsCancellationRequested) ) {
                // Give the reader one additional sec to read from the stream while data is still available
                try {
                    ++x;
                    if (x > 20) {
                        // Every so many seconds send out a NOOP to make sure this client is still alive!
                        x = 0;
                        writer.WriteLine("NOOP");
                    }
                    var client = serverConnections.GetClientGameData(clientInfo);
                    disco = client.quitGAME;
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
                PerClientGameData client;
                string messageToSend = serverConnections.HouseKeeping(out client);
                if (messageToSend != null) {
                    reportingClass.addMessage("Simulate Sending: " + messageToSend);
                    progress.Report(reportingClass);
                    if (messageToSend.ToUpper().StartsWith("REQUEST,")) {
                        string[] split = messageToSend.Split(',');
                        string simAction = client.serverTestAutoResponseOnPlayRequest.ToUpper();
                        switch (simAction) {
                            case "ACCEPTED":
                                server_Command_handleWithAccept(client, split);
                                break;
                            case "REQUESTW":
                                // Instead of accepting ... send them a request instead!
                                handlePLAY(new string[] { "PLAY", split[1], "W" }, client, true);
                                break;
                            case "REQUESTB":
                                // Instead of accepting ... send them a request instead!
                                handlePLAY(new string[] { "PLAY", split[1], "B" }, client, true);
                                break;
                            case "REFUSE":
                                // Instead of accepting ... send them a refuse instead!
                                handleREFUSE(new string[] { "REFUSE", split[1] }, client, true);
                                // Next time let's request Black
                                client.serverTestAutoResponseOnPlayRequest = "REQUESTB";
                                break;
                            default:
                                // Do nothing
                                break;
                        }
                    }
                }
                // Loop again
                Task.Delay(100).Wait();
            }
        }
        private void server_Command_handleWithAccept(PerClientGameData clientGameData, string[] dataSplit) {
            // In this case we're simulating the server accepting the request so the opponent is the real player!
            // Tell bothsides the match is not underway!
            string playerName = dataSplit[1];
            // Whomever I'm pretending to accept will choose the opposite color
            string color = ChessBoard.FlipFlopColor(dataSplit[2]);
            var opRemoteEdPoint = serverConnections.GetRemoteEndPoint(playerName);
            if (opRemoteEdPoint != null) {
                var opClientGameData = serverConnections.GetClientGameData(opRemoteEdPoint);
                handleACCEPTED(clientGameData, opClientGameData, color);
            }
            else {
                // A request from a player that no longer exists?
                // Just don't do anything about it!
            }
        }

        private async Task writeTask(StreamWriter writer, string remoteEndPoint) {
            writer.AutoFlush = true;
            while (true) {
                string messageToSend = serverConnections.GetServerResponse(remoteEndPoint);                
                if (messageToSend != null) {
                    reportingClass.addMessage("Sending: " + messageToSend);
                    progress.Report(reportingClass);
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
                    processCommand(remoteEndPoint, dataFromClient);                    
                }
                progress.Report(reportingClass);
            }
        }        
        private bool processServerTestCommand(string remoteEndPoint, string dataFromClient) {
            string[] split = dataFromClient.ToUpper().Split(',');

            if (split[1].StartsWith("ADD")) {
                string playerName = split[2];
                
                if (serverConnections.GetRemoteEndPoint(playerName)==null) {
                    // Setup a special remote endpoint for testing
                    remoteEndPoint = "_"+ playerName + "_"+ remoteEndPoint;
                    serverConnections.AddClient(remoteEndPoint);
                    serverConnections.SetPlayersName(remoteEndPoint, playerName);
                    if (split.Length>=4) {
                        serverConnections.SetTestAutoResponseOnPlayRequest(remoteEndPoint, split[3]);
                    }
                }
                return true;
            }

            if (split[1].StartsWith("MATCH")) {
                string[] player1 = split[2].Split(':');
                string[] player2 = split[3].Split(':');

                string playerName1 = player1[0];
                string playerName2 = player2[0];
                string playerColor1 = player1[1];
                string playerColor2 = player2[1];
                createMatchBetweenPlayers(playerName1, playerName2, playerColor1, playerColor2);
                return true;
            }
            return false;
        }

        public bool createMatchBetweenPlayers(string playerName1, string playerName2, string playerColor1, string playerColor2) {
            // Preassigned colors must be coming from a server test!
            string remoteEndPoint1 = serverConnections.GetRemoteEndPoint(playerName1);
            string remoteEndPoint2 = serverConnections.GetRemoteEndPoint(playerName2);

            if ((remoteEndPoint1!=null)&&(remoteEndPoint2!= null)) {
                return serverConnections.InitializeMatch(remoteEndPoint1, remoteEndPoint2, playerColor1, playerColor2);
            }
            return false;
        }
        
        private void processCommand(string remoteEndPoint, string dataFromClient) {
            string upperData = dataFromClient.ToUpper();
            string[] dataSplit = dataFromClient.Split(',');
            var clientGameData = serverConnections.GetClientGameData(remoteEndPoint);

            if (upperData.StartsWith("SERVER_COMMAND,")) {
                // Special server command used for testing only!
                processServerTestCommand(remoteEndPoint, dataFromClient);
                return;
            }

            if (upperData.StartsWith("CONNECT,")) {                
                string playerName = dataSplit[1];
                if (serverConnections.GetRemoteEndPoint(playerName) == null) {
                    clientGameData.playersName = playerName;
                    clientGameData.addServerResponse("OK");
                    serverConnections.RefreshAllPlayers();
                    //sendPlayers(clientGameData);
                }
                else {
                    clientGameData.addServerResponse("ERROR,Invalid Name");
                }
                return;
            }            

            if (upperData.StartsWith("PLAY,")) {
                handlePLAY(dataSplit, clientGameData);
                return;
            }

            if (upperData.StartsWith("ACCEPTED,")) {
                string playerName = dataSplit[1];
                string color = dataSplit[2];
                var opRemoteEdPoint = serverConnections.GetRemoteEndPoint(playerName);
                if (opRemoteEdPoint != null) {
                    var opClientGameData = serverConnections.GetClientGameData(opRemoteEdPoint);
                    handleACCEPTED(clientGameData, opClientGameData, color);
                }
                else {
                    // A request from a player that no longer exists?
                    clientGameData.addServerResponse("ERROR," + playerName + " does not exist");
                }
                return;
            }

            if (upperData.StartsWith("GET,BOARD")) {
                clientGameData.addServerResponse(clientGameData.serializeBoard());
                return;
            }

            if (upperData.StartsWith("GET,PLAYERS")) {
                sendPlayers(clientGameData);
                return;
            }

            if (upperData.StartsWith("GET,TURN")) {
                sendTurn(clientGameData);
                return;
            }

            if (upperData.StartsWith("REFUSE,")) {
                handleREFUSE(dataSplit, clientGameData);
                return;
            }

            if (upperData.StartsWith("MOVE,")) {
                string[] split = dataFromClient.Split(',');
                string errorMessage;
                if (serverConnections.MoveChessPiece(clientGameData, split[1], split[2], split.Length>3 ? split[3] : null,out errorMessage)) {
                    clientGameData.addServerResponse("OK");
                }
                else {
                    clientGameData.addServerResponse("ERROR,"+ errorMessage);
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
                    clientGameData.addServerResponse("ERROR,Invalid QUIT Command");
                }
                return;
            }

            // WTH ... just say ERROR
            clientGameData.addServerResponse("ERROR,Unknown Action");
        }

        private void handleREFUSE(string[] dataSplit, PerClientGameData clientGameData, bool serverTest = false) {
            string playerName = dataSplit[1].ToUpper();
            if (!clientGameData.CheckPlayRequests(playerName)) {
                var opRemoteEndPoint = serverConnections.GetRemoteEndPoint(playerName);
                if (opRemoteEndPoint != null) {
                    var opClientGameData = serverConnections.GetClientGameData(opRemoteEndPoint);
                    if (opClientGameData.available) {
                        opClientGameData.RemoveRequests(clientGameData.playersName);
                        opClientGameData.addServerResponse("ERROR," + clientGameData.playersName + " Refused");
                    }                    
                }
            }
            // No else conditions needed ... just don't repond back
        }

        private void handlePLAY(string[] dataSplit, PerClientGameData clientGameData, bool serverTest = false) {
            string playerName = dataSplit[1].ToUpper();
            string color = dataSplit[2].ToUpper();

            if (!clientGameData.CheckPlayRequests(playerName)) {
                var opRemoteEdPoint = serverConnections.GetRemoteEndPoint(playerName);
                if (opRemoteEdPoint != null) {
                    var opClientGameData = serverConnections.GetClientGameData(opRemoteEdPoint);
                    if (opClientGameData.available) {
                        clientGameData.AddPlayRequest(playerName, color, opRemoteEdPoint);
                        opClientGameData.addServerResponse("REQUEST," + clientGameData.playersName + "," + color);
                    }
                    else {
                        clientGameData.addServerResponse("ERROR," + playerName + " is not available");
                    }
                }
                else {
                    clientGameData.addServerResponse("ERROR," + playerName + " does not exist");
                }
            }
            else {
                clientGameData.addServerResponse("ERROR,Already have a pending request sent to " + playerName);
            }
        }   
        private void handleACCEPTED(PerClientGameData clientGameData, PerClientGameData opClientGameData, string color) {
            // Does the opponent have this request in their dict?
            if (!opClientGameData.CheckPlayRequest(clientGameData.playersName)) {
                clientGameData.addServerResponse("ERROR," + opClientGameData.playersName + " never requested to play you");
                return;
            }
            string playerColor1, playerColor2;
            if (!opClientGameData.CheckPlayRequestColor(clientGameData.playersName,color)) {
                // Dang ... both players want to be the same color
                Random r = new Random();
                if (r.Next(0,2)==0) {
                    color = ChessBoard.FlipFlopColor(color);
                }
            }
            // This is the color we've choosen for the player that issued the ACCEPT
            playerColor2 = color;
            playerColor1 = ChessBoard.FlipFlopColor(playerColor2);
            createMatchBetweenPlayers(opClientGameData.playersName, clientGameData.playersName, playerColor1, playerColor2);
            
            clientGameData.addServerResponse("ACCEPTED," + opClientGameData.playersName + "," + opClientGameData.playersColor);
            opClientGameData.addServerResponse("ACCEPTED," + clientGameData.playersName + "," + clientGameData.playersColor);
            
            sendPlayers(opClientGameData);
            if (clientGameData.playersColor.Equals(clientGameData.currentColorsTurn)) {
                sendTurn(clientGameData);
            } else {
                sendTurn(opClientGameData);
            }
        }

        private void sendTurn(PerClientGameData clientGameData) {
            clientGameData.addServerResponse("GO,"+ clientGameData.playersName);
        }

        private void sendPlayers(PerClientGameData clientGameData) {
            string listOfPlayers = serverConnections.SerializePlayers(clientGameData.playersName);
            // It's ok to send an empty list
            clientGameData.addServerResponse("PLAYERS" + listOfPlayers);
        }
        
              
        private void quitMatch(PerClientGameData clientGameData) {
            var opClientGameData = serverConnections.GetClientGameData(clientGameData.opponentsRemoteEndPoint);
            if (opClientGameData!=null) {
                opClientGameData.addServerResponse("WINNER," + clientGameData.opponentsName);
                clientGameData.addServerResponse("WINNER," + clientGameData.opponentsName);

                clientGameData.destroyMatch();
                opClientGameData.destroyMatch();

                sendPlayers(opClientGameData);                
            }
            // Regardless of the circumstances ... let's send a new player list
            sendPlayers(clientGameData);
        }

        private void quitGame(PerClientGameData clientGameData) {
            var opClientGameData = serverConnections.GetClientGameData(clientGameData.opponentsRemoteEndPoint);
            if (opClientGameData != null) {
                opClientGameData.addServerResponse("WINNER," + clientGameData.opponentsName);
                clientGameData.addServerResponse("WINNER," + clientGameData.opponentsName);

                clientGameData.destroyMatch();
                opClientGameData.destroyMatch();

                sendPlayers(opClientGameData);                
            }
            clientGameData.addServerResponse("OK");
            clientGameData.quitGAME = true;
        }
    }
}
