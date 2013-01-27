using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public class GamePiece
    {
        public GamePieceType Type { get; set; }
        public bool IsRed { get; set; }

        public GamePiece(GamePieceType gamePieceType, bool IsRed)
        {
            Type = gamePieceType;
            this.IsRed = IsRed;
        }
        
        public virtual AttackResult Attack(GamePiece defender)
        {
            switch(Math.Sign(defender.Type - Type)) {
                case -1:
                    return AttackResult.Lose;
                case 0:
                    return AttackResult.Tie;
                    
            }

            return AttackResult.Win;
        }
    }
}
