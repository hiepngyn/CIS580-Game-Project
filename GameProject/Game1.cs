using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static GameProject.Animal;

namespace GameProject
{
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
        /// Tiles of the background layout
        /// </summary>
        int[,] animalPaddock = new int[,]
        {
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0}
        };

        /// <summary>
        /// Textures used for background
        /// </summary>
        private Texture2D grassTile;
        private Texture2D fenceTile;
        private Texture2D hillTile;
        private Texture2D waterTile;

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

            animals = new Animal[] {
                new SheepSprite(){ Position = new Vector2(100,100)},
                new BullSprite(){ Position = new Vector2(200,100)},
                new RoosterSprite(){ Position = new Vector2(300,100)},
                new PigletSprite(){ Position = new Vector2(400,100)}
            };

            player = new PlayerSprite();

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

            grassTile = Content.Load<Texture2D>("Background/Grass");
            fenceTile = Content.Load<Texture2D>("Background/Fences");
            hillTile = Content.Load<Texture2D>("Background/Hills");
            waterTile = Content.Load<Texture2D>("Background/Water");

            foreach (var a in animals) { a.LoadContent(Content); }
            player.LoadContent(Content);
        }

        /// <summary>
        /// Updating state of the game
        /// </summary>
        /// <param name="gameTime">In game time</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            foreach (var a in animals) {a.Update(gameTime); }
            player.Update(gameTime);
               
            base.Update(gameTime);
        }

        /// <summary>
        /// Drawing game 
        /// </summary>
        /// <param name="gameTime">Ingame time</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            DrawLayout(_spriteBatch, animalPaddock, 64);
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
                        _spriteBatch.DrawString(_titleFont, "Farm Parade", titlePosition + new Vector2(dx, dy), Color.Black);
                }
            }
            _spriteBatch.DrawString(_titleFont, "Farm Parade", titlePosition, new Color(255, 222, 33));

            foreach (var a in animals) a.Draw(gameTime, _spriteBatch);

            player.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();


            base.Draw(gameTime);
        }

        /// <summary>
        /// Used to draw the background of the game
        /// </summary>
        /// <param name="spriteBatch">Drawing</param>
        /// <param name="layout">Array we are using to draw</param>
        /// <param name="tileSize">Size of the tiles being used</param>
        private void DrawLayout(SpriteBatch spriteBatch, int[,] layout, int tileSize)
        {
            for (int y = 0; y < layout.GetLength(0); y++)
            {
                for (int x = 0; x < layout.GetLength(1); x++)
                {
                    Texture2D tex = grassTile;
                    switch (layout[y, x])
                    {
                        case 1: tex = fenceTile; break;
                        case 2: tex = hillTile; break;
                        case 3: tex = waterTile; break;
                    }

                    spriteBatch.Draw(tex, new Vector2(x * tileSize, y * tileSize), Color.White);
                    
                }
            }
        }

    }
}
