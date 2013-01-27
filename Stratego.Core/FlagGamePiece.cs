using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public class FlagGamePiece : GamePiece
    {
        public FlagGamePiece(bool IsRed) : base(GamePieceType.Flag, IsRed) { }
        public override AttackResult Attack(GamePiece defender)
        {
            throw new Exception("Flags can not attack!");
        }
    }
}
