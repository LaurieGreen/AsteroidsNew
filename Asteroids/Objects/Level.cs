using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Asteroids
{
    class Level
    {
        public Player player;
        public AsteroidEngine asteroidEngine;
        public BulletEngine bulletEngine;
        public bool isActive,isPaused;

        public Level(Model playerModel, Camera camera, Model asteroidModel, Model bulletModel, List<Model> textures, int level)
        {
            player = new Player(playerModel, Vector3.Zero, Vector3.Zero, camera, textures);
            asteroidEngine = new AsteroidEngine(asteroidModel, camera, textures, level);
            bulletEngine = new BulletEngine(bulletModel, camera);
            isActive = true;
            isPaused = false;
            asteroidEngine.ResetAsteroids(asteroidModel, camera, level);
        }

        public void Update(KeyboardState state, Model bulletModel, Camera camera, float timeDelta, SoundEffectInstance engineInstance, Model[] asteroidModel, SoundEffect explosionSound, KeyboardState lastState, SoundEffect laserSound)
        {
            if (isPaused)
            {
                isActive = false;
            }
            else
            {
                isActive = true;
            }
            if (isActive)
            {
               
                player.Update(state, bulletModel, camera, timeDelta, engineInstance);
                asteroidEngine.Update(timeDelta, asteroidModel, camera);
                bulletEngine.Update(state, lastState, player.RotationMatrix.Up, GameConstants.BulletSpeedAdjustment, player.Position + (0.725f * player.RotationMatrix.Up), camera, timeDelta, laserSound);
                CheckCollisions(bulletModel, explosionSound);
            }
        }

        public void CheckCollisions(Model bulletModel, SoundEffect explosionSound)
        {
            //bullet VS asteroid collision check
            for (int i = 0; i < asteroidEngine.asteroidList.Count(); i++)//for each asteroid
            {
                if (asteroidEngine.asteroidList[i].isActive)//check if asteroid is active
                {
                    //give asteroid a bounding sphere
                    BoundingSphere asteroidSphere = new BoundingSphere(asteroidEngine.asteroidList[i].Position, asteroidEngine.asteroidList[i].CurrentTexture.Meshes[0].BoundingSphere.Radius * GameConstants.AsteroidBoundingSphereScale);
                    for (int j = 0; j < bulletEngine.bullets.Count; j++)//for each bullet
                    {
                        if (bulletEngine.bullets[j].isActive)//check if bullet is active
                        {
                            //give bullet a bounding sphere
                            BoundingSphere bulletSphere = new BoundingSphere(bulletEngine.bullets[j].Position, bulletModel.Meshes[0].BoundingSphere.Radius);
                            if (asteroidSphere.Intersects(bulletSphere))//if asteroid and bullet intercept
                            {
                                explosionSound.Play(0.01f, 0, 0);
                                asteroidEngine.asteroidList[i].isActive = false;
                                bulletEngine.bullets[j].TTL = -1;
                                player.hasScored = true;
                            }
                        }
                    }
                }
            }
            //asteroid VS asteroid collision check
            for (int i = 0; i < asteroidEngine.asteroidList.Count(); i++)//for each asteroid
            {
                if (asteroidEngine.asteroidList[i].isActive && !asteroidEngine.asteroidList[i].isColliding)//check if asteroid is active
                {
                    for (int j = i + 1; j < asteroidEngine.asteroidList.Count(); j++)//for each asteroid, loop two
                    {
                        if (asteroidEngine.asteroidList[j].isActive && !asteroidEngine.asteroidList[j].isColliding)//if active
                        {
                            double xDist = asteroidEngine.asteroidList[i].Position.X - asteroidEngine.asteroidList[j].Position.X;
                            double yDist = asteroidEngine.asteroidList[i].Position.Y - asteroidEngine.asteroidList[j].Position.Y;
                            double distSquared = xDist * xDist + yDist * yDist;
                            if (distSquared <= (
                                asteroidEngine.asteroidList[i].CurrentTexture.Meshes[0].BoundingSphere.Radius +
                                asteroidEngine.asteroidList[j].CurrentTexture.Meshes[0].BoundingSphere.Radius) * (
                                asteroidEngine.asteroidList[i].CurrentTexture.Meshes[0].BoundingSphere.Radius +
                                asteroidEngine.asteroidList[j].CurrentTexture.Meshes[0].BoundingSphere.Radius))
                            {
                                double xVelocity = asteroidEngine.asteroidList[j].Velocity.X - asteroidEngine.asteroidList[i].Velocity.X;
                                double yVelocity = asteroidEngine.asteroidList[j].Velocity.Y - asteroidEngine.asteroidList[i].Velocity.Y;
                                double dotProduct = xDist * xVelocity + yDist * yVelocity;
                                if (dotProduct > 0)
                                {
                                    double collisionScale = dotProduct / distSquared;
                                    double xCollision = xDist * collisionScale;
                                    double yCollision = yDist * collisionScale;
                                    //The Collision vector is the speed difference projected on the Dist vector,
                                    //thus it is the component of the speed difference needed for the collision.
                                    double combinedMass = asteroidEngine.asteroidList[i].Mass + asteroidEngine.asteroidList[j].Mass;
                                    double collisionWeightA = 2 * asteroidEngine.asteroidList[j].Mass / combinedMass;
                                    double collisionWeightB = 2 * asteroidEngine.asteroidList[i].Mass / combinedMass;
                                    asteroidEngine.asteroidList[i].Velocity.X += (float) (collisionWeightA * xCollision);
                                    asteroidEngine.asteroidList[i].Velocity.Y += (float) (collisionWeightA * yCollision);
                                    asteroidEngine.asteroidList[j].Velocity.X -= (float) (collisionWeightB * xCollision);
                                    asteroidEngine.asteroidList[j].Velocity.Y -= (float) (collisionWeightB * yCollision);
                                }
                            }
                        }
                    }
                }
            }

            //ship VS asteroid collision check
            if (!player.isInvulnerable)
            {
                if (!player.isSpawning)//only check collisions if the player isn't spawning
                {
                    BoundingSphere shipSphere = new BoundingSphere(player.Position, player.CurrentTexture.Meshes[0].BoundingSphere.Radius * GameConstants.ShipBoundingSphereScale);
                    for (int i = 0; i < asteroidEngine.asteroidList.Count(); i++)
                    {
                        if (asteroidEngine.asteroidList[i].isActive == true)
                        {
                            BoundingSphere b = new BoundingSphere(asteroidEngine.asteroidList[i].Position, asteroidEngine.asteroidList[i].CurrentTexture.Meshes[0].BoundingSphere.Radius * GameConstants.AsteroidBoundingSphereScale);
                            if (b.Intersects(shipSphere))
                            {
                                //blow up ship
                                player.isActive = false;
                                explosionSound.Play(0.1f, 0, 0);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Draw(Camera camera, float timeDelta, SpriteFont font, SpriteBatch spriteBatch, int level, float width, float height)
        {
            spriteBatch.Begin();
            DrawUI(camera, level, player.score, player.lives, asteroidEngine.asteroidList.Count(), player.multiplier, font, spriteBatch, width, height);
            spriteBatch.End();
            player.Draw(camera, timeDelta);
            asteroidEngine.Draw(camera);
            bulletEngine.Draw(camera);
        }

        public void DrawUI(Camera camera, int level, int score, int lives, int asteroids, float multiplier, SpriteFont font, SpriteBatch spriteBatch, float width, float height)
        {
           // spriteBatch.DrawString(large_font, "ASTEROIDS", new Vector2(width / 2 - (large_font.MeasureString("ASTEROIDS").Length() / 2), height / 16), Color.White);

            //spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
            spriteBatch.DrawString(font, "LIVES: " + lives, new Vector2(10, 0), Color.White);
            spriteBatch.DrawString(font, "LEVEL: " + level, new Vector2(10, 20), Color.White);
            spriteBatch.DrawString(font, "SCORE: " + score, new Vector2(10, height- (font.MeasureString("SCORE").Y)), Color.White);
            spriteBatch.DrawString(font, "ASTEROIDS: " + asteroids, new Vector2(width - (font.MeasureString("ASTEROIDS: " + asteroids).Length()+10), height - (font.MeasureString("ASTEROIDS").Y)), Color.White);
            spriteBatch.DrawString(font, "MULTIPLIER: " + multiplier.ToString("0.00"), new Vector2(width - (font.MeasureString("MULTIPLIER: " + multiplier.ToString("0.00")).Length()+10), 0), Color.White);
        }
    }
}
