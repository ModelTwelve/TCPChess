using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers
{
    public class ServerConnections
    {
        private Dictionary<string, PerClientGameData> dictConnections = new Dictionary<string, PerClientGameData>();
        private object _lock = new object();
        public ServerConnections()
        {

        }
        public bool AddClient(string RemoteEndPoint)
        {
            lock (_lock)
            {
                if (!dictConnections.ContainsKey(RemoteEndPoint))
                {
                    dictConnections.Add(RemoteEndPoint, new PerClientGameData());
                    return true;
                }
            }
            return false;
        }

        public PerClientGameData GetClientGameData(string RemoteEndPoint)
        {
            lock (_lock)
            {
                if (dictConnections.ContainsKey(RemoteEndPoint))
                {
                    return dictConnections[RemoteEndPoint];
                }
            }
            return null;
        }

        public bool MoveChessPiece(PerClientGameData currentPlayerGameData, string from, string to, string promotedPiece, out string errorMessage)
        {
            lock (_lock)
            {
                if (currentPlayerGameData.movePiece(from, to, promotedPiece, out errorMessage))
                {  
                    return true;
                }
            }
            return false;
        }

        public void RefreshAllPlayers()
        {
            lock (_lock)
            {
                foreach (var client in dictConnections)
                {
                    string players = SerializePlayers(client.Value.playersName);
                    client.Value.addServerResponse("PLAYERS" + players);
                }
            }
        }

        public string SerializePlayers(string PlayerName)
        {
            StringBuilder sb = new StringBuilder();
            lock (_lock)
            {
                foreach (var client in dictConnections)
                {
                    if (client.Value.playersName != null)
                    {
                        if ((PlayerName == null) ||
                            (!PlayerName.ToUpper().Equals(client.Value.playersName.ToUpper())))
                        {
                            sb.Append("," + client.Value.displayPlayersName + ":" + client.Value.status);
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public bool SetPlayersName(string RemoteEndPoint, string playerName, string displayName)
        {
            lock (_lock)
            {
                if (dictConnections.ContainsKey(RemoteEndPoint))
                {
                    dictConnections[RemoteEndPoint].playersName = playerName;
                    dictConnections[RemoteEndPoint].displayPlayersName = displayName;
                    return false;
                }
            }
            return true;
        }

        public bool SetTestAutoResponseOnPlayRequest(string RemoteEndPoint, string autoResponse)
        {
            lock (_lock)
            {
                if (dictConnections.ContainsKey(RemoteEndPoint))
                {
                    dictConnections[RemoteEndPoint].serverTestAutoResponseOnPlayRequest = autoResponse;
                    return false;
                }
            }
            return true;
        }

        public bool InitializeMatch(string RemoteEndPoint1, string RemoteEndPoint2, string playerColor1, string playerColor2)
        {
            // Preassigned colors must be coming from a server test!
            // Put these two in a match                
            var playerData1 = dictConnections[RemoteEndPoint1];
            var playerData2 = dictConnections[RemoteEndPoint2];

            ChessBoard chessBoard = playerData1.initializeMatch(playerData2.playersName, RemoteEndPoint2, playerColor1, null);
            playerData2.initializeMatch(playerData1.playersName, RemoteEndPoint1, playerColor2, chessBoard);

            return true;
        }

        public string GetRemoteEndPoint(string playerToFind)
        {
            foreach (var player in dictConnections)
            {
                if (player.Value.playersName == null)
                {
                    // This player is not yet initialized!
                    continue;
                }
                if (player.Value.playersName.ToUpper().ToString().Equals(playerToFind.ToUpper()))
                {
                    return player.Key;
                }
            }
            return null;
        }

        public bool Remove(string RemoteEndPoint)
        {
            lock (_lock)
            {
                if (dictConnections.ContainsKey(RemoteEndPoint))
                {
                    dictConnections.Remove(RemoteEndPoint);
                    return true;
                }
            }
            return false;
        }

        public string GetServerResponse(string RemoteEndPoint)
        {
            string rv = null;
            lock (_lock)
            {
                if (dictConnections.ContainsKey(RemoteEndPoint))
                {
                    rv = dictConnections[RemoteEndPoint].removeFirstServerResponse();
                }
                else
                {
                    throw new Exception("RemoteEndPoint NOT Found");
                }
            }
            return rv;
        }

        public string HouseKeeping(out PerClientGameData actingClient)
        {
            actingClient = null;
            lock (_lock)
            {
                string messageToSend = null;
                foreach (var client in dictConnections)
                {
                    if (client.Key.StartsWith("_"))
                    {
                        // This is a pretend client added to the server for testing
                        messageToSend = GetServerResponse(client.Key);
                        if (messageToSend != null)
                        {
                            actingClient = client.Value;
                            break;
                        }
                    }

                    // Check to see if clients engaged in a match are still alive after NOOPs
                    if (client.Value.opponentsRemoteEndPoint.Length > 0)
                    {
                        // We think we're playing someone ... see if they still exist
                        if (!dictConnections.ContainsKey(client.Value.opponentsRemoteEndPoint))
                        {
                            // They're gone ... send the client a WINNER!
                            client.Value.addServerResponse("WINNER," + client.Value.playersName);
                            client.Value.destroyMatch();
                            break;
                        }
                    }
                }
                return messageToSend;
            }
        }
    }
}
