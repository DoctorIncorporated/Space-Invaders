using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Implement XNA
using Microsoft.Xna.Framework;


namespace Space_Invaders
{
    class CollisionManager
    {
        //---------------------------------------------------------------
        //Part 1 - Declarations
        
        //Declare managers to allow us to interact with each of these collideable objects
        private PlayerManager playerManager;
        private EnemyManager enemyManager;

		private Vector2 offScreen = new Vector2(-500, -500);

		//points per enemy
		private int enemyPointValue = 100;

        //---------------------------------------------------------------


        // Part 1 - Constructor
        public CollisionManager(PlayerManager playerManager, EnemyManager enemyManager)
        {
            //Use the managers that were passed into this collision manager
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
        }


        //--------------------------------------------------------------------------
        // Part 1 - Collision Helper Methods
        /*
        Apart from our asteroids bouncing off of each other, we will handle five 
         different types of collisions:
         1) Player shot to enemy ship collisions
         2) Player shot to asteroid collisions
         3) Enemy shot to player collisions
         4) Enemy ship to player collisions
         5) Asteroid to player collisions
        The first two types of collisions represent the player influencing the game world, 
         while the last three types of collisions result in the player being destroyed.
        We will build five different helper functions to check for these collisions, each 
         one following the same pattern. The two lists of objects involved in a potential 
         collision will be compared to each other, with each resulting collision triggering
         an action.
         */

        
        //Manage collisions between our shots hitting the enemy
        private void checkShotToEnemyCollisions()
        {
            //Loop thru all existing shots within the shot manager...
            foreach (Sprite shot in playerManager.PlayerShotManager.Shots)
            {
                //For each shot, we want to loop thru each enemy...
                foreach (Enemy enemy in enemyManager.Enemies)
                {
                    //Is the current enemy colliding with the current shot?
                    if (shot.IsCircleColliding(enemy.EnemySprite.Center, enemy.EnemySprite.CollisionRadius))
                    {
                        //If collision exists...
                        shot.Location = offScreen;      //move the shot off visible screen
						//enemy.Destroyed = true;         // flag the enemy as destroyed
						enemy.gotHit();
                        playerManager.PlayerScore += enemyPointValue;   //increment score
						SoundManager.PlayExplosion();
					}
				}
            }
        }


        //Manage collisions between Shots and the Player
        private void checkShotToPlayerCollisions()
        {
            //Loop thru each of the shots...
            foreach (Sprite shot in enemyManager.EnemyShotManager.Shots)
            {
                //Is the current shot colliding with the player?
                if (shot.IsCircleColliding(playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    // If collision does occur...
                    shot.Location = offScreen;      //Move the shot off the visible screen
					playerManager.gotHit(); //flag this player for destruction
					SoundManager.PlayExplosion();
				}
			}
        }


        //Manage collisions between the player and the enemy
        private void checkEnemyToPlayerCollisions()
        {
            //Loop thru each of the enemies
            foreach (Enemy enemy in enemyManager.Enemies)
            {
                //is the current enemy colliding with the player?
                if (enemy.EnemySprite.IsCircleColliding(playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    //if collision...
                    enemy.gotHit();     //flag the enemy to be destroyed
					playerManager.gotHit(); //flag the player to be destroyed
					SoundManager.PlayExplosion();
				}
			}
        }


        public void CheckCollisions()
        {
            //Manage shot collisions with enemies and asteroids
            checkShotToEnemyCollisions();

            //If the player is not currently flagged to be destroyed...
            if (!playerManager.Destroyed)
            {
                //check player against enemy shots, enemy collisons, and asteroid collisons
                checkShotToPlayerCollisions();
                checkEnemyToPlayerCollisions();
            }
        }
    }
}
