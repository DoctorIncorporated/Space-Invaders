using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Include some XNA libraries
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Space_Invaders
{
    class Sprite
    {
        //------------------------------------------------------------------
        //  
        public Texture2D Texture;   //Texture to load sprite sheet into

        //List of rectangles to hold each of the frames of animation from sprite sheet
        protected List<Rectangle> frames = new List<Rectangle>();
        
        private int frameWidth = 0;     //Width of a frame in the sprite sheet
        private int frameHeight = 0;    //Height of a frame in the sprite sheet
        private int currentFrame;       //Variable to hold the index of the current frame
        private float frameTime = 0.1f; //variable to manage timing per frame
        private float timeForCurrentFrame = 0.0f;   //current frame's time-tracker/counter
        
        private Color tintColor = Color.White;  //Lighting color set to white by default
        private float rotation = 0.0f;          //rotation is set to zero to start
        
        public int CollisionRadius = 0;         //Radius in pixels used to help find collisions
        public int BoundingXPadding = 0;        // Padding for the collision area
        public int BoundingYPadding = 0;        // Padding for the collision area
        
        protected Vector2 location = Vector2.Zero;  //Location of sprite on the screen (0,0)
        protected Vector2 velocity = Vector2.Zero;  //Speed & direction of ship movement (0,0)

        //------------------------------------------------------------------



        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //   - 
        // Constructor - Receives the location, texture, rectangle and velocity
        // Initializes a newly created sprite object
        public Sprite(Vector2 location,Texture2D texture,Rectangle initialFrame,Vector2 velocity)
        {
            this.location = location;   //placement of the sprite
            Texture = texture;          //images for our sprite
            this.velocity = velocity;   //speed & direction
            frames.Add(initialFrame);   //initial rectangle/frame from spritesheet
            frameWidth = initialFrame.Width;    //width of this frame
            frameHeight = initialFrame.Height;  //height of this frame
        }


        //Location Property
        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        //Velocity Property
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        //Lighting color, typically white
        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        //Rotation Property, set restricts max degrees to 360 via modulus
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }


        //Frame Property
        // Clamp/limit the currentFrame within the number of frames
        public int Frame
        {
            get { return currentFrame; }
            set
            {
                currentFrame = (int)MathHelper.Clamp(value, 0,
                    frames.Count - 1);
            }
        }

        //FrameTime Propery allows us to adjust the animation update speed
        public float FrameTime
        {
        get { return frameTime; }

            set { frameTime = MathHelper.Max(0, value); }   //max # between zero and the value
        }

        
        //Property of the current frame...returns the rectangle for this frame.
        public Rectangle Source
        {
            get { return frames[currentFrame]; }
			set { frames[currentFrame] = value; }
        }

        
        //Destination returns a rectangle built from the current location
        // and the the dimensions of the frame.
        public Rectangle Destination
        {
            get
            {
                return new Rectangle(
                (int)location.X,
                (int)location.Y,
                frameWidth,
                frameHeight);
            }
        }

        
        //Returns the center of the sprite in the form of a Vector2
        public Vector2 Center
        {
            get
            {
                return location +
                new Vector2(frameWidth / 2, frameHeight / 2);
            }
        }



        //Returns a rectangle that is positioned at the location and
        // as big as the sprite frame, minus the padding around the edges.
        //  This creates a rectangle a bit smaller than the actual frame
        //  so that we reduce the number of false collisions.
        public Rectangle BoundingBoxRect
        {
            get
            {
                return new Rectangle(
                (int)location.X + BoundingXPadding,
                (int)location.Y + BoundingYPadding,
                frameWidth - (BoundingXPadding * 2),
                frameHeight - (BoundingYPadding * 2));
            }
        }


        //We check this Bounding Box rectangle to see if it 
        //  touches another rectangle 
        public bool IsBoxColliding(Rectangle OtherBox)
        {
            return BoundingBoxRect.Intersects(OtherBox);
        }


        //Radial Collision Detection using circles.
        // Receives the center of the circle and its radius.
        //returns true if there is a collision 
        public bool IsCircleColliding(Vector2 otherCenter, float
        otherRadius)
        {
            //Use the Distance method to calc the distance between the centers
            // of the two objects/circles.
            // Then check if this distance is LESS than the sum of the two radius
            // If distance is less than the sum of 2 radius, then they are colliding.
            if (Vector2.Distance(Center, otherCenter) <
            (CollisionRadius + otherRadius))
                return true;
            else
                return false;
        }


        //Method to allow us to add multiple frames to the list for animation
        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle);
        }

        
        //Update method for the Sprite class
        public virtual void Update(GameTime gameTime)
        {
            //Get the elapsed time in seconds since last update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //take this elapsed time and add it to our animation timer
            timeForCurrentFrame += elapsed;

            //if our timer has reached the time limit per frame (FrameTime)...
            if (timeForCurrentFrame >= FrameTime)
            {
                //Move to the next frame.  Modulus makes sure we wrap to first
                // frame if we go past the last frame
                currentFrame = (currentFrame + 1) % (frames.Count);
                //Reset our animation timer
                timeForCurrentFrame = 0.0f;
            }
            //Move to the next location based on velocity and elapsed time
            // We used elapsed time so speed would be the same on varying PC's
            // with different speeds and hardware.
            location += (velocity * elapsed);
        }


        //Draw method - draws the current frame to the screen
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
            Texture,    // full sprite sheet
            Center,     // instead of destination rectangle, we use the center point
            Source,     // source rectangle copies from specific location on sprite sheet
            tintColor,  // light shining on our object
            rotation,   // rotation of our image
            new Vector2(frameWidth / 2, frameHeight / 2),   //origin point (center)
            1.0f,   //scale
            SpriteEffects.None, //no sprite effects
            0.0f);  //layer depth
        }
        //------------------------------------------------------------------
    
    
    }
}
