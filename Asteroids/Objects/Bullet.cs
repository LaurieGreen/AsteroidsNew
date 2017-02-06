using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public class Bullet
    {
        Model Texture;       // The texture that will be drawn to represent the particle
        Vector3 Position;       // The current position of the particle        
        float Velocity;     // The speed of the particle at the current instance
        Vector3 Direction;
        //Matrix TransformMatrix;
        bool isActive = true;
        int TTL;

        public Bullet(Model texture, float velocity, Vector3 direction,Vector3 position, Camera camera, int ttl)
        {
            Direction = direction;
            Position = position;
            Velocity = velocity;
            Texture = texture;
            TTL = ttl;
        }

        public Vector3 getPosition()
        {
            return Position;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void setIsActive(bool b)
        {
            isActive = b;
        }

        public int getTTL()
        {
            return TTL;
        }

        public void setTTL(int i)
        {
            TTL = i;
        }

        public void Update(float delta)
        {
            TTL--;
            Position += Direction * Velocity * GameConstants.BulletSpeedAdjustment * delta;

            if (Position.X > GameConstants.PlayfieldSizeX || Position.X < -GameConstants.PlayfieldSizeX || Position.Y > GameConstants.PlayfieldSizeY || Position.Y < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
            }
        }

        public void Draw(Camera camera, Matrix[] bulletTransforms)
        {
            Matrix world = Matrix.CreateBillboard(Position, camera.getPosition(), Vector3.Up, camera.getTarget() - camera.getPosition());
            camera.DrawBullet(Texture, camera, world);
        }
    }
}
