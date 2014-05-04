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
 
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState keyboardState;

        //Random object to generate random values
        public static Random rand = new Random();

        //Dimensions
        int windowWidth = 800;
        int windowHeight = 600;

        //Background sprite + Rectangle
        Texture2D background;
        Rectangle windowRect;

        //earth sprite 
        public static Texture2D earthsprite;
        public static Vector2 centerGravity;

        //wormhole sprite
        Texture2D wormholesprite;
        Object wormhole;

        //Player object and shooting variables
        Player player;
        Texture2D playerFull;
        Texture2D PlayerHit;
        Texture2D playerDamaged;
        double prevShootTime = 0;
        int currentBullet = 0;

        //List of stars
        List<Enemy> stars = new List<Enemy>();

        //Const values
        const int MAX_STARS = 20;

        int NUM_STARS = 5;

        enum screenStatus {START, INSTRUCTIONS, PAUSE, INPROGRESS, GAMEOVER, WIN};


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Set size of game
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;

            //Rectangle to draw background image
            windowRect = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

 
        protected override void Initialize()
        {
    
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Create player & load textures
            playerFull = Content.Load<Texture2D>(@"sprites\shipsprite");
            PlayerHit = Content.Load<Texture2D>(@"sprites\shooter2");
            playerDamaged = Content.Load<Texture2D>(@"sprites\shooter3");
            player = new Player(playerFull, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));

            earthsprite = Content.Load<Texture2D>(@"sprites\center");
            centerGravity = new Vector2(windowWidth/2 - earthsprite.Width/2, windowHeight/2 - earthsprite.Height/2);

            wormholesprite = Content.Load<Texture2D>(@"sprites\wormhole2");
            wormhole = new Object(wormholesprite, centerGravity);

            //Populate Stars list
            createStars();

            background = Content.Load<Texture2D>(@"sprites\background");
        }

 
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Handle player input
            keyboardState = Keyboard.GetState();

            //Accelerating player
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                player.accelerate();
            }
            else
                player.accelerating = false;

            //Decelerating player
            if (!player.accelerating && player.speed > 0)
            {
                player.decelerate();
            }

            //Rotating player
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                player.rotateLeft(elapsed);
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                player.rotateRight(elapsed);
            }

            //Player shoot
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                player.bullets.Add(new Bullet(Content.Load<Texture2D>(@"sprites\bullet"), player.position));
                double currentTime = gameTime.TotalGameTime.TotalMilliseconds;

                if (currentTime - prevShootTime > 200 || prevShootTime == 0)
                {
                    prevShootTime = currentTime;

                    player.bullets[currentBullet].initializeAndFire(player.position, player.rotationAngle);
                    //Console.WriteLine(player.bullets[currentBullet].fired+ " - "+ currentBullet);

                    currentBullet++;
                    if (currentBullet > 2)
                        currentBullet = 0;
                }

            }

            //Handle bullet movement
            for (int i = 0; i < player.bullets.Count; i++)
            {
                if (player.bullets[i].fired)
                {
                    player.bullets[i].move();

                    if (player.bullets[i].position.X > graphics.PreferredBackBufferWidth) //Right side
                    {
                        player.bullets[i].position.X = -player.bullets[i].sprite.Width;
                        player.bullets[i].position.Y = graphics.PreferredBackBufferHeight - player.bullets[i].position.Y;
                    }
                    if ((player.bullets[i].position.X + player.bullets[i].sprite.Width) < 0) //Left side
                    {
                        player.bullets[i].position.X = graphics.PreferredBackBufferWidth - player.bullets[i].sprite.Width;
                        player.bullets[i].position.Y = graphics.PreferredBackBufferHeight - player.bullets[i].position.Y;
                    }
                    if (player.bullets[i].position.Y > graphics.PreferredBackBufferHeight + player.bullets[i].sprite.Height) //Bottom side
                    {
                        player.bullets[i].position.Y = -player.bullets[i].sprite.Height;
                    }
                    if ((player.bullets[i].position.Y + player.bullets[i].sprite.Height) < 0) //Top side
                    {
                        player.bullets[i].position.Y= graphics.PreferredBackBufferHeight + player.bullets[i].sprite.Height;
                    }
                }
            }

            //Handle star movement & wrap stars back when out of window
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].move();

                if (stars[i].position.X > graphics.PreferredBackBufferWidth) //Right side
                {
                    stars[i].position.X = -stars[i].sprite.Width;
                    stars[i].position.Y = graphics.PreferredBackBufferHeight - stars[i].position.Y;
                }
                if ((stars[i].position.X + stars[i].sprite.Width) < 0) //Left side
                {
                    stars[i].position.X = graphics.PreferredBackBufferWidth - stars[i].sprite.Width;
                    stars[i].position.Y = graphics.PreferredBackBufferHeight - stars[i].position.Y;
                }
                if (stars[i].position.Y > graphics.PreferredBackBufferHeight + stars[i].sprite.Height) //Bottom side
                {
                    stars[i].position.Y = -stars[i].sprite.Height;
                }
                if ((stars[i].position.Y + stars[i].sprite.Height) < 0) //Top side
                {
                    stars[i].position.Y = graphics.PreferredBackBufferHeight + stars[i].sprite.Height;
                }

                //Check if star hit bullet
                if (player.boundingRect.Intersects(stars[i].boundingRect))
                {
                    //Change player's sprite depending on how many times collided with star
                    if (player.collision(stars[i]))
                    {
                        //player.hp--;

                        if (player.hp == 2)
                        {
                            player.sprite = PlayerHit;
                            player.setPos(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
                        }
                        else if (player.hp == 1)
                        {
                            player.sprite = playerDamaged;
                            player.setPos(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
                        }
                        else if (player.hp == 0)
                            restartGame();
                    }
                }

                //Check if star hit a bullet
                for (int c = 0; c < player.bullets.Count; c++)
                {
                    if (player.bullets[c].fired)
                    {
                        if (player.bullets[c].boundingRect.Intersects(stars[i].boundingRect))
                        {
                            if (player.bullets[c].collision(stars[i]))
                            {
                                //reset bullet
                                player.bullets[c].reset();

                                //Decrease size of star when it's hit
                                stars[i].scale--;
                                if (stars[i].scale <= 0)
                                {
                                    stars.Remove(stars[i]);
                                    i = 0;
                                }

                                //repopulate stars list once they have all been destroyed
                                if (stars.Count == 0)
                                {
                                    createStars();
                                    if (NUM_STARS != MAX_STARS)
                                        NUM_STARS++;
                                }
                            }
                        }
                    }
                }

                //collision with wormhole
                if( player.collision(wormhole) )
                {
                    float targetangle = (float)Math.Atan2(centerGravity.Y+earthsprite.Height/2-player.position.Y, 
                                                          centerGravity.X+earthsprite.Width/2-player.position.X);
                    Vector2 toCenter = new Vector2((float)Math.Cos(targetangle), (float)Math.Sin(targetangle));
                    toCenter = Vector2.Normalize(toCenter);
                    player.position.X += toCenter.X * wormholesprite.Width;
                    player.position.Y += toCenter.Y * wormholesprite.Height;

                }
                
            }

            //Wrap player back when out of window
            if (player.position.X > graphics.PreferredBackBufferWidth) //Right side
            {
                player.position.X = -player.sprite.Width;
                player.position.Y = graphics.PreferredBackBufferHeight - player.position.Y;
            }
            if ((player.position.X + player.sprite.Width) < 0) //Left side
            {
                player.position.X = graphics.PreferredBackBufferWidth - player.sprite.Width;
                player.position.Y = graphics.PreferredBackBufferHeight - player.position.Y;
            }
            if (player.position.Y > graphics.PreferredBackBufferHeight + player.sprite.Height) //Bottom side
            {
                player.position.Y = -player.sprite.Height;
            }
            if ((player.position.Y + player.sprite.Height) < 0) //Top side
            {
                player.position.Y = graphics.PreferredBackBufferHeight + player.sprite.Height;
            }
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            //Draw background
            spriteBatch.Draw(background, windowRect, Color.White);
            
            //draw earth
            spriteBatch.Draw(earthsprite, centerGravity, Color.White);
            
            //draw wormhole
            spriteBatch.Draw(wormholesprite, centerGravity, Color.White);

            //draw sprite
            spriteBatch.Draw(player.sprite, player.position, null, Color.White, player.rotationAngle, player.origin, 1.0f, SpriteEffects.None, 0f);


            //Draw bullets
            for (int i = 0; i < player.bullets.Count; i++)
            {
                if (player.bullets[i].fired)
                {
                    spriteBatch.Draw(player.bullets[i].sprite, player.bullets[i].position, null, Color.White, player.bullets[i].rotationAngle, player.bullets[i].origin, 1.0f, SpriteEffects.None, 0f);
                }
            }

            //Draw Stars
            for (int i = 0; i < stars.Count; i++)
            {
                spriteBatch.Draw(stars[i].sprite, stars[i].position, null, Color.White, stars[i].rotationAngle, stars[i].origin, stars[i].scale, SpriteEffects.None, 0f);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Function to populate stars list
        public void createStars()
        {
            double random;
            int placement;
            int randomX = 0;
            int randomY = 0;
            Vector2 randomDirection = Vector2.Zero;

            for (int i = 0; i < NUM_STARS; i++)
            {
                random = rand.NextDouble();
                placement = rand.Next(0,4);

                //Random Outer Positions

                //Top
                if( placement == 0 ){
                    randomX = (int)random*windowWidth;
                    randomY = 0;
                    randomDirection = new Vector2( 2F*(float)(random-0.5) , 1 );
                }

                //Right
                else if( placement == 1 ){
                    randomX = windowWidth;
                    randomY = (int)random*windowHeight;
                    randomDirection = new Vector2( -1 , 2F*(float)(random-0.5) );
                }

                //Bottom
                else if(placement == 2){
                    randomX = (int)random*windowWidth;
                    randomY = windowHeight;
                    randomDirection = new Vector2(2F*(float)(random-0.5), -1);
                }

                //Left
                else if(placement == 3){
                    randomX = windowWidth;
                    randomY = (int)random*windowHeight;
                    randomDirection = new Vector2(1, 2F*(float)(random-0.5) );
                }
                

                stars.Add(new Enemy(Content.Load<Texture2D>(@"sprites\enemyy2"), new Vector2(randomX, randomY), 
                    randomDirection, ref rand));
            }
        }

        //Reset all game variables
        public void restartGame()
        {
            NUM_STARS = 20;
            stars.RemoveRange(0, stars.Count);
            createStars();

            player = new Player(playerFull, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
            player.bullets.Add(new Bullet(Content.Load<Texture2D>(@"sprites\bullet1"), player.position));
            player.bullets.Add(new Bullet(Content.Load<Texture2D>(@"sprites\bullet1"), player.position));
            player.bullets.Add(new Bullet(Content.Load<Texture2D>(@"sprites\bullet1"), player.position));

        }
    }
}
