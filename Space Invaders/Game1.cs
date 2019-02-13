using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Space_Invaders
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		#region Declarations
		//Enumaration to represent various gamestates
		enum GameStates { TitleScreen, Playing, PlayerDead, GameOver };

		GameStates gameState = GameStates.TitleScreen;

		//Objects to hold image files
		Texture2D titleScreen;
		Texture2D spriteSheet;
		StarField starField;

		PlayerManager playerManager;    //Declare player manager
		EnemyManager enemyManager;  //Declare enemy mgr

		CollisionManager collisionManager; //Declare collision mgr

		SpriteFont pericles14; //Declare font for game text
		private float playerDeathDelayTime = 5f;   //Time delay after player dies
		private float playerDeathTimer = 0f;        //Timer used to track time delay
		private float titleScreenTimer = 0f;        //Timer used to manage time to display titlescreen
		private float titleScreenDelayTime = 1f;    //time alotted to display titlescreen
		private int playerStartingLives = 3;        //variable to handle # of lives

		//Location on the screen where the player starts out
		private Vector2 playerStartLocation = new Vector2((400-(32/2)), (600-32)-16);
		private Vector2 scoreLocation = new Vector2(20, 10); //location to display score
		private Vector2 livesLocation = new Vector2(600, 10); //location to display # of lives

		#endregion

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			this.graphics.PreferredBackBufferWidth = 800;
			this.graphics.PreferredBackBufferHeight = 600;
			this.graphics.ApplyChanges();

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//Load the images into our texture objects
			titleScreen	   = Content.Load<Texture2D>(@"Textures\titleScreen");
			spriteSheet   = Content.Load<Texture2D>(@"Textures\spriteSheet");

			//Create the new instance of the star field
			starField = new StarField(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height,
				200, new Vector2(0, 30f), spriteSheet, new Rectangle(3, 65, 2, 2));

			//Create the instance of the player manager
			playerManager = new PlayerManager(spriteSheet, new Rectangle(0, 0, 32, 32), 1,
				new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height));

			// Create the instance of the enemy manager
			enemyManager = new EnemyManager(spriteSheet, new Rectangle(0, 32, 32, 32), 1, playerManager,
				new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height));

			// Part 1 - Create the collision mgr
			collisionManager = new CollisionManager(playerManager, enemyManager);

			//Initialize the SOund Manager by loading the content into pipeline
			SoundManager.Initialize(Content);

			pericles14 = Content.Load<SpriteFont>(@"Fonts\Pericles14");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		private void resetGame()
		{
			//Set the location of the player's ship
			playerManager.playerSprite.Location = playerStartLocation;

			//enemyManager.Enemies.Clear();                   //no enemies
			enemyManager.Active = true;                     //Set enemy manager to active
			//playerManager.PlayerShotManager.Shots.Clear();  //no player shots
			enemyManager.EnemyShotManager.Shots.Clear();    //no enemy shots
			playerManager.Destroyed = false;                //reset the player to living
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			switch (gameState)
			{
				case GameStates.TitleScreen:
					//------------------------------------------------------------------
					// Part 3
					//Increment timer managing how long to display titlescreen
					titleScreenTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

					//If the approp. time has elapsed...
					if (titleScreenTimer >= titleScreenDelayTime)
					{
						// Check whether the space key or Gamepad's "A" button is pressed...
						if ((Keyboard.GetState().IsKeyDown(Keys.Space)) ||
						(GamePad.GetState(PlayerIndex.One).Buttons.A ==
						ButtonState.Pressed))
						{
							//if so...then
							//Set the # of lives to 3 (or whatever the value is)
							playerManager.LivesRemaining = playerStartingLives;
							playerManager.PlayerScore = 0;  //set score to zero
							resetGame();    //reset the game...clearing bad guys, asteroids, bullets
							gameState = GameStates.Playing; //move state to "Playing"
						}
					}
					break;

				//------------------------------------------------------------------
				// Part 3
				case GameStates.Playing:
					starField.Update(gameTime);             //Update the starfield
					playerManager.Update(gameTime);         //update the player
					enemyManager.Update(gameTime);          //Update the enemies
					collisionManager.CheckCollisions();     //check for collisions
															//If player is tagged as destroyed
					if (playerManager.Destroyed)
					{
						playerDeathTimer = 0f;              //reset timer
						enemyManager.Active = false;        //disable enemy manager
						playerManager.LivesRemaining--;     //deduct a life
															//if player has run out of lives...
						if (playerManager.LivesRemaining < 0)
						{
							gameState = GameStates.GameOver;    //set state to "GameOver"
						}
						//else...we still have a life remaining
						else
						{
							gameState = GameStates.PlayerDead;  //Set to "PlayerDead" state
						}
					}
					break;

				//------------------------------------------------------------------
				// Part 3
				case GameStates.PlayerDead:

					//Increment timer
					playerDeathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
					starField.Update(gameTime);         //Update the starfield
					enemyManager.Update(gameTime);      //Update enemies
					playerManager.PlayerShotManager.Update(gameTime);   //Update player shots

					//If timer has reached its limit...
					if (playerDeathTimer >= playerDeathDelayTime)
					{
						resetGame();                        //reset the game and certain vars
						gameState = GameStates.Playing;     //Reset the state to Playing
					}
					break;

				//------------------------------------------------------------------
				// Part 3
				case GameStates.GameOver:
					//Increment timer
					playerDeathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

					starField.Update(gameTime);         //Update starfield
					enemyManager.Update(gameTime);      //Update enemies
					playerManager.PlayerShotManager.Update(gameTime);   //Update player shots

					//If time limit is reached....
					if (playerDeathTimer >= playerDeathDelayTime)
					{
						gameState = GameStates.TitleScreen;     //Return to titlescreen
					}
					break;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			//Begin drawing
			spriteBatch.Begin();

			//if we are in TitleScreen mode...
			if (gameState == GameStates.TitleScreen)
			{
				//Darw the titleScreen graphic to the screen
				spriteBatch.Draw(titleScreen,
					new Rectangle(0, 0, this.Window.ClientBounds.Width,
					this.Window.ClientBounds.Height), Color.White);
			}

			//if we are in Playing, PlayerDead or GameOver mode...
			if ((gameState == GameStates.Playing) ||
			(gameState == GameStates.PlayerDead) ||
			(gameState == GameStates.GameOver))
			{
				//Draw our star field
				starField.Draw(spriteBatch);

				//  - draw our player
				playerManager.Draw(spriteBatch);
				
				//  - draw our Enemies
				enemyManager.Draw(spriteBatch);
				
				// Part 3 - Displaying score and lives
				//display score
				spriteBatch.DrawString(pericles14,
					"Score: " + playerManager.PlayerScore.ToString(), scoreLocation, Color.DeepSkyBlue);

				//If player still living...
				if (playerManager.LivesRemaining >= 0)
				{
					//Display number of lives
					spriteBatch.DrawString(pericles14,
						"Ships Remaining: " + playerManager.LivesRemaining.ToString(),
						livesLocation, Color.DeepSkyBlue);
				}
			}

			// Part 3 
			//if we are in GameOver mode...
			if ((gameState == GameStates.GameOver))
			{
				//Display Game over
				spriteBatch.DrawString(pericles14, "G A M E O V E R !",
					new Vector2(this.Window.ClientBounds.Width / 2 - pericles14.MeasureString
						("G A M E O V E R !").X / 2, 50),
						Color.DeepSkyBlue);
			}

			//We are done drawing, XNA do your thing
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}