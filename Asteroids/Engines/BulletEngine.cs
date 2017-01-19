using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Asteroids
{
    public class BulletEngine
    {
        Model texture;
        public List<Bullet> bullets;
        Matrix[] bulletTransforms;

        public BulletEngine(Model texture, Camera camera)
        {
            this.texture = texture;
            this.bullets = new List<Bullet>(GameConstants.NumBullets);
            bulletTransforms = camera.SetupEffectDefaults(texture, camera);
        }

        private Bullet GenerateNewBullet(Vector3 direction, float velocity, Vector3 position, Camera camera, int ttl)
        {
            return new Bullet(texture, velocity, direction,  position, camera, ttl);
        }

        public void Update(KeyboardState state, KeyboardState lastState, Vector3 direction, float velocity, Vector3 position, Camera camera, float timeDelta, SoundEffect laserSound)
        {
           if ((state.IsKeyDown(Keys.Space)) && lastState.IsKeyDown(Keys.Space) == false)
           {
               for (int index = 0; index < GameConstants.NumBullets; index++)
               {
                   int ttl = 150; 
                   bullets.Add(GenerateNewBullet(direction, velocity,position, camera, ttl));
                   //float volume = 0.75f;
                   laserSound.Play(0.05f, 0, 0);
               }
           }
           for (int i = 0; i < bullets.Count; i++)
           {
                bullets[i].Update(timeDelta);
                if (bullets[i].TTL <= 0 || bullets[i].isActive == false)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
           }
        }

        public void Draw(Camera camera)
        {
            for (int index = 0; index < bullets.Count; index++)
            {
                bullets[index].Draw(camera, bulletTransforms);
            }
        }
    }
}
