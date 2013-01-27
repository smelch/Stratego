using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stratego.Core
{
    public abstract class BaseAIPlayer : BasePlayer
    {
        protected GamePiece[,] EstimatedState;

        public BaseAIPlayer(StrategoGame game, PlayerTurn playerColor) : base(game, playerColor) {
            
        }

        public override void EndSetup()
        {
            var board = _game.GetBoard();
            EstimatedState = new GamePiece[10, 10];
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if(board[x,y] != null) {
                        if (board[x,y].Type == GamePieceType.Block || board[x, y].IsRed == this.IsRed)
                        {
                            EstimatedState[x, y] = GamePieceFactory.Create(board[x, y].Type, board[x, y].IsRed);
                        }
                        else
                        {
                            EstimatedState[x, y] = GamePieceFactory.Create(GamePieceType.Hidden, !this.IsRed);
                        }
                    }
                }
            }
        }

        public override void PlayerMoved(BasePlayer player, Point from, Point to)
        {
            EstimatedState[to.x, to.y] = EstimatedState[from.x, from.y];
            EstimatedState[from.x, from.y] = null;
        }

        public override void PlayerAttacked(BasePlayer player, Point from, Point to, GamePieceType piece, AttackResult result)
        {
            EstimatedState[from.x, from.y] = null;
            if (result == AttackResult.Win)
            {
                if (player != this)
                {
                    EstimatedState[to.x, to.y] = GamePieceFactory.Create(piece, !this.IsRed);
                }
            }
            else if (result == AttackResult.Tie)
            {
                EstimatedState[to.x, to.y] = null;
            }
            else if (result == AttackResult.Lose)
            {
                if (player == this)
                {
                    EstimatedState[to.x, to.y] = GamePieceFactory.Create(piece, !this.IsRed);
                }
            }
        }
    }
}
