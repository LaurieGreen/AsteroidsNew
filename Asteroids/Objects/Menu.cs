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
    public class Menu
    {
        public int finalSelection = -10;
        //public bool isNew = false;
        public int currentSelection;
        List<String> choiceList;
        String title;
        SpriteFont small_font, medium_font, large_font;
        

        public Menu(SpriteFont small_font, SpriteFont medium_font, SpriteFont large_font, String title,  List<String> choiceList)
        {
            this.small_font = small_font;
            this.medium_font = medium_font;
            this.large_font = large_font;
            this.title = title;
            this.choiceList = choiceList;
            currentSelection = 0;
        }

        public void MoveSelectionUp()
        {
            if (currentSelection > 0)
            {
                currentSelection--;
                System.Console.WriteLine("up: " + currentSelection);
            }
        }

        public void MoveSelectionDown()
        {
            if (currentSelection < choiceList.Count-1)
            {
                currentSelection++;
                System.Console.WriteLine("down: " + currentSelection);
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
            spriteBatch.DrawString(medium_font, title, new Vector2(width / 2 - (medium_font.MeasureString(title).Length() / 2), height / 5 + (2 * medium_font.MeasureString(title).Y)), Color.White);
            for (int i = 0; i < choiceList.Count(); i++)
            {
                // Pulsate the size of the selected menu entry.
                double time = gameTime.TotalGameTime.TotalSeconds;
                float pulsate = (float)Math.Sin(time * 6) + 1;
                float scale;
                Vector2 origin = new Vector2(0, small_font.LineSpacing / 2);
                Color choiceColor = new Color();
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
                spriteBatch.DrawString(small_font, choiceList[i], new Vector2(width / 2, (height /8)* 5 + ((small_font.MeasureString(choiceList[i]).Y)*i)), choiceColor, 0, new Vector2(small_font.MeasureString(choiceList[i]).Length() / 2, small_font.MeasureString(choiceList[i]).Y / 2), scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();

        }
    }
}
