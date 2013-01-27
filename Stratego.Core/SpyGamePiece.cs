using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public class SpyGamePiece : GamePiece
    {
        public SpyGamePiece(bool IsRed) : base(GamePieceType.Spy, IsRed) { }

        public override AttackResult Attack(GamePiece defender)
        {
            if (defender.Type == GamePieceType.One)
                return AttackResult.Win;
            if (defender.Type == GamePieceType.Spy)
                return AttackResult.Tie;
            
            return AttackResult.Lose;
        }
    }
}
