﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        Menu pauseMenu, mainMenu, highScoreMenu;
        Level level;
        int currentLevel;
        Camera camera;
        Model playerModel, bulletModel;
        Texture2D pauseMenuImage;
        Model[] asteroidModel;
        List<Model> textures;
        KeyboardState state, lastState;
        SoundEffect engineSound, laserSound, explosionSound;
        SpriteFont small_font, medium_font, large_font;
        SoundEffectInstance engineInstance;
        List<string> highscoresList;
        float drawTime, oldDrawTime;
        GraphicsDeviceManager graphics;
        GameState gameState;
        private float timeBeginLoad;
        DustEngine dustEngine;

        public StateManager(
            GraphicsDeviceManager graphics, 
            Model playerModel, 
            Model[] asteroidModel, 
            Model bulletModel,
            SoundEffect engineSound, 
            SoundEffect laserSound, 
            SoundEffect explosionSound, 
            SoundEffectInstance engineInstance, 
            SpriteFont small_font,
            SpriteFont medium_font,
            SpriteFont large_font,
            List<Model> textures,
            Texture2D pauseMenuImage,
            int currentLevel)
        {
            gameState = GameState.StartMenu;
            this.graphics = graphics;
            this.playerModel = playerModel;
            this.asteroidModel = asteroidModel;
            this.bulletModel = bulletModel;
            this.engineSound = engineSound;
            this.laserSound = laserSound;
            this.explosionSound = explosionSound;
            this.engineInstance = engineInstance;
            this.small_font = small_font;
            this.medium_font = medium_font;
            this.large_font = large_font;
            this.textures = textures;
            this.currentLevel = currentLevel;
            this.pauseMenuImage = pauseMenuImage;
            pauseMenu = new Menu(small_font, medium_font, large_font, "PAUSE MENU", new List<String> { "RESUME", "MAIN MENU" }, pauseMenuImage);
            mainMenu = new Menu(small_font, medium_font, large_font, "MAIN MENU", new List<String> { "PLAY", "HIGHSCORES", "QUIT" }, pauseMenuImage);
            highScoreMenu = new Menu(small_font, medium_font, large_font, "HIGH SCORES", new List<String> { "BACK" }, pauseMenuImage);
            lastState = Keyboard.GetState();
            camera = new Camera(Vector3.Zero, graphics.GraphicsDevice.Viewport.AspectRatio, MathHelper.ToRadians(90.0f));
            dustEngine = new DustEngine(graphics.GraphicsDevice);
            GetHighScores();
        }

        public void LoadNewLevel()
        {           
            level = new Level(playerModel, camera, asteroidModel[2], bulletModel, textures, currentLevel);
        }

        public void Update(GameTime gameTime, Game1 game, GraphicsDevice graphicsDevice)
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
                    LoadNewLevel();
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
                    System.Console.WriteLine("playing");
                    pauseMenu.finalSelection = -10;
                    pauseMenu.currentSelection = 0;
                }
                if (pauseMenu.finalSelection == 1)
                {
                    gameState = GameState.StartMenu;
                    System.Console.WriteLine("menu");
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
            else if (gameState == GameState.Loading)
            {
                if ((float)gameTime.TotalGameTime.TotalMilliseconds >= timeBeginLoad + 5000f)
                {
                    gameState = GameState.Playing;
                }
            }
            dustEngine.Update(graphicsDevice);
            lastState = state;
        }

        public void GetHighScores()
        {
            string[] highscoresArray = File.ReadAllText("Content/files/highscores.txt").Split('-');
            highscoresList = new List<string>(highscoresArray.Length);
            highscoresList.AddRange(highscoresArray);
            highscoresList.Reverse();
            foreach (string score in highscoresList)
            {
                //System.Console.WriteLine(score);
            }
        }

        public void AddHighScore()
        {
            //highscoresList.OrderByDescending(x => x);
            highscoresList.Add(level.player.score.ToString());
            for (int s = 1; s < highscoresList.Count(); s++)
            {
                highscoresList[s] = highscoresList[s] + "-";
                //System.Console.WriteLine(highscoresList[s]);
            }
            string highscore = String.Join(String.Empty, highscoresList);
            File.WriteAllText("Content/files/highscores.txt", highscore);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
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
                spriteBatch.DrawString(large_font, "LOADING", new Vector2(width / 2, (height / 2) - ((large_font.MeasureString("LOADING").Y/2))), choiceColor, 0, new Vector2(large_font.MeasureString("LOADING").Length() / 2, large_font.MeasureString("LOADING").Y /2), scale, SpriteEffects.None, 0);
                spriteBatch.End();
            }
            dustEngine.Draw(spriteBatch);
        }        
    }
}
