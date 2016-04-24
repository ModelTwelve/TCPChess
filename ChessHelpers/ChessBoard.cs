using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers {
    public class ChessBoard {
        private Dictionary<string, ChessPiece> chessPieces = null;
        public string currentColorsTurn { get; set; }

        private object _lock = new object();

        public ChessBoard() {
            initializeBoard();
        }

        public string serializeBoard() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Board");
            foreach (var kvp in chessPieces) {
                sb.Append("," + kvp.Key + ":" + kvp.Value.Color + ":" + kvp.Value.KindOfPiece);
            }
            return sb.ToString();
        }

        public bool movePiece(string playerColorAttemptingToMove, string from, string to, out string errorMessage) {
            // Start out with no error message            
            lock (_lock) {
                errorMessage = "";
                if (!playerColorAttemptingToMove.Equals(currentColorsTurn)) {
                    errorMessage = "It is not your turn";
                    return false;
                }
                if (!chessPieces.ContainsKey(from)) {
                    errorMessage = "No piece on the board at that location";
                    return false;
                }
                if (from.Equals(to)) {
                    errorMessage = "You did not move anything";
                    return false;
                }    
                if (
                    (chessPieces.ContainsKey(to)) &&
                    (chessPieces[to].Color.Equals(playerColorAttemptingToMove))
                    ) {
                    errorMessage = "You cannot move ontop of your own piece";
                    return false;
                }
                // ******************************************
                // More error checking logic goes here!
                // ******************************************

                ChessPiece copyOfPieceToMove = chessPieces[from];
                // Ok ... from here on out I assume all is well.
                if (chessPieces.ContainsKey(to)) {
                    // This piece needs removed from the board
                    chessPieces.Remove(to);
                }
                // Now add our piece at the new location
                chessPieces.Add(to, copyOfPieceToMove);
                // If we moved it then it's now gone from the other location aye?
                chessPieces.Remove(from);
                // All is good ... now flip flop turn colors
                currentColorsTurn = FlipFlopColor(currentColorsTurn);
                return true;
            }
        }

        public static string FlipFlopColor(string color) {
            return color.Equals("W") ? "B" : "W";
        }

        private void initializeBoard() {
            currentColorsTurn = "W";

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
    }
}
