using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPChess {
    public class ServerConnections {
        private Dictionary<string, PerClientGameData> dictConnections = new Dictionary<string, PerClientGameData>();
        private object _lock = new object();
        public ServerConnections() {

        }
        public bool AddClient(string RemoteEndPoint) {
            lock (_lock) {
                if (!dictConnections.ContainsKey(RemoteEndPoint)) {
                    dictConnections.Add(RemoteEndPoint, new PerClientGameData());
                    return true;
                }
            }
            return false;
        }

        public PerClientGameData GetClientGameData(string RemoteEndPoint) {
            lock (_lock) {
                if (dictConnections.ContainsKey(RemoteEndPoint)) {
                    return dictConnections[RemoteEndPoint];
                }
            }
            return null;
        }

        public bool MoveChessPiece(string remoteEndPoint, string from, string to) {
            bool rv = false;
            lock (_lock) {
                if (dictConnections.ContainsKey(remoteEndPoint)) {
                    var currentPlayer = dictConnections[remoteEndPoint];
                    var opponentPlayer = dictConnections[currentPlayer.opponentsRemoteEndPoint];
                    string playersColor = currentPlayer.playersColor;
                    string turnsColor = currentPlayer.currentColorsTurn;
                    if (playersColor.Equals(turnsColor)){
                        rv = currentPlayer.movePiece(from, to);                        
                        if (rv) {
                            // This was a successful move ... update the opponents data
                            opponentPlayer.movePiece(from, to);

                            // Now send out some new boards
                            currentPlayer.addServerResponse(currentPlayer.serializeBoard());
                            opponentPlayer.addServerResponse(opponentPlayer.serializeBoard());
                        }
                    }
                }
            }
            return rv;
        }

        public void RefreshAllPlayers() {
            lock (_lock) {
                foreach (var client in dictConnections) {
                    string players = SerializePlayers(client.Value.playersName);
                    client.Value.addServerResponse("PLAYERS" + players);
                }
            }
        }

        public string SerializePlayers(string PlayerName) {
            StringBuilder sb = new StringBuilder();
            lock (_lock) {                
                foreach (var client in dictConnections) {
                    if (client.Value.playersName != null) {
                        if ((PlayerName == null) ||
                            (!PlayerName.ToUpper().Equals(client.Value.playersName.ToUpper()))) {
                            sb.Append("," + client.Value.playersName + ":" + client.Value.status);
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public bool SetPlayersName(string RemoteEndPoint, string playerName) {
            lock (_lock) {
                if (dictConnections.ContainsKey(RemoteEndPoint)) {
                    dictConnections[RemoteEndPoint].playersName = playerName;
                    return false;
                }
            }
            return true;
        }

        public bool SetTestAutoResponseOnPlayRequest(string RemoteEndPoint, string autoResponse) {
            lock (_lock) {
                if (dictConnections.ContainsKey(RemoteEndPoint)) {
                    dictConnections[RemoteEndPoint].serverTestAutoResponseOnPlayRequest = autoResponse;
                    return false;
                }
            }
            return true;
        }

        public bool InitializeMatch(string RemoteEndPoint1, string RemoteEndPoint2, string playerColor1=null, string playerColor2=null) {
            // Preassigned colors must be coming from a server test!
            // Put these two in a match                
            var playerData1 = dictConnections[RemoteEndPoint1];
            var playerData2 = dictConnections[RemoteEndPoint2];

            playerData1.initializeMatch(playerData2.playersName, RemoteEndPoint2, playerColor1);
            playerColor2 = playerData1.playersColor.Equals("W") ? "B" : "W";
            playerData2.initializeMatch(playerData1.playersName, RemoteEndPoint1, playerColor2);            
            
            return true;
        }

        public string GetRemoteEndPoint(string playerToFind) {
            foreach (var player in dictConnections) {
                if (player.Value.playersName == null) {
                    // This player is not yet initialized!
                    continue;
                }
                if (player.Value.playersName.ToUpper().ToString().Equals(playerToFind.ToUpper())) {
                    return player.Key;
                }
            }
            return null;
        }

        public bool Remove(string RemoteEndPoint) {
            lock (_lock) {
                if (dictConnections.ContainsKey(RemoteEndPoint)) {
                    dictConnections.Remove(RemoteEndPoint);
                    return true;
                }
            }
            return false;
        }

        public string GetServerResponse(string RemoteEndPoint) {
            string rv = null;
            lock (_lock) {
                if (dictConnections.ContainsKey(RemoteEndPoint)) {
                    rv = dictConnections[RemoteEndPoint].removeFirstServerResponse();
                }
            }
            return rv;
        }

        public string HouseKeeping(out PerClientGameData actingClient) {
            actingClient = null;
            lock (_lock) {
                string messageToSend = null;
                foreach (var client in dictConnections) {
                    if (client.Key.StartsWith("_")) {
                        // This is a pretend client added to the server for testing
                        messageToSend = GetServerResponse(client.Key);
                        if (messageToSend != null) {
                            actingClient = client.Value;
                            break;
                        }
                    }
                }
                return messageToSend;
            }
        }
    }
}
