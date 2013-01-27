using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratego.Core
{
    [Serializable]
    public class PieceBank
    {
        private Dictionary<GamePiece, int> bank { get; set; }

        public PieceBank()
        {
        }

        public void Initialize()
        {
            bank = new Dictionary<GamePiece, int>()
            {
                {GamePiece.Bomb, 6},
                {GamePiece.One, 1},
                {GamePiece.Two, 1},
                {GamePiece.Three, 2},
                {GamePiece.Four, 3},
                {GamePiece.Five, 4},
                {GamePiece.Six,  4},
                {GamePiece.Seven, 4},
                {GamePiece.Eight, 5},
                {GamePiece.Nine, 8},
                {GamePiece.Spy, 1},
                {GamePiece.Flag, 1},

                {GamePiece.Bomb | GamePiece.Red, 6},
                {GamePiece.One | GamePiece.Red, 1},
                {GamePiece.Two | GamePiece.Red, 1},
                {GamePiece.Three | GamePiece.Red, 2},
                {GamePiece.Four | GamePiece.Red, 3},
                {GamePiece.Five | GamePiece.Red, 4},
                {GamePiece.Six | GamePiece.Red, 4},
                {GamePiece.Seven | GamePiece.Red, 4},
                {GamePiece.Eight | GamePiece.Red, 5},
                {GamePiece.Nine | GamePiece.Red, 8},
                {GamePiece.Spy | GamePiece.Red, 1},
                {GamePiece.Flag | GamePiece.Red, 1}

            };
        }

        public bool PlacePiece(GamePiece piece)
        {
            if (bank[piece] > 0)
            {
                bank[piece]--;

                return true;
            }

            return false;
        }

        public int PieceCount(GamePiece piece)
        {
            return bank[piece];
        }


        internal void ReturnPiece(GamePiece Piece)
        {
            bank[Piece]++;
        }

        internal List<KeyValuePair<GamePiece, int>> GetAllAvailablePieces(bool red)
        {
            var ret = new List<KeyValuePair<GamePiece, int>>();

            foreach (var kvp in bank)
            {
                if (kvp.Key.IsRed() == red)
                {
                    ret.Add(new KeyValuePair<GamePiece, int>(kvp.Key, kvp.Value));
                }
            }

            return ret;
        }
    }
}
