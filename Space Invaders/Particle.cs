using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Implement XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Space_Invaders
{
    class Particle : Sprite     //inherit from the Sprite class
    {
        //----------------------------------------------------------------------------
        // Part 1 - Declarations
        private Vector2 acceleration;       //Accelaration for both X & Y
        private float maxSpeed;             // Maximum speed that can be reached
        private int initialDuration;        // particle's duration is set by constr. param
        private int remainingDuration;      // used to track remaining time for a particle
        private Color initialColor;         // set its initial color to start out with
        private Color finalColor;           // final color we will transition to
        //----------------------------------------------------------------------------


        //----------------------------------------------------------------------------
        // Part 1 - Properties

        //Returns the total duration of this particle
        public int ElapsedDuration
        {
            get
            {
                //Calculate how much time has been used up already
                return initialDuration - remainingDuration; 
            }
        }

        //Returns the percentage of time the particle has used
        public float DurationProgress
        {
            get
            {
                return (float)ElapsedDuration /
                    (float)initialDuration;
            }
        }


        //Tells us if a particle is still active or not
        public bool IsActive
        {
            get
            {
                //When remaining duration hits zero, it's no longer active
                return (remainingDuration > 0);
            }
        }
        //----------------------------------------------------------------------------



        //----------------------------------------------------------------------------
        // Part 1 - Constructor
        //Create a particle using typical Sprite params, plus ones specific to a Particle
        // calls the base (Sprite) constructor
        // Params: (Location of particle, spritesheet, source rectangle for first frame,
        // current speed & direction, acceleration value, max speed, lifetime of particle,
        // starting color, ending color)
        public Particle(Vector2 location, Texture2D texture, Rectangle initialFrame,
            Vector2 velocity, Vector2 acceleration, float maxSpeed, int duration, 
            Color initialColor, Color finalColor)
            : base(location, texture, initialFrame, velocity)
        {
            //Set newer particle properties based on params.
            initialDuration = duration;
            remainingDuration = duration;
            this.acceleration = acceleration;
            this.initialColor = initialColor;
            this.maxSpeed = maxSpeed;
            this.finalColor = finalColor;
        }
        //----------------------------------------------------------------------------


        //----------------------------------------------------------------------------
        // Part 1 - Methods

        // Update Method -  Handles the life of a particle instance
        public override void Update(GameTime gameTime)
        {
            // If it is not active, why update it ?
            if (IsActive)
            {
                // Increase speed based on acceleration
                velocity += acceleration;

                //if velocity is greater than max speed...
                if (velocity.Length() > maxSpeed)
                {
                    //Normalize to one unit
                    velocity.Normalize();
                    //Multiply by max speed to limit us to max speed
                    velocity *= maxSpeed;
                }

                //Use linear interpolation (LERP) to slowly transition from one color to another
                TintColor = Color.Lerp(initialColor, finalColor, DurationProgress);
                //deduct from time remaining
                remainingDuration--;
                //Base update call
                base.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //If the particle is active, draw it using the original Sprite draw
            if (IsActive)
            {
                base.Draw(spriteBatch);
            }
        }

    }
}
