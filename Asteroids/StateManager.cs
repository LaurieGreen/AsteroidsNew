using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class StateManager
    {
        enum GameState
        {
            StartMenu,
            HighScore,
            Loading,
            Playing,
            Paused
        }

        Texture2D rect;
        Menu pauseMenu, mainMenu, highScoreMenu;
        Level level;
        int currentLevel;
        Camera camera;
        Model playerModel, bulletModel;
        Model[] asteroidModel;
        List<Model> textures;
        KeyboardState state, lastState;
        SoundEffect engineSound, laserSound, explosionSound;
        SpriteFont small_font, medium_font, large_font;
        SoundEffectInstance engineInstance;
        List<string> highscoresList;
        float drawTime, oldDrawTime, timeBeginLoad;
        GraphicsDeviceManager graphics;
        GameState gameState;
        DustEngine dustEngineFar, dustEngineMiddle, dustEngineNear;
        private bool isLoading = false;
        private Thread backgroundThread;
        Random random;

        public StateManager(
            GraphicsDeviceManager graphics,
            SpriteFont small_font,
            SpriteFont medium_font,
            SpriteFont large_font)
        {
            gameState = GameState.StartMenu;
            this.graphics = graphics;
            this.small_font = small_font;
            this.medium_font = medium_font;
            this.large_font = large_font;
            this.currentLevel = 1;
            pauseMenu = new Menu(small_font, medium_font, large_font, "PAUSE MENU", new List<String> { "RESUME", "MAIN MENU" });
            mainMenu = new Menu(small_font, medium_font, large_font, "MAIN MENU", new List<String> { "PLAY", "HIGHSCORES", "QUIT" });
            highScoreMenu = new Menu(small_font, medium_font, large_font, "HIGH SCORES", new List<String> { "BACK" });
            lastState = Keyboard.GetState();
            camera = new Camera(Vector3.Zero, graphics.GraphicsDevice.Viewport.AspectRatio, MathHelper.ToRadians(90.0f));

            rect = new Texture2D(graphics.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.Black });

            random = new Random();

            dustEngineFar = new DustEngine(graphics.GraphicsDevice, 2000, 0.3f, 0.005f, random);
            dustEngineMiddle = new DustEngine(graphics.GraphicsDevice, 750, 0.5f, 0.01f, random);
            dustEngineNear = new DustEngine(graphics.GraphicsDevice, 300, 0.8f, 0.05f, random);
            GetHighScores();
        }

        public void LoadNewLevel()
        {
            level = new Level(playerModel, camera, asteroidModel[2], bulletModel, textures, currentLevel);
        }

        public void Update(GameTime gameTime, Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            state = Keyboard.GetState();
            if (gameState == GameState.Playing)
            {
                float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                //float thisplayingTime = (float)gameTime.TotalGameTime.TotalSeconds;
                level.Update(state, bulletModel, camera, timeDelta, engineInstance, asteroidModel, explosionSound, lastState, laserSound);

                if (level.player.lives < 0)
                {
                    AddHighScore();
                    gameState = GameState.StartMenu;
                    //playingTime = playingTime + ((float)gameTime.TotalGameTime.TotalSeconds - pausedTime);
                }
                else if (level.asteroidEngine.asteroidList.Count() < 1)
                {
                    currentLevel++;
                    LoadNewLevel();
                }

                if ((state.IsKeyDown(Keys.P) && lastState.IsKeyUp(Keys.P)) || (state.IsKeyDown(Keys.Escape) && lastState.IsKeyUp(Keys.Escape))) //P or ESC to pause
                {
                    gameState = GameState.Paused;
                    //pauseMenu.isNew = true;
                }
            }
            else if (gameState == GameState.StartMenu)
            {
                mainMenu.Update(state, lastState);
                if (mainMenu.finalSelection == 0)
                {
                    System.Console.WriteLine("new level");
                    gameState = GameState.Loading;
                    timeBeginLoad = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    mainMenu.finalSelection = -10;
                    mainMenu.currentSelection = 0;
                }
                else if (mainMenu.finalSelection == 1)
                {
                    gameState = GameState.HighScore;
                    mainMenu.finalSelection = -10;
                    mainMenu.currentSelection = 0;
                }
                else if (mainMenu.finalSelection == 2)
                {
                    mainMenu.finalSelection = -10;
                    game.Quit();
                }
            }
            else if (gameState == GameState.Paused)
            {
                pauseMenu.Update(state, lastState);
                //if (state.IsKeyDown(Keys.P) && lastState.IsKeyUp(Keys.P))
                //{
                //    gameState = GameState.Playing;
                //    pausedTime = pausedTime + ((float)gameTime.TotalGameTime.TotalSeconds-playingTime);
                //}
                if (pauseMenu.finalSelection == 0)
                {
                    gameState = GameState.Playing;
                    //System.Console.WriteLine("playing");
                    pauseMenu.finalSelection = -10;
                    pauseMenu.currentSelection = 0;
                }
                if (pauseMenu.finalSelection == 1)
                {
                    gameState = GameState.StartMenu;
                    //System.Console.WriteLine("menu");
                    pauseMenu.finalSelection = -10;
                    pauseMenu.currentSelection = 0;
                }
            }
            else if (gameState == GameState.HighScore)
            {
                highScoreMenu.Update(state, lastState);
                //System.Console.WriteLine("this is the highscore menu");
                if (highScoreMenu.finalSelection == 0)
                {
                    gameState = GameState.StartMenu;
                    highScoreMenu.finalSelection = -10;
                    highScoreMenu.currentSelection = 0;
                }
            }
            else if (gameState == GameState.Loading && !isLoading)
            {
                //we show a loading screen while we wait for the loading thread to flag that it's finished loading
                backgroundThread = new Thread(() => LoadLevelContent(content));
                isLoading = true;

                //start backgroundthread
                backgroundThread.Start();
            }
            //not updating this one for now because its so big
            //dustEngineFar.Update(graphicsDevice);
            dustEngineMiddle.Update(graphicsDevice);
            dustEngineNear.Update(graphicsDevice);
            lastState = state;
        }

        void LoadLevelContent(ContentManager content)
        {
            //this method gets ran by a thread, it will load our game content on a thread and flag when it is ready
            playerModel = content.Load<Model>("models/ship");
            asteroidModel = new Model[3]
            {
                    content.Load<Model>("models/small"),
                    content.Load<Model>("models/medium"),
                    content.Load<Model>("models/large")
            };
            bulletModel = content.Load<Model>("particles/circle");
            engineSound = content.Load<SoundEffect>("sound/engine_2");
            laserSound = content.Load<SoundEffect>("sound/tx0_fire1");
            explosionSound = content.Load<SoundEffect>("sound/explosion3");
            engineInstance = content.Load<SoundEffect>("sound/engine_2").CreateInstance();
            textures = new List<Model> { content.Load<Model>("particles/circle") };
            Thread.Sleep(1000);//add a 1 second wait onto the end of the loading thread so we avoid nasty flashes of the loading screen when content has already been loaded previously
            LoadNewLevel();
            isLoading = false;
            gameState = GameState.Playing;
        }

        public void GetHighScores()
        {
            string[] highscoresArray = File.ReadAllText("Content/files/highscores.txt").Split('-');
            highscoresList = new List<string>(highscoresArray.Length);
            highscoresList.AddRange(highscoresArray);
            highscoresList.Reverse();
            foreach (string score in highscoresList)
            {
                System.Console.WriteLine(score);
            }
        }

        public void AddHighScore()
        {
            //highscoresList.OrderByDescending(x => x);
            highscoresList.Add(level.player.score.ToString());
            for (int s = 1; s < highscoresList.Count(); s++)
            {
                highscoresList[s] = highscoresList[s] + "-";
                System.Console.WriteLine(highscoresList[s]);
            }
            string highscore = String.Join(String.Empty, highscoresList);
            File.WriteAllText("Content/files/highscores.txt", highscore);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            dustEngineFar.Draw(spriteBatch);
            dustEngineMiddle.Draw(spriteBatch);
            dustEngineNear.Draw(spriteBatch);
            if (gameState == GameState.Playing)
            {
                if (level.isActive)
                {
                    drawTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }
                else
                {
                    drawTime = oldDrawTime;
                }
                level.Draw(camera, drawTime, small_font, spriteBatch, currentLevel, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
                oldDrawTime = drawTime;
            }
            if (gameState != GameState.Playing)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(rect, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.Black * 0.5f);
                spriteBatch.End();
            }
            if (gameState == GameState.Paused)
            {
                level.Draw(camera, drawTime, small_font, spriteBatch, currentLevel, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
                pauseMenu.Draw(spriteBatch, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, Color.White * 0.70f, gameTime);
            }
            if (gameState == GameState.StartMenu)
            {
                mainMenu.Draw(spriteBatch, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, Color.White * 0.00f, gameTime);
            }
            if (gameState == GameState.HighScore)
            {
                highScoreMenu.Draw(spriteBatch, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, Color.White * 0.00f, gameTime);
            }
            if (gameState == GameState.Loading)
            {
                spriteBatch.Begin();
                double time = gameTime.TotalGameTime.TotalSeconds;
                float pulsate = (float)Math.Sin(time * 6) + 1;
                Color choiceColor = new Color(0, 204, 0);//green
                float scale = 1 + pulsate * 0.05f;
                float width = graphicsDevice.Viewport.Width;
                float height = graphicsDevice.Viewport.Height;
                spriteBatch.DrawString(large_font, "LOADING", new Vector2(width / 2, (height / 2) - ((large_font.MeasureString("LOADING").Y / 2))), choiceColor, 0, new Vector2(large_font.MeasureString("LOADING").Length() / 2, large_font.MeasureString("LOADING").Y / 2), scale, SpriteEffects.None, 0);
                spriteBatch.End();
            }
        }
    }
}
