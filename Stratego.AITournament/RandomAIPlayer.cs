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

            List<KeyValuePair<GamePiece, int>> pieces = _game.GetAvailablePieces(this._playerColor == PlayerTurn.Red);
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
                        if (_game.GetPiece(new Stratego.Core.Point(x, y)) == GamePiece.Empty)
                        {
                            int r = rand.Next(pieces.Count);
                            _game.PlacePiece(pieces[r].Key, new Stratego.Core.Point(x, y));
                            pieces[r] = new KeyValuePair<GamePiece, int>(pieces[r].Key, pieces[r].Value - 1);
                            if (pieces[r].Value == 0)
                            {
                                pieces.RemoveAt(r);
                            }
                        }
                    }
                }
                else
                {
                    for (int y = 6; y < 10; y++)
                    {
                        if (_game.GetPiece(new Stratego.Core.Point(x, y)) == GamePiece.Empty)
                        {
                            int r = rand.Next(pieces.Count);
                            _game.PlacePiece(pieces[r].Key, new Stratego.Core.Point(x, y));
                            pieces[r] = new KeyValuePair<GamePiece, int>(pieces[r].Key, pieces[r].Value - 1);
                            if (pieces[r].Value == 0)
                            {
                                pieces.RemoveAt(r);
                            }
                        }
                    }
                }
            }
        }

        public override void BeginTurn()
        {
            //Find some moveable pieces and make a move at random
            KeyValuePair<Point, Point[]>[] movable = FindMoveablePieces();
            Random r = new Random(Environment.TickCount);

            var attacker = movable[r.Next(movable.Length)];
            var move = attacker.Value[r.Next(attacker.Value.Length)];
            if (_game.GetBoard()[move.x, move.y] != 0)
            {
                var suc = _game.Attack(attacker.Key, move);
            }
            else
            {
                var suc2 = _game.Move(attacker.Key, move);
            }
        }

        private KeyValuePair<Point, Point[]>[] FindMoveablePieces()
        {
            var board = _game.GetBoard();

            Dictionary<Point,Point[]> movers = new Dictionary<Point,Point[]>();
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var piece = board[x, y];
                    if (piece != 0 && IsRed == piece.IsRed())
                    {
                        bool canMove = false;
                        List<Point> moves = new List<Point>();
                        if (_game.IsValidMove(piece, new Point(x, y), new Point(x - 1, y))) {
                            moves.Add(new Point(x - 1, y));
                        }
                        if (_game.IsValidMove(piece, new Point(x, y), new Point(x + 1, y))) {
                            moves.Add(new Point(x + 1, y));
                        }
                        if (_game.IsValidMove(piece, new Point(x, y), new Point(x, y - 1))) {
                            moves.Add(new Point(x, y - 1));
                        }
                        if (_game.IsValidMove(piece, new Point(x, y), new Point(x, y + 1))) {
                            moves.Add(new Point(x, y + 1));
                        }

                        if (moves.Count > 0)
                        {
                            movers.Add(new Point(x,y), moves.ToArray());
                        }
                    }
                }
            }

            return movers.ToArray();
        }
    }
}
