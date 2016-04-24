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
        public bool IsMoved { get; protected set; }
    }
    public class ROOK : ChessPiece {
        public ROOK(string color) {
            KindOfPiece = "ROOK";
            Color = color;
        }
    }
    public class KNIGHT : ChessPiece {
        public KNIGHT(string color) {
            KindOfPiece = "KNIGHT";
            Color = color;
        }
    }
    public class BISHOP : ChessPiece {
        public BISHOP(string color) {
            KindOfPiece = "BISHOP";
            Color = color;
        }
    }
    public class KING : ChessPiece {
        public KING(string color) {
            KindOfPiece = "KING";
            Color = color;
        }
    }
    public class QUEEN : ChessPiece {
        public QUEEN(string color) {
            KindOfPiece = "QUEEN";
            Color = color;
        }
    }
    public class PAWN : ChessPiece {
        public PAWN(string color) {
            KindOfPiece = "PAWN";
            Color = color;
        }
    }
}
