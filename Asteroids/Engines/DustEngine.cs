using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class DustEngine
    {
        List<Dust> dustList;
        Texture2D texture;
        //Random random = new Random();
        float x, y, x2, y2, opacity, speedMultiplier;

        public DustEngine(GraphicsDevice graphicsDevice, int numSpecs, float opacity, float speedMultiplier, Random random)
        {
            //this.texture = texture;
            dustList = new List<Dust>();
            this.opacity = opacity;
            this.speedMultiplier = speedMultiplier;
            texture = new Texture2D(graphicsDevice, 1, 1);
            //System.Console.WriteLine("width: "+graphicsDevice.Viewport.Width);
            //System.Console.WriteLine("height: "+graphicsDevice.Viewport.Height);
            texture.SetData<Color>(new Color[] { Color.White });// fill the texture with white
            for (int i = 0; i < numSpecs; i++)
            {
                // Where will the dust spawn?
                if (random.Next(3) >= 0)// 2/3 chance of it being zero or higher
                {
                    //if its positive then we have a range of 2 x the width
                    x = (float)random.NextDouble() * (2*graphicsDevice.Viewport.Width);
                }
                else
                {
                    //if its negative then we only want a range of -width 
                    x = (float)-random.NextDouble() * graphicsDevice.Viewport.Width;
                }
                
                if (random.Next(3) >= 0)
                {
                    y = (float)random.NextDouble() * (2*graphicsDevice.Viewport.Height);
                }
                else
                {
                    y = (float)-random.NextDouble() * graphicsDevice.Viewport.Height;
                }
                
                double angle = random.NextDouble() * 2 * Math.PI;

                x2 = -((float)Math.Sin(angle)*speedMultiplier);
                y2 = ((float)Math.Cos(angle)*speedMultiplier); 
                Dust spec = new Dust(texture, x, y, x2, y2, random);
                dustList.Add(spec);
            }
        }
        public void Update(GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < dustList.Count(); i++)
            {
                dustList[i].Update(graphicsDevice);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < dustList.Count(); i++)
            {
                dustList[i].Draw(spriteBatch, opacity);
            }
        }
    }
}
