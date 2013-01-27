using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public class EightGamePiece : GamePiece
    {
        public EightGamePiece(bool IsRed) : base(GamePieceType.Eight, IsRed) { }
        public override AttackResult Attack(GamePiece defender)
        {
            if (defender.Type == GamePieceType.Bomb)
            {
                return AttackResult.Win;
            }
            return base.Attack(defender);
        }
    }
}
