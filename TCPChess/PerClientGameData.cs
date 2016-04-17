using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPChess {
    public class PerClientGameData {
        private OutBoundMessageQueue responseQueue;
        public string playersName { get; set; }
        public string playersColor { get; set; }
        public string opponentsName { get; set; }
        public string opponentsRemoteEndPoint { get; set; }

        public string currentColorsTurn { get; set; }

        private Dictionary<string, ChessPiece> chessPieces = null;
        // ToPlayername and ColorRequested
        private Dictionary<string, PlayRequest> dictPendingPlayRequests;
        public string serverTestAutoResponseOnPlayRequest = "";
        public bool quitGAME = false;
        private object _lock = new object();

        public string status {
            get {
                return chessPieces == null ? "Available In the Lobby" : "Playing " + opponentsName;
            }
        }
        public bool available {
            get {
                return chessPieces == null;
            }
        }

        public PerClientGameData() {
            init();
        }

        public void addServerResponse(string data) {
            lock (_lock) {
                responseQueue.AddMessage(data);
            }
        }

        public string removeFirstServerResponse() {
            string rv = null;
            lock (_lock) {
                rv = responseQueue.RemoveMessage();
            }
            return rv;
        }

        public bool CheckPlayRequests(string playerName) {
            lock (_lock) {
                return dictPendingPlayRequests.ContainsKey(playerName.ToUpper());
            }
        }
        public bool CheckPlayRequestAndColor(string playerName, string color) {
            lock (_lock) {
                // Did I request to play this person as this color?
                if (dictPendingPlayRequests.ContainsKey(playerName.ToUpper())) {
                    var request = dictPendingPlayRequests[playerName.ToUpper()];
                    // What color did I want to be?
                    return color.Equals(request.Color);
                }
            }
            return false;
        }
        public void AddPlayRequest(string playerName, string myRequestedColor, string opRemoteEdPoint) {
            lock (_lock) {
                dictPendingPlayRequests.Add(playerName.ToUpper(), new PlayRequest(opRemoteEdPoint, myRequestedColor));
            }
        }
        public void RemoveRequests(string playerName) {
            lock (_lock) {
                playerName = playerName.ToUpper();
                List<string> toRemove = new List<string>();
                foreach (var player in dictPendingPlayRequests) {
                    if (player.Key.ToUpper().StartsWith(playerName + ":")) { 
                        toRemove.Add(playerName + ":");
                    }
                    if (player.Key.ToUpper().Equals(playerName)) { 
                        toRemove.Add(playerName);
                    }
                }
                foreach(var item in toRemove) {
                    dictPendingPlayRequests.Remove(item);
                }
            }
        }

        public string serializeBoard() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Board");
            foreach (var kvp in chessPieces) {
                sb.Append("," + kvp.Key + ":" + kvp.Value.Color + ":" + kvp.Value.KindOfPiece);
            }
            return sb.ToString();
        }

        public bool movePiece(string from, string to) {
            bool rv = false;            
            lock(_lock) {
                if (chessPieces.ContainsKey(from)) {
                    ChessPiece cp = chessPieces[from];
                    if (!chessPieces.ContainsKey(to)) {
                        chessPieces.Add(to, cp);
                        // If we moved it then it's now gone from the other location aye?
                        chessPieces.Remove(from);

                        // All is good ... now flip flop turn colors
                        currentColorsTurn = currentColorsTurn.Equals("W") ? "B" : "W";

                        return true;
                    }
                }
            }
            return rv;
        }

        private void init() {
            responseQueue = new OutBoundMessageQueue();
            playersName = null;
            chessPieces = null;
            opponentsName = "";
            opponentsRemoteEndPoint = "";
            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();
        }

        public void initializeMatch(string opName, string opRemoteEndPoint, string forcedColor=null) {
            
            opponentsName = opName;
            opponentsRemoteEndPoint = opRemoteEndPoint;

            currentColorsTurn = "W";

            // forcedColor is only possible from a server test or if you're the player that accepted the play request!
            if (forcedColor == null) {
                var playRequest = dictPendingPlayRequests[opName.ToUpper()];
                playersColor = playRequest.Color;
            }
            else {
                playersColor = forcedColor;
            }

            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();

            chessPieces = new Dictionary<string, ChessPiece>();
            chessPieces.Add("0:0", new ROOK("B"));
            chessPieces.Add("1:0", new KNIGHT("B"));
            chessPieces.Add("2:0", new BISHOP("B"));
            chessPieces.Add("3:0", new KING("B"));
            chessPieces.Add("4:0", new QUEEN("B"));
            chessPieces.Add("5:0", new BISHOP("B"));
            chessPieces.Add("6:0", new KNIGHT("B"));
            chessPieces.Add("7:0", new ROOK("B"));
            chessPieces.Add("0:1", new PAWN("B"));
            chessPieces.Add("1:1", new PAWN("B"));
            chessPieces.Add("2:1", new PAWN("B"));
            chessPieces.Add("3:1", new PAWN("B"));
            chessPieces.Add("4:1", new PAWN("B"));
            chessPieces.Add("5:1", new PAWN("B"));
            chessPieces.Add("6:1", new PAWN("B"));
            chessPieces.Add("7:1", new PAWN("B"));
            chessPieces.Add("0:6", new PAWN("W"));
            chessPieces.Add("1:6", new PAWN("W"));
            chessPieces.Add("2:6", new PAWN("W"));
            chessPieces.Add("3:6", new PAWN("W"));
            chessPieces.Add("4:6", new PAWN("W"));
            chessPieces.Add("5:6", new PAWN("W"));
            chessPieces.Add("6:6", new PAWN("W"));
            chessPieces.Add("7:6", new PAWN("W"));
            chessPieces.Add("0:7", new ROOK("W"));
            chessPieces.Add("1:7", new KNIGHT("W"));
            chessPieces.Add("2:7", new BISHOP("W"));
            chessPieces.Add("3:7", new KING("W"));
            chessPieces.Add("4:7", new QUEEN("W"));
            chessPieces.Add("5:7", new BISHOP("W"));
            chessPieces.Add("6:7", new KNIGHT("W"));
            chessPieces.Add("7:7", new ROOK("W"));
        }

        public void destroyMatch() {
            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();
            opponentsName = "";
            opponentsRemoteEndPoint = "";
            chessPieces = null;
        }
    }
}
