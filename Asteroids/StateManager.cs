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
            Paused,
            GameOver,
            Debug,
        }

        Texture2D rect;
        BasicMenu pauseMenu, mainMenu, debugMenu;
        HighScoreMenu highScoreMenu;
        GameOverScreen gameOverMenu;
        Level level;
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
        int currentLevel = 1;
        List<String> mainMenuOptions;

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
            if (GameConstants.Debug)
            {
#pragma warning disable CS0162 // Unreachable code detected
                mainMenuOptions = new List<String> { "PLAY", "HIGHSCORES", "QUIT", "DEBUG" };
#pragma warning restore CS0162 // Unreachable code detected
            }
            else
            {
                mainMenuOptions = new List<String> { "PLAY", "HIGHSCORES", "QUIT" };
            }
            pauseMenu = new BasicMenu(small_font, medium_font, large_font, "PAUSE MENU", new List<String> { "RESUME", "MAIN MENU" });
            mainMenu = new BasicMenu(small_font, medium_font, large_font, "MAIN MENU", mainMenuOptions);
            highScoreMenu = new HighScoreMenu(small_font, medium_font, large_font, "HIGH SCORES", new List<String> { "BACK" });
            gameOverMenu = new GameOverScreen(small_font, medium_font, large_font, "GAME OVER", new List<String> {"_", "_", "_", "MAIN MENU" });
            if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                debugMenu = new BasicMenu(small_font, medium_font, large_font, "DEBUG MENU", new List<string> { "START MENU", "HIGHSCORE", "LOADING", "PLAYING", "PAUSED", "GAME OVER" });
#pragma warning restore CS0162 // Unreachable code detected
            lastState = Keyboard.GetState();
            camera = new Camera(Vector3.Zero, graphics.GraphicsDevice.Viewport.AspectRatio, MathHelper.ToRadians(90.0f));

            rect = new Texture2D(graphics.GraphicsDevice, 1, 1);
            rect.SetData(new[] { Color.Black });

            random = new Random();

            dustEngineFar = new DustEngine(graphics.GraphicsDevice, 2000, 0.3f, 0.005f, random);
            dustEngineMiddle = new DustEngine(graphics.GraphicsDevice, 750, 0.5f, 0.01f, random);
            dustEngineNear = new DustEngine(graphics.GraphicsDevice, 300, 0.8f, 0.05f, random);
            SortHighScores();
        }

        public void LoadNewLevel(int lives, int score)
        {
            level = new Level(playerModel, camera, asteroidModel[2], bulletModel, textures, currentLevel, lives, score);
        }

        public void Update(GameTime gameTime, Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            state = Keyboard.GetState();
            if (gameState == GameState.Playing)
            {
                float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                //float thisplayingTime = (float)gameTime.TotalGameTime.TotalSeconds;
                level.Update(state, bulletModel, camera, timeDelta, engineInstance, asteroidModel, explosionSound, lastState, laserSound);

                if (level.getPlayer().getLives() < 0)
                {
                    gameState = GameState.GameOver;
                    //playingTime = playingTime + ((float)gameTime.TotalGameTime.TotalSeconds - pausedTime);
                }
                else if (level.getAsteroidEngine().getAsteroidList().Count() < 1)
                {
                    currentLevel++;
                    LoadNewLevel(level.getPlayer().getLives(), level.getPlayer().getScore());
                }

                if ((state.IsKeyDown(Keys.P) && lastState.IsKeyUp(Keys.P)) || (state.IsKeyDown(Keys.Escape) && lastState.IsKeyUp(Keys.Escape))) //P or ESC to pause
                {
                    gameState = GameState.Paused;
                    //pauseMenu.isNew = true;
                }
            }
            else if(gameState == GameState.GameOver)
            {
                try
                {
                    gameOverMenu.setScore(level.getPlayer().getScore());
                }
                catch(NullReferenceException)
                {
                    gameOverMenu.setScore(0);
                }                
                gameOverMenu.Update(state, lastState);
                if (gameOverMenu.getFinalSelection() != "")
                {
                    gameState = GameState.StartMenu;
                    
                    AddHighScore();
                    gameOverMenu.setFinalSelection("");
                }
            }
            else if (gameState == GameState.StartMenu)
            {
                mainMenu.Update(state, lastState);
                if (mainMenu.getFinalSelection() == 0)
                {
                    if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                    System.Console.WriteLine("new level");
#pragma warning restore CS0162 // Unreachable code detected

                    gameState = GameState.Loading;
                    timeBeginLoad = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    mainMenu.setFinalSelection(-10);
                    mainMenu.setCurrentSelection(0);
                }
                else if (mainMenu.getFinalSelection() == 1)
                {
                    gameState = GameState.HighScore;
                    mainMenu.setFinalSelection(-10);
                    mainMenu.setCurrentSelection(0);
                }
                else if (mainMenu.getFinalSelection() == 2)
                {
                    mainMenu.setFinalSelection(-10);
                    game.Quit();
                }
                else if (mainMenu.getFinalSelection() == 3)
                {
                    gameState = GameState.Debug;
                    mainMenu.setFinalSelection(-10);
                    mainMenu.setCurrentSelection(0);
                }
            }
            else if (gameState == GameState.Paused)
            {
                pauseMenu.Update(state, lastState);
                if (pauseMenu.getFinalSelection() == 0)
                {
                    gameState = GameState.Playing;
                    //System.Console.WriteLine("playing");
                    pauseMenu.setFinalSelection(-10);
                    pauseMenu.setCurrentSelection(0);
                }
                if (pauseMenu.getFinalSelection() == 1)
                {
                    gameState = GameState.GameOver;
                    //System.Console.WriteLine("menu");
                    pauseMenu.setFinalSelection(-10);
                    pauseMenu.setCurrentSelection(0);
                }
            }
            else if (gameState == GameState.HighScore)
            {
                highScoreMenu.Update(state, lastState);
                //System.Console.WriteLine("this is the highscore menu");
                if (highScoreMenu.getFinalSelection() == 0)
                {
                    gameState = GameState.StartMenu;
                    highScoreMenu.setFinalSelection(-10);
                    highScoreMenu.setCurrentSelection(0);
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
            else if (GameConstants.Debug && gameState == GameState.Debug)
            {
                debugMenu.Update(state, lastState);
                if (debugMenu.getFinalSelection() == 0)
                {
                    gameState = GameState.StartMenu;
                    debugMenu.setFinalSelection(-10);
                    debugMenu.setCurrentSelection(0);
                }
                else if (debugMenu.getFinalSelection() == 1)
                {
                    gameState = GameState.HighScore;
                    debugMenu.setFinalSelection(-10);
                    debugMenu.setCurrentSelection(0);
                }
                else if(debugMenu.getFinalSelection() == 2)
                {
                    gameState = GameState.Loading;
                    debugMenu.setFinalSelection(-10);
                    debugMenu.setCurrentSelection(0);
                }
                else if(debugMenu.getFinalSelection() == 3)
                {
                    gameState = GameState.Playing;
                    debugMenu.setFinalSelection(-10);
                    debugMenu.setCurrentSelection(0);
                }
                else if(debugMenu.getFinalSelection() == 4)
                {
                    gameState = GameState.Paused;
                    debugMenu.setFinalSelection(-10);
                    debugMenu.setCurrentSelection(0);
                }
                else if(debugMenu.getFinalSelection() == 5)
                {
                    gameState = GameState.GameOver;
                    debugMenu.setFinalSelection(-10);
                    debugMenu.setCurrentSelection(0);
                }
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
            currentLevel = 1;//reset level to 1 so that
            LoadNewLevel(GameConstants.NumLives, 0);
            isLoading = false;
            gameState = GameState.Playing;
        }

        public string[] GetHighScores()
        {
            //reads the highscores from the file and returns an array holding all of them plus an empty element at the end due to the way 'split' works
            string[] highscoresArray = File.ReadAllText("Content/files/highscores.txt").Split(',');//go through txt file and create a new array from the things on either side of the commas
            return highscoresArray;
        }           

        public void SortHighScores()
        {
            //calls the method to read from the high scores file
            //sorts the highscores and resaves them to the file

            string[] highscoresArray = GetHighScores();
            //System.Console.WriteLine("this is the array now: " + highscoresArray + "and it is " + highscoresArray.Length + " objects long");
            int[] scoreArray = new int[highscoresArray.Length];//array for scores
            String[] nameArray = new String[highscoresArray.Length];//array for names
            for (int i = 0; i < highscoresArray.Length - 1; i++)//-1 from length because it'll have an empty element at the end of the array because the scores end with a commma
            {
                string[] parts = highscoresArray[i].Split(':');
                //System.Console.WriteLine("is it an integer?" + parts[0]);
                //System.Console.WriteLine("is it an name?" + parts[i]);
                scoreArray[i] = Int32.Parse(parts[1]);
                nameArray[i] = parts[0];
                if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                System.Console.WriteLine(highscoresArray[i]);
#pragma warning restore CS0162 // Unreachable code detected
            }
            Array.Sort(scoreArray, nameArray);
            //System.Console.WriteLine("array length: " + highscoresArray.Length);
            for (int i = 0; i < highscoresArray.Length; i++)//-1 from length because it'll have an empty element at the end of the array because the scores end with a commma
            {
                highscoresArray[i] = nameArray[i] + ":" + scoreArray[i].ToString() + ",";
                if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                System.Console.WriteLine(nameArray[i] + ":" + scoreArray[i].ToString() + ",");
#pragma warning restore CS0162 // Unreachable code detected
            }
            string[] transferArray = new string[highscoresArray.Length - 1];
            for (int i = 0; i < transferArray.Length; i++)
            {
                transferArray[i] = highscoresArray[i + 1];//plus 1 to cut out the empty element at the start
            }
            highscoresList = new List<string>(transferArray.Length);
            highscoresList.AddRange(transferArray);
            highscoresList.Reverse();
            if (GameConstants.Debug)
            {
#pragma warning disable CS0162 // Unreachable code detected
                System.Console.WriteLine("\n\n THIS IS THE SORTED ARRAY:\n\n");
#pragma warning restore CS0162 // Unreachable code detected
                for (int i = 0; i < highscoresList.Count; i++)
                {
                    System.Console.WriteLine(highscoresList[i]);
                }
            }
            string highscore = String.Join(String.Empty, highscoresList);
            File.WriteAllText("Content/files/highscores.txt", highscore);
            highScoreMenu.SetHighScores(highscoresList);
        }

        public void AddHighScore()
        {
            try
            {
                highscoresList.Add(gameOverMenu.getFinalSelection() + ":" + level.getPlayer().getScore().ToString() + ",");
            }
            catch(NullReferenceException)
            {
                highscoresList.Add(gameOverMenu.getFinalSelection() + ":" + 0
                    + ",");
            }
            string highscore = String.Join(String.Empty, highscoresList);
            File.WriteAllText("Content/files/highscores.txt", highscore);
            SortHighScores();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            int width = graphicsDevice.Viewport.Width;
            int height = graphicsDevice.Viewport.Height;
            dustEngineFar.Draw(spriteBatch);
            dustEngineMiddle.Draw(spriteBatch);
            dustEngineNear.Draw(spriteBatch);
            if (gameState == GameState.Playing)
            {
                if (level.getIsActive())
                {
                    drawTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }
                else
                {
                    drawTime = oldDrawTime;
                }
                level.Draw(camera, drawTime, small_font, spriteBatch, currentLevel, width, height);
                oldDrawTime = drawTime;
            }
            if (gameState == GameState.Paused)
            {
                level.Draw(camera, drawTime, small_font, spriteBatch, currentLevel, width, height);
                spriteBatch.Begin();
                spriteBatch.Draw(rect, new Rectangle(0, 0, width, height), Color.Black * 0.5f);
                spriteBatch.End();
                pauseMenu.Draw(spriteBatch, width, height, Color.White * 0.70f, gameTime);
            }
            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(rect, new Rectangle(0, 0, width, height), Color.Black * 0.5f);
                spriteBatch.End();
                mainMenu.Draw(spriteBatch, width, height, Color.White * 0.00f, gameTime);
            }
            if (gameState == GameState.HighScore)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(rect, new Rectangle(0, 0, width, height), Color.Black * 0.5f);
                spriteBatch.End();
                highScoreMenu.Draw(spriteBatch, width, height, Color.White * 0.00f, gameTime);
            }
            if (gameState == GameState.GameOver)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(rect, new Rectangle(0, 0, width, height), Color.Black * 0.5f);
                spriteBatch.End();
                gameOverMenu.Draw(spriteBatch, width, height, Color.White * 0.00f, gameTime);
            }
            if ((GameConstants.Debug) && gameState == GameState.Debug)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(rect, new Rectangle(0, 0, width, height), Color.Black * 0.5f);
                spriteBatch.End();
                debugMenu.Draw(spriteBatch, width, height, Color.White * 0.00f, gameTime);
            }
            if (gameState == GameState.Loading)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(rect, new Rectangle(0, 0, width, height), Color.Black * 0.5f);
                double time = gameTime.TotalGameTime.TotalSeconds;
                float pulsate = (float)Math.Sin(time * 6) + 1;
                Color choiceColor = new Color(0, 204, 0);//green
                float scale = 1 + pulsate * 0.05f;
                spriteBatch.DrawString(large_font, "LOADING", new Vector2(width / 2, (height / 2) - ((large_font.MeasureString("LOADING").Y / 2))), choiceColor, 0, new Vector2(large_font.MeasureString("LOADING").Length() / 2, large_font.MeasureString("LOADING").Y / 2), scale, SpriteEffects.None, 0);
                spriteBatch.End();
            }
        }
    }
}
