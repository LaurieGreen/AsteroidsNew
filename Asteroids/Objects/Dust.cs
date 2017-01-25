using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class Dust
    {
        public Texture2D Texture;        // The texture that will be drawn to represent the particle
        public Vector2 Position;       // The current position of the particle        
        public Vector2 Velocity;     // The speed of the particle at the current instance

        public Dust(Texture2D texture, float x, float y, float x2, float y2, Random random)
        {
            Texture = texture;
            Position = new Vector2(x, y);
            Velocity = new Vector2 (x2, y2);
            //System.Console.WriteLine("position: " + Position);
        }

        public void Update(GraphicsDevice graphicsDevice)
        {
            Position -= Velocity;
            ////System.Console.WriteLine("update: " + Position);

            ////respawn space dust
            if (Position.X > (graphicsDevice.Viewport.Width * 2))
                Position.X = -graphicsDevice.Viewport.Width;
            if (Position.X < -graphicsDevice.Viewport.Width)
                Position.X = 2*graphicsDevice.Viewport.Width;
            if (Position.Y > (graphicsDevice.Viewport.Height * 2))
                Position.Y = -graphicsDevice.Viewport.Height;
            if (Position.Y < -graphicsDevice.Viewport.Height)
                Position.Y = 2 * graphicsDevice.Viewport.Height;
        }

        public void Draw(SpriteBatch spriteBatch, float opacity)
        {
            //Matrix world = Matrix.CreateBillboard(Position, camera.Position, Vector3.Up, camera.Target - camera.Position);
            //camera.DrawSpaceDust(Texture, camera, world);
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, new Vector2(Position.X, Position.Y), Color.White *opacity);
            spriteBatch.End();
        }
    }
}
