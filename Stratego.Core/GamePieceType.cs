using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public enum GamePieceType : int
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
        Block = 16,
        Hidden = 32
    }

    public static class GamePieceExtensions
    {
        public static bool IsBlock(this GamePieceType piece)
        {
            return piece == GamePieceType.Block;
        }

        public static bool IsHidden(this GamePieceType piece)
        {
            return (piece & GamePieceType.Hidden) == GamePieceType.Hidden;
        }

        public static GamePieceType GetPieceType(this GamePieceType piece)
        {
            return piece & (GamePieceType)15;
        }
    }
}
