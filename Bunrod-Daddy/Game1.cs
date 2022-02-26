using System;
using System.Collections.Generic;
using BunrodDaddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bunrod_Daddy
{
    public class Game1 : Game
    {
        public static List<Bubble> bubblesStuck { get; private set; }
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D launcher;
        private Texture2D background;
        float elapsed; //
        float rotacion; //การหมุน

        Vector2 locationPointer; //ตำแหน่งปืน
        Vector2 positionBubble; //ตำแหน่งบอล

        Bubble bubbleLaunched;

        Texture2D bubble;

        public static Int32 limitRight = 540;
        public static Int32 limitLeft = 60;
        public static Int32 limitUp = 30;
        public static Int32 limitDown = 650;

        //Texture2D pointText;

        public static Int32 bubbleSize =  60;
        Texture2D pointText;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            rotacion = 0f; //ตำแหน่งหมุนเริ่มต้น
            elapsed = 0f; //มุม
            locationPointer = new Vector2(300, 800); //ตำแหน่งตัวยิง
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            //ครึ่งหนึ่งของขนาดฟองจะถูกลบออก
            //bubbleLaunched = new Bubble(new Vector2(350 - 30, 740 - 30), Color.White);
            positionBubble = new Vector2(300-30, 800 - 100 - 30); //ตำแหน่งลูกบอล
            GeneredNewBubble();
            bubblesStuck = new List<Bubble>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("BG gameplay"); //bg
            launcher = Content.Load<Texture2D>("gun"); //arrow
            bubble = Content.Load<Texture2D>("ball1"); //bubble
            pointText = Content.Load<Texture2D>("1x1"); 

        }

        public void GeneredNewBubble()
        {
            Int32 color = new Random().Next(0, 7);
            Color colorBubble = new Color();
            switch (color)
            {
                case 0:
                    colorBubble = Color.White;
                    break;
                case 1:
                    colorBubble = Color.Red;
                    break;
                case 2:
                    colorBubble = Color.Black;
                    break;
                case 3:
                    colorBubble = Color.DarkBlue;
                    break;
                case 4:
                    colorBubble = Color.Green;
                    break;
                case 5:
                    colorBubble = Color.Yellow;
                    break;
                case 6:
                    colorBubble = Color.Purple;
                    break;
                case 7:
                    colorBubble = Color.Orange;
                    break;
            }
            bubbleLaunched = new Bubble(positionBubble, colorBubble);
        }


        public void pasteBubble(Bubble bubble)
        {

            //List<Bubble> equalGroups = bubble.findTheSame();
            //if (equalGroups.Count < 3)
            //{
            //    bubble.Posicion = positionBox((Int32)bubble.positionBox.X,
            //       (Int32)bubble.positionBox.Y);
            //    bubblesStuck.Add(bubble);

            //}
            //else
            //{
            //    //There are more than 2 equal bubbles
            //    bubble.Posicion = positionBox((Int32)bubble.positionBox.X,
            //       (Int32)bubble.positionBox.Y);
            //    bubblesStuck.Add(bubble);
            //}

            bubble.Posicion = positionBox((Int32)bubble.positionBox.X, (Int32)bubble.positionBox.Y);
            bubblesStuck.Add(bubble);
        }


        private Vector2 positionBox(int cx, int cy)
        {
                Rectangle dRect = new Rectangle((cx * bubbleSize) + limitLeft +
                   ((cy % 2) * (bubbleSize / 2)),
                   (int)cy * bubbleSize + limitUp, bubbleSize, bubbleSize);

                return new Vector2(dRect.X, dRect.Y);
        }

        //bubble check ติดกับตัวบนรึเปล่า แล้วลงตำแหน่ง
        public Boolean CrashBubbles(Bubble currentObject, Bubble object2)
        {
            float dx = object2.Posicion.X - currentObject.Posicion.X;
            float dy = object2.Posicion.Y - currentObject.Posicion.Y;
            Int32 radio1 = bubble.Width / 2;
            Int32 radio2 = bubble.Width / 2;
            Int32 radioTotal = radio1 + radio2;
            double diff1 = Math.Pow(dx, 2) + Math.Pow(dy, 2);
            double radioE = Math.Pow(radioTotal, 2);
            if (diff1 <= radioE)
            {
                return true;
            }
            return false;
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            bubbleLaunched.Mover();
            // ตรวจสอบการชนกับขอบหน้าจอ
            if (bubbleLaunched.Posicion.X <= limitLeft || bubbleLaunched.Posicion.X >= limitRight - 42) // collides ชนกัน
            {
                //เปลี่ยนทิศทาง X ให้ไปในทิศทางตรงกันข้าม
                bubbleLaunched.Direccion.X *= -1;
            }

            if (bubbleLaunched.Posicion.Y <= limitUp) //crashes on the roof, stop her and hit her
            {
                // ชนบนหลังคา แล้วหยุด
                //bubbleLaunched.Direccion = Vector2.Zero;

                bubbleLaunched.findClosestBox();
                //bubblesStuck.Add(bubbleLaunched);
                pasteBubble(bubbleLaunched); //check ตำแหน่งลงแยกช่อง
                bubbleLaunched = null;
            }
            else
            {
                // check collision with all bubbles stuck
                for (int i = 0; i <= bubblesStuck.Count - 1; i++)
                {
                    if (bubblesStuck[i] != bubbleLaunched)
                    {
                        if (CrashBubbles(bubbleLaunched, bubblesStuck[i]))
                        {
                            bubbleLaunched.findClosestBox();
                            pasteBubble(bubbleLaunched);
                            bubbleLaunched = null;
                            break;
                        }
                    }
                }
            }

            if (elapsed > 0.05f)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    rotacion -= 0.1f;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    rotacion += 0.1f;
                }
                elapsed = 0f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                bubbleLaunched.moving = true;
                // ยิงฟองขึ้นอยู่กับการหมุนของตัวชี้
                // 1.5f คือความเร็ว
                Matrix m = Matrix.CreateRotationZ(rotacion);
                bubbleLaunched.Direccion.X += m.M12 * 1.5f;
                bubbleLaunched.Direccion.Y -= m.M11 * 1.5f;
            }

            if (bubbleLaunched == null)
            {
                GeneredNewBubble();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            //draw background
            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            //Draw the boxes where the bubbles will be positioned
            //draw rectangle
            //_spriteBatch.Draw(pointText, new Rectangle(limitLeft, limitUp, limitRight - limitLeft, limitDown), null, Color.Green);
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    Rectangle dRect = new Rectangle((x * bubbleSize) + limitLeft + ((y % 2) * (bubbleSize / 2)),
                       (int)y * bubbleSize + limitUp, bubbleSize, bubbleSize);
                    if (x < 8)
                        _spriteBatch.Draw(pointText, new Rectangle(dRect.X, dRect.Y, bubbleSize, 1), new Color(255, 0, 0, 100));
                    if (y < 8)
                        _spriteBatch.Draw(pointText, new Rectangle(dRect.X, dRect.Y, 1, bubbleSize), new Color(255, 0, 0, 100));
                }
            }

            _spriteBatch.Draw(launcher, locationPointer, null, Color.White, rotacion, new Vector2(launcher.Width / 2, launcher.Height / 2), 1f, SpriteEffects.None, 1);
            // auncher.Width / 2, launcher.Height / 2 -> จุดหมุน
            //1f ขนาดเท่า
            _spriteBatch.Draw(bubble, bubbleLaunched.Posicion, bubbleLaunched.Color);
            //draw rectangle
            //_spriteBatch.Draw(pointText, new Rectangle(limitLeft, limitUp, limitRight - limitLeft, limitDown), null, Color.Blue);

            if (bubbleLaunched != null)
            {
                _spriteBatch.Draw(bubble, bubbleLaunched.Posicion, bubbleLaunched.Color);
            }

            foreach (Bubble burb in bubblesStuck)
            {
                _spriteBatch.Draw(bubble, burb.Posicion, null, burb.Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
