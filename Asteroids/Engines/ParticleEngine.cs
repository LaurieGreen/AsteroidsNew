using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    public class ParticleEngine
    {
        private Random random;
        //public Vector3 EmitterLocation { get; set; }
        public List<Particle> particles;
        private List<Model> textures;
        

        public ParticleEngine(List<Model> textures)
        {
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }

        public Particle GenerateNewParticle(Vector3 position, Vector3 velocity, Camera camera)
        {
            Model texture = textures[random.Next(textures.Count)];
            //Vector3 position = player.Position + (-0.725f*player.RotationMatrix.Up);
            //Vector3 velocity = player.Velocity*0.5f;
            int ttl = 20 + random.Next(40);
            return new Particle(texture, position, velocity, ttl);
        }

        //public void Update(Vector3 position, Vector3 velocity, KeyboardState state, Camera camera)
        public void Update()
        {
            //int total = 5;

            //for (int i = 0; i < total; i++)
            //{
            //    if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
            //    {
            //        particles.Add(GenerateNewParticle(position, velocity, camera));
            //    }
            //}
            

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].getTTL() <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }            
        }

        public void Draw(Camera camera)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(camera);
            }
        }
    }
}
