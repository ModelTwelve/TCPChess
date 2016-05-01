using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers
{
    public class PerClientGameData
    {
        private OutBoundMessageQueue responseQueue;
        public string playersName { get; set; }
        public string playersColor { get; set; }
        public string opponentsName { get; set; }
        public string opponentsRemoteEndPoint { get; set; }
        public string displayPlayersName { get; set; }
        public string currentColorsTurn
        {
            get
            {
                return chessBoard.currentColorsTurn;
            }
        }

        private ChessBoard chessBoard = null;

        private Dictionary<string, PlayRequest> dictPendingPlayRequests;
        public string serverTestAutoResponseOnPlayRequest = "";
        public bool quitGAME = false;
        private object _lock = new object();

        public string status
        {
            get
            {
                return chessBoard == null ? "Available In the Lobby" : "Playing " + opponentsName;
            }
        }
        public bool available
        {
            get
            {
                return chessBoard == null;
            }
        }

        public PerClientGameData()
        {
            init();
        }

        public void addServerResponse(string data)
        {
            lock (_lock)
            {
                responseQueue.AddMessage(data);
            }
        }

        public string removeFirstServerResponse()
        {
            string rv = null;
            lock (_lock)
            {
                rv = responseQueue.RemoveMessage();
            }
            return rv;
        }

        public bool CheckPlayRequests(string playerName)
        {
            lock (_lock)
            {
                return dictPendingPlayRequests.ContainsKey(playerName.ToUpper());
            }
        }
        public bool CheckPlayRequest(string playerName)
        {
            lock (_lock)
            {
                // Did I request to play this person?
                if (dictPendingPlayRequests.ContainsKey(playerName.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckPlayRequestColor(string playerName, string color)
        {
            lock (_lock)
            {
                // Did I request to play this person as this color?
                if (dictPendingPlayRequests.ContainsKey(playerName.ToUpper()))
                {
                    var request = dictPendingPlayRequests[playerName.ToUpper()];
                    // What color did I want to be?   
                    // request.Color is my color ... so if this request matches my color then
                    // the opponent wants to be the same color.
                    // Return false and the server will have to randomize it                 
                    return !color.Equals(request.Color);
                }
            }
            return false;
        }

        public void AddPlayRequest(string playerName, string myRequestedColor, string opRemoteEdPoint)
        {
            lock (_lock)
            {
                dictPendingPlayRequests.Add(playerName.ToUpper(), new PlayRequest(opRemoteEdPoint, myRequestedColor));
            }
        }
        public void RemoveRequests(string playerName)
        {
            lock (_lock)
            {
                playerName = playerName.ToUpper();
                List<string> toRemove = new List<string>();
                foreach (var player in dictPendingPlayRequests)
                {
                    if (player.Key.ToUpper().Equals(playerName))
                    {
                        toRemove.Add(playerName);
                    }
                }
                foreach (var item in toRemove)
                {
                    dictPendingPlayRequests.Remove(item);
                }
            }
        }
        public LinkedList<string> getPossible(string from, out string errorMessage)
        {
            return chessBoard.getPossible(playersColor, from, out errorMessage);
        }

        public bool movePiece(string from, string to, string promotedPiece, out string errorMessage)
        {
            return chessBoard.movePiece(playersColor, from, to, promotedPiece, out errorMessage);
        }

        public string serializeBoard()
        {
            return chessBoard.serializeBoard();
        }

        private void init()
        {
            responseQueue = new OutBoundMessageQueue();
            playersName = null;
            chessBoard = null;
            opponentsName = "";
            opponentsRemoteEndPoint = "";
            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();
        }

        public ChessBoard initializeMatch(string opName, string opRemoteEndPoint, string forcedColor, ChessBoard opponentsChessBoard = null)
        {

            opponentsName = opName;
            opponentsRemoteEndPoint = opRemoteEndPoint;

            chessBoard = opponentsChessBoard ?? new ChessBoard();

            playersColor = forcedColor;

            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();

            return chessBoard;
        }

        public void destroyMatch()
        {
            dictPendingPlayRequests = new Dictionary<string, PlayRequest>();
            opponentsName = "";
            opponentsRemoteEndPoint = "";
            chessBoard = null;
        }
    }
}