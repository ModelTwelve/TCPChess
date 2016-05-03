using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ChessHelpers;
using System.Threading;
using System.Threading.Tasks;

namespace ServerForm
{
    public class ChessServer
    {
        private int _listeningPort;
        private CancellationToken cToken;
        private IProgress<ReportingClass> progress;
        private ReportingClass reportingClass = new ReportingClass();
        private ServerConnections serverConnections = new ServerConnections();

        public ChessServer(int port, CancellationToken cToken, IProgress<ReportingClass> progress)
        {
            _listeningPort = port;
            this.cToken = cToken;
            this.progress = progress;
        }
        public async Task Start()
        {
            IPAddress ipAddre = IPAddress.Any;
            TcpListener listener = new TcpListener(ipAddre, _listeningPort);
            listener.Start();
            reportingClass.addMessage("Server is running");
            reportingClass.addMessage("Listening on port " + _listeningPort);

            Task.Run(() => serverHouseKeepingTask());

            int connectionNumber = 0;
            while (true)
            {
                ++connectionNumber;
                reportingClass.addMessage(String.Format("Waiting for connection {0} ...", connectionNumber.ToString()));
                try
                {
                    progress.Report(reportingClass);
                    var tcpClient = await listener.AcceptTcpClientAsync();

                    serverConnections.AddClient(tcpClient.Client.RemoteEndPoint.ToString());

                    Task.Run(() => HandleConnectionAsync(tcpClient));
                }
                catch (Exception exp)
                {
                    reportingClass.addMessage(exp.ToString());
                }
            }
        }

        private async void HandleConnectionAsync(TcpClient tcpClient)
        {
            string clientInfo = tcpClient.Client.RemoteEndPoint.ToString();
            reportingClass.addMessage(string.Format("Got connection request from {0}", clientInfo));
            try
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    using (var writer = new StreamWriter(networkStream))
                    {
                        using (var reader = new StreamReader(networkStream))
                        {
                            Task.Run(() => readTask(reader, clientInfo));
                            Task.Run(() => writeTask(writer, clientInfo));
                            await connectionMonitor(writer, clientInfo);
                            serverConnections.Remove(clientInfo);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                reportingClass.addMessage(exp.Message);
            }
            finally
            {
                reportingClass.addMessage(string.Format("Closing the client connection - {0}", clientInfo));
                tcpClient.Close();
            }

            progress.Report(reportingClass);

        }
        private async Task writeTask(StreamWriter writer, string remoteEndPoint)
        {
            writer.AutoFlush = true;
            bool disco = false;
            while (!disco)
            {
                try
                {
                    string messageToSend = serverConnections.GetServerResponse(remoteEndPoint);
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
                    serverConnections.Remove(remoteEndPoint);
                    reportingClass.addMessage("WRITE: " + e.Message);
                }
                Task.Delay(100).Wait();
            }
        }
        private async Task readTask(StreamReader reader, string remoteEndPoint)
        {
            bool disco = false;
            while (!disco)
            {
                try
                {
                    var dataFromClient = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(dataFromClient))
                    {
                        reportingClass.addMessage(dataFromClient);
                        processCommand(remoteEndPoint, dataFromClient);
                    }
                    progress.Report(reportingClass);
                }
                catch (Exception e)
                {
                    disco = true;
                    serverConnections.Remove(remoteEndPoint);
                    reportingClass.addMessage("READ: " + e.Message);
                }
            }
        }
        private async Task connectionMonitor(StreamWriter writer, string clientInfo)
        {
            bool disco = false;
            int x = 0;
            while ((!disco) & (!cToken.IsCancellationRequested))
            {
                // Give the reader one additional sec to read from the stream while data is still available
                try
                {
                    ++x;
                    if (x > 20)
                    {
                        // Every so many seconds send out a NOOP to make sure this client is still alive!
                        x = 0;
                        writer.WriteLine("NOOP");
                    }
                    var client = serverConnections.GetClientGameData(clientInfo);
                    disco = client.quitGAME;
                    Task.Delay(1000).Wait();
                }
                catch (Exception e)
                {
                    // TCP Disconnect
                    disco = true;
                }
            }
        }

        private async Task serverHouseKeepingTask()
        {
            while (!cToken.IsCancellationRequested)
            {
                PerClientGameData client;
                string messageToSend = serverConnections.HouseKeeping(out client);
                if (messageToSend != null)
                {
                    // The only messageToSend ever returned from HouseKeeping
                    // are simulated server messages!
                    processSimulatedServerAction(client, messageToSend);
                }
                // Loop again
                Task.Delay(100).Wait();
            }
        }

        private void processSimulatedServerAction(PerClientGameData client, string messageToSend)
        {
            reportingClass.addMessage("Simulate Sending: " + messageToSend);
            progress.Report(reportingClass);
            if (messageToSend.ToUpper().StartsWith("REQUEST,"))
            {
                string[] split = messageToSend.Split(',');
                string simAction = client.serverTestAutoResponseOnPlayRequest;
                switch (simAction)
                {
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
        private void server_Command_handleWithAccept(PerClientGameData clientGameData, string[] dataSplit)
        {
            // In this case we're simulating the server accepting the request so the opponent is the real player!
            // Tell bothsides the match is not underway!
            string playerName = dataSplit[1];
            // Whomever I'm pretending to accept will choose the opposite color
            string color = ChessBoard.FlipFlopColor(dataSplit[2]);
            var opRemoteEdPoint = serverConnections.GetRemoteEndPoint(playerName);
            if (opRemoteEdPoint != null)
            {
                var opClientGameData = serverConnections.GetClientGameData(opRemoteEdPoint);
                handleACCEPTED(clientGameData, opClientGameData, color);
            }
            else
            {
                // A request from a player that no longer exists?
                // Just don't do anything about it!
            }
        }
        private bool processServerTestCommand(string remoteEndPoint, string[] origSplit, string[] upperSplit)
        {
            if (upperSplit[1].StartsWith("ADD"))
            {
                string playerName = upperSplit[2];
                string displayName = origSplit[2];

                if (serverConnections.GetRemoteEndPoint(playerName) == null)
                {
                    // Setup a special remote endpoint for testing
                    remoteEndPoint = "_" + playerName + "_" + remoteEndPoint;
                    serverConnections.AddClient(remoteEndPoint);
                    serverConnections.SetPlayersName(remoteEndPoint, playerName, displayName);
                    if (upperSplit.Length >= 4)
                    {
                        serverConnections.SetTestAutoResponseOnPlayRequest(remoteEndPoint, upperSplit[3]);
                    }
                }
                return true;
            }

            if (upperSplit[1].StartsWith("MATCH"))
            {
                string[] player1 = upperSplit[2].Split(':');
                string[] player2 = upperSplit[3].Split(':');

                string playerName1 = player1[0];
                string playerName2 = player2[0];
                string playerColor1 = player1[1];
                string playerColor2 = player2[1];
                createMatchBetweenPlayers(playerName1, playerName2, playerColor1, playerColor2);
                return true;
            }
            return false;
        }
        public bool createMatchBetweenPlayers(string playerName1, string playerName2, string playerColor1, string playerColor2)
        {
            // Preassigned colors must be coming from a server test!
            string remoteEndPoint1 = serverConnections.GetRemoteEndPoint(playerName1);
            string remoteEndPoint2 = serverConnections.GetRemoteEndPoint(playerName2);

            if ((remoteEndPoint1 != null) && (remoteEndPoint2 != null))
            {
                return serverConnections.InitializeMatch(remoteEndPoint1, remoteEndPoint2, playerColor1, playerColor2);
            }
            return false;
        }

        private void processCommand(string remoteEndPoint, string receivedClientData)
        {
            var clientGameData = serverConnections.GetClientGameData(remoteEndPoint);

            try
            {
                string originalClientData = receivedClientData;
                string[] origClientSplit = originalClientData.Split(',');
                for (int x = 0; x < origClientSplit.Length; x++)
                {
                    origClientSplit[x] = origClientSplit[x].Trim();
                }
                originalClientData = string.Join(",", origClientSplit);

                string upperClientData = originalClientData.ToUpper();
                string[] upperClientSplit = upperClientData.Split(',');
                for (int x = 0; x < upperClientSplit.Length; x++)
                {
                    upperClientSplit[x] = upperClientSplit[x].Trim();
                }
                upperClientData = string.Join(",", upperClientSplit);
                
                string action = upperClientSplit[0];
                string playerName, displayPlayerName;

                switch (action)
                {
                    case "SERVER_COMMAND":
                        // Special server command used for testing only!
                        processServerTestCommand(remoteEndPoint, origClientSplit, upperClientSplit);
                        break;
                    case "CONNECT":
                        playerName = upperClientSplit[1];
                        displayPlayerName = origClientSplit[1];
                        if (serverConnections.GetRemoteEndPoint(playerName) == null)
                        {
                            clientGameData.playersName = playerName;
                            clientGameData.displayPlayersName = displayPlayerName;
                            clientGameData.addServerResponse("OK");
                            serverConnections.RefreshAllPlayers();
                            //sendPlayers(clientGameData);
                        }
                        else
                        {
                            clientGameData.addServerResponse("ERROR,Invalid Name");
                        }
                        break;
                    case "PLAY":
                        handlePLAY(upperClientSplit, clientGameData);
                        break;
                    case "ACCEPTED":
                        playerName = upperClientSplit[1];
                        string color = upperClientSplit[2];
                        var opRemoteEndPoint = serverConnections.GetRemoteEndPoint(playerName);
                        if (opRemoteEndPoint != null)
                        {
                            var opClientGameData = serverConnections.GetClientGameData(opRemoteEndPoint);
                            handleACCEPTED(clientGameData, opClientGameData, color);
                        }
                        else
                        {
                            // A request from a player that no longer exists?
                            clientGameData.addServerResponse("ERROR," + playerName + " does not exist");
                        }
                        break;
                    case "GET":
                        string getWhat = upperClientSplit[1];
                        switch (getWhat)
                        {
                            case "BOARD":
                                clientGameData.addServerResponse(clientGameData.serializeBoard());
                                break;
                            case "PLAYERS":
                                sendPlayers(clientGameData);
                                break;
                            case "TURN":
                                opRemoteEndPoint = serverConnections.GetRemoteEndPoint(clientGameData.opponentsName);
                                if (opRemoteEndPoint != null)
                                {
                                    var opClientGameData = serverConnections.GetClientGameData(opRemoteEndPoint);
                                    sendTurn(clientGameData, opClientGameData);
                                }
                                break;
                            case "POSSIBLE":
                                sendPossible(clientGameData, upperClientSplit[2]);
                                break;
                            default:
                                clientGameData.addServerResponse("ERROR,Invalid GET Command");
                                break;
                        }
                        break;
                    case "REFUSE":
                        handleREFUSE(upperClientSplit, clientGameData);
                        break;
                    case "MOVE":
                        string errorMessage;
                        if (serverConnections.MoveChessPiece(clientGameData, upperClientSplit[1], upperClientSplit[2], upperClientSplit.Length > 3 ? upperClientSplit[3] : null, out errorMessage))
                        {
                            // This was a successful move ... send back an OK 
                            // Now send out some new boards and turns
                            clientGameData.addServerResponse("OK");
                            clientGameData.addServerResponse(clientGameData.serializeBoard());
                            opRemoteEndPoint = serverConnections.GetRemoteEndPoint(clientGameData.opponentsName);
                            if (opRemoteEndPoint != null)
                            {
                                var opClientGameData = serverConnections.GetClientGameData(opRemoteEndPoint);
                                opClientGameData.addServerResponse(opClientGameData.serializeBoard());
                                sendTurn(opClientGameData, clientGameData);
                            }
                        }
                        else
                        {
                            clientGameData.addServerResponse("ERROR," + errorMessage);
                        }
                        break;
                    case "QUIT":
                        string quitWhat = upperClientSplit[1];
                        switch (quitWhat)
                        {
                            case "GAME":
                                quitGame(clientGameData);
                                break;
                            case "MATCH":
                                quitMatch(clientGameData);
                                break;
                            default:
                                clientGameData.addServerResponse("ERROR,Invalid QUIT Command");
                                break;
                        }
                        break;
                    case "ERROR":
                        // What is the client sending me this for?
                        break;
                    default:
                        // WTH ... just say ERROR
                        clientGameData.addServerResponse("ERROR,Unknown Action from Client " + action);
                        break;
                }
            }
            catch(Exception eAction)
            {
                clientGameData.addServerResponse("ERROR," + eAction.Message);
            }
        }

        private void handleREFUSE(string[] dataSplit, PerClientGameData clientGameData, bool serverTest = false)
        {
            string playerName = dataSplit[1];
            if (!clientGameData.CheckPlayRequests(playerName))
            {
                var opRemoteEndPoint = serverConnections.GetRemoteEndPoint(playerName);
                if (opRemoteEndPoint != null)
                {
                    var opClientGameData = serverConnections.GetClientGameData(opRemoteEndPoint);
                    if (opClientGameData.available)
                    {
                        opClientGameData.RemoveRequests(clientGameData.playersName);
                        opClientGameData.addServerResponse("ERROR," + clientGameData.playersName + " Refused");
                    }
                }
            }
            // No else conditions needed ... just don't repond back
        }

        private void handlePLAY(string[] dataSplit, PerClientGameData clientGameData, bool serverTest = false)
        {
            string playerName = dataSplit[1];
            string color = dataSplit[2];

            if (!clientGameData.CheckPlayRequests(playerName))
            {
                var opRemoteEdPoint = serverConnections.GetRemoteEndPoint(playerName);
                if (opRemoteEdPoint != null)
                {
                    var opClientGameData = serverConnections.GetClientGameData(opRemoteEdPoint);
                    if (opClientGameData.available)
                    {
                        clientGameData.AddPlayRequest(playerName, color, opRemoteEdPoint);
                        opClientGameData.addServerResponse("REQUEST," + clientGameData.displayPlayersName + "," + color);
                    }
                    else
                    {
                        clientGameData.addServerResponse("ERROR," + playerName + " is not available");
                    }
                }
                else
                {
                    clientGameData.addServerResponse("ERROR," + playerName + " does not exist");
                }
            }
            else
            {
                clientGameData.addServerResponse("ERROR,Already have a pending request sent to " + playerName);
            }
        }
        private void handleACCEPTED(PerClientGameData clientGameData, PerClientGameData opClientGameData, string color)
        {
            // Does the opponent have this request in their dict?
            if (!opClientGameData.CheckPlayRequest(clientGameData.playersName))
            {
                clientGameData.addServerResponse("ERROR," + opClientGameData.playersName + " never requested to play you");
                return;
            }
            string playerColor1, playerColor2;
            if (!opClientGameData.CheckPlayRequestColor(clientGameData.playersName, color))
            {
                // Dang ... both players want to be the same color
                Random r = new Random();
                if (r.Next(0, 2) == 0)
                {
                    color = ChessBoard.FlipFlopColor(color);
                }
            }
            // This is the color we've choosen for the player that issued the ACCEPT
            playerColor2 = color;
            playerColor1 = ChessBoard.FlipFlopColor(playerColor2);
            createMatchBetweenPlayers(opClientGameData.playersName, clientGameData.playersName, playerColor1, playerColor2);

            clientGameData.addServerResponse("ACCEPTED," + opClientGameData.playersName + "," + opClientGameData.playersColor);
            opClientGameData.addServerResponse("ACCEPTED," + clientGameData.playersName + "," + clientGameData.playersColor);

            // Gratuitous player refresh
            sendPlayers(clientGameData);
            sendPlayers(opClientGameData);

            // Gratuitous board refresh
            clientGameData.addServerResponse(clientGameData.serializeBoard());
            opClientGameData.addServerResponse(opClientGameData.serializeBoard());

            if (clientGameData.currentColorsTurn.Equals(clientGameData.playersColor))
            {
                sendTurn(clientGameData, opClientGameData);
            }
            else
            {
                sendTurn(opClientGameData, clientGameData);
            }
        }

        private void sendTurn(PerClientGameData clientGameData, PerClientGameData opClientGameData)
        {
            var goName = clientGameData.currentColorsTurn.Equals(clientGameData.playersColor) ? clientGameData.displayPlayersName : opClientGameData.displayPlayersName;
            clientGameData.addServerResponse("GO," + goName);
        }
        private void sendPossible(PerClientGameData clientGameData, string from)
        {
            string errorMessage = "";
            var list = clientGameData.getPossible(from, out errorMessage);
            if (errorMessage.Length>0)
            {
                clientGameData.addServerResponse("ERROR," + errorMessage);
                return;
            }
            else if (list!=null)
            {
                string rv = "POSSIBLE," + from + "," + string.Join(",", list);
                clientGameData.addServerResponse(rv);
            }
        }

        private void sendPlayers(PerClientGameData clientGameData)
        {
            string listOfPlayers = serverConnections.SerializePlayers(clientGameData.playersName);
            // It's ok to send an empty list
            clientGameData.addServerResponse("PLAYERS" + listOfPlayers);
        }

        private void quitMatch(PerClientGameData clientGameData)
        {
            var opClientGameData = serverConnections.GetClientGameData(clientGameData.opponentsRemoteEndPoint);
            if (opClientGameData != null)
            {
                opClientGameData.addServerResponse("WINNER," + opClientGameData.displayPlayersName);
                clientGameData.addServerResponse("WINNER," + opClientGameData.displayPlayersName);

                clientGameData.destroyMatch();
                opClientGameData.destroyMatch();

                sendPlayers(opClientGameData);
            }
            // Regardless of the circumstances ... let's send a new player list
            sendPlayers(clientGameData);
        }

        private void quitGame(PerClientGameData clientGameData)
        {
            var opClientGameData = serverConnections.GetClientGameData(clientGameData.opponentsRemoteEndPoint);
            if (opClientGameData != null)
            {
                opClientGameData.addServerResponse("WINNER," + opClientGameData.displayPlayersName);
                clientGameData.addServerResponse("WINNER," + opClientGameData.displayPlayersName);

                clientGameData.destroyMatch();
                opClientGameData.destroyMatch();

                sendPlayers(opClientGameData);
            }
            clientGameData.addServerResponse("OK");
            clientGameData.quitGAME = true;
        }
    }
}
