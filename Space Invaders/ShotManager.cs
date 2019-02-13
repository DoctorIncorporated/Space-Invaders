using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Implement XNA Libraries
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Space_Invaders
{
	class ShotManager
	{
		//------------------------------------------------------------
		//   - Declarations
		public List<Sprite> Shots = new List<Sprite>();     //List of sprites for bullets
		private Rectangle screenBounds;                     //rectangle representing screen area
		private static Texture2D Texture;                   //texture for spritesheet
		private static Rectangle InitialFrame;              //First frame in possible animation
		private static int FrameCount;                      //How many frames of animation
		private float shotSpeed;                            //speed of each shot
		private static int CollisionRadius;                 //Radius of circle for collision det.
															//------------------------------------------------------------


		//------------------------------------------------------------
		//   - Constructor
		//Required params
		//Sprite sheet, location of initial sprite frame, how many frames, radius of collis.
		//  circle, speed of bullet, screen/window boundaries
		public ShotManager(Texture2D texture, Rectangle initialFrame, int frameCount,
			int collisionRadius, float shotSpeed, Rectangle screenBounds)
		{
			Texture = texture;
			InitialFrame = initialFrame;
			FrameCount = frameCount;
			CollisionRadius = collisionRadius;
			this.shotSpeed = shotSpeed;
			this.screenBounds = screenBounds;
		}
		//------------------------------------------------------------


		//------------------------------------------------------------
		//   - Methods

		//Add a new shot to theShot Manager
		public void FireShot(Vector2 location, Vector2 velocity, bool playerFired)
		{
			//Create new sprite instance
			 Sprite thisShot = new Sprite(location, Texture, InitialFrame, velocity);

			//Multiply sprites velocity times our shot speed
			thisShot.Velocity *= shotSpeed;

			//Loop thru framecount and load in each animation frame
			for (int x = 1; x < FrameCount; x++)
			{
				thisShot.AddFrame(new Rectangle(
				InitialFrame.X + (InitialFrame.Width * x),
				InitialFrame.Y,
				InitialFrame.Width,
				InitialFrame.Height));
			}
			//Set collision radius
			thisShot.CollisionRadius = CollisionRadius;
			//Add this shot instance to the sprite list
			Shots.Add(thisShot);

			//----------------------------------------------------------------------
			// Part 2 - If either player or enemy fired, play appropriate sound
			//----------------------------------------------------------------------
			if (playerFired)
			{
				SoundManager.PlayPlayerShot();
			}
			else
			{
				SoundManager.PlayEnemyShot();
			}
			//----------------------------------------------------------------------


		}


		//Update the shots
		public void Update(GameTime gameTime)
		{
			//Loop thru the shots backwards so we can easily remove shots as needed
			for (int x = Shots.Count - 1; x >= 0; x--)
			{
				//Call the sprite's base update
				Shots[x].Update(gameTime);
				//If the shot is not within the screen rectangle (off screen), remove it
				if (!screenBounds.Intersects(Shots[x].Destination))
				{
					Shots.RemoveAt(x);
				}
			}
		}


		//Draw each shot
		public void Draw(SpriteBatch spriteBatch)
		{
			//Loop thru shots sprite list and call base draw method
			foreach (Sprite shot in Shots)
			{
				shot.Draw(spriteBatch);
			}
		}

		//------------------------------------------------------------


	}
}
