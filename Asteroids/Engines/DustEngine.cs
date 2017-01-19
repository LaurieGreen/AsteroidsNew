using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class DustEngine
    {
        public List<SpaceDust> dustList;
        private Model texture;
        int numSpecs = 1000;
        Random random = new Random();
        float maxX, maxY;

        public DustEngine(Model texture, GraphicsDevice graphicsDevice)
        {
            this.texture = texture;
            this.dustList = new List<SpaceDust>();
            maxX = graphicsDevice.Viewport.Width;
            maxY = graphicsDevice.Viewport.Height;
            for (int i = 0; i < numSpecs; i++)
            {
                // Where will the dust spawn?
                SpaceDust spec = new SpaceDust(texture, maxX, maxY, random);
                dustList.Add(spec);
            }
        }
        public void Update()
        {
            for (int i = 0; i < dustList.Count(); i++)
            {
                dustList[i].Update();
            }
        }

        public void Draw(Camera camera)
        {
            for (int i = 0; i < dustList.Count(); i++)
            {
                dustList[i].Draw(camera);
            }
        }
    }
}
