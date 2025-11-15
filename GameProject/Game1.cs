using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using static GameProject.Animal;
using System.IO;
using System.Text.Json;
///using SharpDX.Direct2D1;

namespace GameProject
{
    public enum GameState
    {
        TitleScreen,
        Dialog,
        LevelOne
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

        private Texture2D dialogTexture;

        private Texture2D menuBoxTexture;
        private Texture2D startButtonTexture;
        private Rectangle startButtonRect;

        private Vector2 menuPosition;
        private float menuScale = 0.6f;

        _3D theShape;
        private Texture2D starTexture;
        private Vector2 starPosition;
        private float starScale = 0.6f;
        private Rectangle cubeViewportRect;

        private SoundEffect cubeClickSound;


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
            menuBoxTexture = Content.Load<Texture2D>("UI/menu");
            startButtonTexture = Content.Load<Texture2D>("UI/start");
            dialogTexture = Content.Load<Texture2D>("DialogBox/testing");
            starTexture = Content.Load<Texture2D>("UI/star");
            cubeClickSound = Content.Load<SoundEffect>("Sfx/TheSound");

            theShape = new _3D(this);
            starPosition = new Vector2(150, 200); 

            int scaledStarWidth = (int)(starTexture.Width * starScale);
            int scaledStarHeight = (int)(starTexture.Height * starScale);

            cubeViewportRect = new Rectangle(
                (int)(starPosition.X + scaledStarWidth * 0.23f),
                (int)(starPosition.Y + scaledStarHeight * 0.23f),
                (int)(scaledStarWidth * 0.55f),
                (int)(scaledStarHeight * 0.55f)
            );

            menuPosition = new Vector2(
                _graphics.PreferredBackBufferWidth * 0.7f - (menuBoxTexture.Width * menuScale) / 2f,
                (_graphics.PreferredBackBufferHeight - menuBoxTexture.Height * menuScale) / 2f
            );

            startButtonRect = new Rectangle(
                (int)(menuPosition.X + (menuBoxTexture.Width * menuScale - startButtonTexture.Width * menuScale) / 2f),
                (int)(menuPosition.Y + (menuBoxTexture.Height * menuScale - startButtonTexture.Height * menuScale) / 2f),
                (int)(startButtonTexture.Width * menuScale),
                (int)(startButtonTexture.Height * menuScale)
            );
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
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
                SaveGame();

            if (Keyboard.GetState().IsKeyDown(Keys.F9))
                LoadGame();


            var mouseState = Mouse.GetState();

            if (currentGameState == GameState.TitleScreen)
            {
                theShape.Update(gameTime);

                if (titleZoom < 1f)
                    titleZoom += (float)(titleZoomSpeed * gameTime.ElapsedGameTime.TotalSeconds);
                else
                    titleZoom = 1f;

                isHoveringPlayButton = startButtonRect.Contains(mouseState.Position);

                if (mouseState.LeftButton == ButtonState.Pressed &&
                    previousMouseState.LeftButton == ButtonState.Released &&
                    startButtonRect.Contains(mouseState.Position))
                {
                    currentGameState = GameState.Dialog;
                }

                if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                {
                    float scale = titleZoom;

                    Rectangle scaledCubeRect = new Rectangle(
                        (int)(cubeViewportRect.X * scale),
                        (int)(cubeViewportRect.Y * scale),
                        (int)(cubeViewportRect.Width * scale),
                        (int)(cubeViewportRect.Height * scale)
                    );

                    if (scaledCubeRect.Contains(mouseState.Position))
                    {
                        cubeClickSound.Play();
                    }
                }
            }
            else if (currentGameState == GameState.Dialog)
            {
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    previousMouseState.LeftButton == ButtonState.Released ||
                    (Keyboard.GetState().IsKeyDown(Keys.Enter) &&
                    !Keyboard.GetState().IsKeyDown(Keys.LeftShift)))
                {
                    currentGameState = GameState.LevelOne;
                }
            }
            else if (currentGameState == GameState.LevelOne)
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

            // Draw background
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

                _spriteBatch.Begin(transformMatrix: transform);
                _spriteBatch.Draw(
                    starTexture,
                    starPosition,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    starScale,
                    SpriteEffects.None,
                    0f
                );
                _spriteBatch.End();

                var oldViewport = GraphicsDevice.Viewport;

                GraphicsDevice.Viewport = new Viewport(
                    cubeViewportRect.X,
                    cubeViewportRect.Y,
                    cubeViewportRect.Width,
                    cubeViewportRect.Height
                );

                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                theShape.Draw();
                GraphicsDevice.Viewport = oldViewport;
            }

            _spriteBatch.Begin(
                transformMatrix: transform,
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend
            );

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

                _spriteBatch.DrawString(
                    _titleFont,
                    "Farm Parade",
                    titlePosition,
                    Color.Yellow
                );


                _spriteBatch.Draw(
                    menuBoxTexture,
                    menuPosition,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    menuScale,
                    SpriteEffects.None,
                    0f
                );

                Vector2 startCenter = new Vector2(
                    startButtonRect.X + startButtonRect.Width / 2f,
                    startButtonRect.Y + startButtonRect.Height / 2f
                );
                Vector2 startOrigin = new Vector2(
                    startButtonTexture.Width / 2f,
                    startButtonTexture.Height / 2f
                );

                _spriteBatch.Draw(
                    startButtonTexture,
                    startCenter,
                    null,
                    Color.White,
                    playButtonRotation,
                    startOrigin,
                    menuScale,
                    SpriteEffects.None,
                    0f
                );
            }
            else if (currentGameState == GameState.Dialog)
            {
                Vector2 dialogPos = new Vector2(
                    (_graphics.PreferredBackBufferWidth - dialogTexture.Width) / 2,
                    _graphics.PreferredBackBufferHeight - dialogTexture.Height - 50
                );
                _spriteBatch.Draw(dialogTexture, dialogPos, Color.White);
            }
            else if (currentGameState == GameState.LevelOne)
            {
                foreach (var a in animals)
                    a.Draw(gameTime, _spriteBatch);

                player.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private const string SaveFilePath = "savegame.json";

        private void SaveGame()
        {
            var saveData = new SaveData
            {
                PlayerPosition = player.Position,
                CurrentLevel = currentGameState.ToString()
            };

            foreach (var a in animals)
            {
                saveData.Animals.Add(new AnimalData
                {
                    Type = a.GetType().Name,
                    Position = a.Position,
                    IsInPen = a.IsInPen
                });
            }

            string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFilePath, json);
        }

        private void LoadGame()
        {
            if (!File.Exists(SaveFilePath))
                return;

            string json = File.ReadAllText(SaveFilePath);
            var saveData = JsonSerializer.Deserialize<SaveData>(json);

            // Restore player
            player.Position = saveData.PlayerPosition;

            // Restore animals
            animals = new Animal[saveData.Animals.Count];
            for (int i = 0; i < saveData.Animals.Count; i++)
            {
                var ad = saveData.Animals[i];
                SoundEffect sfx = scaredSound;
                Animal animal = ad.Type switch
                {
                    "SheepSprite" => new SheepSprite(sfx),
                    "BullSprite" => new BullSprite(sfx),
                    "RoosterSprite" => new RoosterSprite(sfx),
                    "PigletSprite" => new PigletSprite(sfx),
                    _ => null
                };
                if (animal != null)
                {
                    animal.Position = ad.Position;
                    if (ad.IsInPen)
                    {
                        animal.AssignPen(new Rectangle((int)ad.Position.X, (int)ad.Position.Y, 100, 100));
                    }
                    animals[i] = animal;
                }
            }

            foreach (var a in animals)
                a.LoadContent(Content);

            currentGameState = Enum.Parse<GameState>(saveData.CurrentLevel);
        }
    }
}
