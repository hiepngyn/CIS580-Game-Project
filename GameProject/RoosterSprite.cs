using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace GameProject
{
    public class RoosterSprite : Animal
    {
        private Texture2D texture;
        private double animationTimer;
        private int animationFrame;
        private SoundEffect scaredSound;
        private bool wasScared = false;

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Animals/Rooster_animation_without_shadow");
        }

        public RoosterSprite(SoundEffect scaredSound)
        {
            this.scaredSound = scaredSound;
        }

        public override void Update(GameTime gameTime, int screenWidth, int screenHeight, Vector2 playerPosition)
        {
            if (IsInPen) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate desired movement direction using potential field
            Vector2 desiredDirection = CalculateFleeDirection(playerPosition, screenWidth, screenHeight);

            // If scared, accelerate in desired direction
            if (IsScared)
            {
                // Play sound when first scared
                if (!wasScared)
                {
                    scaredSound?.Play();
                    wasScared = true;
                }

                // Accelerate toward desired direction
                if (desiredDirection.Length() > 0)
                {
                    desiredDirection.Normalize();
                    velocity += desiredDirection * ACCELERATION * dt;

                    // Limit to max speed
                    if (velocity.Length() > MAX_SPEED)
                    {
                        velocity.Normalize();
                        velocity *= MAX_SPEED;
                    }
                }
            }
            else
            {
                wasScared = false;

                // Idle wandering behavior
                idleTimer += dt;
                if (idleTimer > 1.5) // Change direction every 1.5 seconds
                {
                    // Random direction for idle movement
                    Vector2 randomDir = new Vector2(
                        (float)(rng.NextDouble() - 0.5),
                        (float)(rng.NextDouble() - 0.5)
                    );
                    if (randomDir.Length() > 0)
                    {
                        randomDir.Normalize();
                        velocity = randomDir * IDLE_SPEED;
                    }
                    idleTimer = 0;
                }

                // Gradual slowdown
                velocity *= 0.98f;
            }

            // Update position
            Position += velocity * dt;

            // Update facing direction
            UpdateDirection();

            // Keep in bounds
            int spriteSize = 32; // Rooster sprite is 32x32
            if (PenBounds != null)
            {
                Rectangle pen = PenBounds.Value;
                Position.X = MathHelper.Clamp(Position.X, pen.Left, pen.Right - spriteSize);
                Position.Y = MathHelper.Clamp(Position.Y, pen.Top, pen.Bottom - spriteSize);
            }
            else
            {
                Position.X = MathHelper.Clamp(Position.X, 0, screenWidth - spriteSize);
                Position.Y = MathHelper.Clamp(Position.Y, 0, screenHeight - spriteSize);
            }

            // Stop velocity if hit wall
            if (Position.X <= 0 || Position.X >= screenWidth - spriteSize)
                velocity.X = 0;
            if (Position.Y <= 0 || Position.Y >= screenHeight - spriteSize)
                velocity.Y = 0;
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
            Color drawColor = IsScared ? Color.Lerp(Color.White, Color.Red, 0.5f) * 0.6f : Color.White;
            spriteBatch.Draw(texture, Position, source, drawColor);
        }
    }
}
