using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject
{
    public class RoosterSprite : Animal
    {
        private Texture2D texture;
        public double directionTimer;
        private double animationTimer;
        private int animationFrame;
        public bool IsScared { get; private set; }
        private double scaredTimer;
        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Animals/Rooster_animation_without_shadow");
        }

        public override void Update(GameTime gameTime, int screenWidth, int screenHeight, Vector2 playerPosition)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            float speed = 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float distance = Vector2.Distance(Position, playerPosition);

            if (distance < 50 && !IsScared)
            {
                IsScared = true;
                scaredTimer = 1.5;

                Vector2 diff = Position - playerPosition;
                if (Math.Abs(diff.X) > Math.Abs(diff.Y))
                    Direction = diff.X > 0 ? Direction.Right : Direction.Left;
                else
                    Direction = diff.Y > 0 ? Direction.Down : Direction.Up;
            }

            if (IsScared)
            {
                scaredTimer -= elapsed;
                if (scaredTimer <= 0) IsScared = false;
            }
            else
            {
                directionTimer += elapsed;
                if (directionTimer > 1.0)
                {
                    Direction = (Direction)rng.Next(0, 4);
                    directionTimer = 0;
                }
            }

            Vector2 velocity = Vector2.Zero;
            switch (Direction)
            {
                case Direction.Up: velocity += new Vector2(0, -1); break;
                case Direction.Down: velocity += new Vector2(0, 1); break;
                case Direction.Left: velocity += new Vector2(-1, 0); break;
                case Direction.Right: velocity += new Vector2(1, 0); break;
            }
            Position += velocity * speed;
            int spriteSize = 64;
            Position.X = MathHelper.Clamp(Position.X, 0, screenWidth - spriteSize);
            Position.Y = MathHelper.Clamp(Position.Y, 0, screenHeight - spriteSize);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer > 0.2)
            {
                animationFrame++;
                if (animationFrame > 3) animationFrame = 1;
                animationTimer -= 0.2;
            }

            var source = new Rectangle(animationFrame * 32, (int)Direction * 32, 32, 32);
            spriteBatch.Draw(texture, Position, source, Color.White);
        }
    }
}
