﻿using System;
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
    }
    public class ROOK : ChessPiece {
        public ROOK(string color) {
            KindOfPiece = "ROOK";
            Color = color;
            hasMoved = false;
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
            bool right = true;
            bool left = true;
            bool down = true;
            bool up = true;

            int rightX = fromX, leftX = fromX;
            int downY = fromY, upY = fromY;
            while (right || left || down || up)
            {
                if (right == true)
                {
                    goTo = "" + (++rightX) + ":" + (fromY);

                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightX <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        right = false;
                    }

                }
                if (left == true)
                {
                    goTo = "" + (--leftX) + ":" + (fromY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftX >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        left = false;
                    }

                }
                if (down == true)
                {
                    goTo = "" + (fromX) + ":" + (++downY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (downY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        down = false;
                    }
                }
                if (up == true)
                {
                    goTo = "" + (fromX) + ":" + (--upY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (upY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        up = false;
                    }
                }

            }

            return moveList;


        }

        public String oppositeColor()
        {
            if (this.Color.Equals("W"))
            {
                return "B";
            }
            return "W";
        }

    }
    public class KNIGHT : ChessPiece {
        public KNIGHT(string color) {
            KindOfPiece = "KNIGHT";
            Color = color;
            hasMoved = false;
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
            
            //2 up 1 right
            goTo = "" + (fromX + 1) + ":" + (fromY + 2);

            if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && ((fromX + 1) <= 7 && (fromY + 2) <= 7))
            {
                moveList.AddLast(goTo);
            }
            //2 up 1 left
            goTo = "" + (fromX - 1) + ":" + (fromY + 2);

            if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && ((fromX - 1) >= 0 && (fromY + 2) <= 7))
            {
                moveList.AddLast(goTo);
            }
            //1 up 2 right
            goTo = "" + (fromX + 2) + ":" + (fromY + 1);

            if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && ((fromX + 2) <= 7 && (fromY + 1) <= 7))
            {
                moveList.AddLast(goTo);
            }
            //1 up 2 left
            goTo = "" + (fromX - 2) + ":" + (fromY + 1);

            if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && ((fromX - 2) >= 0 && (fromY + 1) <= 7))
            {
                moveList.AddLast(goTo);
            }
            //2 down 1 right
            goTo = "" + (fromX + 1) + ":" + (fromY - 2);

            if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && ((fromX + 1) <= 7 && (fromY - 2) >= 0))
            {
                moveList.AddLast(goTo);
            }
            //2 down 1 left
            goTo = "" + (fromX - 1) + ":" + (fromY - 2);

            if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && ((fromX - 1) >= 0 && (fromY - 2) >= 0))
            {
                moveList.AddLast(goTo);
            }
            //1 down 2 right
            goTo = "" + (fromX + 2) + ":" + (fromY - 1);

            if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && ((fromX + 2) <= 7 && (fromY - 1) >= 0))
            {
                moveList.AddLast(goTo);
            }
            //1 down 2 left
            goTo = "" + (fromX - 2) + ":" + (fromY - 1);

            if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && ((fromX - 2) >= 0 && (fromY - 1) >= 0))
            {
                moveList.AddLast(goTo);
            }

            return moveList;
        }

        public String oppositeColor()
        {
            if (this.Color.Equals("W"))
            {
                return "B";
            }
            return "W";
        }

    }

    public class BISHOP : ChessPiece {
        public BISHOP(string color) {
            KindOfPiece = "BISHOP";
            Color = color;
            hasMoved = false;
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
            bool rightFrontDiag = true;
            bool leftFrontDiag = true;
            bool rightBackDiag = true;
            bool leftBackDiag = true;

            int rightFrontX = fromX, leftFrontX = fromX, rightBackX = fromX, leftBackX = fromX;
            int rightFrontY = fromY, leftFrontY = fromY, rightBackY = fromY, leftBackY = fromY;
            while (rightBackDiag || leftBackDiag || leftFrontDiag || rightFrontDiag)
            {
                //chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())
                if (rightFrontDiag == true)
                {
                    goTo = "" + (++rightFrontX) + ":" + (--rightFrontY);
                   
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightFrontX <= 7 && rightFrontY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }else
                    {
                        rightFrontDiag = false;
                    }

                }
                if (leftFrontDiag == true)
                {
                    goTo = "" + (--leftFrontX) + ":" + (--leftFrontY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftFrontX >= 0 && leftFrontY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }else
                    {
                        leftFrontDiag = false;
                    }

                }
                if (rightBackDiag == true)
                {
                    goTo = "" + (++rightBackX) + ":" + (++rightBackY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightBackX <= 7 && rightBackY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        rightBackDiag = false;
                    }
                }
                if (leftBackDiag == true)
                {
                    goTo = "" + (--leftBackX) + ":" + (++leftBackY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftBackX >= 0 && leftBackY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }else
                    {
                        leftBackDiag = false;
                    }
                }

            }

            return moveList;
            
            
        }

        public String oppositeColor()
        {
            if (this.Color.Equals("W"))
            {
                return "B";
            }
            return "W";
        }

    }
    public class KING : ChessPiece {
        public KING(string color) {
            KindOfPiece = "KING";
            Color = color;
            hasMoved = false;
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
            
            int rightX = fromX, leftX = fromX;
            int downY = fromY, upY = fromY;
            
            int rightFrontX = fromX, leftFrontX = fromX, rightBackX = fromX, leftBackX = fromX;
            int rightFrontY = fromY, leftFrontY = fromY, rightBackY = fromY, leftBackY = fromY;

            //get horizontal moves
            int i = 0;
            while (i<1)
            {
                    goTo = "" + (++rightX) + ":" + (fromY);

                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightX <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    
                      
                    goTo = "" + (--leftX) + ":" + (fromY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftX >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    
              
                    goTo = "" + (fromX) + ":" + (++downY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (downY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                   
                
                    goTo = "" + (fromX) + ":" + (--upY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (upY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    
                ++i;

            }

            //getting diagonal moves
            i = 0;
            while (i<1)
            {
                //chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())
               
                    goTo = "" + (++rightFrontX) + ":" + (--rightFrontY);

                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightFrontX <= 7 && rightFrontY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    
                
                    goTo = "" + (--leftFrontX) + ":" + (--leftFrontY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftFrontX >= 0 && leftFrontY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    
               
                    goTo = "" + (++rightBackX) + ":" + (++rightBackY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightBackX <= 7 && rightBackY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    
                
                    goTo = "" + (--leftBackX) + ":" + (++leftBackY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftBackX >= 0 && leftBackY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                ++i;
                  

            }
            return moveList;


        }

        public String oppositeColor()
        {
            if (this.Color.Equals("W"))
            {
                return "B";
            }
            return "W";
        }
    }
    public class QUEEN : ChessPiece {
        public QUEEN(string color) {
            KindOfPiece = "QUEEN";
            Color = color;
            hasMoved = false;
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
            bool right = true;
            bool left = true;
            bool down = true;
            bool up = true;

            int rightX = fromX, leftX = fromX;
            int downY = fromY, upY = fromY;

            bool rightFrontDiag = true;
            bool leftFrontDiag = true;
            bool rightBackDiag = true;
            bool leftBackDiag = true;

            int rightFrontX = fromX, leftFrontX = fromX, rightBackX = fromX, leftBackX = fromX;
            int rightFrontY = fromY, leftFrontY = fromY, rightBackY = fromY, leftBackY = fromY;

            //get horizontal moves
            while (right || left || down || up)
            {
                if (right == true)
                {
                    goTo = "" + (++rightX) + ":" + (fromY);

                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightX <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        right = false;
                    }

                }
                if (left == true)
                {
                    goTo = "" + (--leftX) + ":" + (fromY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftX >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        left = false;
                    }

                }
                if (down == true)
                {
                    goTo = "" + (fromX) + ":" + (++downY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (downY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        down = false;
                    }
                }
                if (up == true)
                {
                    goTo = "" + (fromX) + ":" + (--upY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (upY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        up = false;
                    }
                }

            }

            //getting diagonal moves
            
            while (rightBackDiag || leftBackDiag || leftFrontDiag || rightFrontDiag)
            {
                //chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())
                if (rightFrontDiag == true)
                {
                    goTo = "" + (++rightFrontX) + ":" + (--rightFrontY);

                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightFrontX <= 7 && rightFrontY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        rightFrontDiag = false;
                    }

                }
                if (leftFrontDiag == true)
                {
                    goTo = "" + (--leftFrontX) + ":" + (--leftFrontY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftFrontX >= 0 && leftFrontY >= 0))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        leftFrontDiag = false;
                    }

                }
                if (rightBackDiag == true)
                {
                    goTo = "" + (++rightBackX) + ":" + (++rightBackY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (rightBackX <= 7 && rightBackY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        rightBackDiag = false;
                    }
                }
                if (leftBackDiag == true)
                {
                    goTo = "" + (--leftBackX) + ":" + (++leftBackY);
                    if ((!chessBoard.getChessPieces().ContainsKey(goTo) || chessBoard.getChessPieces()[goTo].Color.Equals(oppositeColor())) && (leftBackX >= 0 && leftBackY <= 7))
                    {
                        moveList.AddLast(goTo);
                    }
                    else
                    {
                        leftBackDiag = false;
                    }
                }

            }
            return moveList;


        }

        public String oppositeColor()
        {
            if (this.Color.Equals("W"))
            {
                return "B";
            }
            return "W";
        }
    }
    public class PAWN : ChessPiece {
        public bool allowEnPassant = false;
        public PAWN(string color) {
            KindOfPiece = "PAWN";
            Color = color;
            hasMoved = false;
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
            var pieces = chessBoard.getChessPieces();
            try
            {
                //Checking for EnPassant
                if (this.Color.Equals("B"))
                {
                    //If pawn directly right 
                    goTo = "" + (fromX + 1) + ":" + (fromY);
                    //var pieces = chessBoard.getChessPieces();
                    if ((pieces.ContainsKey(goTo)) && pieces[goTo].KindOfPiece.Equals("PAWN") && ((PAWN)pieces[goTo]).allowEnPassant)
                    {
                        moveList.AddLast("" + (fromX + 1) + ":" + (fromY + 1));
                    }
                    //If pawn directly left 
                    goTo = "" + (fromX - 1) + ":" + (fromY);
                    if ((pieces.ContainsKey(goTo)) && pieces[goTo].KindOfPiece.Equals("PAWN") && ((PAWN)pieces[goTo]).allowEnPassant)
                    {
                        moveList.AddLast("" + (fromX - 1) + ":" + (fromY + 1));
                    }
                }
                else
                {
                    //is white
                    //If pawn directly right 
                    goTo = "" + (fromX + 1) + ":" + (fromY);
                    //var pieces = chessBoard.getChessPieces();
                    if ( (pieces.ContainsKey(goTo)) && (pieces[goTo].KindOfPiece.Equals("PAWN") && ((PAWN)pieces[goTo]).allowEnPassant) )
                    {
                        moveList.AddLast("" + (fromX + 1) + ":" + (fromY - 1));
                    }
                    //If pawn directly left 
                    goTo = "" + (fromX - 1) + ":" + (fromY);
                    if ((pieces.ContainsKey(goTo)) && (pieces[goTo].KindOfPiece.Equals("PAWN") && ((PAWN)pieces[goTo]).allowEnPassant) )
                    {
                        moveList.AddLast("" + (fromX - 1) + ":" + (fromY - 1));
                    }
                }
            }
            catch(Exception e)
            {
                return new LinkedList<string>();
            }

            //var pieces = chessBoard.getChessPieces();
            //check if white pawn has enemy piece at diagonal right
            if (color.Equals("W")){ goTo = "" + (fromX + 1) + ":" + (fromY - 1);}
            //check if black pawn has enemy piece at diagonal right
            else { goTo = "" + (fromX + 1) + ":" + (fromY + 1); }
            if (pieces.ContainsKey(goTo) && pieces[goTo].Color.Equals(oppositeColor()))
            {
                moveList.AddLast(goTo);
            }
            //check if white pawn has enemy piece at diagonal left
            if (color.Equals("W")) { goTo = "" + (fromX - 1) + ":" + (fromY - 1); }
            //check if black pawn has enemy piece at diagonal left
            else { goTo = "" + (fromX - 1) + ":" + (fromY + 1); }
            if (pieces.ContainsKey(goTo) && pieces[goTo].Color.Equals(oppositeColor()))
            {
                moveList.AddLast(goTo);
            }
            //check if no piece in front

                if (color.Equals("W")) { goTo = "" + (fromX) + ":" + (fromY - 1); }
                else { goTo = "" + (fromX) + ":" + (fromY + 1); }
                if (!pieces.ContainsKey(goTo))
                {
                    moveList.AddLast(goTo);
                //if hasn't moved check if front ok, then check next
                    if (color.Equals("W")) { goTo = "" + (fromX) + ":" + (fromY - 2); }
                    else { goTo = "" + (fromX) + ":" + (fromY + 2); }
                        if (!pieces.ContainsKey(goTo) && hasMoved == false)
                        {
                        moveList.AddLast(goTo);
                        }
                }
                
            return moveList;
        }

        public String oppositeColor()
        {
            if (this.Color.Equals("W"))
            {
                return "B";
            }
            return "W";
        }


        public bool enPassantCheck(ChessBoard chessBoard, String from, String to)
        {
            LinkedList<String> enPassantList = new LinkedList<string>();
            string[] split = from.Split(':');
            int fromX = Convert.ToInt32(split[0]);
            int fromY = Convert.ToInt32(split[1]);
            //where it will be going
            String goTo;
            var pieces = chessBoard.getChessPieces();
            
                //Checking for EnPassant
                if (this.Color.Equals("B"))
                {
                    //If pawn directly right 
                    goTo = "" + (fromX + 1) + ":" + (fromY);
                    //var pieces = chessBoard.getChessPieces();
                    if ((pieces.ContainsKey(goTo)) && pieces[goTo].KindOfPiece.Equals("PAWN") && ((PAWN)pieces[goTo]).allowEnPassant)
                    {
                        enPassantList.AddLast("" + (fromX + 1) + ":" + (fromY + 1));
                    }
                    //If pawn directly left 
                    goTo = "" + (fromX - 1) + ":" + (fromY);
                    if ((pieces.ContainsKey(goTo)) && pieces[goTo].KindOfPiece.Equals("PAWN") && ((PAWN)pieces[goTo]).allowEnPassant)
                    {
                        enPassantList.AddLast("" + (fromX - 1) + ":" + (fromY + 1));
                    }
                }
                else
                {
                    //is white
                    //If pawn directly right 
                    goTo = "" + (fromX + 1) + ":" + (fromY);
                    //var pieces = chessBoard.getChessPieces();
                    if ((pieces.ContainsKey(goTo)) && (pieces[goTo].KindOfPiece.Equals("PAWN") && ((PAWN)pieces[goTo]).allowEnPassant))
                    {
                        enPassantList.AddLast("" + (fromX + 1) + ":" + (fromY - 1));
                    }
                    //If pawn directly left 
                    goTo = "" + (fromX - 1) + ":" + (fromY);
                    if ((pieces.ContainsKey(goTo)) && (pieces[goTo].KindOfPiece.Equals("PAWN") && ((PAWN)pieces[goTo]).allowEnPassant))
                    {
                        enPassantList.AddLast("" + (fromX - 1) + ":" + (fromY - 1));
                    }
                }

            if (enPassantList.Contains(to))
            {
                return true;
            }
            return false;
          }
            
        }
    }

