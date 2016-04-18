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
using ChessHelpers;

namespace ChessClient {
    public class ChessClient {
        private object _lock = new object();
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        private OutBoundMessageQueue responseQueue;
        private string playerName = "";
        private string sendACCEPTED = "";

        public ChessClient(CancellationToken cToken, IProgress<ReportingClass> progress, List<string> clientCommands) {
            this.cToken = cToken;
            this.progress = progress;
            this.responseQueue = new OutBoundMessageQueue();
            foreach(var message in clientCommands) {
                responseQueue.AddMessage(message);
            }            
        }

        public async Task Start(IPAddress ipAddress, int port, string playerName) {
            this.playerName = playerName;

            // Let's get the party started!
            addMessage("CONNECT," + playerName);
            addMessage("GET,players");

            TcpClient client = new TcpClient();
            await client.ConnectAsync(ipAddress, port);

            reportingClass.addMessage("Connected to Server");
            progress.Report(reportingClass);

            using (var networkStream = client.GetStream()) {
                using (var writer = new StreamWriter(networkStream)) {
                    using (var reader = new StreamReader(networkStream)) {
                        Task.Run(() => readTask(reader));
                        Task.Run(() => writeTask(writer));
                        await connectionMonitor();
                    }
                }
            }
            if (client != null) {
                client.Close();
            }
            progress.Report(reportingClass);
        }

        private async Task connectionMonitor() {
            while (!cToken.IsCancellationRequested) {
                // Give the reader one additional sec to read from the stream while data is still available
                Task.Delay(1000).Wait();
            }
        }

        private async Task writeTask(StreamWriter writer) {
            writer.AutoFlush = true;
            while (true) {
                string messageToSend = removeMessage();
                if (messageToSend != null) {    
                    reportingClass.addMessage("Sending: " + messageToSend);
                    progress.Report(reportingClass);

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
                        processCommand(dataFromServer);
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

            if (upperData.StartsWith("GO,")) {
                handleTURN(upperData);
                return;
            }

            if ( (sendACCEPTED.Length>0)&& (upperData.StartsWith("OK")) ) {
                string[] split = sendACCEPTED.Split(':');
                sendACCEPTED = "";
                // The the frontend what's happening
                reportingClass.addMessage("ACCEPTED,"+ split[0]+","+split[1]);
            }

            if ((sendACCEPTED.Length > 0) && (upperData.StartsWith("ERROR,"))) {
                // The the frontend what's happening
                reportingClass.addMessage("REFUSED," + sendACCEPTED);
                // Refused or not available anymore or something bad happened
                sendACCEPTED = "";                
            }
        }
        private void handleACCEPTED(string[] dataSplit) {
            string playerName = dataSplit[1];
            addMessage("GET,BOARD");
        }
        private void handleWINNER(string data) {
            reportingClass.addMessage(data);
            progress.Report(reportingClass);
        }
        private void handleTURN(string data) {
            reportingClass.addMessage(data);
            progress.Report(reportingClass);
        }

        private void addMessage(string data) {
            lock (_lock) {
                responseQueue.AddMessage(data);
            }
        }
        private string removeMessage() {
            string messageToSend = null;
            lock (_lock) {
                messageToSend = responseQueue.RemoveMessage();
            }
            return messageToSend;
        }
        public void requestMove(string data) {
            addMessage("MOVE,"+data);            
        }

        public void requestGetPlayers() {
            addMessage("GET,Players");
        }

        public void requestPlay(string data, string color) {
            string[] split = data.Split(':');
            addMessage("PLAY," + split[0]+","+color);
        }

        public void acceptPlay(string data) {
            // Remember who we're trying to ACCEPT
            sendACCEPTED = data;
            string[] split = data.Split(':');
            addMessage("ACCEPTED," + split[0] +","+ split[1]);
            addMessage("GET,BOARD");
        }

        public void getBoard() {
            addMessage("GET,board");
        }

        public void quitMatch() {
            addMessage("QUIT,MATCH");
        }

        public void quitGame() {
            addMessage("QUIT,GAME");
        }
    }
}
