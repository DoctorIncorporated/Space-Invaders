using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;   //Added to allow us to write to the debugger window
//Implement XNA Framework files pertaining to sound
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

//-------------------------------------------------------
//Part 2 - This whole file was created in Part 2
//-------------------------------------------------------

namespace Space_Invaders
{
    //Notice this class is listed as public static...Meaning it does not require an
    //instance to be created.  Acts more like a "function" library file.
    public static class SoundManager
    {
		//List to manage and play explosion sounds
		private static SoundEffect explosion;

        private static SoundEffect shot;      //Manages sound effect when a shot is fired

        private static Random rand = new Random();  // Random # generator



        // Initialize function acts like a constructor in many ways
        // We need to point it to the content manager, in order for this function to
        //  access the sound assets
        public static void Initialize(ContentManager content)
        {
            //Try-catches will be used to access sound files because the game would
            // fail/error-out if no sound card is found on the machine this game plays on.
            try
            {
                //Load the two shot sounds
                shot = content.Load<SoundEffect>(@"Sounds\shot");
                explosion = content.Load<SoundEffect>(@"Sounds\death");
			}
            //If a failure/error occurs...
            catch
            {
                //Write this feedback to the debugger window
                Debug.Write("SoundManager Initialization Failed");
            }
        }


        //Randomly play one of the explosion sounds
        public static void PlayExplosion()
        {
            try
            {
                //Pick one of the explosions in the list and play it
                explosion.Play();
            }
            catch
            {
                //if error, leave feedback in debug window
                Debug.Write("PlayExplosion Failed");
            }
        }


        //Function to play the sound effect when a player fires its bullets
        public static void PlayPlayerShot()
        {
            try
            {
                shot.Play();  //play firing sound
            }
            catch
            {
                Debug.Write("PlayPlayerShot Failed");   //else...diplay error
            }
        }



        //Function to play the sound effect when an enemy fires its bullets
        public static void PlayEnemyShot()
        {
            try
            {
                shot.Play();   //Play enemy firing sound
            }
            catch
            {
                Debug.Write("PlayEnemyShot Failed");    //else...display error
            }
        }
	}
}
