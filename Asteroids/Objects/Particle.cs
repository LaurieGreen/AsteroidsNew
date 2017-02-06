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
        Model Texture;     // The texture that will be drawn to represent the particle
        Vector3 Position;    // The current position of the particle        
        Vector3 Velocity;        // The speed of the particle at the current instance
        //Color Color;           // The color of the particle
        //float Size;                // The size of the particle
        int TTL;              // The 'time to live' of the particle

        public Particle(Model texture, Vector3 position, Vector3 velocity, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            TTL = ttl;
        }

        public int getTTL()
        {
            return TTL;
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
