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
        private int _listeningPort;
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        private List<string[]> serverResponses;

        public ChessServer(int port, CancellationToken cToken, IProgress<ReportingClass> progress, List<string[]> serverResponses= null) {
            _listeningPort = port;
            this.cToken = cToken;
            this.progress = progress;
            this.serverResponses = serverResponses ?? new List<string[]>();
        }
        public async Task Start() {
            IPAddress ipAddre = IPAddress.Any;
            TcpListener listener = new TcpListener(ipAddre, _listeningPort);
            listener.Start();
            reportingClass.addMessage("Server is running");
            reportingClass.addMessage("Listening on port " + _listeningPort);

            while (true) {
                reportingClass.addMessage("Waiting for connections...");
                try {
                    progress.Report(reportingClass);
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    HandleConnectionAsync(tcpClient);
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
                string currentBoard = getInitialBoard();
                using (var networkStream = tcpClient.GetStream())
                using (var reader = new StreamReader(networkStream))
                using (var writer = new StreamWriter(networkStream)) {
                    writer.AutoFlush = true;
                    while (true) {
                        progress.Report(reportingClass);
                        var dataFromClient = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(dataFromClient)) {
                            break;
                        }
                        string responseToClient = "UNKNOWN";
                        reportingClass.addMessage("Client " + clientInfo + " says: " + dataFromClient);
                        if (dataFromClient.ToUpper().StartsWith("GET,BOARD")) {
                            responseToClient = currentBoard;
                        }
                        else if (dataFromClient.ToUpper().StartsWith("MOVE,")) {
                            string[] split = dataFromClient.Split(',');
                            currentBoard = movePieceOnBoard(currentBoard, split[1], split[2]);
                            responseToClient = currentBoard;
                        }
                        else {
                            responseToClient = findAutoResponse(dataFromClient, currentBoard) ?? "UNKNOWN";
                        }
                        reportingClass.addMessage("Sending: " + responseToClient);
                        await writer.WriteLineAsync(responseToClient);
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

        private string findAutoResponse(string dataFromClient, string currentBoard) {
            foreach(var sa in serverResponses) {
                if (dataFromClient.ToUpper().StartsWith(sa[0].ToUpper())) {                    
                    return sa[1];
                }
            }
            return null;
        }

        private string movePieceOnBoard(string currentBoard, string from, string to) {
            string[] pieces = currentBoard.Split(',');

            for(int x = 0; x < pieces.Length; x++) {
                if(pieces[x].StartsWith(from)) {
                    pieces[x] = to + pieces[x].Remove(0, 3);
                    break;
                }
            }
            return string.Join(",", pieces);
        }

        private string getInitialBoard() {
            List<string> rv = new List<string>();

            // Setup Black on top for now
            rv.Add("0:0:B:ROOK");
            rv.Add("1:0:B:KNIGHT");
            rv.Add("2:0:B:BISHOP");
            rv.Add("3:0:B:KING");
            rv.Add("4:0:B:QUEEN");
            rv.Add("5:0:B:BISHOP");
            rv.Add("6:0:B:KNIGHT");
            rv.Add("7:0:B:ROOK");

            rv.Add("0:1:B:PAWN");
            rv.Add("1:1:B:PAWN");
            rv.Add("2:1:B:PAWN");
            rv.Add("3:1:B:PAWN");
            rv.Add("4:1:B:PAWN");
            rv.Add("5:1:B:PAWN");
            rv.Add("6:1:B:PAWN");
            rv.Add("7:1:B:PAWN");

            // White on the bottom

            rv.Add("0:6:W:PAWN");
            rv.Add("1:6:W:PAWN");
            rv.Add("2:6:W:PAWN");
            rv.Add("3:6:W:PAWN");
            rv.Add("4:6:W:PAWN");
            rv.Add("5:6:W:PAWN");
            rv.Add("6:6:W:PAWN");
            rv.Add("7:6:W:PAWN");

            rv.Add("0:7:W:ROOK");
            rv.Add("1:7:W:KNIGHT");
            rv.Add("2:7:W:BISHOP");
            rv.Add("3:7:W:KING");
            rv.Add("4:7:W:QUEEN");
            rv.Add("5:7:W:BISHOP");
            rv.Add("6:7:W:KNIGHT");
            rv.Add("7:7:W:ROOK");

            StringBuilder sb = new StringBuilder();
            sb.Append("Board,");
            foreach (var square in rv) {
                sb.Append(square + ",");
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
