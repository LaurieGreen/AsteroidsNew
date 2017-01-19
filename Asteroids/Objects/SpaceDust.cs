using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class SpaceDust
    {
        public Model Texture { get; set; }        // The texture that will be drawn to represent the particle
        public Vector3 Position { get; set; }        // The current position of the particle        
        public Vector3 Velocity { get; set; }        // The speed of the particle at the current instance
        float maxX, maxY;

        public SpaceDust(Model texture, float width, float height, Random random)
        {
            Texture = texture;
            maxX = width;
            maxY = height;
            Position = new Vector3(random.Next((int)Math.Round(width)), random.Next((int)Math.Round(height)), 0);
            Velocity = new Vector3 (random.Next(2), random.Next(2),0);
        }

        public void Update()
        {
            Position -= Velocity;

            //respawn space dust
            if (Position.X < 0)
            {
                //Position = new Vector3(maxX, maxY, 0);
                //Random generator = new Random();
                //y = generator.nextInt(maxY);
                //speed = generator.nextInt(15);
            }
        }

        public void Draw(Camera camera)
        {
            Matrix world = Matrix.CreateBillboard(Position, camera.Position, Vector3.Up, camera.Target - camera.Position);
            camera.DrawSpaceDust(Texture, camera, world);
        }
    }
}
