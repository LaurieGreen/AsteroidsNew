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
        Model CurrentTexture;
        Vector3 position, velocity, Rotation;
        float speed, RotationXamount, RotationYamount,RotationZamount;
        Matrix TransformMatrix, RotationMatrix;
        bool isActive, isColliding;
        int life, mass;

        float collideTimer = GameConstants.AsteroidCollideCooldownTimer;

        public Asteroid(Model currentTexture, Vector3 position, float velocityx, float velocityy, float speed, int life, int mass, Random rand)
        {
            CurrentTexture = currentTexture;
            this.position = position;
            velocity.X = velocityx;
            velocity.Y = velocityy;
            this.speed = speed;
            this.life = life;
            this.mass = mass;
            isActive = true;
            isColliding = true;
            RotationXamount = (float) ((rand.NextDouble() * 0.075));
            //Console.WriteLine("\nrotation X rand: " + RotationXamount);

            RotationYamount = (float)((rand.NextDouble() * 0.075));
            //Console.WriteLine("rotation Y rand: " + RotationYamount);

            RotationZamount = (float)((rand.NextDouble() * 0.075));
            //Console.WriteLine("rotation Z rand: " + RotationZamount+"\n");

        }

        public bool IsActive()
        {
            return isActive;
        }

        public int getLife()
        {
            return life;
        }

        public void setLife(int i)
        {
            life = i;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Vector3 getVelocity()
        {
            return velocity;
        }

        public void setVelocityX(float f)
        {
            velocity.X = f;
        }

        public void setVelocityY(float f)
        {
            velocity.Y = f;
        }

        public float getSpeed()
        {
            return speed;
        }
        
        public void addLife(int i)
        {
            life += i;
        }

        public void setIsActive (bool b)
        {
            isActive = b;
        }

        public bool IsColliding()
        {
            return isColliding;
        }

        public int getMass()
        {
            return mass;
        }

        public Model getCurrentTexture()
        {
            return CurrentTexture;
        }

        public void Update(float delta)
        {
            collideTimer -= delta;
            position += velocity * speed * GameConstants.AsteroidSpeedAdjustment * delta;

            Rotation.X += RotationXamount;
            Rotation.Y += RotationYamount;
            Rotation.Z += RotationZamount;
            RotationMatrix = Matrix.CreateRotationX(Rotation.X);
            RotationMatrix = Matrix.CreateRotationY(Rotation.Y);
            RotationMatrix = Matrix.CreateRotationZ(Rotation.Z);

            if (position.X > GameConstants.PlayfieldSizeX)
                position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (position.X < -GameConstants.PlayfieldSizeX)
                position.X += 2 * GameConstants.PlayfieldSizeX;
            if (position.Y > GameConstants.PlayfieldSizeY)
                position.Y -= 2 * GameConstants.PlayfieldSizeY;
            if (position.Y < -GameConstants.PlayfieldSizeY)
                position.Y += 2 * GameConstants.PlayfieldSizeY;
            if (collideTimer <= 0.0f)
            {
                isColliding = false;
                collideTimer = GameConstants.AsteroidCollideCooldownTimer;
            }
        }

        public void Draw(Camera camera, Matrix[] asteroidTransforms)
        {
            TransformMatrix = RotationMatrix * Matrix.CreateTranslation(position);
            camera.DrawModel(CurrentTexture, TransformMatrix, asteroidTransforms, camera, new Vector3(255, 0, 0));
        }
    }
}
