using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static GameProject.Animal;

namespace GameProject
{
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

        private PlayerSprite player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

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

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _titleFont = Content.Load<SpriteFont>("TitleFont");

            foreach(var a in animals) { a.LoadContent(Content); }
            player.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            foreach (var a in animals) {a.Update(gameTime); }
            player.Update(gameTime);
               
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.DrawString(_titleFont, "Farm Parade", new Vector2(40, 30), Color.White);

            foreach (var a in animals) a.Draw(gameTime, _spriteBatch);

            player.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
