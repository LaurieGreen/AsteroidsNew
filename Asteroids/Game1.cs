using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Asteroids
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        StateManager stateManager;
        SpriteFont _spr_font;
        Texture2D t; //base for the line texture
        //Texture2D horrible_tex;
        int _total_frames = 0;
        float _elapsed_time = 0.0f;
        int _fps = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferHeight = 400;
            //graphics.PreferredBackBufferWidth = 400;
            //graphics.GraphicsDevice.Viewport.AspectRatio.
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _spr_font = Content.Load<SpriteFont>("fonts/small_font");
            //we're only going to load the stuff we need for the menu, the other stuff will be loaded when we load our first level
            stateManager = new StateManager(
                graphics,
                _spr_font,
                Content.Load<SpriteFont>("fonts/medium_font"),
                Content.Load<SpriteFont>("fonts/large_font")
                );
            if (GameConstants.Debug)
            {
#pragma warning disable CS0162 // Unreachable code detected
                t = new Texture2D(GraphicsDevice, 1, 1);
#pragma warning restore CS0162 // Unreachable code detected
                t.SetData<Color>(new Color[] { Color.White });
            }
        }

        public void Quit()
        {
            this.Exit();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Update
            _elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // 1 Second has passed
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }
            stateManager.Update(gameTime, this, graphics.GraphicsDevice, Content);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _total_frames++;


            stateManager.Draw(gameTime, spriteBatch, GraphicsDevice);
            //spriteBatch.Begin();
            ////
            //spriteBatch.End();
            if (GameConstants.Debug)
            {
#pragma warning disable CS0162 // Unreachable code detected
                spriteBatch.Begin();
#pragma warning restore CS0162 // Unreachable code detected
                spriteBatch.DrawString(_spr_font, string.Format("FPS={0}", _fps), new Vector2(10.0f, 50.0f), Color.White);

                float width = graphics.GraphicsDevice.Viewport.Width;
                float height = graphics.GraphicsDevice.Viewport.Height;

                //
                // FROM TOP TO BOTTOM
                //

                DrawLine(spriteBatch, new Vector2(width / 8, 0), new Vector2(width / 8, height));//down the left leftside top to bottom
                DrawLine(spriteBatch, new Vector2(width / 4, 0), new Vector2(width / 4, height));//down the middle leftside top to bottom
                DrawLine(spriteBatch, new Vector2((width / 8) + width / 4, 0), new Vector2((width / 8) + width / 4, height));//down the right leftside top to bottom

                DrawLine(spriteBatch, new Vector2(width / 2, 0), new Vector2(width / 2, height));//down the middle top to bottom

                DrawLine(spriteBatch, new Vector2((width / 8) + width / 2, 0), new Vector2((width / 8) + width / 2, height));//down the left rightside top to bottom
                DrawLine(spriteBatch, new Vector2((width / 4) + width / 2, 0), new Vector2((width / 4) + width / 2, height));//down the middle rightside top to bottom
                DrawLine(spriteBatch, new Vector2((width / 8) + (width / 2) + width / 4, 0), new Vector2((width / 8) + (width / 2) + width / 4, height));//down the right rightside top to bottom

                //
                // FROM LEFT TO RIGHT
                //

                DrawLine(spriteBatch, new Vector2(0, height / 8), new Vector2(width, height / 8));//across the top topside left to right
                DrawLine(spriteBatch, new Vector2(0, height / 4), new Vector2(width, height / 4));//across the middle topside left to right
                DrawLine(spriteBatch, new Vector2(0, (height / 8) + height / 4), new Vector2(width, (height / 8) + height / 4));//across the bottom topside left to right

                DrawLine(spriteBatch, new Vector2(0, height / 2), new Vector2(width, height / 2));//across the middle left to right

                DrawLine(spriteBatch, new Vector2(0, (height / 4) + height / 2), new Vector2(width, (height / 4) + height / 2));//across the top bottomside to right   
                DrawLine(spriteBatch, new Vector2(0, (height / 8) + height / 2), new Vector2(width, (height / 8) + height / 2));//across the middle bottomside left to right 
                DrawLine(spriteBatch, new Vector2(0, (height / 8) + (height / 2) + height / 4), new Vector2(width, ((height / 8) + height / 2) + height / 4));//across the bottom bottomside left to right

                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.Red, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}