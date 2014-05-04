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
    class Player : Object
    {
        //Const values
        public const float MAX_SPEED = 4;

        //Speed variables
        public float accel = .0625f;
        public bool accelerating = false;
        public float rotateMultiplier = 4.5f;

        //Bullets
        public List<Bullet> bullets = new List<Bullet>();

        //Health
        public int hp = 3;

        //Constructor
        public Player(Texture2D spr, Vector2 pos)
            : base(spr, pos)
        {
            this.boundingRect = new Rectangle((int)(pos.X - spr.Width / 2), (int)(pos.Y - spr.Height / 2), spr.Height, spr.Width);
            this.rotationAngle = 0;
            this.originalBoundingRect = new Rectangle(0, 0, spr.Height, spr.Width);
        }

        public void accelerate()
        {
            accelerating = true;

            if (speed <= MAX_SPEED)
                speed += accel;

            direction.X = (float)Math.Cos(rotationAngle-Math.PI/2);
            direction.Y = (float)Math.Sin(rotationAngle-Math.PI/2);
            
            move(direction * speed);
        }

        public void decelerate()
        {
            speed -= accel * .4f;
            move(direction * speed);

            if (speed < 0.4f)
                speed = 0.4f;
        }

        public void move(Vector2 velocity)
        {
            position += velocity;
            boundingRect.X = (int)(position.X - sprite.Width / 2);
            boundingRect.Y = (int)(position.Y - sprite.Height / 2);
        }

        public void rotateLeft(float elapsed)
        {
            rotationAngle -= (float)(Math.PI / 180) * rotateMultiplier;

            updateBoundingRect();
        }

        public void rotateRight(float elapsed)
        {
            rotationAngle += (float)(Math.PI / 180) * rotateMultiplier;

            updateBoundingRect();
        }

        public void setPos(int x, int y)
        {
            position.X = x;
            position.Y = y;
        }

    }
}
