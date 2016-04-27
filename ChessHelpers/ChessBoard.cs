using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers
{
    public class ChessBoard
    {
        private Dictionary<string, ChessPiece> chessPieces = null;
        public string currentColorsTurn { get; set; }
        public String blackKingsPlace = "4:0";
        public String whiteKingsPlace = "4:7";
        public bool whiteCheck = false;
        public bool blackCheck = false;
        private object _lock = new object();

        public ChessBoard()
        {
            //initializeBoard();
            initializePromotePawn();
        }

        public string serializeBoard()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BOARD");
            foreach (var kvp in chessPieces)
            {
                sb.Append("," + kvp.Key + ":" + kvp.Value.Color + ":" + kvp.Value.KindOfPiece);
            }
            return sb.ToString();
        }

        public bool movePiece(string playerColorAttemptingToMove, string from, string to, string promotedPiece, out string errorMessage)
        {
            // Start out with no error message            
            lock (_lock)
            {

                string[] split = to.Split(':');
                int toX = Convert.ToInt32(split[0]);
                int toY = Convert.ToInt32(split[1]);

                split = from.Split(':');
                int fromX = Convert.ToInt32(split[0]);
                int fromY = Convert.ToInt32(split[1]);

                errorMessage = "";
                if (!playerColorAttemptingToMove.Equals(currentColorsTurn))
                {
                    errorMessage = "It is not your turn";
                    return false;
                }
                if (!chessPieces.ContainsKey(from))
                {
                    errorMessage = "No piece on the board at that location";
                    return false;
                }
                if (from.Equals(to))
                {
                    errorMessage = "You did not move anything";
                    return false;
                }
                if (
                    (chessPieces.ContainsKey(to)) &&
                    (chessPieces[to].Color.Equals(playerColorAttemptingToMove))
                    )
                {
                    errorMessage = "You cannot move ontop of your own piece";
                    return false;
                }
                if (
                    (chessPieces.ContainsKey(from)) &&
                    (!chessPieces[from].Color.Equals(playerColorAttemptingToMove))
                    )
                {
                    errorMessage = "You cannot move your opponents piece";
                    return false;
                }
                if (
                    (chessPieces.ContainsKey(to)) &&
                    (chessPieces[from].KindOfPiece.Equals("PAWN")) &&
                    (fromX == toX)
                    )
                {
                    errorMessage = "A pawn cannot kill moving forward";
                    return false;
                }

                //checks to see all valid moves for a piece
                ChessPiece checkMoves = chessPieces[from];
                LinkedList<String> check = checkMoves.generatePossibleMoves(this, from);
                if (chessPieces[from].KindOfPiece.Equals("PAWN"))
                {
                    int numSpaces = Math.Abs(fromY - toY);
                    if (numSpaces == 2)
                    {
                        ((PAWN)chessPieces[from]).allowEnPassant = true;
                    }
                }
                if (!check.Contains(to))
                {
                    errorMessage = "A " + chessPieces[from].KindOfPiece + " cant move there!";
                    return false;
                }


                {
                    //we now have determined that this move is legal
                    //checks to see if your potential move put your king into check or keeps your king in check
                    String pos;
                    String type;
                    bool isInCheck = false;

                    //create potential chess board
                    ChessBoard potentialChessBoard = clone();
                    //move piece to new location
                    potentialChessBoard.chessPieces.Remove(from);
                    //if there is a peice there delete it
                    if (potentialChessBoard.chessPieces.ContainsKey(to))
                    {
                        // This piece needs removed from the board
                        potentialChessBoard.chessPieces.Remove(to);
                    }
                    //add the piece to the new location in the potential board
                    potentialChessBoard.chessPieces.Add(to, checkMoves);

                    //checking mock board (with potential move)
                    foreach (var piece in potentialChessBoard.chessPieces)
                    {
                        //if piece isnt on your team
                        if (!piece.Value.Color.Equals(currentColorsTurn))
                        {
                            //returns list of that pieces avaiable moves
                            check = piece.Value.generatePossibleMoves(potentialChessBoard, piece.Key);
                            pos = piece.Key;
                            type = piece.Value.KindOfPiece;

                            //if kings position is in that list then we set boolean to true return an error message

                            //if white move then check white king vs all black pieces
                            if (currentColorsTurn.Equals("W"))
                            {
                                isInCheck = check.Contains(whiteKingsPlace);
                                if (isInCheck)
                                {
                                    errorMessage = "Your opponents " + type + " at position - " + pos + " puts you in Check!";
                                    return false;
                                }
                            }
                            //if black move check black king vs all white
                            else
                            {
                                isInCheck = check.Contains(blackKingsPlace);
                                if (isInCheck)
                                {
                                    errorMessage = "Your opponents " + type + " at position - " + pos + " puts you in Check!";
                                    return false;
                                }
                            }

                        }

                    }
                }
                //catch(Exception e)
                //{
                //    errorMessage = e.ToString();
                //    return false;
                //}
                try
                {
                    //If your king is not (or no longer) in check then we want to check to see if your move put their king in check
                    //check from spot of new move
                    check = checkMoves.generatePossibleMoves(this, to);
                    //black move so whitecheck
                    if (currentColorsTurn.Equals("B"))
                    {
                        //change boolean to show you are now in check
                        whiteCheck = check.Contains(whiteKingsPlace);
                    }
                    //if white move check black king
                    else
                    {
                        //change boolean to show you are now in check
                        blackCheck = check.Contains(blackKingsPlace);
                    }
                }
                catch (Exception e)
                {
                    errorMessage = e.ToString();
                    return false;
                }

                // ******************************************
                // More error checking logic goes here!
                // ******************************************
                ChessPiece copyOfPieceToMove = chessPieces[from];

                //checks to see if promotion
                //input string formatted wrong bug?
                if (copyOfPieceToMove.KindOfPiece.Equals("PAWN") && (toY == 0) || (toY == 7))
                {
                    errorMessage = promotePawn(promotedPiece, from);
                    if (errorMessage.Length > 0)
                    {
                        return false;
                    }
                    // Redo copy with new promoted piece
                    copyOfPieceToMove = chessPieces[from];
                }

                //if pawn moved in enpassant then remove piece behind it
                if (copyOfPieceToMove.KindOfPiece.Equals("PAWN") && ((PAWN)copyOfPieceToMove).enPassantCheck(this, from, to))
                {
                    copyOfPieceToMove.hasMoved = true;
                    String behindPawnEnPassantPiece;
                    //black you are coming "up" the board
                    if (copyOfPieceToMove.Color.Equals("B")) { behindPawnEnPassantPiece = "" + (toX) + ":" + (toY - 1); }
                    //white you are going "down" the board
                    else { behindPawnEnPassantPiece = "" + (toX) + ":" + (toY + 1); }
                    //remove pawn behind it
                    chessPieces.Remove(behindPawnEnPassantPiece);
                    //remove place where it was from
                    chessPieces.Remove(from);
                    //put it in new to location
                    chessPieces.Add(to, copyOfPieceToMove);
                    //flip
                    currentColorsTurn = FlipFlopColor(currentColorsTurn);
                    //end turn
                    return true;
                }

                // Ok ... from here on out I assume all is well.
                // For normal moves (not enpassant)

                // if you moved a king we update where it is on the board for easy reference for check
                if (copyOfPieceToMove.KindOfPiece.Equals("KING"))
                {
                    if (copyOfPieceToMove.Equals("B")) { blackKingsPlace = to; }
                    else { whiteKingsPlace = to; }
                }

                copyOfPieceToMove.hasMoved = true;
                if (chessPieces.ContainsKey(to))
                {
                    // This piece needs removed from the board
                    chessPieces.Remove(to);
                }
                // If we moved it then it's now gone from the other location aye?
                chessPieces.Remove(from);


                // Now add our piece at the new location
                chessPieces.Add(to, copyOfPieceToMove);

                // All is good ... now flip flop turn colors
                currentColorsTurn = FlipFlopColor(currentColorsTurn);
                return true;
            }
        }

        //Possibly something wrong here
        private string promotePawn(string promotedPiece, string from)
        {
            if (promotedPiece != null)
            {
                switch (promotedPiece)
                {
                    case "QUEEN":
                        chessPieces[from] = new QUEEN(currentColorsTurn);
                        break;
                    case "ROOK":
                        chessPieces[from] = new ROOK(currentColorsTurn);
                        break;
                    case "KNIGHT":
                        chessPieces[from] = new KNIGHT(currentColorsTurn);
                        break;
                    case "BISHOP":
                        chessPieces[from] = new BISHOP(currentColorsTurn);
                        break;
                    default:
                        return "Unknown promotion piece type";
                }
            }
            else
            {
                // We have to have a promoted piece!
                return "You cannot move a PAWN to the final row without specifying a promotion piece type";
            }

            return "";
        }

        private void checkForEnPassant(int toX, int toY, int fromY, ChessPiece copyOfPieceToMove)
        {

            // Possible En passant?
            // The only way En passant can be is if we're moving a pawn to the 2 or 5 row
            // as that is the only row behind a pawn that jumped two spaces
            string testX = "X", testY = "Y";
            switch (toY)
            {
                case 2:
                    // See if 3 is a pawn that just finished jumping two spaces
                    testX = toX.ToString();
                    testY = "3";
                    break;
                case 5:
                    // See if 6 is a pawn that just finished jumping two spaces
                    testX = toX.ToString();
                    testY = "4";
                    break;
                default:
                    break;
            }
            string testLocation = testX + ":" + testY;
            if (chessPieces.ContainsKey(testLocation))
            {
                var enpassantCheck = chessPieces[testLocation];
                if (enpassantCheck.KindOfPiece.Equals("PAWN"))
                {
                    if (((PAWN)enpassantCheck).allowEnPassant)
                    {
                        // This was it!
                        chessPieces.Remove(testLocation);
                    }
                }
            }
        }

        public static string FlipFlopColor(string color)
        {
            return color.Equals("W") ? "B" : "W";
        }

        private void initializeBoard()
        {
            currentColorsTurn = "W";

            chessPieces = new Dictionary<string, ChessPiece>();
            chessPieces.Add("0:0", new ROOK("B"));
            chessPieces.Add("1:0", new KNIGHT("B"));
            chessPieces.Add("2:0", new BISHOP("B"));
            chessPieces.Add("4:0", new KING("B"));
            chessPieces.Add("3:0", new QUEEN("B"));
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
            chessPieces.Add("4:7", new KING("W"));
            chessPieces.Add("3:7", new QUEEN("W"));
            chessPieces.Add("5:7", new BISHOP("W"));
            chessPieces.Add("6:7", new KNIGHT("W"));
            chessPieces.Add("7:7", new ROOK("W"));
        }

        private void initializePromotePawn()
        {
            currentColorsTurn = "W";

            chessPieces = new Dictionary<string, ChessPiece>();
            chessPieces.Add("0:0", new ROOK("B"));
            chessPieces.Add("1:0", new KNIGHT("B"));
            chessPieces.Add("2:0", new BISHOP("B"));
            chessPieces.Add("4:0", new KING("B"));
            chessPieces.Add("3:0", new QUEEN("B"));
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
            chessPieces.Add("7:1", new PAWN("W"));
            chessPieces.Add("0:6", new PAWN("B"));
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
            chessPieces.Add("4:7", new KING("W"));
            chessPieces.Add("3:7", new QUEEN("W"));
            chessPieces.Add("5:7", new BISHOP("W"));
            chessPieces.Add("6:7", new KNIGHT("W"));
            chessPieces.Add("7:7", new ROOK("W"));
        }

        public Dictionary<string, ChessPiece> getChessPieces()
        {
            return chessPieces;
        }

        public ChessBoard clone()
        {
            ChessBoard cloneBoard = new ChessBoard();
            foreach (var piece in this.chessPieces)
            {
                cloneBoard.chessPieces[piece.Key] = piece.Value;
            }
            return cloneBoard;
        }
    }
}