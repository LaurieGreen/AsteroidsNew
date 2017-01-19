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
        public Model Texture { get; set; }        // The texture that will be drawn to represent the particle
        public Vector3 Position { get; set; }        // The current position of the particle        
        public float Velocity { get; set; }        // The speed of the particle at the current instance
        public Vector3 Direction;
        public Matrix TransformMatrix;
        public bool isActive = true;
        public int TTL;

        public Bullet(Model texture, float velocity, Vector3 direction,Vector3 position, Camera camera, int ttl)
        {
            Direction = direction;
            Position = position;
            Velocity = velocity;
            Texture = texture;
            TTL = ttl;
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
            Matrix world = Matrix.CreateBillboard(Position, camera.Position, Vector3.Up, camera.Target - camera.Position);
            camera.DrawBullet(Texture, camera, world);
        }
    }
}
