using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public static class GamePieceFactory
    {
        public static GamePiece Create(GamePieceType gamePieceType, bool IsRed)
        {
            if (gamePieceType == GamePieceType.Bomb)
            {
                return new BombGamePiece(IsRed);
            }

            if (gamePieceType == GamePieceType.Spy)
            {
                return new SpyGamePiece(IsRed);
            }

            if (gamePieceType == GamePieceType.Flag)
            {
                return new FlagGamePiece(IsRed);
            }

            if (gamePieceType == GamePieceType.Eight)
            {
                return new EightGamePiece(IsRed);
            }

            return new GamePiece(gamePieceType, IsRed);
        }
    }
}
