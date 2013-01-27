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
        double timedSpeed = 2;
        
        bool playerIsRed = true;

        Dictionary<GamePieceType, string> Text;
        SpriteFont Font;
        private bool spaceDown;
        private Dictionary<GamePieceType, Texture2D> RedPieceTextures, BluePieceTextures;
        private GamePiece ActivePiece;
        private bool mouseDown;
        private NetworkSession session;
        private int redWins;
        private int totalGames;
        private double ratio;
        

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
            StartNewGame();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            Font = Content.Load<SpriteFont>("PieceFont");

            Text = new Dictionary<GamePieceType, string>() {
                { GamePieceType.One, "1" },
                { GamePieceType.Two, "2" },
                { GamePieceType.Three, "3" },
                { GamePieceType.Four, "4" },
                { GamePieceType.Five, "5" },
                { GamePieceType.Six, "6" },
                { GamePieceType.Seven, "7" },
                { GamePieceType.Eight, "8" },
                { GamePieceType.Nine, "9" },
                { GamePieceType.Bomb, "B" },
                { GamePieceType.Spy, "S" },
                { GamePieceType.Flag, "F" }
            };

            RedPieceTextures = new Dictionary<GamePieceType, Texture2D>();
            BluePieceTextures = new Dictionary<GamePieceType, Texture2D>();

            foreach (GamePieceType piece in Enum.GetValues(typeof(GamePieceType)))
            {
                BluePieceTextures.Add(piece, RenderPieceTexture(GRID_SIZE, GRID_SIZE, piece, false));
                if (piece != GamePieceType.Block)
                {
                    RedPieceTextures.Add(piece, RenderPieceTexture(GRID_SIZE, GRID_SIZE, piece, true));
                }
            }

            // TODO: use this.Content to load your game content here
        }

        private void StartNewGame()
        {
            playerIsRed = true;
            stratego = new StrategoGame();
            Red = new RetreatAIPlayer(stratego, PlayerTurn.Red);
            Blue = new RandomAIPlayer(stratego, PlayerTurn.Blue);
            stratego.Red = Red;
            stratego.Blue = Blue;
            Red.PlacePieces();
            Blue.PlacePieces();
            stratego.EndSetup();
        }

        private Texture2D RenderPieceTexture(int width, int height, GamePieceType gamePiece, bool isRed)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height);
            var renderTarget = new RenderTarget2D(GraphicsDevice, width, height);
            GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.Begin();
            RenderPiece(width, gamePiece, isRed);
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
                    if (playerIsRed)
                    {
                        Red.BeginTurn();
                    }
                    else
                    {
                        Blue.BeginTurn();
                    }
                    playerIsRed = !playerIsRed;
                    timer -= timedSpeed;
                }

                if (stratego.IsOver)
                {
                    redWins += (stratego.GetTurn() == PlayerTurn.Red) ? 1 : 0;
                    totalGames++;
                    ratio = (double)redWins / (double)totalGames;
                    StartNewGame();
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

            List<KeyValuePair<GamePieceType, int>> pieces = stratego.GetAvailablePieces(false);
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
                    if (stratego.GetPiece(new Stratego.Core.Point(x, y)) == null)
                    {
                        int r = rand.Next(pieces.Count - 1);
                        stratego.PlacePiece(pieces[r].Key, false, new Stratego.Core.Point(x, y));
                        pieces[r] = new KeyValuePair<GamePieceType, int>(pieces[r].Key, pieces[r].Value - 1);
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
                    if (stratego.GetPiece(new Stratego.Core.Point(x, y)) == null)
                    {
                        int r = rand.Next(pieces.Count - 1);
                        stratego.PlacePiece(pieces[r].Key, true, new Stratego.Core.Point(x, y));
                        pieces[r] = new KeyValuePair<GamePieceType, int>(pieces[r].Key, pieces[r].Value - 1);
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
                    if (ActivePiece != null)
                    {
                        int x = (int)(state.X / GRID_SIZE);
                        int y = (int)(state.Y / GRID_SIZE);

                        if (x < 10 && y < 10)
                        {
                            DropPiece(x, y);
                        }
                        ActivePiece = null;
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
                    if (!stratego.PlacePiece(ActivePiece.Type, ActivePiece.IsRed, new Stratego.Core.Point(X, Y)))
                    {
                        stratego.PlacePiece(ActivePiece.Type, ActivePiece.IsRed, new Stratego.Core.Point(x, y));
                        ActivePiece = null;
                    }
                }
                else
                {
                    if (stratego.IsValidMove(ActivePiece, new Stratego.Core.Point(x, y), new Stratego.Core.Point(X, Y)))
                    {
                        var piece = stratego.GetPiece(new Stratego.Core.Point(X, Y));
                        if (piece != null)
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
            ActivePiece = null;
        }

        private void HandleMouseForBank(MouseState state)
        {
            GamePieceType piece = GamePieceType.Empty;
            if (state.X <= 548 && state.Y <= 432)
            {
                int offset = (int)state.Y / 48 + 1;
                piece = (GamePieceType)offset;
            }

            if (state.X >= 600 && state.X <= 648 && state.Y <= 144)
            {
                int offset = (int)state.Y / 48;
                switch (offset)
                {
                    case 0:
                        piece = GamePieceType.Spy;
                        break;
                    case 1:
                        piece = GamePieceType.Bomb;
                        break;
                    case 2:
                        piece = GamePieceType.Flag;
                        break;
                }
            }

            if (piece != GamePieceType.Empty)
            {
                PickUpPiece(GamePieceFactory.Create(piece, playerIsRed), (X, Y) => stratego.PlacePiece(ActivePiece.Type, playerIsRed, new Stratego.Core.Point(X, Y)));
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
            if (LastAttacker != null)
            {
                DrawPiece(500, 48, LastAttacker.Type, LastAttacker.IsRed, false);
                DrawPiece(550, 48, LastDefender.Type, LastDefender.IsRed, false);
            }
        }

        private void DrawActivePiece()
        {
            if(ActivePiece != null) {
                var t = (ActivePiece.IsRed) ? RedPieceTextures : BluePieceTextures;
                int x = Mouse.GetState().X - GRID_SIZE / 2;
                int y = Mouse.GetState().Y - GRID_SIZE / 2;
                spriteBatch.Draw(t[ActivePiece.Type], new Vector2(x, y), new Color(1, 1, 1, 0.5f));
            }
        }

        private void DrawPieceBank()
        {
            DrawBankItem(GamePieceType.One, 500, 0);
            DrawBankItem(GamePieceType.Two, 500, 48);
            DrawBankItem(GamePieceType.Three, 500, 96);
            DrawBankItem(GamePieceType.Four, 500, 144);
            DrawBankItem(GamePieceType.Five, 500, 192);
            DrawBankItem(GamePieceType.Six, 500, 240);
            DrawBankItem(GamePieceType.Seven, 500, 288);
            DrawBankItem(GamePieceType.Eight, 500, 336);
            DrawBankItem(GamePieceType.Nine, 500, 384);
            DrawBankItem(GamePieceType.Spy, 596, 0);
            DrawBankItem(GamePieceType.Bomb, 596, 48);
            DrawBankItem(GamePieceType.Flag, 596, 96);
        }

        private void DrawBankItem(GamePieceType piece, int x, int y)
        {
            DrawPiece(x, y, piece, playerIsRed);
            string text = stratego.GetAvailablePieces(piece, playerIsRed).ToString();
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
                    if (piece != null)
                    {
                        DrawPiece(rect.X, rect.Y, piece.Type, piece.IsRed, false);
                    }
                    rect.Y += grid_size;
                }
                rect.Y = 0;
                rect.X += grid_size;
            }
        }

        private void DrawPiece(int x, int y, GamePieceType piece, bool isRed, bool hideOtherColor = true)
        {
            if (piece != null)
            {
                Texture2D texture = null;
                if (!piece.IsBlock() && hideOtherColor)
                {
                    if (isRed != playerIsRed)
                    {
                        piece = GamePieceType.Hidden;
                    }
                }

                if (texture == null)
                {
                    texture = ((isRed)?RedPieceTextures:BluePieceTextures)[piece];
                }
                spriteBatch.Draw(texture, new Vector2(x, y), Color.White);
            }
        }

        private void RenderPiece(int grid_size, GamePieceType piece, bool isRed)
        {
            if (piece > 0)
            {
                Rectangle rect = new Rectangle(1, 1, grid_size - 1, grid_size - 1);
                spriteBatch.Draw(pixel, rect, piece.IsBlock() ? Color.Black : isRed ? Color.Red : Color.Blue);
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

        public SignedInGamer Player1 { get; set; }
    }
}
