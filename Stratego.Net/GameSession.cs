using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratego.Core;

namespace Stratego.Net
{
    public class GameSession
    {
        public Guid SessionID { get; private set; }
        public Guid RedPlayer { get; private set; }
        public Guid BluePlayer { get; private set; }
        
        private StrategoGame game;

        public GameSession(Guid sessionID, Guid redPlayer, Guid bluePlayer, StrategoGame game)
        {
            SessionID = sessionID;
            RedPlayer = redPlayer;
            BluePlayer = bluePlayer;
            this.game = game;
        }

        public bool GetPlayerIsRed(Guid player)
        {
            bool playerIsRed = RedPlayer == player;
            bool playerIsBlue = BluePlayer == player;
            if (!playerIsRed && !playerIsBlue)
            {
                throw new Exception("Invalid player");
            }

            return playerIsRed;
        }

        public bool MovePiece(Guid player, Point from, Point to)
        {
            if (game.GetTurn() != PlayerTurn.Setup)
            {
                bool playerIsRed = GetPlayerIsRed(player);

                var piece = game.GetPiece(from);
                if (playerIsRed == piece.IsRed())
                {
                    return game.MovePiece(from, to);
                }
            }

            return false;
        }

        public GamePiece Attack(Guid player, Point from, Point to)
        {
            if (game.GetTurn() == PlayerTurn.Setup)
            {
                throw new Exception("Invalid Operation in Setup phase");
            }

            bool playerIsRed = GetPlayerIsRed(player);

            if (playerIsRed != (game.GetTurn() == PlayerTurn.Red))
            {
                throw new Exception(String.Format("It is {0}'s turn. Player {1} is {2}", game.GetTurn().ToString(), player.ToString(), (playerIsRed)?"Red":"Blue"));
            }

            var attacker = game.GetPiece(from);
            var defender = game.GetPiece(to);

            game.Attack(from, to);
            return defender;
        }

        public bool PlacePiece(Guid player, GamePiece piece, Point to)
        {
            if (game.GetTurn() == PlayerTurn.Setup)
            {
                bool playerIsRed = GetPlayerIsRed(player);
                if (playerIsRed)
                {
                    piece |= GamePiece.Red;
                }
                else
                {
                    piece &= ~GamePiece.Red;
                }

                return game.PlacePiece(piece, to);
            }

            return false;
        }

        public bool RemovePiece(Guid player, Point from)
        {
            if (game.GetTurn() == PlayerTurn.Setup)
            {
                bool playerIsRed = GetPlayerIsRed(player);
                var piece = game.GetPiece(from);
                if (piece != GamePiece.Empty)
                {
                    if (piece.IsRed() == playerIsRed)
                    {
                        return game.RemovePiece(from);
                    }
                }
            }

            return false;
        }

        public GamePiece[,] GetState(Guid player)
        {
            var playerIsRed = GetPlayerIsRed(player);
            var board = game.GetBoard();
            for (int x = 0; x <= board.GetUpperBound(0); x++)
            {
                for (int y=0; y <= board.GetUpperBound(1); y++) {
                    var piece = board[x, y];
                    if (!(piece == GamePiece.Empty || piece == GamePiece.Block))
                    {
                        if (playerIsRed != piece.IsRed())
                        {
                            board[x, y] = (piece & GamePiece.Red) | GamePiece.Hidden;
                        }
                    }
                }
            }
            return board;
        }

        public List<KeyValuePair<GamePiece, int>> GetAvailablePieces(Guid player)
        {
            return game.GetAvailablePieces(GetPlayerIsRed(player));
        }

        public PlayerTurn GetTurn()
        {
            return game.GetTurn();
        }
    }
}
