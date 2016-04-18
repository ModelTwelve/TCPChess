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
        public string currentColorsTurn {
            get {
                return chessBoard.currentColorsTurn;
            }
        }

        private ChessBoard chessBoard = null;
        
        // ToPlayername and ColorRequested
        private Dictionary<string, PlayRequest> dictPendingPlayRequests;
        public string serverTestAutoResponseOnPlayRequest = "";
        public bool quitGAME = false;
        private object _lock = new object();

        public string status {
            get {
                return chessBoard == null ? "Available In the Lobby" : "Playing " + opponentsName;
            }
        }
        public bool available {
            get {
                return chessBoard == null;
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

        public bool movePiece(string from, string to, out string errorMessage) {            
           return chessBoard.movePiece(playersColor, from, to, out errorMessage);
        }

        public string serializeBoard() {
            return chessBoard.serializeBoard();
        }

        private void init() {
            responseQueue = new OutBoundMessageQueue();
            playersName = null;
            chessBoard = null;
            opponentsName = "";
            opponentsRemoteEndPoint = "";
            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();
        }

        public ChessBoard initializeMatch(string opName, string opRemoteEndPoint, ChessBoard opponentsChessBoard=null, string forcedColor =null) {
            
            opponentsName = opName;
            opponentsRemoteEndPoint = opRemoteEndPoint;

            chessBoard = opponentsChessBoard ?? new ChessBoard();            

            // forcedColor is only possible from a server test or if you're the player that accepted the play request!
            if (forcedColor == null) {
                var playRequest = dictPendingPlayRequests[opName.ToUpper()];
                playersColor = playRequest.Color;
            }
            else {
                playersColor = forcedColor;
            }

            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();

            return chessBoard;
        }

        public void destroyMatch() {
            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();
            opponentsName = "";
            opponentsRemoteEndPoint = "";
            chessBoard = null;
        }
    }
}
