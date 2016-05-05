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

namespace ChessClient
{
    public class ChessClient
    {
        private object _lock = new object();
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        private OutBoundMessageQueue responseQueue;
        private string playerName = "";
        private bool connectOK = false;

        public ChessClient(CancellationToken cToken, IProgress<ReportingClass> progress, List<string> clientCommands)
        {
            this.cToken = cToken;
            this.progress = progress;
            this.responseQueue = new OutBoundMessageQueue();
            foreach (var message in clientCommands)
            {
                responseQueue.AddMessage(message);
            }
        }

        public async Task Start(IPAddress ipAddress, int port, string playerName)
        {
            connect(playerName);

            var client = new TcpClient();
            Socket _socket = client.Client;
            _socket.Connect(ipAddress, port);

            reportingClass.addMessage("Connected to Server");
            progress.Report(reportingClass);

            using (var networkStream = new NetworkStream(_socket))
            {
                using (var writer = new StreamWriter(networkStream))
                {
                    using (var reader = new StreamReader(networkStream))
                    {
                        Task.Run(() => readTask(reader));
                        Task.Run(() => writeTask(writer));
                        await connectionMonitor();
                    }
                }
            }
            if (client != null)
            {
                client.Close();
            }
            progress.Report(reportingClass);
        }

        private async Task connectionMonitor()
        {
            while (!cToken.IsCancellationRequested)
            {
                // Give the reader one additional sec to read from the stream while data is still available
                Task.Delay(1000).Wait();
            }
        }

        private async Task writeTask(StreamWriter writer)
        {
            writer.AutoFlush = true;
            bool disco = false;
            while (!disco)
            {
                try
                {
                    string messageToSend = removeMessage();
                    if (messageToSend != null)
                    {
                        reportingClass.addMessage("Sending: " + messageToSend);
                        progress.Report(reportingClass);

                        await writer.WriteLineAsync(messageToSend);
                    }
                }
                catch (Exception e)
                {
                    disco = true;
                    reportingClass.addMessage("WRITE: " + e.Message);
                }
                Task.Delay(100).Wait();
            }
        }

        private async Task readTask(StreamReader reader)
        {
            bool disco = false;
            while (!disco)
            {
                try
                {
                    var dataFromServer = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(dataFromServer))
                    {
                        if (!dataFromServer.ToUpper().Equals("NOOP"))
                        {
                            reportingClass.addMessage(dataFromServer);
                            processCommand(dataFromServer);
                        }
                    }
                    progress.Report(reportingClass);
                }
                catch (Exception e)
                {
                    disco = true;
                    reportingClass.addMessage("READ: " + e.Message);
                }
                Task.Delay(100).Wait();
            }
        }
        private void processCommand(string dataFromServer)
        {
            string upperData = dataFromServer.ToUpper();
            string[] dataSplit = dataFromServer.Split(',');
            string action = dataSplit[0].ToUpper();

            // If name not yet allowed from the server then the only commands we're looking for are OK and ERROR
            if (!connectOK)
            {
                if (action.Equals("OK"))
                {
                    // Tell the frontend the name was good
                    reportingClass.addMessage("REPORT_CONNECT_OK");
                    connectOK = true;
                    addMessage("GET,Players");
                }
                else if (action.Equals("ERROR"))
                {
                    // Tell the frontend the name was bad
                    reportingClass.addMessage("REPORT_CONNECT_ERROR");
                }
                return;
            }

            switch (action)
            {
                case "ACCEPTED":
                    reportingClass.addMessage("REPORT_ACCEPTED," + dataSplit[1] + "," + dataSplit[2]);
                    addMessage("GET,BOARD");
                    break;
                case "BOARD":
                case "PLAYERS":
                case "REQUEST":
                case "WINNER":
                case "ERROR":
                case "GO":
                case "POSSIBLE":
                    reportingClass.addMessage("REPORT_" + dataFromServer);
                    progress.Report(reportingClass);
                    break;
                case "NOOP":
                case "OK":
                    break;
                default:
                    addMessage("ERROR,Unknown Action from Server " + action);
                    break;
            }
        }

        private void addMessage(string data)
        {
            lock (_lock)
            {
                responseQueue.AddMessage(data);
            }
        }
        private string removeMessage()
        {
            string messageToSend = null;
            lock (_lock)
            {
                messageToSend = responseQueue.RemoveMessage();
            }
            return messageToSend;
        }
        public void requestMove(string data)
        {
            addMessage("MOVE," + data);
        }

        public void requestPossible(string data)
        {
            addMessage("GET,POSSIBLE," + data);
        }

        public void requestGetPlayers()
        {
            addMessage("GET,Players");
        }

        public void requestPlay(string data, string color)
        {
            string[] split = data.Split(':');
            addMessage("PLAY," + split[0] + "," + color);
        }

        public void acceptPlay(string data)
        {
            string[] split = data.Split(':');
            addMessage("ACCEPTED," + split[0] + "," + split[1]);
            addMessage("GET,BOARD");
        }

        public void getBoard()
        {
            addMessage("GET,BOARD");
        }
        public void getTurn()
        {
            addMessage("GET,TURN");
        }

        public void quitMatch()
        {
            addMessage("QUIT,MATCH");
        }

        public void quitGame()
        {
            addMessage("QUIT,GAME");
        }
        public void connect(string incomingPlayerName)
        {
            this.playerName = incomingPlayerName.ToUpper();
            addMessage("CONNECT," + incomingPlayerName);
        }
    }
}
