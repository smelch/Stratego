using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public abstract class BaseAIPlayer : BasePlayer
    {
        public BaseAIPlayer(StrategoGame game, PlayerTurn playerColor) : base(game, playerColor) { }
    }
}
