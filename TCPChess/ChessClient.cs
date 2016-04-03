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

namespace TCPTest {
    public class ChessClient {

        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        private bool doneWrite = false;
        private bool doneRead = false;
        private List<string> clientTestCommands;

        public ChessClient(CancellationToken cToken, IProgress<ReportingClass> progress, List<string> clientTestCommands=null) {
            this.cToken = cToken;
            this.progress = progress;
            this.clientTestCommands = clientTestCommands ?? new List<string>();
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
            while (!doneWrite || !doneRead) {
                if (doneWrite) {
                    doneRead = true;
                }
                // Give the reader one additional sec to read from the stream while data is still available
                Task.Delay(1000, cToken).Wait(cToken);
            }
        }

        private async Task writeTask(StreamWriter writer) {
            writer.AutoFlush = true;
            for (int i = 0; i < clientTestCommands.Count; i++) {
                string messageToSend = clientTestCommands[i];

                reportingClass.addMessage("Sending: " + messageToSend);
                progress.Report(reportingClass);

                await writer.WriteLineAsync(messageToSend);

                Task.Delay(1000).Wait();
            }

            doneWrite = true;
        }

        private async Task readTask(StreamReader reader) {
            while (true) {
                var dataFromServer = await reader.ReadLineAsync();

                doneRead = false;

                if (!string.IsNullOrEmpty(dataFromServer)) {
                    reportingClass.addMessage(dataFromServer);
                }
            }
        }
    }
}
