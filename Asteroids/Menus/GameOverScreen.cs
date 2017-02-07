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
    public class GameOverScreen
    {
        String finalSelection = "";
        //public bool isNew = false;
        int currentSelection;
        String[] letterList = new String[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        String confirmationMessage = "PRESS ENTER WHEN DONE";
        int firstletter = 0;
        int secondletter = 0;
        int thirdletter = 0;
        int score = 0;
        String[] choiceList;
        String title;
        SpriteFont small_font, medium_font, large_font;

        public GameOverScreen(SpriteFont small_font, SpriteFont medium_font, SpriteFont large_font, String title, List<String> choiceList)
        {
            this.small_font = small_font;
            this.medium_font = medium_font;
            this.large_font = large_font;
            this.title = title;
            this.choiceList = new String[] { letterList[0], letterList[0], letterList[0] };
            currentSelection = 0;
        }

        public int getScore()
        {
            return score;
        }

        public void setScore(int i)
        {
            score = i;
        }

        public void setCurrentSelection(int i)
        {
            currentSelection = i;
        }

        public void setFinalSelection(String s)
        {
            finalSelection = s;
        }

        public String getFinalSelection()
        {
            return finalSelection;
        }

        public int getCurrentSelection()
        {
            return currentSelection;
        }
        public void MoveSelectionRight()
        {
            if (currentSelection < 2)
            {
                currentSelection++;
                if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                    System.Console.WriteLine("right: " + currentSelection);
#pragma warning restore CS0162 // Unreachable code detected
            }
        }

        public void MoveSelectionLeft()
        {
            if (currentSelection > 0)
            {
                currentSelection--;
                if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                    System.Console.WriteLine("left: " + currentSelection);
#pragma warning restore CS0162 // Unreachable code detected
            }
        }

        public void MoveSelectionUp()
        {
            if (GameConstants.Debug)
#pragma warning disable CS0162 // Unreachable code detected
                System.Console.WriteLine(firstletter);
#pragma warning restore CS0162 // Unreachable code detected
            if (currentSelection == 0)
            {
                if (firstletter == 0)
                {
                    firstletter = 25;
                }
                else
                {
                    firstletter--;
                }
                choiceList[0] = letterList[firstletter];
            }
            if (currentSelection == 1)
            {
                if (secondletter == 0)
                {
                    secondletter = 25;
                }
                else
                {
                    secondletter--;
                }
                choiceList[1] = letterList[secondletter];
            }
            if (currentSelection == 2)
            {
                if (thirdletter == 0)
                {
                    thirdletter = 25;
                }
                else
                {
                    thirdletter--;
                }
                choiceList[2] = letterList[thirdletter];
            }
        }

        public void MoveSelectionDown()
        {
            if (currentSelection == 0)
            {
                if (firstletter == 25)
                {
                    firstletter = 0;
                }
                else
                {
                    firstletter++;
                }
                choiceList[0] = letterList[firstletter];
            }
            if (currentSelection == 1)
            {
                if (secondletter == 25)
                {
                    secondletter = 0;
                }
                else
                {
                    secondletter++;
                }
                choiceList[1] = letterList[secondletter];
            }
            if (currentSelection == 2)
            {
                if (thirdletter == 25)
                {
                    thirdletter = 0;
                }
                else
                {
                    thirdletter++;
                }
                choiceList[2] = letterList[thirdletter];
            }
            //System.Console.WriteLine("up: " + currentSelection);
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
            if (state.IsKeyDown(Keys.Left) && lastState.IsKeyUp(Keys.Left))
            {
                MoveSelectionLeft();
            }
            if (state.IsKeyDown(Keys.Right) && lastState.IsKeyUp(Keys.Right))
            {
                MoveSelectionRight();
            }
            if (state.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
            {
                finalSelection = choiceList[0] + choiceList[1] + choiceList[2];//make a string from the choice list which are strings and should be set to the players name  
            }
        }

        public void Draw(SpriteBatch spriteBatch, float width, float height, Color color, GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(large_font, "ASTEROIDS", new Vector2(width / 2 - (large_font.MeasureString("ASTEROIDS").Length() / 2), height / 16), Color.White);
            spriteBatch.DrawString(medium_font, title, new Vector2(width / 2 - (medium_font.MeasureString(title).Length() / 2), height / 5 + (medium_font.MeasureString(title).Y)), Color.White);
            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale;
            Vector2 origin = new Vector2(0, small_font.LineSpacing / 2);
            Color choiceColor = new Color();
            int offset = 0;
            spriteBatch.DrawString(small_font, "YOU SCORED " + score + ". ENTER YOUR NAME.", new Vector2(width / 2 - (small_font.MeasureString("YOU SCORED " + score + ". ENTER YOUR NAME.").Length() / 2), height / 2 - (small_font.MeasureString("YOU SCORED " + score + ". ENTER YOUR NAME.").Y)), Color.White);
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

                if (i == 0)
                {
                    offset = -30;
                }
                if (i == 1)
                {
                    offset = 0;
                }
                if (i == 2)
                {
                    offset = 30;
                }
                spriteBatch.DrawString(medium_font, choiceList[i], new Vector2((width / 2) +offset, (height / 8)*5), choiceColor, 0, new Vector2(medium_font.MeasureString(choiceList[i]).Length() / 4, medium_font.MeasureString(choiceList[i]).Y / 2), scale, SpriteEffects.None, 0);
            }
            spriteBatch.DrawString(small_font, confirmationMessage, new Vector2(width / 2 - (small_font.MeasureString(confirmationMessage).Length() / 2), height / 2 + (4 * small_font.MeasureString(confirmationMessage).Y)), Color.White);
            spriteBatch.End();

        }
    }
}
