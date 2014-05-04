using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace STARShooter
{
    class Object
    {

        public Texture2D sprite;
        public Rectangle boundingRect;
        public Rectangle originalBoundingRect;

        public Vector2 origin;
        public Vector2 position;
        public Vector2 direction = new Vector2(0, -1);
        public float rotationAngle;

        public float speed = 0f;
        public float scale = 1f;


        public Object(Texture2D sprite, Vector2 position)
        {
            this.sprite = sprite;
            this.origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            this.position = position;
        }

        public Matrix getTransformation()
        {
            Matrix transform = Matrix.CreateTranslation(new Vector3(-1 * new Vector2(sprite.Width / 2, sprite.Height / 2), 0f));
            transform *= Matrix.CreateScale(scale);
            transform *= Matrix.CreateRotationZ(-rotationAngle);
            transform *= Matrix.CreateTranslation(new Vector3(position, 0f));


            return transform;
        }

        public void updateBoundingRect()
        {
            //Create Transformation
            Matrix transform = getTransformation();

            //Getting the corners of the transformation's current rectangle
            Vector2 topLeft = new Vector2(originalBoundingRect.Left, originalBoundingRect.Top);
            Vector2 topRight = new Vector2(originalBoundingRect.Right, originalBoundingRect.Top);
            Vector2 bottomLeft = new Vector2(originalBoundingRect.Left, originalBoundingRect.Bottom);
            Vector2 bottomRight = new Vector2(originalBoundingRect.Right, originalBoundingRect.Bottom);

            //Apply the transformation
            //to the corners of the rectangle
            topLeft = Vector2.Transform(topLeft, transform);
            topRight = Vector2.Transform(topRight, transform);
            bottomLeft = Vector2.Transform(bottomLeft, transform);
            bottomRight = Vector2.Transform(bottomRight, transform);

            //Get lowest points
            Vector2 min = Vector2.Min(Vector2.Min(topLeft, topRight), Vector2.Min(bottomLeft, bottomRight));
            Vector2 max = Vector2.Max(Vector2.Max(topLeft, topRight), Vector2.Max(bottomLeft, bottomRight));

            //Assign a new rectangle
            boundingRect = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public bool collision(Object obj)
        {
            Matrix transformToOther = getTransformation() * Matrix.Invert(obj.getTransformation());

            //get pixel data
            Color[] myData = new Color[sprite.Width * sprite.Height];
            sprite.GetData(myData);


            Color[] otherData = new Color[obj.sprite.Width * obj.sprite.Height];
            obj.sprite.GetData(otherData);

            //Check pixels
            for (int myY = 0; myY < sprite.Height; myY++)
            {
                for (int myX = 0; myX < sprite.Width; myX++)
                {
                    Vector2 transformedCoord = Vector2.Transform(new Vector2(myX, myY), transformToOther);

                    int otherX = (int)Math.Round(transformedCoord.X);
                    int otherY = (int)Math.Round(transformedCoord.Y);

                    if (otherX >= 0 && otherY >= 0 && otherX < obj.sprite.Width && otherY < obj.sprite.Height)
                    {
                        if (myData[myX + myY * sprite.Width].A != 0 && otherData[otherX + otherY * obj.sprite.Width].A != 0)
                        {
                            return true;
                        }

                    }
                }
            }
            return false;
        }
    }
}
