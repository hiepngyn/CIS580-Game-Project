using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject
{
    public class ParticleSystem
    {
        private readonly List<DustParticle> particles = new();
        private readonly Texture2D texture;
        private readonly Rectangle spawnArea;
        private readonly Random rng = new();

        public ParticleSystem(Texture2D tex, Rectangle area)
        {
            texture = tex;
            spawnArea = area;
        }

        public void Update(GameTime gameTime)
        {
            // spawn new particles
            if (particles.Count < 100)
            {
                var pos = new Vector2(spawnArea.X + rng.Next(spawnArea.Width), spawnArea.Y + rng.Next(spawnArea.Height));
                var vel = new Vector2(rng.Next(-20, -5), rng.Next(-5, 5));
                particles.Add(new DustParticle(pos, vel, 5f + (float)rng.NextDouble() * 3f));
            }

            // update and remove dead ones
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(gameTime);
                if (particles[i].IsDead) particles.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var p in particles)
                p.Draw(sb, texture);
        }
    }
}