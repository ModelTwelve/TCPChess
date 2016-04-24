using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessHelpers {

    public interface IChessPiece {
        
    }    
    public abstract class ChessPiece : IChessPiece {
        public string KindOfPiece { get; protected set; }
        public string Color { get; protected set; }
        public bool hasMoved { get; internal set; }
        abstract public LinkedList<String> generatePossibleMoves(ChessBoard chessBoard, String from);
        abstract public String formatMoves(int toX, int toY);
    }
    public class ROOK : ChessPiece {
        public ROOK(string color) {
            KindOfPiece = "ROOK";
            Color = color;
            hasMoved = false;
        }

        public override string formatMoves(int toX, int toY)
        {
            throw new NotImplementedException();
        }

        public override LinkedList<String> generatePossibleMoves(ChessBoard chessBoard, String from)
        {
            throw new NotImplementedException();
        }
    }
    public class KNIGHT : ChessPiece {
        public KNIGHT(string color) {
            KindOfPiece = "KNIGHT";
            Color = color;
            hasMoved = false;
        }

        public override string formatMoves(int toX, int toY)
        {
            throw new NotImplementedException();
        }

        public override LinkedList<String> generatePossibleMoves(ChessBoard chessBoard, String from)
        {
            throw new NotImplementedException();
        }
    }
    public class BISHOP : ChessPiece {
        public BISHOP(string color) {
            KindOfPiece = "BISHOP";
            Color = color;
            hasMoved = false;
        }

        public override string formatMoves(int toX, int toY)
        {
            throw new NotImplementedException();
        }

        public override LinkedList<String> generatePossibleMoves(ChessBoard chessBoard, String from)
        {
            throw new NotImplementedException();
        }
    }
    public class KING : ChessPiece {
        public KING(string color) {
            KindOfPiece = "KING";
            Color = color;
            hasMoved = false;
        }

        public override string formatMoves(int toX, int toY)
        {
            throw new NotImplementedException();
        }

        public override LinkedList<String> generatePossibleMoves(ChessBoard chessBoard, String from)
        {
            throw new NotImplementedException();
        }
    }
    public class QUEEN : ChessPiece {
        public QUEEN(string color) {
            KindOfPiece = "QUEEN";
            Color = color;
            hasMoved = false;
        }

        public override string formatMoves(int toX, int toY)
        {
            throw new NotImplementedException();
        }

        public override LinkedList<String> generatePossibleMoves(ChessBoard chessBoard, String from)
        {

            throw new NotImplementedException();
        }
    }
    public class PAWN : ChessPiece {
        public bool allowEnPassant = false;
        public PAWN(string color) {
            KindOfPiece = "PAWN";
            Color = color;
            hasMoved = false;
        }

        public override string formatMoves(int toX, int toY)
        {
            throw new NotImplementedException();
        }

        public override LinkedList<String> generatePossibleMoves(ChessBoard chessBoard, String from)
        {
            LinkedList<String> moveList = new LinkedList<string>();
            String color = this.Color;
            //convert to int
            string[] split = from.Split(':');
            int fromX = Convert.ToInt32(split[0]);
            int fromY = Convert.ToInt32(split[1]);
            //where it will be going
            String goTo;

            
                //subtract Y axis for white
                //check if enemy piece at diagonal right
                if (color.Equals("W")){ goTo = "" + (fromX + 1) + ":" + (fromY - 1);}
                else{ goTo = "" + (fromX + 1) + ":" + (fromY + 1); }
                if(chessBoard.getChessPieces().ContainsKey(goTo) && chessBoard.getChessPieces()[goTo].Color.Equals("B"))
                {
                    moveList.AddLast(goTo);
                }
                //check if enemy piece at diagonal left
                if (color.Equals("W")) { goTo = "" + (fromX - 1) + ":" + (fromY - 1); }
                 else { goTo = "" + (fromX - 1) + ":" + (fromY + 1); }
               
                if (chessBoard.getChessPieces().ContainsKey(goTo) && chessBoard.getChessPieces()[goTo].Color.Equals("B"))
                {
                    moveList.AddLast(goTo);
                }
            //check if no piece in front

                if (color.Equals("W")) { goTo = "" + (fromX) + ":" + (fromY - 1); }
                else { goTo = "" + (fromX) + ":" + (fromY + 1); }
                if (!chessBoard.getChessPieces().ContainsKey(goTo))
                {
                    moveList.AddLast(goTo);
                //if hasn't moved check if front ok, then check next
                    if (color.Equals("W")) { goTo = "" + (fromX) + ":" + (fromY - 2); }
                    else { goTo = "" + (fromX) + ":" + (fromY + 2); }
                        if (!chessBoard.getChessPieces().ContainsKey(goTo) && hasMoved == false)
                        {
                        moveList.AddLast(goTo);
                        }
                }
                
            return moveList;
        }
    }
}
