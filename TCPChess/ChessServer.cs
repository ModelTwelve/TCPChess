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
        private Dictionary<string, List<string>> serverResponses;

        public ChessServer(int port, CancellationToken cToken, IProgress<ReportingClass> progress, Dictionary<string, List<string>> serverResponses= null) {
            _listeningPort = port;
            this.cToken = cToken;
            this.progress = progress;
            this.serverResponses = serverResponses ?? new Dictionary<string, List<string>>();
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
                        reportingClass.addMessage("Client "+ clientInfo+" says: "+dataFromClient);

                        string responseToClient = "UNKNOWN";
                        if (serverResponses.ContainsKey(dataFromClient)) {
                            List<string> cmdList = serverResponses[dataFromClient];
                            if (cmdList.Count > 0) {
                                responseToClient = cmdList[0];
                                cmdList.RemoveAt(0);
                                serverResponses[dataFromClient] = cmdList;
                            }                            
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
    }
}
