using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace STARShooter
{
    class Bullet: Object
        {
            public bool fired = false; //Determines if bullet should be drawn
            private float pixelsTraveled = 0; //Keeps track of how far the bullet traveled

            //Consts
            private const float MAX_TRAVEL = 500; //How far the bullet can travel before it disappears

            //Constructor
            public Bullet(Texture2D spr, Vector2 pos)
                : base(spr, pos)
            {
                this.boundingRect = new Rectangle((int)(pos.X - spr.Width / 2), (int)(pos.Y - spr.Height / 2), spr.Width, spr.Height);
                this.originalBoundingRect = new Rectangle(0, 0, spr.Width, spr.Height);
                this.speed = 20f;
            }

            //Moves the bullet
            public void move()
            {
                position.X += (int)(direction.X * speed);
                position.Y += (int)(direction.Y * speed);

                boundingRect.X += (int)(direction.X * speed);
                boundingRect.Y += (int)(direction.Y * speed);

                pixelsTraveled += speed;

                if (pixelsTraveled > MAX_TRAVEL)
                {
                    reset();
                }
            }

            //Prepare boolet for firing
            public void initializeAndFire(Vector2 newposition, float angle)
            {
                rotationAngle = angle;
                position = newposition;

                direction.X = (float)Math.Cos(rotationAngle-Math.PI/2);
                direction.Y = (float)Math.Sin(rotationAngle-Math.PI/2);

                updateBoundingRect();

                fired = true;
            }

            //Reset bullet for future use
            public void reset()
            {
                fired = false;
                pixelsTraveled = 0;
            }
        }
    }