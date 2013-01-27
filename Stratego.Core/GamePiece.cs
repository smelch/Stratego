using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public enum GamePiece : byte
    {
        Empty = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Spy = 10,
        Bomb = 11,
        Flag = 12,
        Red = 16,
        Block = 32,
        Hidden = 64
    }

    public static class GamePieceExtensions
    {
        public static bool IsRed(this GamePiece piece)
        {
            return (piece & GamePiece.Red) == GamePiece.Red;
        }

        public static bool IsBlue(this GamePiece piece)
        {
            return (piece & GamePiece.Red) == 0;
        }

        public static bool IsBlock(this GamePiece piece)
        {
            return piece == GamePiece.Block;
        }

        public static bool IsHidden(this GamePiece piece)
        {
            return (piece & GamePiece.Hidden) == GamePiece.Hidden;
        }

        public static GamePiece GetPieceType(this GamePiece piece)
        {
            return piece & (GamePiece)15;
        }
    }
}
