using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    public class HighScoreMenu
    {
        int finalSelection = -10;
        //public bool isNew = false;
        int currentSelection;
        List<String> choiceList;
        String[] highScores = new String[10];
        String title;
        SpriteFont small_font, medium_font, large_font;


        public HighScoreMenu(SpriteFont small_font, SpriteFont medium_font, SpriteFont large_font, String title, List<String> choiceList)
        {
            this.small_font = small_font;
            this.medium_font = medium_font;
            this.large_font = large_font;
            this.title = title;
            this.choiceList = choiceList;
            currentSelection = 0;
        }

        public void setCurrentSelection(int i)
        {
            currentSelection = i;
        }

        public void setFinalSelection(int i)
        {
            finalSelection = i;
        }
        public int getFinalSelection()
        {
            return finalSelection;
        }

        public int getCurrentSelection()
        {
            return currentSelection;
        }

        public void MoveSelectionUp()
        {
            if (currentSelection > 0)
            {
                currentSelection--;
                if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                    System.Console.WriteLine("up: " + currentSelection);
#pragma warning restore CS0162 // Unreachable code detected
            }
        }

        public void MoveSelectionDown()
        {
            if (currentSelection < choiceList.Count - 1)
            {
                currentSelection++;
                if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                    System.Console.WriteLine("down: " + currentSelection);
#pragma warning restore CS0162 // Unreachable code detected
            }
        }

        public void SetHighScores(List<String> highScores)
        {
            for (int i = 0; i < this.highScores.Length; i++)
            {
                string[] parts = highScores[i].Split(':');//strip out colon
                string[] parts2 = parts[1].Split(',');//strip out comma
                this.highScores[i] = parts[0] + " " + parts2[0];
            }
        }

        public void Update(KeyboardState state, KeyboardState lastState)
        {
            //System.Console.WriteLine("isNew: " + isNew);
            if (state.IsKeyDown(Keys.Up) && lastState.IsKeyUp(Keys.Up))
            {
                MoveSelectionUp();
            }
            if (state.IsKeyDown(Keys.Down) && lastState.IsKeyUp(Keys.Down))
            {
                MoveSelectionDown();
            }
            if (state.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
            {
                finalSelection = currentSelection;
            }
        }

        public void Draw(SpriteBatch spriteBatch, float width, float height, Color color, GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(large_font, "ASTEROIDS", new Vector2(width / 2 - (large_font.MeasureString("ASTEROIDS").Length() / 2), height / 16), Color.White);
            spriteBatch.DrawString(medium_font, title, new Vector2(width / 2 - (medium_font.MeasureString(title).Length() / 2), height / 5 + (medium_font.MeasureString(title).Y)), Color.White);
            for (int i = 0; i < highScores.Count()/2; i++)
            {
                spriteBatch.DrawString(small_font, highScores[i], new Vector2(width / 3 - (small_font.MeasureString(highScores[i]).Length() / 2), height / 2 + (small_font.MeasureString(highScores[i]).Y)*i), Color.White);
            }
            for (int i = 5; i < highScores.Count(); i++)
            {
                spriteBatch.DrawString(small_font, highScores[i], new Vector2(width / 3 + (width/3) - (small_font.MeasureString(highScores[i]).Length() / 2), height / 2 + (small_font.MeasureString(highScores[i]).Y) * (i-5)), Color.White);
            }



            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale;
            Vector2 origin = new Vector2(0, small_font.LineSpacing / 2);
            Color choiceColor = new Color();
            for (int i = 0; i < choiceList.Count(); i++)
            {

                if (currentSelection == i)
                {
                    choiceColor = new Color(0, 204, 0);//green
                    scale = 1 + pulsate * 0.05f;
                }
                else
                {
                    choiceColor = new Color(255, 255, 255);//white
                    scale = 1;
                }
                spriteBatch.DrawString(small_font, choiceList[i], new Vector2(width / 2, (height / 8) * 6 + (2*(small_font.MeasureString(choiceList[i]).Y))), choiceColor, 0, new Vector2(small_font.MeasureString(choiceList[i]).Length() / 2, small_font.MeasureString(choiceList[i]).Y / 2), scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();

        }
    }
}
