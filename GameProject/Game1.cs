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
                new SheepSprite(scaredSound){ Position = new Vector2(100,100)},
                new BullSprite(scaredSound){ Position = new Vector2(200,100)},
                new RoosterSprite(scaredSound){ Position = new Vector2(300,100)},
                new PigletSprite(scaredSound){ Position = new Vector2(400,100)}
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

                // TODO: Add your update logic here

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

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_background, Vector2.Zero, Color.White);

            if (currentGameState == GameState.TitleScreen)
            {
                Vector2 titleSize = _titleFont.MeasureString("Farm Parade");
                Vector2 titlePosition = new Vector2(
                    (_graphics.PreferredBackBufferWidth - titleSize.X) / 2,
                    50
                );

                for (int dx = -2; dx <= 2; dx++)
                {
                    for (int dy = -2; dy <= 2; dy++)
                    {
                        if (dx != 0 || dy != 0)
                            _spriteBatch.DrawString(_titleFont, "Farm Parade",
                                titlePosition + new Vector2(dx, dy), Color.Black);
                    }
                }
                _spriteBatch.DrawString(_titleFont, "Farm Parade", titlePosition, new Color(255, 222, 33));

                Vector2 playButtonPos = new Vector2(
                    (_graphics.PreferredBackBufferWidth - playButtonTexture.Width) / 2,
                    250
                );
                _spriteBatch.Draw(playButtonTexture, playButtonRect, Color.White);
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
