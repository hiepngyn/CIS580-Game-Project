using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using static GameProject.Animal;
using System.IO;
using System.Text.Json;
using MonoGame.Extended.Tiled;
///using SharpDX.Direct2D1;

namespace GameProject
{
    public enum GameState
    {
        TitleScreen,
        Dialog,
        Playing,
        Victory
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
        private MouseState previousMouseState;

        private Random rng = new Random();

        private float titleZoom = 0.1f;
        private float titleZoomSpeed = 1.2f;

        private float playButtonRotation = 0f;
        private bool isHoveringPlayButton = false;

        private Rectangle[] pens;
        private Type[] penAnimalTypes; // Track which animal type is in each pen

        private int currentLevel = 1;
        private const int MAX_LEVELS = 3;

        private Texture2D dialogTexture;
        private Texture2D victoryButtonTexture;
        private Rectangle victoryButtonRect;

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

            penAnimalTypes = new Type[pens.Length];

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
            MediaPlayer.Volume = 0.01f;
            MediaPlayer.Play(backgroundMusic);

            scaredSound = Content.Load<SoundEffect>("Sfx/Scared");

            // Victory button uses the start button texture
            victoryButtonTexture = startButtonTexture;

            // Initialize level (will be called when game starts)
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
                    currentGameState = GameState.Playing;
                    InitializeLevel(currentLevel);
                }
            }
            else if (currentGameState == GameState.Playing)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                // Check if animals enter pens and validate pen compatibility
                foreach (var a in animals)
                {
                    if (!a.IsInPen)
                    {
                        for (int i = 0; i < pens.Length; i++)
                        {
                            if (pens[i].Contains(a.Position.ToPoint()))
                            {
                                // Only assign to pen if it can accept this animal type
                                if (CanPenAcceptAnimal(i, a))
                                {
                                    a.AssignPen(pens[i]);
                                    penAnimalTypes[i] = a.GetType();
                                }
                                break;
                            }
                        }
                    }
                }

                // Update animals and player
                foreach (var a in animals) { a.Update(gameTime, windowWidth, windowHeight, player.Position); }
                player.Update(gameTime);

                // Check for level completion
                if (IsLevelComplete())
                {
                    currentGameState = GameState.Victory;

                    // Set up victory button
                    victoryButtonRect = new Rectangle(
                        windowWidth / 2 - 100,
                        windowHeight / 2 + 50,
                        200,
                        60
                    );
                }

                base.Update(gameTime);
            }
            else if (currentGameState == GameState.Victory)
            {
                // Handle victory button click
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    previousMouseState.LeftButton == ButtonState.Released &&
                    victoryButtonRect.Contains(mouseState.Position))
                {
                    if (currentLevel < MAX_LEVELS)
                    {
                        // Go to next level
                        currentLevel++;
                        currentGameState = GameState.Playing;
                        InitializeLevel(currentLevel);
                    }
                    else
                    {
                        // Last level - return to menu
                        currentLevel = 1;
                        currentGameState = GameState.TitleScreen;
                    }
                }
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
            else if (currentGameState == GameState.Playing)
            {
                // Draw animals and player
                if (animals != null)
                {
                    foreach (var a in animals)
                        a.Draw(gameTime, _spriteBatch);
                }
                player.Draw(gameTime, _spriteBatch);

                // Draw level indicator
                string levelText = $"Level {currentLevel}";
                Vector2 levelTextSize = _titleFont.MeasureString(levelText);
                Vector2 levelTextPos = new Vector2(windowWidth - levelTextSize.X - 20, 10);
                _spriteBatch.DrawString(_titleFont, levelText, levelTextPos + new Vector2(2, 2), Color.Black);
                _spriteBatch.DrawString(_titleFont, levelText, levelTextPos, Color.Yellow);
            }
            else if (currentGameState == GameState.Victory)
            {
                // Draw the playing field with animals in pens
                if (animals != null)
                {
                    foreach (var a in animals)
                        a.Draw(gameTime, _spriteBatch);
                }
                player.Draw(gameTime, _spriteBatch);

                _spriteBatch.End();

                // Draw semi-transparent overlay
                _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
                Texture2D overlay = CreateSolidTexture(GraphicsDevice, windowWidth, windowHeight, Color.Black);
                _spriteBatch.Draw(overlay, Vector2.Zero, Color.White * 0.6f);

                // Draw victory text
                string victoryText = "LEVEL COMPLETE!";
                Vector2 victoryTextSize = _titleFont.MeasureString(victoryText);
                Vector2 victoryTextPos = new Vector2(
                    (windowWidth - victoryTextSize.X) / 2,
                    windowHeight / 2 - 100
                );

                // Draw text shadow
                _spriteBatch.DrawString(_titleFont, victoryText, victoryTextPos + new Vector2(3, 3), Color.Black);
                _spriteBatch.DrawString(_titleFont, victoryText, victoryTextPos, Color.Yellow);

                _spriteBatch.End();

                // Draw button
                _spriteBatch.Begin(blendState: BlendState.AlphaBlend);

                // Draw button background
                Texture2D buttonBg = CreateSolidTexture(GraphicsDevice, victoryButtonRect.Width, victoryButtonRect.Height, Color.White);
                _spriteBatch.Draw(buttonBg, victoryButtonRect, Color.Green * 0.8f);

                // Draw button text
                string buttonText = currentLevel < MAX_LEVELS ? "Next Level" : "Return to Menu";
                Vector2 buttonTextSize = _titleFont.MeasureString(buttonText);
                Vector2 buttonTextPos = new Vector2(
                    victoryButtonRect.X + (victoryButtonRect.Width - buttonTextSize.X) / 2,
                    victoryButtonRect.Y + (victoryButtonRect.Height - buttonTextSize.Y) / 2
                );
                _spriteBatch.DrawString(_titleFont, buttonText, buttonTextPos + new Vector2(2, 2), Color.Black);
                _spriteBatch.DrawString(_titleFont, buttonText, buttonTextPos, Color.White);

                _spriteBatch.End();

                // Restart the main sprite batch for the remaining code
                _spriteBatch.Begin(
                    transformMatrix: transform,
                    samplerState: SamplerState.PointClamp,
                    blendState: BlendState.AlphaBlend
                );
            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Initialize level with appropriate number of animals
        /// </summary>
        private void InitializeLevel(int level)
        {
            currentLevel = level;

            // Reset pen animal types
            for (int i = 0; i < penAnimalTypes.Length; i++)
                penAnimalTypes[i] = null;

            // Number of each animal type increases with level
            int animalsPerType = level;
            var animalList = new List<Animal>();

            // Create animals for each type
            Type[] animalTypes = { typeof(SheepSprite), typeof(BullSprite), typeof(RoosterSprite), typeof(PigletSprite) };

            for (int type = 0; type < animalTypes.Length; type++)
            {
                for (int i = 0; i < animalsPerType; i++)
                {
                    Animal animal = animalTypes[type].Name switch
                    {
                        "SheepSprite" => new SheepSprite(scaredSound),
                        "BullSprite" => new BullSprite(scaredSound),
                        "RoosterSprite" => new RoosterSprite(scaredSound),
                        "PigletSprite" => new PigletSprite(scaredSound),
                        _ => null
                    };

                    if (animal != null)
                    {
                        animal.Position = new Vector2(rng.Next(400, windowWidth - 100), rng.Next(100, windowHeight - 100));
                        animal.LoadContent(Content);
                        animalList.Add(animal);
                    }
                }
            }

            animals = animalList.ToArray();
        }

        /// <summary>
        /// Check if a pen can accept this animal type
        /// </summary>
        private bool CanPenAcceptAnimal(int penIndex, Animal animal)
        {
            // If pen is empty, any animal can go in
            if (penAnimalTypes[penIndex] == null)
                return true;

            // If pen has animals, only same type can enter
            return penAnimalTypes[penIndex] == animal.GetType();
        }

        /// <summary>
        /// Check if all animals are captured in valid pens
        /// </summary>
        private bool IsLevelComplete()
        {
            if (animals == null || animals.Length == 0)
                return false;

            foreach (var animal in animals)
            {
                if (!animal.IsInPen)
                    return false;
            }

            return true;
        }

        private const string SaveFilePath = "savegame.json";

        private void SaveGame()
        {
            var saveData = new SaveData
            {
                PlayerPosition = player.Position,
                CurrentLevel = currentGameState.ToString(),
                LevelNumber = currentLevel
            };

            if (animals != null)
            {
                foreach (var a in animals)
                {
                    saveData.Animals.Add(new AnimalData
                    {
                        Type = a.GetType().Name,
                        Position = a.Position,
                        IsInPen = a.IsInPen
                    });
                }
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

            // Restore level number
            currentLevel = saveData.LevelNumber > 0 ? saveData.LevelNumber : 1;

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

        /// <summary>
        /// Creates a radial gradient texture for lighting effect (no external assets needed)
        /// </summary>
        private Texture2D CreateRadialGradient(GraphicsDevice device, int radius, Color center, Color edge)
        {
            int diameter = radius * 2;
            Texture2D texture = new Texture2D(device, diameter, diameter);
            Color[] data = new Color[diameter * diameter];

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    float normalizedDistance = Math.Min(distance / radius, 1f);

                    // Smooth gradient falloff
                    float alpha = 1f - normalizedDistance;
                    alpha = alpha * alpha; // Quadratic falloff for smoother effect

                    data[y * diameter + x] = Color.White * alpha;
                }
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates a simple circle texture for dust particles (no external assets needed)
        /// </summary>
        private Texture2D CreateCircleTexture(GraphicsDevice device, int radius, Color color)
        {
            int diameter = radius * 2;
            Texture2D texture = new Texture2D(device, diameter, diameter);
            Color[] data = new Color[diameter * diameter];

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (distance <= radius)
                    {
                        float alpha = 1f - (distance / radius);
                        data[y * diameter + x] = color * alpha;
                    }
                    else
                    {
                        data[y * diameter + x] = Color.Transparent;
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Creates a solid color texture (for darkness overlay)
        /// </summary>
        private Texture2D CreateSolidTexture(GraphicsDevice device, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(device, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;
            texture.SetData(data);
            return texture;
        }
    }
}
