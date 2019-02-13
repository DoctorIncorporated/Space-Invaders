using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Implement XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Space_Invaders
{
    class EnemyManager
    {
        //-----------------------------------------------------------------------
        //  - Declarations
        private Texture2D texture;          //Texture for sprite sheet
        private Rectangle initialFrame;     //initial frame for animations
        private int frameCount;             //How many animation frames?

        public List<Enemy> Enemies = new List<Enemy>(); //List of enemies

        public ShotManager EnemyShotManager;    //Shot manager
        private PlayerManager playerManager;    //player manager

		private Rectangle enemyWaveArea; //The rectangle where all the enemies will be located in
		private bool isWaveMovingRight = true; //To determine what direction to move the enemies in
		private int screenWidth; //The width of the screen, used for determining when to move the Wave down
		private bool justMovedDown = false;

        private float shipShotChance = 3f;    //small chance of firing at player every frame per enemy
		private int rowsInWave = 2; //The amount of rows of enemies in a wave
		private int enemiesInRow = 14, //The amount of enemies per row
			H_EnemySpacing = 8, //The horizontal spacing between enemies
			V_EnemySpacing = 8, //The vertical spacing between enemies
			distanceFromWall = 3 * (32 + 8), //Distance away from the wall where the enemies will spawn
			distanceFromCeiling = (32 + 8); //Distance away from the ceiling where the enemies will spawn

		private float waveMovementWaitTimer = 2f, //How long until the wave moves: default = 2f
			waveMovementTimer = 0.0f; //Timer to determine when to move the wave

		// Part 3 - Modified - Set it to false by default
		public bool Active = false;      //active is set to false
		
        private Random rand = new Random(); //random # generator

        //Constructor
        public EnemyManager(Texture2D texture, Rectangle initialFrame, int frameCount, 
            PlayerManager playerManager, Rectangle screenBounds)
        {
            //Set default values based on parameters
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.frameCount = frameCount;
            this.playerManager = playerManager;

			this.screenWidth = screenBounds.Width;

			//Create our enemies shot manager
			EnemyShotManager = new ShotManager(texture, new Rectangle(0, 64, 2, 6),
					1, 2, 150f, screenBounds);
        }


		public void SpawnEnemy(Vector2 startLocation)
		{
			Enemy thisEnemy;
			//Create an enemy
			thisEnemy = new Enemy(texture, startLocation, initialFrame, frameCount);

			//Add the enemy to the list
			Enemies.Add(thisEnemy);
		}


		//Spawn a specific wave/level
		public void SpawnWave()
        {
			if (Enemies.Count != 0) //If there are enemies left, don't make more
				return;

			rowsInWave++; //Add difficulty everytime a wave is defeated, or the player dies

			//A rectangle used to know when the enemies have to move right, down, or left
			enemyWaveArea = new Rectangle(distanceFromWall, distanceFromCeiling,
				enemiesInRow * (initialFrame.Width + H_EnemySpacing),
				rowsInWave * (initialFrame.Width + V_EnemySpacing));

			for (int i = 0; i < rowsInWave; i++)
			{
				for (int j = 0; j < enemiesInRow; j++)
				{//Filling in all of the enemies
					SpawnEnemy(new Vector2(
						(j*(initialFrame.Width + H_EnemySpacing)) + distanceFromWall,
						(i*(initialFrame.Width + V_EnemySpacing)) + distanceFromCeiling));
				}
			}
        }

		private void MoveEnemyWave(GameTime gameTime)
		{
			if (Enemies.Count == 0) //If there are no enemies, leave
				return;
			
			waveMovementTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (waveMovementTimer > waveMovementWaitTimer)
			{
				if ( (!justMovedDown) && ((enemyWaveArea.Location.X + enemyWaveArea.Width) >= (screenWidth - (distanceFromWall / 3)))) //Determine if the block has to shift down
				{
					enemyWaveArea.Location += new Point(0, (initialFrame.Height + V_EnemySpacing));

					//Cycle through all the enemies, moving them
					foreach (Enemy enemy in Enemies)
						enemy.Move(new Vector2(0, (initialFrame.Height + V_EnemySpacing)));

					isWaveMovingRight = !isWaveMovingRight; //When they reach a side, they then have to move in the opposite direction than they were when they encountered the edge
					justMovedDown = true; //Prevents the enemies from always moving down
				}
				else if ((!justMovedDown) && ((enemyWaveArea.Location.X) <= (distanceFromWall / 3)))
				{
					enemyWaveArea.Location += new Point(0, (initialFrame.Height + V_EnemySpacing));

					foreach (Enemy enemy in Enemies)
						enemy.Move(new Vector2(0, (initialFrame.Height + V_EnemySpacing)));

					isWaveMovingRight = !isWaveMovingRight;
					justMovedDown = true;
				}
				else //Otherwise just move the wave horizontally
				{
					enemyWaveArea.Location += new Point(((isWaveMovingRight) ? 1 : -1) * (initialFrame.Width + H_EnemySpacing), 0);

					foreach (Enemy enemy in Enemies)
						enemy.Move(new Vector2(((isWaveMovingRight) ? 1 : -1) * (initialFrame.Width + H_EnemySpacing), 0));
					justMovedDown = false;
				}
				waveMovementTimer = 0f;

				if (enemyWaveArea.Top > playerManager.playerSprite.Center.Y) //If the top row of enemies goes lower than the player center
				{//Then remove all the enemies and hit the player
					Enemies.Clear();
					playerManager.gotHit();
				}
			}
		}

        //Perform the update(s)
        public void Update(GameTime gameTime)
        {
            EnemyShotManager.Update(gameTime);

			//Update the enemy wave and move if it is time
			MoveEnemyWave(gameTime);

			for (int x = Enemies.Count - 1; x >= 0; x--)
            {
				//Update and remove enemies as needed
				Enemies[x].Update(gameTime);
								
                if (Enemies[x].Destroyed == true)
                {
                    Enemies.RemoveAt(x);
				}
                else if ((float)rand.Next(0, 100000) / 10 <= shipShotChance) //
                {
                    Vector2 fireLoc = Enemies[x].EnemySprite.Location;
                    fireLoc += Enemies[x].gunOffset;
                    
                    //Calc the shot direction
                    Vector2 shotDirection = playerManager.playerSprite.Center - fireLoc;
                    shotDirection.Normalize();  //Normalize

                    EnemyShotManager.FireShot(fireLoc, shotDirection, false);
                }
            }

            if (Active)
            {
                SpawnWave();
            }
		}

		//Draw the enemies
		public void Draw(SpriteBatch spriteBatch)
        {
            EnemyShotManager.Draw(spriteBatch);

            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

    }
}
