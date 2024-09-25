using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using static ui.WindowUtil;

namespace ui
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Form _formWrapper;
        SpriteFont font;
        public IntPtr FormHandle { get; private set; }
        public Form Form { get; private set; }
        bool showForm = false;
        bool dontHide = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            FormHandle = this.Window.Handle;
            Form = Control.FromHandle(FormHandle).FindForm();


            Form.BackColor = System.Drawing.Color.Yellow;
            // Avoid the flash the window shows when the application launches (-32000x-32000 is where windows places minimized windows)
            Form.Location = new System.Drawing.Point(100, 100);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("myfont");


            // TODO: use this.Content to load your game content here
        }
        protected override void BeginRun()
        {
            // 視窗大小
            this._graphics.PreferredBackBufferWidth = 200;
            this._graphics.PreferredBackBufferHeight = 200;
            this._graphics.ApplyChanges();

            var clientRect = new RECT();

            // 無邊框
            var marg = new Margins
            {
                cxLeftWidth = 0,
                cyTopHeight = 0,
                cxRightWidth = clientRect.Right,
                cyBottomHeight = clientRect.Bottom
            };
            // 沒設會黑畫面
            DwmExtendFrameIntoClientArea(FormHandle, ref marg);
            // 修改視窗樣式 (關鍵)
            SetWindowLong(FormHandle, GWL_STYLE, CS_HREDRAW | CS_VREDRAW);
            // 視窗透明度(沒差)
            SetLayeredWindowAttributes(FormHandle, 0, 255, 2);
            base.BeginRun();

        }

        protected override void Update(GameTime gameTime)
        {
            var clientRect = new RECT();
            var marg = new Margins
            {
                cxLeftWidth = 0,
                cyTopHeight = 0,
                cxRightWidth = clientRect.Right,
                cyBottomHeight = clientRect.Bottom
            };
            // 沒設會黑畫面
            DwmExtendFrameIntoClientArea(FormHandle, ref marg);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
            {


            }
            // TODO: Add your update logic here


            //{


            //    //當窗口的水平垂直尺寸改變時，整個窗口區域都會被重新繪製
            //    WindowUtil.SetWindowLong(FormHandle, WindowUtil.GWL_STYLE, WindowUtil.CS_HREDRAW | WindowUtil.CS_VREDRAW);
            //    //WindowUtil.SetLayeredWindowAttributes(FormHandle, 0, 255, 2);
            //}

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "FPS: 123", new Vector2(Window.ClientBounds.Width / 2, 25), Color.Red);

            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
