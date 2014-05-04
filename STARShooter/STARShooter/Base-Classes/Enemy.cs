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
    class Enemy : Object
    {
        Random rand;

        //Constructor

        public Enemy(Texture2D sprite, Vector2 position, Vector2 newDirection, ref Random randomObject)
            : base(sprite, position)
        {
            this.boundingRect = new Rectangle((int)(position.X - sprite.Width / 2), (int)(position.Y - sprite.Width / 2), sprite.Height, sprite.Width);
            this.originalBoundingRect = new Rectangle(0, 0, sprite.Height, sprite.Width);
            speed = Game1.rand.Next(2,5);
            rand = randomObject;
            direction = newDirection;

        }

        public void move()
        {
            float angleToEarth = (float)Math.Atan2( Game1.centerGravity.Y+Game1.earthsprite.Height/2-position.Y, 
                                                    Game1.centerGravity.X+Game1.earthsprite.Width/2-position.X );
            direction.X += (float)Math.Cos(angleToEarth) * .02F;
            direction.Y += (float)Math.Sin(angleToEarth) * .015F;
            direction = Vector2.Normalize(direction);
            position.X += (float)(direction.X * speed);
            position.Y += (float)(direction.Y * speed);

            boundingRect.X += (int)(direction.X * speed);
            boundingRect.Y += (int)(direction.Y * speed);

            updateBoundingRect();
        }


    }
}
