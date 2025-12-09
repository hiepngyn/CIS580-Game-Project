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

    public enum Direction
    {
        Down = 0,
        Up = 1,
        Left = 2,
        Right = 3
    }
    public abstract class Animal
    {
        public Vector2 Position;
        public Direction Direction;

        protected static Random rng = new Random();

        public Rectangle? PenBounds { get; private set; } = null;
        public bool IsInPen => PenBounds != null;

        // Improved herding fields
        public bool IsScared { get; protected set; }
        protected Vector2 velocity = Vector2.Zero;
        protected double idleTimer = 0;
        protected const float SCARE_RADIUS = 120f;
        protected const float PANIC_RADIUS = 60f;
        protected const float WALL_AVOID_DISTANCE = 60f;
        protected const float ACCELERATION = 180f;
        protected const float MAX_SPEED = 120f;
        protected const float FRICTION = 0.92f;
        protected const float IDLE_SPEED = 40f;

        public void AssignPen(Rectangle pen)
        {
            PenBounds = pen;
        }

        /// <summary>
        /// Improved movement using potential field approach
        /// </summary>
        protected Vector2 CalculateFleeDirection(Vector2 playerPosition, int screenWidth, int screenHeight)
        {
            Vector2 desiredDirection = Vector2.Zero;
            Vector2 toPlayer = Position - playerPosition;
            float distanceToPlayer = toPlayer.Length();

            // Flee from player with gradient (closer = stronger)
            if (distanceToPlayer < SCARE_RADIUS && distanceToPlayer > 0)
            {
                IsScared = true;
                Vector2 fleeDirection = Vector2.Normalize(toPlayer);

                // Stronger panic when very close
                float panicMultiplier = distanceToPlayer < PANIC_RADIUS ? 1.5f : 1.0f;
                float fleeStrength = (1.0f - (distanceToPlayer / SCARE_RADIUS)) * panicMultiplier * 0.6f; // Reduced strength

                desiredDirection += fleeDirection * fleeStrength;
            }
            else
            {
                IsScared = false;
            }

            // Wall avoidance using potential field
            Vector2 wallAvoidance = CalculateWallAvoidance(screenWidth, screenHeight);
            desiredDirection += wallAvoidance;

            // Pen boundary avoidance when not in pen
            if (PenBounds == null)
            {
                Vector2 penAvoidance = CalculatePenAvoidance();
                desiredDirection += penAvoidance;
            }

            return desiredDirection;
        }

        /// <summary>
        /// Calculate force to avoid walls
        /// </summary>
        protected Vector2 CalculateWallAvoidance(int screenWidth, int screenHeight)
        {
            Vector2 avoidance = Vector2.Zero;
            float spriteSize = 64f;

            // Left wall
            if (Position.X < WALL_AVOID_DISTANCE)
            {
                float force = (WALL_AVOID_DISTANCE - Position.X) / WALL_AVOID_DISTANCE;
                avoidance.X += force * 1.0f; // Reduced from 2
            }
            // Right wall
            if (Position.X > screenWidth - spriteSize - WALL_AVOID_DISTANCE)
            {
                float force = (WALL_AVOID_DISTANCE - (screenWidth - spriteSize - Position.X)) / WALL_AVOID_DISTANCE;
                avoidance.X -= force * 1.0f; // Reduced from 2
            }
            // Top wall
            if (Position.Y < WALL_AVOID_DISTANCE)
            {
                float force = (WALL_AVOID_DISTANCE - Position.Y) / WALL_AVOID_DISTANCE;
                avoidance.Y += force * 1.0f; // Reduced from 2
            }
            // Bottom wall
            if (Position.Y > screenHeight - spriteSize - WALL_AVOID_DISTANCE)
            {
                float force = (WALL_AVOID_DISTANCE - (screenHeight - spriteSize - Position.Y)) / WALL_AVOID_DISTANCE;
                avoidance.Y -= force * 1.0f; // Reduced from 2
            }

            return avoidance;
        }

        /// <summary>
        /// Calculate force to avoid pen boundaries (when not captured)
        /// </summary>
        protected Vector2 CalculatePenAvoidance()
        {
            // This can be extended to avoid pen areas
            // For now, returning zero
            return Vector2.Zero;
        }

        /// <summary>
        /// Update animal direction based on velocity
        /// </summary>
        protected void UpdateDirection()
        {
            if (velocity.Length() > 10f) // Only update if moving significantly
            {
                if (Math.Abs(velocity.X) > Math.Abs(velocity.Y))
                    Direction = velocity.X > 0 ? Direction.Right : Direction.Left;
                else
                    Direction = velocity.Y > 0 ? Direction.Down : Direction.Up;
            }
        }

        public abstract void LoadContent(ContentManager content);
        public abstract void Update(GameTime gameTime, int screenWidth, int screenHeight, Vector2 player);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}