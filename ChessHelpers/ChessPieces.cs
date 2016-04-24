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
        abstract public LinkedList<ChessPiece> generatePossibleMoves(ChessPiece piece);
    }
    public class ROOK : ChessPiece {
        public ROOK(string color) {
            KindOfPiece = "ROOK";
            Color = color;
        }

        public override LinkedList<ChessPiece> generatePossibleMoves(ChessPiece piece)
        {
            throw new NotImplementedException();
        }
    }
    public class KNIGHT : ChessPiece {
        public KNIGHT(string color) {
            KindOfPiece = "KNIGHT";
            Color = color;
        }

        public override LinkedList<ChessPiece> generatePossibleMoves(ChessPiece piece)
        {
            throw new NotImplementedException();
        }
    }
    public class BISHOP : ChessPiece {
        public BISHOP(string color) {
            KindOfPiece = "BISHOP";
            Color = color;
        }

        public override LinkedList<ChessPiece> generatePossibleMoves(ChessPiece piece)
        {
            throw new NotImplementedException();
        }
    }
    public class KING : ChessPiece {
        public KING(string color) {
            KindOfPiece = "KING";
            Color = color;
        }

        public override LinkedList<ChessPiece> generatePossibleMoves(ChessPiece piece)
        {
            throw new NotImplementedException();
        }
    }
    public class QUEEN : ChessPiece {
        public QUEEN(string color) {
            KindOfPiece = "QUEEN";
            Color = color;
        }

        public override LinkedList<ChessPiece> generatePossibleMoves(ChessPiece piece)
        {
            throw new NotImplementedException();
        }
    }
    public class PAWN : ChessPiece {
        public bool allowEnPassant = false;
        public PAWN(string color) {
            KindOfPiece = "PAWN";
            Color = color;
        }

        public override LinkedList<ChessPiece> generatePossibleMoves(ChessPiece piece)
        {
            throw new NotImplementedException();
        }
    }
}
