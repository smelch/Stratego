using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Stratego.AITournament;
using Stratego.Core;

namespace Stratego.Windows
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private const int GRID_SIZE = 48;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        StrategoGame stratego;
        Texture2D pixel;
        BasePlayer Red, Blue;
        double timer = 0;
        double timedSpeed = 1;
        
        bool playerIsRed = true;

        Dictionary<GamePiece, string> Text;
        SpriteFont Font;
        private bool spaceDown;
        private Dictionary<GamePiece, Texture2D> PieceTextures;
        private GamePiece ActivePiece;
        private bool mouseDown;
        private NetworkSession session;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SignedInGamer.SignedIn += SignedInGamer_SignedIn;
            NetworkSession.InviteAccepted += NetworkSession_InviteAccepted;
            base.Initialize();
        }

        void NetworkSession_InviteAccepted(object sender, InviteAcceptedEventArgs e)
        {
            // if this is from a session that other local gamers are already in
            if (e.IsCurrentSession)
            {
                //and that session is still running...
                if (session != null)
                    session.AddLocalGamer(e.Gamer);
            }
            else
            {
                if (session != null)
                {
                    session.Dispose();
                    session = null;
                }
                session = NetworkSession.JoinInvited(1);
                session.GamerJoined += session_GamerJoined;
            }

            //session.GameStarted
        }

        void session_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            Player1 = e.Gamer;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            stratego = new StrategoGame();
            Red = new RandomAIPlayer(stratego, PlayerTurn.Red);
            Blue = new RandomAIPlayer(stratego, PlayerTurn.Blue);
            Red.PlacePieces();
            Blue.PlacePieces();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            Font = Content.Load<SpriteFont>("PieceFont");

            Text = new Dictionary<GamePiece, string>() {
                { GamePiece.One, "1" },
                { GamePiece.Two, "2" },
                { GamePiece.Three, "3" },
                { GamePiece.Four, "4" },
                { GamePiece.Five, "5" },
                { GamePiece.Six, "6" },
                { GamePiece.Seven, "7" },
                { GamePiece.Eight, "8" },
                { GamePiece.Nine, "9" },
                { GamePiece.Bomb, "B" },
                { GamePiece.Spy, "S" },
                { GamePiece.Flag, "F" }
            };

            PieceTextures = new Dictionary<GamePiece, Texture2D>();
            foreach (GamePiece piece in Enum.GetValues(typeof(GamePiece)))
            {
                if (piece != GamePiece.Red && piece != GamePiece.Empty && piece != GamePiece.Hidden)
                {
                    PieceTextures.Add(piece, RenderPieceTexture(GRID_SIZE, GRID_SIZE, piece));
                    if (piece != GamePiece.Block)
                    {
                        PieceTextures.Add(piece | GamePiece.Red, RenderPieceTexture(GRID_SIZE, GRID_SIZE, piece | GamePiece.Red));
                    }
                }
            }

            RedTexture = RenderPieceTexture(GRID_SIZE, GRID_SIZE, GamePiece.Hidden | GamePiece.Red);
            BlueTexture = RenderPieceTexture(GRID_SIZE, GRID_SIZE, GamePiece.Hidden);

            // TODO: use this.Content to load your game content here
        }

        private Texture2D RenderPieceTexture(int width, int height, GamePiece gamePiece)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height);
            var renderTarget = new RenderTarget2D(GraphicsDevice, width, height);
            GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.Begin();
            RenderPiece(width, gamePiece);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            Color[] c = new Color[width * height];
            renderTarget.GetData<Color>(c);
            texture.SetData<Color>(c);
            return texture;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!Guide.IsVisible && !stratego.IsOver)
            {
                timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                while (timer > timedSpeed && !stratego.IsOver)
                {
                    playerIsRed = !playerIsRed;
                    if (playerIsRed)
                    {
                        Red.BeginTurn();
                    }
                    else
                    {
                        Blue.BeginTurn();
                    }
                    timer -= timedSpeed;
                }
                //if (Player1 == null)
                //{
                //    Guide.ShowSignIn(1, false);
                //}
                //else
                //{
                //    //Guide.ShowGameInvite(PlayerIndex.One, null);
                //}
                //// Allows the game to exit
                //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                //    this.Exit();

                //if (Keyboard.GetState().IsKeyDown(Keys.Space))
                //{
                //    if (!spaceDown)
                //    {
                //        spaceDown = true;
                //        playerIsRed = !playerIsRed;
                //    }
                //}
                //else
                //{
                //    spaceDown = false;
                //}

                //if (Keyboard.GetState().IsKeyDown(Keys.F2))
                //{
                //    RandomizeRemainingPieces();
                //    stratego.EndSetup();
                //}

                //HandleMouse();
            }
            base.Update(gameTime);
        }

        private void RandomizeRemainingPieces()
        {
            Random rand = new Random();

            List<KeyValuePair<GamePiece, int>> pieces = stratego.GetAvailablePieces(false);
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].Value == 0)
                {
                    pieces.RemoveAt(i);
                }
            }

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (stratego.GetPiece(new Stratego.Core.Point(x, y)) == GamePiece.Empty)
                    {
                        int r = rand.Next(pieces.Count - 1);
                        stratego.PlacePiece(pieces[r].Key, new Stratego.Core.Point(x, y));
                        pieces[r] = new KeyValuePair<GamePiece, int>(pieces[r].Key, pieces[r].Value - 1);
                        if (pieces[r].Value == 0)
                        {
                            pieces.RemoveAt(r);
                        }
                    }
                }
            }

            pieces = stratego.GetAvailablePieces(true);
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].Value == 0)
                {
                    pieces.RemoveAt(i);
                }
            }
            for (int x = 0; x < 10; x++)
            {
                for (int y = 6; y < 10; y++)
                {
                    if (stratego.GetPiece(new Stratego.Core.Point(x, y)) == GamePiece.Empty)
                    {
                        int r = rand.Next(pieces.Count - 1);
                        stratego.PlacePiece(pieces[r].Key, new Stratego.Core.Point(x, y));
                        pieces[r] = new KeyValuePair<GamePiece, int>(pieces[r].Key, pieces[r].Value - 1);
                        if (pieces[r].Value == 0)
                        {
                            pieces.RemoveAt(r);
                        }
                    }
                }
            }
        }

        private void HandleMouse()
        {
            MouseState state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed)
            {
                if (!mouseDown)
                {
                    mouseDown = true;
                    if (state.Y >= 0 && state.Y <= 480)
                    {
                        if (state.X >= 500)
                        {
                            HandleMouseForBank(state);
                        }
                        else if (state.X >= 0 && state.X <= 480)
                        {
                            HandleMouseForBoard(state);
                        }
                    }
                }
            }
            else
            {
                if (mouseDown)
                {
                    if (ActivePiece != GamePiece.Empty)
                    {
                        int x = (int)(state.X / GRID_SIZE);
                        int y = (int)(state.Y / GRID_SIZE);

                        if (x < 10 && y < 10)
                        {
                            DropPiece(x, y);
                        }
                        ActivePiece = GamePiece.Empty;
                    }

                    mouseDown = false;
                }
            }
        }

        private void HandleMouseForBoard(MouseState state)
        {
            int x = state.X / GRID_SIZE;
            int y = state.Y / GRID_SIZE;

            PickUpPiece(stratego.GetPiece(new Stratego.Core.Point(x, y)), (X, Y) => {
                if (stratego.GetTurn() == PlayerTurn.Setup)
                {
                    stratego.RemovePiece(new Stratego.Core.Point(x, y));
                    if (!stratego.PlacePiece(ActivePiece, new Stratego.Core.Point(X, Y)))
                    {
                        stratego.PlacePiece(ActivePiece, new Stratego.Core.Point(x, y));
                        ActivePiece = GamePiece.Empty;
                    }
                }
                else
                {
                    if (stratego.IsValidMove(ActivePiece, new Stratego.Core.Point(x, y), new Stratego.Core.Point(X, Y)))
                    {
                        var piece = stratego.GetPiece(new Stratego.Core.Point(X, Y));
                        if (piece != GamePiece.Empty)
                        {
                            ShowAttack(ActivePiece, piece);
                        }
                        stratego.Move(new Stratego.Core.Point(x, y), new Stratego.Core.Point(X, Y));
                    }
                }
            });
        }

        private void ShowAttack(GamePiece attacker, GamePiece defender)
        {
            LastAttacker = attacker;
            LastDefender = defender;
        }

        private void PickUpPiece(GamePiece piece, Action<int, int> callback)
        {
            ActivePiece = piece;
            PieceDropAction = callback;
        }

        private void DropPiece(int x, int y)
        {
            if (PieceDropAction != null)
            {
                PieceDropAction(x, y);
                PieceDropAction = null;
            }
            ActivePiece = GamePiece.Empty;
        }

        private void HandleMouseForBank(MouseState state)
        {
            GamePiece piece = GamePiece.Empty;
            if (state.X <= 548 && state.Y <= 432)
            {
                int offset = (int)state.Y / 48 + 1;
                piece = (GamePiece)offset;
                piece |= (playerIsRed) ? GamePiece.Red : GamePiece.Empty;
            }
            if (state.X >= 600 && state.X <= 648 && state.Y <= 144)
            {
                int offset = (int)state.Y / 48;
                switch (offset)
                {
                    case 0:
                        piece = GamePiece.Spy;
                        break;
                    case 1:
                        piece = GamePiece.Bomb;
                        break;
                    case 2:
                        piece = GamePiece.Flag;
                        break;
                }
                piece |= (playerIsRed) ? GamePiece.Red : GamePiece.Empty;
            }

            if (piece != GamePiece.Empty)
            {
                PickUpPiece(piece, (X, Y) => stratego.PlacePiece(ActivePiece, new Stratego.Core.Point(X, Y)));
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            DrawGrid();
            DrawPieces();
            if (stratego.GetTurn() == PlayerTurn.Setup)
            {
                DrawPieceBank();
            }
            else
            {
                DrawLastAttack();
            }
            DrawActivePiece();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawLastAttack()
        {
            if (LastAttacker != GamePiece.Empty)
            {
                DrawPiece(500, 48, LastAttacker, false);
                DrawPiece(550, 48, LastDefender, false);
            }
        }

        private void DrawActivePiece()
        {
            if(ActivePiece != GamePiece.Empty) {
                int x = Mouse.GetState().X - GRID_SIZE / 2;
                int y = Mouse.GetState().Y - GRID_SIZE / 2;
                spriteBatch.Draw(PieceTextures[ActivePiece], new Vector2(x, y), new Color(1, 1, 1, 0.5f));
            }
        }

        private void DrawPieceBank()
        {
            DrawBankItem(GamePiece.One | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 0);
            DrawBankItem(GamePiece.Two | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 48);
            DrawBankItem(GamePiece.Three | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 96);
            DrawBankItem(GamePiece.Four | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 144);
            DrawBankItem(GamePiece.Five | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 192);
            DrawBankItem(GamePiece.Six | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 240);
            DrawBankItem(GamePiece.Seven | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 288);
            DrawBankItem(GamePiece.Eight | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 336);
            DrawBankItem(GamePiece.Nine | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 500, 384);
            DrawBankItem(GamePiece.Spy | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 596, 0);
            DrawBankItem(GamePiece.Bomb | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 596, 48);
            DrawBankItem(GamePiece.Flag | (playerIsRed ? GamePiece.Red : GamePiece.Empty), 596, 96);
        }

        private void DrawBankItem(GamePiece piece, int x, int y)
        {
            DrawPiece(x, y, piece);
            string text = stratego.GetAvailablePieces(piece).ToString();
            var size = Font.MeasureString(text);
            spriteBatch.DrawString(Font, text, new Vector2(x + 72 - size.X / 2, y + (GRID_SIZE - size.Y) / 2), Color.White);
        }

        private void DrawPieces()
        {
            int grid_size = 48;
            Rectangle rect = new Rectangle(0,0,grid_size - 1, grid_size - 1);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var piece = stratego.GetPiece(new Stratego.Core.Point(x, y));
                    DrawPiece(rect.X, rect.Y, piece, false);
                    rect.Y += grid_size;
                }
                rect.Y = 0;
                rect.X += grid_size;
            }
        }

        private void DrawPiece(int x, int y, GamePiece piece, bool hideOtherColor = true)
        {
            if (piece != GamePiece.Empty)
            {
                Texture2D texture = null;
                if (!piece.IsBlock() && hideOtherColor)
                {
                    if (piece.IsRed() && !playerIsRed)
                    {
                        texture = RedTexture;
                    }
                    else if(!piece.IsRed() && playerIsRed)
                    {
                        texture = BlueTexture;
                    }
                }

                if (texture == null)
                {
                    texture = PieceTextures[piece];
                }
                spriteBatch.Draw(texture, new Vector2(x, y), Color.White);
            }
        }

        private void RenderPiece(int grid_size, GamePiece piece)
        {
            if (piece > 0)
            {
                Rectangle rect = new Rectangle(1, 1, grid_size - 1, grid_size - 1);
                spriteBatch.Draw(pixel, rect, piece.IsBlock() ? Color.Black : piece.IsRed() ? Color.Red : Color.Blue);
                if (!piece.IsBlock() && !piece.IsHidden())
                {
                    var text = Text[piece.GetPieceType()];
                    var size = Font.MeasureString(text);
                    GraphicsDevice.Clear(Color.Transparent);
                    spriteBatch.DrawString(Font, text, new Vector2((grid_size - size.X) / 2, (grid_size - size.Y) / 2), Color.White);
                }
            }
        }

        private void DrawGrid()
        {
            int grid_size = 48;
            for (int x = 0; x < 11; x++)
            {
                spriteBatch.Draw(pixel, new Rectangle(x * grid_size, 0, 1, 10 * grid_size), Color.Black);
            }
            for (int y = 0; y < 11; y++)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, y * grid_size, 10 * grid_size, 1), Color.Black);
            }
        }

        public Action<int, int> PieceDropAction { get; set; }

        public GamePiece LastDefender { get; set; }

        public GamePiece LastAttacker { get; set; }

        public Texture2D RedTexture { get; set; }

        public Texture2D BlueTexture { get; set; }

        public SignedInGamer Player1 { get; set; }
    }
}
