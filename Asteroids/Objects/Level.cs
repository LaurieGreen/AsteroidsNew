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
        Player player;
        AsteroidEngine asteroidEngine;
        BulletEngine bulletEngine;
        bool isActive,isPaused;

        public Level(Model playerModel, Camera camera, Model asteroidModel, Model bulletModel, List<Model> textures, int level, int lives, int score)
        {
            player = new Player(playerModel, Vector3.Zero, Vector3.Zero, camera, textures, lives, score);
            asteroidEngine = new AsteroidEngine(asteroidModel, camera, textures, level);
            bulletEngine = new BulletEngine(bulletModel, camera);
            isActive = true;
            isPaused = false;
            asteroidEngine.ResetAsteroids(asteroidModel, camera, level);
        }

        public Player getPlayer()
        {
            return player;
        }

        public AsteroidEngine getAsteroidEngine()
        {
            return asteroidEngine;
        }

        public bool getIsActive()
        {
            return isActive;
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
                bulletEngine.Update(state, lastState, player.getRotationMatrix().Up, GameConstants.BulletSpeedAdjustment, player.getPosition() + (0.725f * player.getRotationMatrix().Up), camera, timeDelta, laserSound);
                CheckCollisions(bulletModel, explosionSound);
            }
        }

        public void CheckCollisions(Model bulletModel, SoundEffect explosionSound)
        {
            List<Asteroid> asteroids = asteroidEngine.getAsteroidList();
            //bullet VS asteroid collision check
            for (int i = 0; i < asteroidEngine.getAsteroidList().Count(); i++)//for each asteroid
            {
                if (asteroidEngine.getAsteroidList()[i].IsActive())//check if asteroid is active
                {
                    //give asteroid a bounding sphere
                    BoundingSphere asteroidSphere = new BoundingSphere(asteroidEngine.getAsteroidList()[i].getPosition(), asteroidEngine.getAsteroidList()[i].getCurrentTexture().Meshes[0].BoundingSphere.Radius * GameConstants.AsteroidBoundingSphereScale);
                    for (int j = 0; j < bulletEngine.getBullets().Count; j++)//for each bullet
                    {
                        if (bulletEngine.getBullets()[j].IsActive())//check if bullet is active
                        {
                            //give bullet a bounding sphere
                            BoundingSphere bulletSphere = new BoundingSphere(bulletEngine.getBullets()[j].getPosition(), bulletModel.Meshes[0].BoundingSphere.Radius);
                            if (asteroidSphere.Intersects(bulletSphere))//if asteroid and bullet intercept
                            {
                                explosionSound.Play(0.01f, 0, 0);
                                asteroidEngine.getAsteroidList()[i].setIsActive(false);
                                bulletEngine.getBullets()[j].setTTL(-1);
                                player.setHasScored(true);
                            }
                        }
                    }
                }
            }
            //asteroid VS asteroid collision check
            for (int i = 0; i < asteroidEngine.getAsteroidList().Count(); i++)//for each asteroid
            {
                if (asteroidEngine.getAsteroidList()[i].IsActive() && !asteroidEngine.getAsteroidList()[i].IsColliding())//check if asteroid is active
                {
                    for (int j = i + 1; j < asteroidEngine.getAsteroidList().Count(); j++)//for each asteroid, loop two
                    {
                        if (asteroidEngine.getAsteroidList()[j].IsActive() && !asteroidEngine.getAsteroidList()[j].IsColliding())//if active
                        {
                            double xDist = asteroidEngine.getAsteroidList()[i].getPosition().X - asteroidEngine.getAsteroidList()[j].getPosition().X;
                            double yDist = asteroidEngine.getAsteroidList()[i].getPosition().Y - asteroidEngine.getAsteroidList()[j].getPosition().Y;
                            double distSquared = xDist * xDist + yDist * yDist;
                            if (distSquared <= (
                                asteroidEngine.getAsteroidList()[i].getCurrentTexture().Meshes[0].BoundingSphere.Radius +
                                asteroidEngine.getAsteroidList()[j].getCurrentTexture().Meshes[0].BoundingSphere.Radius) * (
                                asteroidEngine.getAsteroidList()[i].getCurrentTexture().Meshes[0].BoundingSphere.Radius +
                                asteroidEngine.getAsteroidList()[j].getCurrentTexture().Meshes[0].BoundingSphere.Radius))
                            {
                                double xVelocity = asteroidEngine.getAsteroidList()[j].getVelocity().X - asteroidEngine.getAsteroidList()[i].getVelocity().X;
                                double yVelocity = asteroidEngine.getAsteroidList()[j].getVelocity().Y - asteroidEngine.getAsteroidList()[i].getVelocity().Y;
                                double dotProduct = xDist * xVelocity + yDist * yVelocity;
                                if (dotProduct > 0)
                                {
                                    double collisionScale = dotProduct / distSquared;
                                    double xCollision = xDist * collisionScale;
                                    double yCollision = yDist * collisionScale;
                                    //The Collision vector is the speed difference projected on the Dist vector,
                                    //thus it is the component of the speed difference needed for the collision.
                                    double combinedMass = asteroidEngine.getAsteroidList()[i].getMass() + asteroidEngine.getAsteroidList()[j].getMass();
                                    double collisionWeightA = 2 * asteroidEngine.getAsteroidList()[j].getMass() / combinedMass;
                                    double collisionWeightB = 2 * asteroidEngine.getAsteroidList()[i].getMass() / combinedMass;
                                    asteroidEngine.getAsteroidList()[i].setVelocityX(asteroidEngine.getAsteroidList()[i].getVelocity().X + (float)(collisionWeightA * xCollision));
                                    asteroidEngine.getAsteroidList()[i].setVelocityY(asteroidEngine.getAsteroidList()[i].getVelocity().Y + (float) (collisionWeightA * yCollision));
                                    asteroidEngine.getAsteroidList()[j].setVelocityX(asteroidEngine.getAsteroidList()[i].getVelocity().X - (float) (collisionWeightB * xCollision));
                                    asteroidEngine.getAsteroidList()[j].setVelocityY(asteroidEngine.getAsteroidList()[i].getVelocity().X - (float) (collisionWeightB * yCollision));
                                }
                            }
                        }
                    }
                }
            }

            //ship VS asteroid collision check
            if (!player.getIsInvulerable())
            {
                if (!player.getIsSpawning())//only check collisions if the player isn't spawning
                {
                    BoundingSphere shipSphere = new BoundingSphere(player.getPosition(), player.getCurrentTexture().Meshes[0].BoundingSphere.Radius * GameConstants.ShipBoundingSphereScale);
                    for (int i = 0; i < asteroidEngine.getAsteroidList().Count(); i++)
                    {
                        if (asteroidEngine.getAsteroidList()[i].IsActive() == true)
                        {
                            BoundingSphere b = new BoundingSphere(asteroidEngine.getAsteroidList()[i].getPosition(), asteroidEngine.getAsteroidList()[i].getCurrentTexture().Meshes[0].BoundingSphere.Radius * GameConstants.AsteroidBoundingSphereScale);
                            if (b.Intersects(shipSphere))
                            {
                                //blow up ship
                                player.setIsActive(false);
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
            DrawUI(camera, level, player.getScore(), player.getLives(), asteroidEngine.getAsteroidList().Count(), player.getMultiplier(), font, spriteBatch, width, height);
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
