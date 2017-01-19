using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public class Asteroid
    {
        public Model CurrentTexture;
        public Vector3 Position, Velocity, Rotation;
        public float Speed, RotationXamount, RotationYamount,RotationZamount;
        public Matrix TransformMatrix, RotationMatrix;
        public bool isActive, isColliding;
        public int Life, Mass;

        float collideTimer = GameConstants.AsteroidCollideCooldownTimer;

        public Asteroid(Model currentTexture, Vector3 position, float velocityx, float velocityy, float speed, int life, int mass, Random rand)
        {
            CurrentTexture = currentTexture;
            Position = position;
            Velocity.X = velocityx;
            Velocity.Y = velocityy;
            Speed = speed;
            Life = life;
            Mass = mass;
            isActive = true;
            isColliding = true;
            RotationXamount = (float) ((rand.NextDouble() * 0.075));
            //Console.WriteLine("\nrotation X rand: " + RotationXamount);

            RotationYamount = (float)((rand.NextDouble() * 0.075));
            //Console.WriteLine("rotation Y rand: " + RotationYamount);

            RotationZamount = (float)((rand.NextDouble() * 0.075));
            //Console.WriteLine("rotation Z rand: " + RotationZamount+"\n");

        }

        public void Update(float delta)
        {
            collideTimer -= delta;
            Position += Velocity * Speed * GameConstants.AsteroidSpeedAdjustment * delta;

            Rotation.X += RotationXamount;
            Rotation.Y += RotationYamount;
            Rotation.Z += RotationZamount;
            RotationMatrix = Matrix.CreateRotationX(Rotation.X);
            RotationMatrix = Matrix.CreateRotationY(Rotation.Y);
            RotationMatrix = Matrix.CreateRotationZ(Rotation.Z);

            if (Position.X > GameConstants.PlayfieldSizeX)
                Position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (Position.X < -GameConstants.PlayfieldSizeX)
                Position.X += 2 * GameConstants.PlayfieldSizeX;
            if (Position.Y > GameConstants.PlayfieldSizeY)
                Position.Y -= 2 * GameConstants.PlayfieldSizeY;
            if (Position.Y < -GameConstants.PlayfieldSizeY)
                Position.Y += 2 * GameConstants.PlayfieldSizeY;
            if (collideTimer <= 0.0f)
            {
                isColliding = false;
                collideTimer = GameConstants.AsteroidCollideCooldownTimer;
            }
        }

        public void Draw(Camera camera, Matrix[] asteroidTransforms)
        {
            TransformMatrix = RotationMatrix * Matrix.CreateTranslation(Position);
            camera.DrawModel(CurrentTexture, TransformMatrix, asteroidTransforms, camera, new Vector3(255, 0, 0));
        }
    }
}
