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
            public List<string> serverResponses = new List<string>();
            public string playersName = null;
            public Dictionary<string, ChessPiece> chessPieces = null;

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
        }
        private int _listeningPort;
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        
        private Dictionary<string, PerClientGameData> dictConnections = new Dictionary<string, PerClientGameData>();
        private Dictionary<string, string> dictPlayers = new Dictionary<string, string>();
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
                            await allDone();
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
        private async Task allDone() {
            while (!cToken.IsCancellationRequested) {
                // Give the reader one additional sec to read from the stream while data is still available
                Task.Delay(1000).Wait();
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
        private void processServerTestCommand(string remoteEndPoint, string dataFromClient) {
            string[] split = dataFromClient.ToUpper().Split(',');

            if (split[1].StartsWith("CONNECT")) {
                if (!dictPlayers.ContainsKey(split[2])) {
                    // Setup a special remote endpoint for testing
                    remoteEndPoint = "_"+split[2]+"_"+ remoteEndPoint;
                    dictConnections.Add(remoteEndPoint,new PerClientGameData());
                    dictPlayers.Add(split[2], remoteEndPoint);
                    dictConnections[remoteEndPoint].playersName = split[2];
                }
                return;
            }
        }

        private void processCommand(string remoteEndPoint, string dataFromClient) {            
            string upperData = dataFromClient.ToUpper();
            string[] upperSplit = upperData.Split(','), dataSplit = dataFromClient.Split(',');
            var clientGameData = dictConnections[remoteEndPoint];

            if (upperData.StartsWith("SERVER_COMMAND,")) {
                // Special server command used for testing only!
                processServerTestCommand(remoteEndPoint, dataFromClient);
                return;
            }
            if (upperData.StartsWith("CONNECT,")) {
                if (!dictPlayers.ContainsKey(upperSplit[1])) {
                    dictPlayers.Add(upperSplit[1], remoteEndPoint);
                    clientGameData.playersName = dataSplit[1];
                    clientGameData.serverResponses.Add("OK");
                }
                else {
                    clientGameData.serverResponses.Add("ERROR,Invalid Name");
                }
                return;
            }

            if (upperData.StartsWith("GET,BOARD")) {
                clientGameData.serverResponses.Add("BOARD," + clientGameData.serializeBoard());
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

            // WTH ... just say OK
            clientGameData.serverResponses.Add("OK");
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
    }
}
