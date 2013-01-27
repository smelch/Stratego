using System;
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

        public BasePlayer Red { get; set; }
        public BasePlayer Blue { get; set; }

        public StrategoGame()
        {
            bank = new PieceBank();
            bank.Initialize();
            state = new GameState();
            state.Board[2, 4] = GamePieceFactory.Create(GamePieceType.Block, false);
            state.Board[2, 5] = GamePieceFactory.Create(GamePieceType.Block, false);
            state.Board[3, 4] = GamePieceFactory.Create(GamePieceType.Block, false);
            state.Board[3, 5] = GamePieceFactory.Create(GamePieceType.Block, false);
            state.Board[6, 4] = GamePieceFactory.Create(GamePieceType.Block, false);
            state.Board[6, 5] = GamePieceFactory.Create(GamePieceType.Block, false);
            state.Board[7, 4] = GamePieceFactory.Create(GamePieceType.Block, false);
            state.Board[7, 5] = GamePieceFactory.Create(GamePieceType.Block, false);
        }

        public StrategoGame(GameState state)
        {
            this.state = state;
        }

        public bool PlacePiece(GamePieceType pieceType, bool red, Point p)
        {
            if (state.Turn == PlayerTurn.Setup)
            {
                if (red)
                {
                    if (p.y < 6)
                    {
                        return false;
                    }
                } else {
                    if (p.y > 3)
                    {
                        return false;
                    }  
                }
                
                if(state.Board[p.x, p.y] == null)
                {
                    var piece = bank.PlacePiece(pieceType, red);
                    if (piece != null)
                    {
                        state.Board[p.x, p.y] = piece;
                        return true;
                    }
                }    

            }
           
             return false;
        }

        public bool RemovePiece(Point p) {
            var Piece = GetPiece(p);
            state.Board[p.x, p.y] = null;
            bank.ReturnPiece(Piece);
            return true;
        }

        public bool Move(Point from, Point to) 
        {
            PlayerTurn turn = GetTurn();
            var piece = GetPiece(from);
            var destinationPiece = GetPiece(to);

            var PieceType = piece.Type;

            if (IsValidMove(piece, from, to))
            {
                state.Board[from.x, from.y] = null;
                if (destinationPiece != null)
                {
                    throw new Exception("Destination is occupied.");
                }
                state.Board[to.x, to.y] = piece;
                if (state.Turn == PlayerTurn.Red)
                {
                    state.Turn = PlayerTurn.Blue;
                    Blue.PlayerMoved(from, to);
                }
                else
                {
                    state.Turn = PlayerTurn.Red;
                    Red.PlayerMoved(from, to);
                }

                return true;
            }

            return false;
        }

        public AttackResult Attack(Point from, Point to)
        {

            var defender = GetPiece(to);
            var attacker = GetPiece(from);

            if (!IsValidMove(attacker, from, to))
            {
                throw new Exception("Invalid move");
            }

            var result = attacker.Attack(defender);
            switch (result)
            {
                case AttackResult.Win:
                    RemovePiece(to);
                    state.Board[from.x, from.y] = null;
                    state.Board[to.x, to.y] = attacker;
                    if (defender.Type == GamePieceType.Flag)
                    {
                        this.IsOver = true;
                    }
                    break;
                case AttackResult.Lose:
                    RemovePiece(from);
                    break;
                case AttackResult.Tie:
                    RemovePiece(from);
                    RemovePiece(to);
                    break;
            }

            if (state.Turn == PlayerTurn.Red)
            {
                state.Turn = PlayerTurn.Blue;
                Blue.PlayerAttacked(from, to, attacker.Type, result);
            }
            else
            {
                state.Turn = PlayerTurn.Red;
                Red.PlayerAttacked(from, to, attacker.Type, result);
            }
            return result;
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

            if (Piece.IsRed && state.Turn != PlayerTurn.Red || !Piece.IsRed && state.Turn == PlayerTurn.Red)
            {
                return false;
            }

            if (Piece.Type == GamePieceType.Bomb || Piece.Type == GamePieceType.Flag || Piece.Type == GamePieceType.Block)
            {
                return false;
            }

            if (GetPiece(to) != null && GetPiece(to).IsRed == Piece.IsRed)
            {
                return false;
            }

            if (deltaX != 0 && deltaY != 0)
            {
                return false;
            }

            //Nine Movement
            if (Piece.Type == GamePieceType.Nine)
            {
                Point offset = new Point(Math.Sign(deltaX), Math.Sign(deltaY));

                while ((from += offset) != to)
                {
                    if (GetPiece(from) != null)
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
            
            var destination = GetPiece(to);
            if (destination == null)
            {
                return true;
            }
            if (destination.Type == GamePieceType.Block || destination.IsRed == Piece.IsRed)
            {
                return false;
            }

            return true;
        }

        public GamePiece GetPiece(Point p)
        {
            return state.Board[p.x, p.y];
        }

        public int GetAvailablePieces(GamePieceType piece, bool red)
        {
            return bank.PieceCount(piece, red);
        }

        public void EndSetup()
        {
            state.Turn = PlayerTurn.Red;
            Red.EndSetup();
            Blue.EndSetup();
        }

        public PlayerTurn GetTurn()
        {
            return state.Turn;
        }

        public List<KeyValuePair<GamePieceType, int>> GetAvailablePieces(bool red)
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
