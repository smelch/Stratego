using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stratego.Core;

namespace Stratego.AITournament
{
    public class RandomAIPlayer : BaseAIPlayer
    {
        public RandomAIPlayer(StrategoGame game, PlayerTurn playerColor) : base(game, playerColor) { }



        public override void PlacePieces()
        {
            Random rand = new Random(Environment.TickCount);

            List<KeyValuePair<GamePieceType, int>> pieces = _game.GetAvailablePieces(this._playerColor == PlayerTurn.Red);
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].Value == 0)
                {
                    pieces.RemoveAt(i);
                }
            }

            for (int x = 0; x < 10; x++)
            {
                if (this._playerColor == PlayerTurn.Blue)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (_game.GetPiece(new Stratego.Core.Point(x, y)) == null)
                        {
                            PlacePiece(rand, pieces, x, y);
                        }
                    }
                }
                else
                {
                    for (int y = 6; y < 10; y++)
                    {
                        if (_game.GetPiece(new Stratego.Core.Point(x, y)) == null)
                        {
                            PlacePiece(rand, pieces, x, y);
                        }
                    }
                }
            }
        }

        protected void PlacePiece(Random rand, List<KeyValuePair<GamePieceType, int>> pieces, int x, int y)
        {
            int r = rand.Next(pieces.Count);
            _game.PlacePiece(pieces[r].Key, this.IsRed, new Stratego.Core.Point(x, y));
            pieces[r] = new KeyValuePair<GamePieceType, int>(pieces[r].Key, pieces[r].Value - 1);
            if (pieces[r].Value == 0)
            {
                pieces.RemoveAt(r);
            }
        }

        public override void BeginTurn()
        {
            //Find some moveable pieces and make a move at random
            KeyValuePair<Point, Point>[] movable = FindMoveablePieces();
            Random r = new Random(Environment.TickCount);

            if (movable.Length == 0)
            {
                _game.Surrender();
            }
            else
            {
                var m = movable[r.Next(movable.Length)];
                var attacker = m.Key;
                var move = m.Value;
                if (_game.GetBoard()[move.x, move.y] != null)
                {
                    var suc = _game.Attack(attacker, move);
                }
                else
                {
                    var suc2 = _game.Move(attacker, move);
                }
            }
        }

        protected KeyValuePair<Point, Point>[] FindMoveablePieces()
        {
            var board = _game.GetBoard();

            List<KeyValuePair<Point,Point>> moves = new List<KeyValuePair<Point,Point>>();
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var piece = board[x, y];
                    if (piece != null && IsRed == piece.IsRed)
                    {
                        bool canMove = false;
                        for (int dx = -1; x + dx >= 0 && x + dx < 10; dx--)
                        {
                            var dest = new Point(x + dx, y);
                            if (_game.IsValidMove(piece, new Point(x, y), dest))
                            {
                                moves.Add(new KeyValuePair<Point, Point>(new Point(x, y), dest));
                            }
                            else
                            {
                                break;
                            }
                        }
                        for (int dx = 1; x + dx >= 0 && x + dx < 10; dx++)
                        {
                            var dest = new Point(x + dx, y);
                            if (_game.IsValidMove(piece, new Point(x, y), dest))
                            {
                                moves.Add(new KeyValuePair<Point, Point>(new Point(x, y), dest));
                            }
                            else
                            {
                                break;
                            }
                        }
                        for (int dy = -1; y + dy >= 0 && y + dy < 10; dy--)
                        {
                            var dest = new Point(x, y + dy);
                            if (_game.IsValidMove(piece, new Point(x, y), dest))
                            {
                                moves.Add(new KeyValuePair<Point, Point>(new Point(x, y), dest));
                            }
                            else
                            {
                                break;
                            }
                        }
                        for (int dy = 1; y + dy >= 0 && y + dy < 10; dy++)
                        {
                            var dest = new Point(x, y + dy);
                            if (_game.IsValidMove(piece, new Point(x, y), dest))
                            {
                                moves.Add(new KeyValuePair<Point, Point>(new Point(x, y), dest));
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return moves.ToArray();
        }
    }
}
