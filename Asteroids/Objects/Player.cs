using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Asteroids
{
    public class Player
    {
        public Model CurrentTexture;
        public ParticleEngine engineParticle, explosionParticle;
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 VelocityAdd;
        public float Rotation;
        public Matrix[] Transforms;
        public Matrix shipTransformMatrix;
        public Matrix RotationMatrix;
        public int score;
        public bool isActive;
        public bool isSpawning;
        public bool hasScored;
        Random random;
        float flashTimer;
        bool isVisible;
        float lastTime;
        public bool isInvulnerable;

        //consts
        public int lives = GameConstants.NumLives;
        public float multiplier = GameConstants.MultiplierStart;
        float spawnTimer = GameConstants.SpawnTimer;

        public Player(Model currentTexture, Vector3 position, Vector3 velocity, Camera camera, List<Model> particleModel)
        {
            CurrentTexture = currentTexture;
            engineParticle = new ParticleEngine(particleModel);
            explosionParticle = new ParticleEngine(particleModel);
            Position = position;
            Velocity = velocity;
            isActive = true;
            hasScored = false;
            isSpawning = true;
            isVisible = true;
            flashTimer = 0.15f;
            random = new Random();
            score = 0;
            Transforms = camera.SetupEffectDefaults(CurrentTexture, camera);
            //cheat
            isInvulnerable = false;
        }

        public Vector3 getPosition()
        {
            return Position;
        }

        public void Update(KeyboardState state, Model bulletModel, Camera camera, float timeDelta, SoundEffectInstance engineInstance)
        {
            spawnTimer -= timeDelta;
            if (multiplier > 1)
            {
                multiplier -= timeDelta * 5;
            }
            else
            {
                multiplier = 1;
            }

            if (spawnTimer <= 0.0f)
            {
                isSpawning = false;
                isVisible = true;
            }
            else
            {
                isSpawning = true;
            }

            if (hasScored)
            {
                score = score + (10*(int)multiplier);
                hasScored = false;
            }

            if (!isActive)
            {
                explosion(camera);
                lives--;
                Reset();
            }


            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
            {
                Rotation += 0.095f;
            }

            if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
            {
                Rotation -= 0.095f;
            }
            RotationMatrix = Matrix.CreateRotationZ(Rotation);

            Vector3 VelocityAdd = Vector3.Zero;
            VelocityAdd.X = (float)Math.Sin(Rotation);
            VelocityAdd.Y = -(float)Math.Cos(Rotation);
            VelocityAdd /= 75;

            if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
            {
                Velocity += VelocityAdd;
                engineParticle.particles.Add(engineParticle.GenerateNewParticle(Position + (-0.725f * RotationMatrix.Up), Velocity * 0.5f, camera));
                if (engineInstance.State == SoundState.Stopped)
                {
                    engineInstance.Volume = 0.75f;
                    engineInstance.IsLooped = true;
                    engineInstance.Play();
                }
                else
                    engineInstance.Resume();
            }
            else
            {
                if (engineInstance.State == SoundState.Playing)
                {
                    engineInstance.Volume *= 0.75f;
                    if (engineInstance.State == SoundState.Playing && engineInstance.Volume < 0.05f)
                    {
                        engineInstance.Pause();
                        engineInstance.Volume = 0.75f;
                    }
                }

            }
            engineParticle.Update();
            explosionParticle.Update();

            Position -= Velocity;
            Velocity *= 0.999f;
            if (Position.X > GameConstants.PlayfieldSizeX)
                Position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (Position.X < -GameConstants.PlayfieldSizeX)
                Position.X += 2 * GameConstants.PlayfieldSizeX;
            if (Position.Y > GameConstants.PlayfieldSizeY)
                Position.Y -= 2 * GameConstants.PlayfieldSizeY;
            if (Position.Y < -GameConstants.PlayfieldSizeY)
                Position.Y += 2 * GameConstants.PlayfieldSizeY;

        }

        private void explosion(Camera camera)
        {
            int total = 25;
            for (int j = 0; j < total; j++)
            {
                Vector3 velocity = new Vector3(1f * (float)(random.NextDouble() * 2 - 1), 1f * (float)(random.NextDouble() * 2 - 1), 0);
                explosionParticle.particles.Add(explosionParticle.GenerateNewParticle(Position, Vector3.Multiply(velocity, 0.1f), camera));
            }
        }

        public void Reset()
        {
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Rotation = 0.0f;
            isActive = true;
            isSpawning = true;
            isVisible = true;
            spawnTimer = 3f;            
        }

        public void Draw(Camera camera, float gameTime)
        {
            if ((gameTime - lastTime) > flashTimer && isVisible == false && isSpawning == true) 
            {
                isVisible = true;
                lastTime = gameTime;
            }
            else if ((gameTime - lastTime) > flashTimer && isVisible == true && isSpawning == true)
            {
                isVisible = false;
                lastTime = gameTime;
            }

            if (isVisible)
            {
                shipTransformMatrix = RotationMatrix * Matrix.CreateTranslation(Position);
                camera.DrawModel(CurrentTexture, shipTransformMatrix, Transforms, camera, new Vector3(0,0,255));
            }
            engineParticle.Draw(camera);
            explosionParticle.Draw(camera);
        }
    }
}
