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
        Texture2D horrible_tex;
        int _total_frames = 0;
        float _elapsed_time = 0.0f;
        int _fps = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
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
            horrible_tex = Content.Load<Texture2D>("horrible_texture");

            stateManager = new StateManager(
                graphics, 
                Content.Load<Model>("models/ship"), 
                new Model[3]
                {
                    Content.Load<Model>("models/small"),
                    Content.Load<Model>("models/medium"),
                    Content.Load<Model>("models/large")
                },
                Content.Load<Model>("particles/circle"), 
                Content.Load<SoundEffect>("sound/engine_2"),
                Content.Load<SoundEffect>("sound/tx0_fire1"),
                Content.Load<SoundEffect>("sound/explosion3"),
                Content.Load<SoundEffect>("sound/engine_2").CreateInstance(),
                Content.Load<SpriteFont>("fonts/small_font"),
                Content.Load<SpriteFont>("fonts/medium_font"),
                Content.Load<SpriteFont>("fonts/large_font"),
                new List<Model> {Content.Load<Model>("particles/circle")},
                Content.Load<Texture2D>("pause_menu"),
                1
                );
            //System.Threading.Thread.Sleep(10000);

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
            stateManager.Update(gameTime, this, graphics.GraphicsDevice);
            //state = Keyboard.GetState();
            //if (gameIsRunning)
            //{
            //    float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    level.Update(state, bulletModel, camera, timeDelta, engineInstance, asteroidModel, explosionSound, lastState, laserSound);

            //    if (state.IsKeyDown(Keys.Escape) || level.player.lives < 0)
            //    {
            //        AddHighScore();
            //        gameIsRunning = false;
            //    }
            //    else if (level.asteroidEngine.asteroidList.Count() < 1)
            //    {
            //        currentLevel++;
            //        level = new Level(playerModel, camera, asteroidModel[2], bulletModel, textures, currentLevel);
            //    }

            //    if (state.IsKeyDown(Keys.P) && level.isPaused == false && lastState.IsKeyUp(Keys.P))
            //    {
            //        level.isPaused = true;
            //    }
            //    else if (state.IsKeyDown(Keys.P) && level.isPaused == true && lastState.IsKeyUp(Keys.P))
            //    {
            //        level.isPaused = false;
            //    }

            //    lastState = state;
            //}
            //else
            //{
            //    if (state.IsKeyDown(Keys.Escape))
            //    {
            //        this.Exit();
            //    }
            //    if (state.IsKeyDown(Keys.Enter))
            //    {
            //        LoadFirstLevel();
            //        startedAt = (float)gameTime.TotalGameTime.TotalSeconds;
            //        gameIsRunning = true;
            //    }
            //}

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
            spriteBatch.Begin();
            //spriteBatch.DrawString(_spr_font, string.Format("FPS={0}", _fps,new Vector2(10.0f, 50.0f), Color.White);
            //spriteBatch.Draw(horrible_tex, new Vector2(300, 200), Color.Wheat);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}