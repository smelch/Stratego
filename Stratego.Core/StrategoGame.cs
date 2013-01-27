﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Stratego.Core
{
    public class StrategoGame
    {
        private GameState state;
        private PieceBank bank;

        public StrategoGame()
        {
            bank = new PieceBank();
            bank.Initialize();
            state = new GameState();
            state.Board[2, 4] = GamePiece.Block;
            state.Board[2, 5] = GamePiece.Block;
            state.Board[3, 4] = GamePiece.Block;
            state.Board[3, 5] = GamePiece.Block;
            state.Board[6, 4] = GamePiece.Block;
            state.Board[6, 5] = GamePiece.Block;
            state.Board[7, 4] = GamePiece.Block;
            state.Board[7, 5] = GamePiece.Block;
        }

        public StrategoGame(GameState state)
        {
            this.state = state;
        }

        public bool PlacePiece(GamePiece Piece, Point p)
        {
            if (state.Turn == PlayerTurn.Setup)
            {
                if (Piece.IsBlue())
                {
                    if (p.y > 3)
                    {
                        return false;
                    }  
                } else {
                    if(p.y < 6) {
                        return false;
                    }
                }
                
                if(state.Board[p.x, p.y] == 0 && bank.PlacePiece(Piece))
                {
                    state.Board[p.x, p.y] = Piece;
                    return true;
                }    

            }
           
             return false;
        }

        public bool RemovePiece(Point p) {
            var Piece = GetPiece(p);
            state.Board[p.x, p.y] = GamePiece.Empty;
            bank.ReturnPiece(Piece);
            return true;
        }


        public bool Move(Point from, Point to) 
        {
            PlayerTurn turn = GetTurn();
            GamePiece piece = GetPiece(from);
            GamePiece destinationPiece = GetPiece(to);

            var PieceType = piece.GetPieceType();

            if (IsValidMove(piece, from, to))
            {
                state.Board[from.x, from.y] = 0;
                if (destinationPiece != 0)
                {
                    throw new Exception("Destination is occupied.");
                }
                state.Board[to.x, to.y] = piece;
                state.Turn = (state.Turn == PlayerTurn.Red) ? PlayerTurn.Blue : PlayerTurn.Red;
                return true;
            }

            return false;
        }

        public GamePiece Attack(Point from, Point to)
        {

            var defender = GetPiece(to);
            var attacker = GetPiece(from);

            if (IsValidMove(attacker, from, to))
            {
                bool? success = null;
                
                if (defender.GetPieceType() == GamePiece.Bomb) { success = attacker.GetPieceType() == GamePiece.Eight; }
                else if (attacker.GetPieceType() == GamePiece.Spy) { success = true; }
                else if (defender.GetPieceType() == GamePiece.Flag) { success = true; }
                else if (defender.GetPieceType() == GamePiece.Spy) { success = true; }

                if ((success.HasValue && success.Value) || (!success.HasValue && attacker.GetPieceType() < defender.GetPieceType()))
                {
                    if (defender.GetPieceType() == GamePiece.Flag)
                    {
                        IsOver = true;
                    }
                    RemovePiece(to);
                    state.Board[from.x, from.y] = 0;
                    state.Board[to.x, to.y] = attacker;
                }
                else
                {
                    RemovePiece(from);
                    if (!success.HasValue && attacker.GetPieceType() == defender.GetPieceType())
                    {
                        RemovePiece(to);
                    }
                }
                state.Turn = (state.Turn == PlayerTurn.Red) ? PlayerTurn.Blue : PlayerTurn.Red;
                return defender;
            }
            else
            {
                return (GamePiece)0;
            }
        }

        public bool IsValidMove(GamePiece Piece, Point from, Point to)
        {
            if (this.IsOver)
            {
                return false;
            }

            if (to.x < 0 || to.x > 9 || to.y < 0 || to.y > 9)
            {
                return false;
            }

            int deltaX = to.x - from.x;
            int deltaY = to.y - from.y;

            if (Piece.IsRed() && state.Turn != PlayerTurn.Red || !Piece.IsRed() && state.Turn == PlayerTurn.Red)
            {
                return false;
            }

            if (Piece.GetPieceType() == GamePiece.Bomb || Piece.GetPieceType() == GamePiece.Flag || Piece == GamePiece.Block)
            {
                return false;
            }

            if (GetPiece(to) != GamePiece.Empty && GetPiece(to).IsRed() == Piece.IsRed())
            {
                return false;
            }

            if (deltaX != 0 && deltaY != 0)
            {
                return false;
            }

            //Nine Movement
            if (Piece.GetPieceType() == GamePiece.Nine)
            {
                Point offset = new Point(Math.Sign(deltaX), Math.Sign(deltaY));

                while ((from += offset) != to)
                {
                    if (GetPiece(from) != 0)
                    {
                        return false;
                    }
                }
            }
            else
            {
                //Check for multiple move
                if (Math.Abs(deltaX) > 1 || Math.Abs(deltaY) > 1)
                {
                    return false;
                }
            }
            
            GamePiece destination = GetPiece(to);
            if (destination == GamePiece.Empty)
            {
                return true;
            }
            if (destination == GamePiece.Block || destination.IsRed() == Piece.IsRed())
            {
                return false;
            }

            return true;
        }

        public GamePiece GetPiece(Point p)
        {
            return state.Board[p.x, p.y];
        }

        public int GetAvailablePieces(GamePiece piece)
        {
            return bank.PieceCount(piece);
        }

        public void EndSetup()
        {
            state.Turn = PlayerTurn.Red;
        }

        public PlayerTurn GetTurn()
        {
            return state.Turn;
        }

        public List<KeyValuePair<GamePiece, int>> GetAvailablePieces(bool red)
        {
            return bank.GetAllAvailablePieces(red);
        }

        public void SaveBinary(Stream stream)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            bf.Serialize(stream, this.state);
        }

        public void SaveXml(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameState));
            serializer.Serialize(stream, this.state);
        }

        public void Load(GameState state)
        {
            this.state = state;
        }

        public GamePiece[,] GetBoard()
        {
            return (GamePiece[,])state.Board.Clone();
        }

        public bool IsOver { get; set; }
    }
}
