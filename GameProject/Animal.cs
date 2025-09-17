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


        public abstract void LoadContent(ContentManager content);
        public abstract void Update(GameTime gameTime, int screenWidth, int screenHeight, Vector2 player);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}