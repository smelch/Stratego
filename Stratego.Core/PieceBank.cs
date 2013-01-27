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
        private Dictionary<GamePieceType, List<GamePiece>> bank { get; set; }
        private Dictionary<GamePieceType, List<GamePiece>> redBank { get; set; }

        public PieceBank()
        {
        }

        public void Initialize()
        {
            bank = new Dictionary<GamePieceType, List<GamePiece>>();
            redBank = new Dictionary<GamePieceType, List<GamePiece>>();
            AddPieces(GamePieceType.Bomb, 6);
            AddPieces(GamePieceType.One, 1);
            AddPieces(GamePieceType.Two, 1);
            AddPieces(GamePieceType.Three, 2);
            AddPieces(GamePieceType.Four, 3);
            AddPieces(GamePieceType.Five, 4);
            AddPieces(GamePieceType.Six,  4);
            AddPieces(GamePieceType.Seven, 4);
            AddPieces(GamePieceType.Eight, 5);
            AddPieces(GamePieceType.Nine, 8);
            AddPieces(GamePieceType.Spy, 1);
            AddPieces(GamePieceType.Flag, 1);
        }

        private void AddPieces(GamePieceType gamePieceType,int count)
        {
            List<GamePiece> pieces = new List<GamePiece>();
            List<GamePiece> redPieces = new List<GamePiece>();
            for (int i = 0; i < count; i++)
            {
                pieces.Add(GamePieceFactory.Create(gamePieceType, false));
                redPieces.Add(GamePieceFactory.Create(gamePieceType, true));
            }
            bank.Add(gamePieceType, pieces);
            redBank.Add(gamePieceType, redPieces);
        }

        public GamePiece PlacePiece(GamePieceType piece, bool red)
        {
            var b = (red) ? redBank : bank;
            GamePiece ret = null;

            if (b[piece].Count > 0)
            {
                ret = b[piece][0];
                b[piece].RemoveAt(0);
            }

            return ret;
        }

        public int PieceCount(GamePieceType piece, bool red)
        {
            var b = (red) ? redBank : bank;
            return b[piece].Count;
        }


        internal void ReturnPiece(GamePiece Piece)
        {
            var b = (Piece.IsRed) ? redBank : bank;
            b[Piece.Type].Add(Piece);
        }

        internal List<KeyValuePair<GamePieceType, int>> GetAllAvailablePieces(bool red)
        {
            var ret = new List<KeyValuePair<GamePieceType, int>>();
            var b = (red) ? redBank : bank;
            foreach (var kvp in b)
            {
                ret.Add(new KeyValuePair<GamePieceType, int>(kvp.Key, kvp.Value.Count));
            }

            return ret;
        }
    }
}
