using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    [Serializable]
    public class GameState
    {
        public GamePiece[,] Board { get; set; }
        public PlayerTurn Turn { get; set; }
        public PieceBank Bank { get; set; }

        public GameState()
        {
            Board = new GamePiece[10, 10];
            Turn = PlayerTurn.Setup;
            Bank = new PieceBank();
        }
    }
}
