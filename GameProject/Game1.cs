using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using static GameProject.Animal;
namespace GameProject
{
    public enum GameState
    {
        TitleScreen,
        Playing
    }


    /// <summary>
    /// Main game
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        /// <summary>
        /// Used for the title of the game
        /// </summary>
        private SpriteFont _titleFont;

        /// <summary>
        /// NPC animals that will be moving around
        /// </summary>
        private Animal[] animals;

        /// <summary>
        /// Player
        /// </summary>
        private PlayerSprite player;

        /// <summary>
        /// Textures used for background
        /// </summary>
        private Texture2D _background;

        private int windowHeight;
        private int windowWidth;
        private Song backgroundMusic;
        private SoundEffect scaredSound;

        private GameState currentGameState = GameState.TitleScreen;
        private Texture2D playButtonTexture;
        private Rectangle playButtonRect;
        private MouseState previousMouseState;

        private Random rng = new Random();

        private float titleZoom = 0.1f;
        private float titleZoomSpeed = 1.2f;

        private float playButtonRotation = 0f;
        private bool isHoveringPlayButton = false;
        private float wiggleTimer = 0f;

        private Rectangle[] pens;


        /// <summary>
        /// Constructor
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Init game
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here



            player = new PlayerSprite();

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            pens = new Rectangle[]
            {
                new Rectangle(50, 50, 100, 100),
                new Rectangle(50, 200, 100, 100),
                new Rectangle(50, 350, 100, 100),
                new Rectangle(50, 500, 100, 100)
            };

            base.Initialize();
        }

        /// <summary>
        /// Loading textures
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _titleFont = Content.Load<SpriteFont>("TitleFont");

            _background = Content.Load<Texture2D>("Background/untitled");


            playButtonTexture = Content.Load<Texture2D>("UI/PlayButton");

            int buttonWidth = playButtonTexture.Width / 3; 
            int buttonHeight = playButtonTexture.Height / 3;
            playButtonRect = new Rectangle(
                (_graphics.PreferredBackBufferWidth - buttonWidth) / 2,
                (_graphics.PreferredBackBufferHeight / 2) + 100,
                buttonWidth,
                buttonHeight);

            
            windowWidth = _graphics.PreferredBackBufferWidth;
            windowHeight = _graphics.PreferredBackBufferHeight;

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundMusic = Content.Load<Song>("Sfx/Background_Music");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.Play(backgroundMusic);

            scaredSound = Content.Load<SoundEffect>("Sfx/Scared");

            animals = new Animal[] {
                new SheepSprite(scaredSound){ Position = new Vector2(rng.Next(400, 801), rng.Next(100, 1001)) },
                new BullSprite(scaredSound){ Position = new Vector2(rng.Next(400, 801), rng.Next(100, 1001)) },
                new RoosterSprite(scaredSound){ Position = new Vector2(rng.Next(400, 801), rng.Next(100, 1001)) },
                new PigletSprite(scaredSound){ Position = new Vector2(rng.Next(400, 801), rng.Next(100, 1001)) }
            };

            foreach (var a in animals) { a.LoadContent(Content); }
            player.LoadContent(Content);

        }

        /// <summary>
        /// Updating state of the game
        /// </summary>
        /// <param name="gameTime">In game time</param>
        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (currentGameState == GameState.TitleScreen)
            {
                if (titleZoom < 1f)
                    titleZoom += (float)(titleZoomSpeed * gameTime.ElapsedGameTime.TotalSeconds);
                else
                    titleZoom = 1f;

                isHoveringPlayButton = playButtonRect.Contains(mouseState.Position);


                if (isHoveringPlayButton)
                {
                    wiggleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * 10f;
                    playButtonRotation = (float)Math.Sin(wiggleTimer) * 0.15f;
                }
                else
                {
                    playButtonRotation = MathHelper.Lerp(playButtonRotation, 0f, 0.1f);
                }

                if (mouseState.LeftButton == ButtonState.Pressed &&
                    previousMouseState.LeftButton == ButtonState.Released &&
                    playButtonRect.Contains(mouseState.Position))
                {
                    currentGameState = GameState.Playing;
                }
            }
            else if (currentGameState == GameState.Playing)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                foreach (var a in animals)
                {
                    if (!a.IsInPen)
                    {
                        foreach (var pen in pens)
                        {
                            if (pen.Contains(a.Position.ToPoint()))
                            {
                                a.AssignPen(pen);
                                break;
                            }
                        }
                    }
                }


                foreach (var a in animals) { a.Update(gameTime, windowWidth, windowHeight, player.Position); }
                player.Update(gameTime);

                base.Update(gameTime);
            }

            previousMouseState = mouseState;

        }

        /// <summary>
        /// Drawing game 
        /// </summary>
        /// <param name="gameTime">Ingame time</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
            _spriteBatch.End();

            Matrix transform = Matrix.Identity;
            if (currentGameState == GameState.TitleScreen)
            {
                transform =
                    Matrix.CreateTranslation(
                        -_graphics.PreferredBackBufferWidth / 2,
                        -_graphics.PreferredBackBufferHeight / 2,
                        0) *
                    Matrix.CreateScale(titleZoom) *
                    Matrix.CreateTranslation(
                        _graphics.PreferredBackBufferWidth / 2,
                        _graphics.PreferredBackBufferHeight / 2,
                        0);
            }

            _spriteBatch.Begin(
                transformMatrix: transform,
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend
            );

            if (currentGameState == GameState.TitleScreen)
            {
                // --- Title Text ---
                Vector2 titleSize = _titleFont.MeasureString("Farm Parade");
                Vector2 titlePosition = new Vector2(
                    (_graphics.PreferredBackBufferWidth - titleSize.X) / 2,
                    50
                );

                // black outline
                for (int dx = -2; dx <= 2; dx++)
                {
                    for (int dy = -2; dy <= 2; dy++)
                    {
                        if (dx != 0 || dy != 0)
                            _spriteBatch.DrawString(
                                _titleFont,
                                "Farm Parade",
                                titlePosition + new Vector2(dx, dy),
                                Color.Black
                            );
                    }
                }
                _spriteBatch.DrawString(_titleFont, "Farm Parade", titlePosition, new Color(255, 222, 33));
                Vector2 playButtonOrigin = new Vector2(playButtonTexture.Width / 2f, playButtonTexture.Height / 2f);

                Vector2 playButtonCenter = new Vector2(playButtonRect.X + playButtonRect.Width / 2f,playButtonRect.Y + playButtonRect.Height / 2f);
                _spriteBatch.Draw(playButtonTexture,playButtonCenter,null,Color.White,playButtonRotation,playButtonOrigin,1f,SpriteEffects.None,0f);
            }

            else if (currentGameState == GameState.Playing)
            {
                foreach (var a in animals)
                    a.Draw(gameTime, _spriteBatch);

                player.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
