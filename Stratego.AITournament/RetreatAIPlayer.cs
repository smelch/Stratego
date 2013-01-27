using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stratego.Core;

namespace Stratego.AITournament
{
    public class RetreatAIPlayer : RandomAIPlayer
    {
        public RetreatAIPlayer(StrategoGame game, PlayerTurn playerColor) : base(game, playerColor) { }
        
        public override void BeginTurn()
        {
            //Find some moveable pieces and make a move at random
            KeyValuePair<Point, Point>[] movable = FindMoveablePieces();
            if (movable.Length == 0)
            {
                _game.Surrender();
            }
            else
            {
                Random r = new Random(Environment.TickCount);

                var pmoves = movable.Select(x => new { move = x, p = AssignPriority(x) });
                var mp = pmoves.Max(x => x.p);
                var moves = pmoves.Where(x => x.p == mp).Select(x => x.move).ToList();
                var move = moves[r.Next(moves.Count)];

                if (_game.GetBoard()[move.Value.x, move.Value.y] != null)
                {
                    var suc = _game.Attack(move.Key, move.Value);
                }
                else
                {
                    var suc2 = _game.Move(move.Key, move.Value);
                }
            }
        }

        public int AssignPriority(KeyValuePair<Point, Point> x)
        {
            var occupant = EstimatedState[x.Value.x, x.Value.y];
            var mover = EstimatedState[x.Key.x, x.Key.y];
            if (occupant != null && occupant.Type != GamePieceType.Hidden)
            {
                switch (mover.Attack(occupant))
                {
                    case AttackResult.Lose:
                        return -1;
                    case AttackResult.Win:
                        return 1;
                    case AttackResult.Tie:
                        return 0;
                }
            }
            return 0;
        }
    }
}
