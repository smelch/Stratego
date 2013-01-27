using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public class BombGamePiece : GamePiece
    {
        public BombGamePiece(bool IsRed) : base(GamePieceType.Bomb, IsRed) { }
        public override AttackResult Attack(GamePiece defender)
        {
            throw new Exception("Bombs can not attack");
        }
    }
}
