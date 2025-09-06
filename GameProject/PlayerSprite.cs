using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    public enum PlayerDirection
    {
        Down = 0,
        Up = 1,
        Left = 2,
        Right = 3
    }
    public class PlayerSprite
    {
        private Texture2D texture;
        private double animationTimer;
        private int animationFrame;

        public Vector2 Position = new Vector2(200, 200);
        public float Speed = 150f;
        public PlayerDirection Direction = PlayerDirection.Down;

        private const int FrameWidth = 48;
        private const int FrameHeight = 48;

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Charakter");
        }

        public void Update(GameTime gameTime)
        {
            var kb = Keyboard.GetState();
            Vector2 movement = Vector2.Zero;

            if (kb.IsKeyDown(Keys.W)) { movement.Y -= 1; Direction = PlayerDirection.Up; }
            if (kb.IsKeyDown(Keys.S)) { movement.Y += 1; Direction = PlayerDirection.Down; }
            if (kb.IsKeyDown(Keys.A)) { movement.X -= 1; Direction = PlayerDirection.Left; }
            if (kb.IsKeyDown(Keys.D)) { movement.X += 1; Direction = PlayerDirection.Right; }

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                Position += movement * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (animationTimer > 0.2)
                {
                    animationFrame = (animationFrame + 1) % 4;
                    animationTimer -= 0.2;
                }
            }
            else
            {
                animationFrame = 0;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var source = new Rectangle(animationFrame * FrameWidth, (int)Direction * FrameHeight, FrameWidth, FrameHeight);

            spriteBatch.Draw(texture, Position, source, Color.White, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
        }
    }
}
