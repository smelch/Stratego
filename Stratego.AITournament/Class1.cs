using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratego.Core;

namespace Stratego.AITournament
{
    public class AITournament<TRed, TBlue> where TRed : BasePlayer where TBlue : BasePlayer
    {
        public AITournament() 
        {
            StrategoGame game = new StrategoGame();

            var constructor = typeof(TRed).GetConstructor(new Type[] { typeof(StrategoGame), typeof(PlayerTurn) });
            BasePlayer Red = (BasePlayer)constructor.Invoke(new object[] { game, PlayerTurn.Red });
            BasePlayer Blue = (BasePlayer)constructor.Invoke(new object[] { game, PlayerTurn.Blue });

            Red.PlacePieces();
            Blue.PlacePieces();

            while (!game.IsOver)
            {
                Red.BeginTurn();
                if (!game.IsOver)
                {
                    Blue.BeginTurn();
                }
            }
        }
    }
}
