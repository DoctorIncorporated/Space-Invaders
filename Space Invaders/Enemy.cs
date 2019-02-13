using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Implement XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Space_Invaders
{
    class Enemy
    {
		//   - Declarations
		enum ShipState { DYING, ALIVE, DEAD }; //State tracking for help with animation
		private ShipState currShipState = ShipState.ALIVE;
		private float dyingTime = 0.9f, dyingTimer = 0f;  //Timer for how long to play dying animation


		public Sprite EnemySprite;                          //Sprite for an Enemy
		private Rectangle initialFrame;
        public Vector2 gunOffset = new Vector2(15, 16);     //offset to place bullet starting point
        private float speed;                         //speed that the enemy moves at
        public bool Destroyed = false;                      // Is enemy destroyed?
        private int enemyRadius = 15;                       //Radius for circular collision det.
		private int shotsToKill;

        //Constrcutor
        public Enemy(Texture2D texture, Vector2 location, Rectangle initialFrame, int frameCount, int shotsToKill = 1, float speed = 120f)
        {
            //Create a new sprite for the enemy using the ship's location, sprite sheet,
            //  Starting frame from sprite sheet, velocity of zero, to start with
            EnemySprite = new Sprite(location, texture, initialFrame, Vector2.Zero);
            
            //Loop thru frameCount and add each legit animation block
            for (int x = 1; x < frameCount; x++)
            {
                EnemySprite.AddFrame(new Rectangle( initialFrame.X = (initialFrame.Width * x),
                    initialFrame.Y, initialFrame.Width, initialFrame.Height));
            }

			this.shotsToKill = shotsToKill;
			this.speed = speed;

            EnemySprite.CollisionRadius = enemyRadius;  //Set collision radius

			this.initialFrame = initialFrame;
        }

		public void gotHit()
		{
			--shotsToKill;
			if (shotsToKill <= 0 && currShipState != ShipState.DEAD) //Checking to see if it's already dead to prevent two shots hitting it 
				currShipState = ShipState.DYING;                    //and causing it to repeat the death cycle, but still allowing for the second shot to be absorbed
		}

		//Allows the enemy manager to control the exact position of the enemies
		//via teleportation
		public void Move(Vector2 amount)
		{
			EnemySprite.Location += amount;
		}

		public void Update(GameTime gameTime)
		{
			if (currShipState == ShipState.DYING) //After an enemy is hit, they begin to die, but not imediatley
			{
				EnemySprite.Source = new Rectangle(    //Change the only frame to be the first dying frame
					initialFrame.Location + new Point(initialFrame.Width+1, 0), initialFrame.Size);
				EnemySprite.AddFrame(new Rectangle(initialFrame.Location + new Point((initialFrame.Width + 1) * 2, 0), initialFrame.Size)); //Add the two more dying
				EnemySprite.AddFrame(new Rectangle(initialFrame.Location + new Point((initialFrame.Width + 1) * 3, 0), initialFrame.Size)); //frames to the sprite
				EnemySprite.FrameTime = 0.33f; //Lengthen the amount of time it takes between frames

				currShipState = ShipState.DEAD;
			}
			else if (currShipState == ShipState.DEAD)
			{
				//Timer to allow the death animation to finish first
				dyingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
				//Timer to track when to change frames
				if (dyingTimer >= dyingTime)
				{
					Destroyed = true; //Mark the ship for complete removal once the animation has finished
				}
			}

			//If the enemy is still active...
			if (!Destroyed)
			{
				//Call sprite update
				EnemySprite.Update(gameTime);
			}
		}


		public void Draw(SpriteBatch spriteBatch)
        {
            //If this enemy is active, draw the enemy
            if (!Destroyed)
            {
                EnemySprite.Draw(spriteBatch);
            }
        }
    }

}
