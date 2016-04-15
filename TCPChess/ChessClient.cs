﻿using System;
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
    public class ChessClient {
        private object _lock = new object();
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        private List<string> clientCommands;

        public ChessClient(CancellationToken cToken, IProgress<ReportingClass> progress, List<string> clientCommands = null) {
            this.cToken = cToken;
            this.progress = progress;
            this.clientCommands = clientCommands ?? new List<string>();
        }

        public async Task Start(IPAddress ipAddress, int port) {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(ipAddress, port);

            reportingClass.addMessage("Connected to Server");
            progress.Report(reportingClass);

            using (var networkStream = client.GetStream()) {
                using (var writer = new StreamWriter(networkStream)) {
                    using (var reader = new StreamReader(networkStream)) {
                        Task.Run(() => readTask(reader));
                        Task.Run(() => writeTask(writer));
                        await allDone();                        
                    }
                }
            }
            if (client != null) {
                client.Close();
            }
            progress.Report(reportingClass);
        }

        private async Task allDone() {
            while (!cToken.IsCancellationRequested) {
                // Give the reader one additional sec to read from the stream while data is still available
                Task.Delay(1000).Wait();
            }
        }

        private async Task writeTask(StreamWriter writer) {
            writer.AutoFlush = true;
            while (true) {
                string messageToSend = null;
                if (clientCommands.Count > 0) {
                    lock (_lock) {
                        messageToSend = clientCommands[0];

                        reportingClass.addMessage("Sending: " + messageToSend);
                        progress.Report(reportingClass);
                        clientCommands.RemoveAt(0);
                    }
                    await writer.WriteLineAsync(messageToSend);
                }
                Task.Delay(100).Wait();
            }
        }

        private async Task readTask(StreamReader reader) {
            while (true) {
                var dataFromServer = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(dataFromServer)) {
                    if (!dataFromServer.ToUpper().Equals("NOOP")) {
                        reportingClass.addMessage(dataFromServer);
                        lock (_lock) {
                            processCommand(dataFromServer);
                        }
                    }
                }
                progress.Report(reportingClass);
            }
        }
        private void processCommand(string dataFromServer) {
            string upperData = dataFromServer.ToUpper();
            string[] dataSplit = dataFromServer.Split(',');          

            if (upperData.StartsWith("ACCEPTED,")) {
                handleACCEPTED(dataSplit);
                return;
            }

            if (upperData.StartsWith("WINNER,")) {
                handleWINNER(upperData);
                return;
            }
        }       
        private void handleACCEPTED(string[] dataSplit) {
            string playerName = dataSplit[1];
            clientCommands.Add("GET,BOARD");            
        }
        private void handleWINNER(string data) {
            reportingClass.addMessage(data);
            progress.Report(reportingClass);
        }

        public void requestMove(string data) {
            lock (_lock) {
                clientCommands.Add("MOVE,"+data);
            }
        }

        public void requestPlay(string data, string color) {
            string[] split = data.Split(':');
            lock (_lock) {
                clientCommands.Add("PLAY," + split[0]+","+color);
            }
        }

        public void acceptPlay(string data) {
            string[] split = data.Split(':');
            lock (_lock) {
                clientCommands.Add("ACCEPT," + split[0]);
                clientCommands.Add("GET,BOARD");
            }
        }

        public void getBoard() {
            lock (_lock) {
                clientCommands.Add("GET,board");
            }
        }

        public void quitMatch() {
            lock (_lock) {
                clientCommands.Add("QUIT,MATCH");
            }
        }

        public void quitGame() {
            lock (_lock) {
                clientCommands.Add("QUIT,GAME");
            }
        }
    }
}
