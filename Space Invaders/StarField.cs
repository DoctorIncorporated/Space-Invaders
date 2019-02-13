using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// pull in XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Space_Invaders
{
    class StarField
    {
        //--------------------------------------------------------------------------------
        //   - Variables/Properties
        //--------------------------------------------------------------------------------
        private List<Sprite> stars = new List<Sprite>();    //List of star sprites
        private int screenWidth = 800;                      //Screen width
        private int screenHeight = 600;                     //Screen Height
        private Random rand = new Random();                 // Random # generator
        //Array of possible colors for our stars
        private Color[] colors = 
            { Color.White, Color.Yellow, Color.Wheat, Color.WhiteSmoke, Color.SlateGray };

        //--------------------------------------------------------------------------------



        //--------------------------------------------------------------------------------
        //   - Constructor - Creates our randomly designed star field
        //--------------------------------------------------------------------------------
        public StarField(
            int screenWidth,    //screen width
            int screenHeight,   //screen height
            int starCount,      //how many stars
            Vector2 starVelocity,       //speed and direction
            Texture2D texture,          //texture to use
            Rectangle frameRectangle)   //rectangle for current frame
        {
            //Set the screen/window dimensions
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            //Loop for each of the stars
            for (int x = 0; x < starCount; x++)
            {
                //Create a new star sprite with a random location
                stars.Add(new Sprite(new Vector2(rand.Next(0, screenWidth),
                    rand.Next(0, screenHeight)), texture, frameRectangle, starVelocity));

                // Choose a random color from the array
                Color starColor = colors[rand.Next(0, colors.Count())];

                //calulate a random percentage of the color and light
                starColor *= (float)(rand.Next(30, 80) / 100f);
                stars[stars.Count() - 1].TintColor = starColor;
            }
        }
        //--------------------------------------------------------------------------------



        //--------------------------------------------------------------------------------
        //   - Methods
        //--------------------------------------------------------------------------------


        //Update Method
        public void Update(GameTime gameTime)
        {
            //Loop thru the stars
            foreach (Sprite star in stars)
            {
                //Call each star sprite's Update
                star.Update(gameTime);

                //If the star has gone below the bottom of the screen
                // recycle the star by giving it a new X at the top of the screen
                if (star.Location.Y > screenHeight)
                {
                    star.Location = new Vector2(
                    rand.Next(0, screenWidth), 0);
                }
            }
        }
        

        //Use our current spritbatch and use it to loop thru the stars and draw them
        // using the sprite's base-draw method.
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite star in stars)
            {
                star.Draw(spriteBatch);
            }
        }

        //--------------------------------------------------------------------------------


    }
}
