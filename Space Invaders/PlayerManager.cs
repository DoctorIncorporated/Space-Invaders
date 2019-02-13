using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Implement XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace Space_Invaders
{
    class PlayerManager
    {
        //--------------------------------------------------------------------
        //  - Declarations
        enum ShipState {DYING, REVIVING, STABLE, STABLIZING}; //State tracking for help with animation
		private ShipState currShipState = ShipState.STABLE;
		private float dyingTime = 1f, dyingTimer = 0f;	//Timer for how long to play dying animation

        public Sprite playerSprite;             //Sprite for player
		private Rectangle initialFrame;         //initial frame for animations
		private Rectangle screenBounds;
		private float playerSpeed = 160.0f;     //Speed
        public long PlayerScore = 0;            //Score
        public int LivesRemaining = 3;          //Lives counter
        public bool Destroyed = false;          //Has the ship been destroyed
        private Vector2 gunOffset = new Vector2(15, 0);    //offset for shots to start from //CHECK LATER
        private float shotTimer = 0.0f;         //timer variable to manage timing for next shot
        private float minShotTimer = 0.75f;//0.2f;      //minimum wait time till next shot
        private int playerRadius = 15;          //Collision radius
        public ShotManager PlayerShotManager;   //instance of player's shot manager
        //--------------------------------------------------------------------


        //--------------------------------------------------------------------
        //  - Constructor
        public PlayerManager(Texture2D texture, Rectangle initialFrame, int frameCount,
            Rectangle screenBounds)
        {
            //Create a sprite instance for the player
            playerSprite = new Sprite(new Vector2((400-(32/2)), (600-32)-16), texture, initialFrame, Vector2.Zero);

            //Create the Player's shot manager
            PlayerShotManager = new ShotManager(texture, new Rectangle(0, 64, 2, 6), 1,
                2, 250f, screenBounds);

			//Loop thru framecount and load any animation frames necessary
			for (int x = 1; x < frameCount; x++)
			{
				playerSprite.AddFrame(
				new Rectangle(
				initialFrame.X + (initialFrame.Width * x),
				initialFrame.Y,
				initialFrame.Width,
				initialFrame.Height));
			}

			//Set the collision radius
			playerSprite.CollisionRadius = playerRadius;

			//Hold onto the initialFrame
			this.initialFrame = initialFrame;

			//Hold onto the screen bounds
			this.screenBounds = screenBounds;

		}


		//   - Methods

		public void gotHit()
		{
			currShipState = ShipState.DYING; //Start the death animation process without destorying it just yet
		}

        //Fire a shot
        private void FireShot()
        {
            //If timer has reached its time limit, fire a shot via the Shot manager
            if (shotTimer >= minShotTimer)
            {
                //Fire shot
                PlayerShotManager.FireShot(playerSprite.Location + gunOffset, new Vector2(0, -1), true);
                //Reset timer
                shotTimer = 0.0f;
            }
        }


        //Method to manage Keyboard input (receives a KB snaptshot)
        private void HandleKeyboardInput(KeyboardState keyState)
        {
            //Player is not allowed to move up or down

            //if (keyState.IsKeyDown(Keys.Up))
            //{
            //    playerSprite.Velocity += new Vector2(0, -1);    //Move up the screen
            //}
            //if (keyState.IsKeyDown(Keys.Down))
            //{
            //    playerSprite.Velocity += new Vector2(0, 1);     //Move downward
            //}
            if (keyState.IsKeyDown(Keys.Left))
            {
                playerSprite.Velocity += new Vector2(-1, 0);    //Move left
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                playerSprite.Velocity += new Vector2(1, 0);     //move right
            }
            if (keyState.IsKeyDown(Keys.Space))                 //try to fire a shot
            {
                FireShot();
            }
        }


        //Handle GamePad input (receives a GamePad snapshot)
        private void HandleGamepadInput(GamePadState gamePadState)
        {
            //Use the gamepad's analog thumbstick coordinates to move the ship
            //We have to subtract the Y because on thumbstick, Up is positive and down negative
            playerSprite.Velocity += new Vector2(gamePadState.ThumbSticks.Left.X,
                /*-gamePadState.ThumbSticks.Left.Y*/0); //The player is not allowed to move vertically
            
            //If "A" button on gamepad is pressed, try to fire a shot
            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                FireShot();
            }
        }


        //Method to make sure player stays in restricted area
        private void imposeMovementLimits()
        {
            //Vector of the player's ship
            Vector2 location = playerSprite.Location;

            //If too far left, bring it back
            if (location.X < screenBounds.X)
                location.X = screenBounds.X;
            
            //if too far right (including the sprites width), move them back
            if (location.X > (screenBounds.Right - playerSprite.Source.Width))
                location.X = (screenBounds.Right - playerSprite.Source.Width);

            //if too high, move them back
            //if (location.Y < playerAreaLimit.Y)
            //    location.Y = playerAreaLimit.Y;

            //if too low, move them back
            //if (location.Y > (playerAreaLimit.Bottom - playerSprite.Source.Height))
            //    location.Y = (playerAreaLimit.Bottom - playerSprite.Source.Height);

            //set the sprite's location to our regulated coordinate
            playerSprite.Location = location;
        }


        //Update player
        public void Update(GameTime gameTime)
        {
			//Shot manager update is run
			PlayerShotManager.Update(gameTime);

			if (currShipState == ShipState.DYING)
			{
				//Change the player sprite to be the exploded sprite
				playerSprite.Source = new Rectangle(    
					initialFrame.Location + new Point(initialFrame.Width, 0),
					initialFrame.Size + new Point(initialFrame.Size.X, 0));
				currShipState = ShipState.STABLIZING;
				return;
			}
			else if (currShipState == ShipState.REVIVING) //Reset the player sprite
			{
				playerSprite.Source = initialFrame; //Reset the player back to the origional frame
				currShipState = ShipState.STABLE;
			}
			else if (currShipState == ShipState.STABLIZING)
			{
				dyingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
				//Timer to track when to change frames
				if (dyingTimer >= dyingTime)
				{
					currShipState = ShipState.REVIVING;
					Destroyed = true;
					dyingTimer = 0f;
				}
				return; //Return here so the player can't shoot from, or move, an exploded sprite
			}			

            //Check to make sure player ship is not destroyed
            if (!Destroyed)
            {
                //Velocity set to zero
                playerSprite.Velocity = Vector2.Zero;
                
                //Accumulate time in timer for next shot
                shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Handle any input
                HandleKeyboardInput(Keyboard.GetState());
                HandleGamepadInput(GamePad.GetState(PlayerIndex.One));
                
                //Normalize the velocity to one unit
                playerSprite.Velocity.Normalize();
                //include the player speed
                playerSprite.Velocity *= playerSpeed;
                //Call the sprite update
                playerSprite.Update(gameTime);
                //Keep the ship in limited area
                imposeMovementLimits();
            }
        }


        //Draw method for Player ship
        public void Draw(SpriteBatch spriteBatch)
        {
            //Call shot manager's draw method
            PlayerShotManager.Draw(spriteBatch);

			//If player ship is not destroyed
			if (!Destroyed || currShipState == ShipState.STABLIZING)
			{
				//Draw the player's ship
				playerSprite.Draw(spriteBatch);
			}
        }
    }
}