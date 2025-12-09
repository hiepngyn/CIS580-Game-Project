using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject
{
    public class DustParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Lifetime;
        public float MaxLifetime;

        public DustParticle(Vector2 pos, Vector2 vel, float lifetime)
        {
            Position = pos;
            Velocity = vel;
            Lifetime = MaxLifetime = lifetime;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * dt;
            Lifetime -= dt;
        }

        public void Draw(SpriteBatch sb, Texture2D texture)
        {
            float alpha = Lifetime / MaxLifetime * 0.6f;
            sb.Draw(texture, Position, Color.White * alpha);
        }

        public bool IsDead => Lifetime <= 0;
    }
}
