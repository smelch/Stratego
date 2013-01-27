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
            Random rand = new Random();

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
                            int r = rand.Next(pieces.Count - 1);
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
                            int r = rand.Next(pieces.Count - 1);
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
        }
    }
}
