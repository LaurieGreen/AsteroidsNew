using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public class Particle
    {
        public Model Texture { get; set; }        // The texture that will be drawn to represent the particle
        public Vector3 Position { get; set; }        // The current position of the particle        
        public Vector3 Velocity { get; set; }        // The speed of the particle at the current instance
        public Color Color { get; set; }            // The color of the particle
        public float Size { get; set; }                // The size of the particle
        public int TTL { get; set; }                // The 'time to live' of the particle

        public Particle(Model texture, Vector3 position, Vector3 velocity,
                float angle, float angularVelocity, int ttl, Camera camera)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            TTL = ttl;
        }

        public void Update()
        {
            TTL--;
            Position += Velocity;
        }

        public void Draw(Camera camera)
        {
            Matrix world = Matrix.CreateBillboard(Position, camera.Position, Vector3.Up, camera.Target - camera.Position);
            camera.DrawParticle(Texture,camera, world);
        }
    }
}
