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
    public class ChessClient {
        private object _lock = new object();
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();        
        private List<string> clientCommands;

        public ChessClient(CancellationToken cToken, IProgress<ReportingClass> progress, List<string> clientCommands=null) {
            this.cToken = cToken;
            this.progress = progress;
            this.clientCommands = clientCommands ?? new List<string>();
        }

        public async Task Start(int port) {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port);

            reportingClass.addMessage("Connected to Server");
            progress.Report(reportingClass);

            using (var networkStream = client.GetStream()) {
                using (var writer = new StreamWriter(networkStream)) {
                    using (var reader = new StreamReader(networkStream)) {
                        Task.Run(() => readTask(reader));
                        Task.Run(() => writeTask(writer));
                        await allDone();                        

                        int i = 0;
                        ++i;
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
                    reportingClass.addMessage(dataFromServer);
                }
                progress.Report(reportingClass);
            }
        }

        public void requestMove(string moveTo) {
            lock (_lock) {
                clientCommands.Add("MOVE,"+moveTo);
            }
        }

        public void getBoard() {
            lock (_lock) {
                clientCommands.Add("GET,board");
            }
        }
    }
}
