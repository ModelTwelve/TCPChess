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
            initializeCastleScenario();
            //initializeCheckmate();
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

        public LinkedList<string> getPossible(string playerColorAttemptingToMove, string from, out string errorMessage)
        {
            errorMessage = "";
            // Just verify that it's really their piece to move and that something
            // really does exist there
            if (!chessPieces.ContainsKey(from))
            {
                errorMessage = "No piece on the board at that location";
                return null;
            }
            if (!chessPieces[from].Color.Equals(playerColorAttemptingToMove))
            {
                errorMessage = "You cannot move your opponents piece";
                return null;
            }
            return chessPieces[from].generatePossibleMoves(this, from); ;
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
                LinkedList<string> check = checkMoves.generatePossibleMoves(this, from);

                if (!check.Contains(to))
                {
                    errorMessage = "A " + chessPieces[from].KindOfPiece + " cant move there!";
                    return false;
                }
                bool maybeCheck = true;
                try
                {
                    maybeCheck = isInCheck(check, checkMoves, from, to);
                }
                catch (Exception e)
                {

                }

                //it is in check
                if (!maybeCheck)
                {
                    //is it checkmate?
                    if (isCheckMate())
                    {
                        errorMessage = "WINNER";
                        return false;
                    }
                    errorMessage = "You are in check!";
                    return false;
                }

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

                // Needed to move this down below the legal moves as it was setting
                // allowEnPassant even when the PAWN was denied a two place move
                if (chessPieces[from].KindOfPiece.Equals("PAWN"))
                {
                    int numSpaces = Math.Abs(fromY - toY);
                    if (numSpaces == 2)
                    {
                        ((PAWN)chessPieces[from]).allowEnPassant = true;
                    }
                }
                //if you have moved one square at any point
                if (chessPieces[from].KindOfPiece.Equals("PAWN"))
                {
                    int numSpaces = Math.Abs(fromY - toY);
                    if (numSpaces == 1)
                    {
                        ((PAWN)chessPieces[from]).allowEnPassant = false;
                    }
                }

                ChessPiece copyOfPieceToMove = chessPieces[from];

                //checks to see if promotion
                if (copyOfPieceToMove.KindOfPiece.Equals("PAWN") && ((toY == 0) || (toY == 7)))
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

                // Enpassant only lasts for one move
                foreach (var somePiece in chessPieces)
                {
                    if (somePiece.Value.KindOfPiece.Equals("PAWN"))
                    {
                        // We're a PAWN ... are we the opposite color?
                        if (!somePiece.Value.Color.Equals(currentColorsTurn))
                        {
                            // Turn off enpassant
                            ((PAWN)somePiece.Value).allowEnPassant = false;
                        }
                    }
                }

                //castle
                if (copyOfPieceToMove.KindOfPiece.Equals("KING") && ((KING)(copyOfPieceToMove)).castleCheck(this, from, to))
                {
                    ChessPiece rookCastle = new ROOK(this.currentColorsTurn);//we will put rook in right place, remove old one
                    rookCastle.hasMoved = true;
                    copyOfPieceToMove.hasMoved = true;
                    String rookMove = ((KING)copyOfPieceToMove).rookCastleMovePlace(from, to);
                    string[] rookMoveArray = rookMove.Split('|');//split on pipe
                    String rookTo = rookMoveArray[0];//where our rook is going to (add)
                    String rookFrom = rookMoveArray[1];//where it is coming from (remove)
                    //remove place where it was from
                    chessPieces.Remove(from);//king
                    chessPieces.Remove(rookFrom);//rook
                    //put it in new to location
                    chessPieces.Add(rookTo, rookCastle);//rook
                    chessPieces.Add(to, copyOfPieceToMove);//king
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

        public bool isInCheck(LinkedList<string> check, ChessPiece checkMoves, String from, String to)
        {
            //we now have determined that this move is legal
            //checks to see if your potential move put your king into check or keeps your king in check
            string pos;
            string type;
            bool isInCheck = false;
            String potentialKingsPlace;
            if (currentColorsTurn.Equals("W"))
            {
                potentialKingsPlace = whiteKingsPlace;
                
            }
            //if black move check black king vs all white
            else
            {
                potentialKingsPlace = blackKingsPlace;
               
            }
            //create potential chess board
            ChessBoard potentialChessBoard = this.clone();
            ChessPiece copyOfPieceToMove = potentialChessBoard.getChessPieces()[from];
            if (copyOfPieceToMove.KindOfPiece.Equals("KING")) { 
                potentialKingsPlace = to;
            }
            copyOfPieceToMove.hasMoved = true;
            if (potentialChessBoard.chessPieces.ContainsKey(to))
            {
                // This piece needs removed from the board
                potentialChessBoard.chessPieces.Remove(to);
            }
            // If we moved it then it's now gone from the other location aye?
            potentialChessBoard.chessPieces.Remove(from);

            // Now add our piece at the new location
            potentialChessBoard.chessPieces.Add(to, copyOfPieceToMove);
            try
            {
                //checking mock board (with potential move)
                foreach (var piece in potentialChessBoard.chessPieces)
                {
                    //if piece isnt on your team
                    if (!piece.Value.Color.Equals(currentColorsTurn))
                    {
                        try
                        {
                            //returns list of that pieces avaiable moves
                            check = piece.Value.generatePossibleMoves(potentialChessBoard, piece.Key);
                        }catch
                        {

                        }

                        //if kings position is in that list then we set boolean to true return an error message

                        //if white move then check white king vs all black pieces
                        if (currentColorsTurn.Equals("W"))
                        {
                            isInCheck = check.Contains(potentialKingsPlace);
                            if (isInCheck)
                            {

                                return false;
                            }
                        }
                        //if black move check black king vs all white
                        else
                        {
                            isInCheck = check.Contains(potentialKingsPlace);
                            if (isInCheck)
                            {

                                return false;
                            }
                        }
                    }
                }
            }catch(Exception e)
            {

            }
           
            return true;
        }
        public bool isCheckMate()
        {
            //no possible move you can make that will take king out of check
            foreach (var piece in this.chessPieces)
            {
                //if piece is on your team
                if (piece.Value.Color.Equals(currentColorsTurn))
                {
                    //list of all possible moves for that piece
                    LinkedList<string> piecePossibleMoves = piece.Value.generatePossibleMoves(this, piece.Key);
                    //do any of these possible moves make king not in check
                    foreach(var pieceMove in piecePossibleMoves)
                    {
                        if(isInCheck(piecePossibleMoves, piece.Value, piece.Key, pieceMove))
                        {
                            return false;
                        }

                    }

                }
            }
            return true;
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

        private void initializeCastleScenario()
        {
            currentColorsTurn = "W";

            chessPieces = new Dictionary<string, ChessPiece>();
            chessPieces.Add("0:0", new ROOK("B"));
            chessPieces.Add("4:0", new KING("B"));
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
            chessPieces.Add("4:7", new KING("W"));
            chessPieces.Add("7:7", new ROOK("W"));
        }

        private void initializeCheckmate()
        {
            currentColorsTurn = "W";
            //all you gotta do is move the queen to 7:3
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
            chessPieces.Add("5:3", new PAWN("B"));
            chessPieces.Add("6:3", new PAWN("B"));
            chessPieces.Add("7:1", new PAWN("B"));
            chessPieces.Add("0:6", new PAWN("W"));
            chessPieces.Add("1:6", new PAWN("W"));
            chessPieces.Add("2:6", new PAWN("W"));
            chessPieces.Add("3:6", new PAWN("W"));
            chessPieces.Add("4:5", new PAWN("W"));
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
            cloneBoard.chessPieces.Clear();
            foreach (var piece in this.chessPieces)
            {
                cloneBoard.chessPieces[piece.Key] = piece.Value;
            }
            return cloneBoard;
        }
    }
}