using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static ui.WindowUtil;

namespace ui.Services.Overlay
{
    public class Overlay : IService
    {
        List<Gadget> gadgets = new List<Gadget>();
        public Rectangle Rect = new Rectangle(0, 0, 0, 0);
        Gadget HeadGadget;
        public void Load()
        {
            setRect();
            gadgets.Add(HeadGadget = new HeadGadget());
        }
        public void Update(GameTime gametime)
        {
            MouseState mouse = Mouse.GetState();
            foreach (Gadget gadget in gadgets)
            {
                gadget.Update(gametime, mouse);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Gadget gadget in gadgets)
            {
                gadget.Draw(spriteBatch);
            }


        }
        void setRect()
        {
            int winWidth = ReadHelper.Instance.Graphics.PreferredBackBufferWidth;
            int winHeight = ReadHelper.Instance.Graphics.PreferredBackBufferHeight;
            int w = (int)(winWidth * 0.8);
            int h = (int)(w * 0.4);
            Rect = new Rectangle(winWidth / 2  - w / 2, winHeight / 2 - h / 2, w, h);
        }
    }
    public class Gadget
    {
        protected Rectangle rect = new Rectangle(0, 0, 0, 0);
        private bool leftMouseBtnPressed = false;
        private bool rightMouseBtnPressed = false;
        private bool leftMouseBtnReleased = false;
        private bool rightMouseBtnReleased = false;
        public event EventHandler<ChangeEventArgs<bool>> OnLeftMouseBtnPress;
        public event EventHandler<ChangeEventArgs<bool>> OnRightMouseBtnPress;
        public event EventHandler<ChangeEventArgs<bool>> OnLeftMouseBtnRelease;
        public event EventHandler<ChangeEventArgs<bool>> OnRightMouseBtnRelease;
        private bool readyForLeftClick = false;
        private bool readyForRightClick = false;
        public event EventHandler<ChangeEventArgs<bool>> OnLeftMouseBtnClick;
        public event EventHandler<ChangeEventArgs<bool>> OnRightMouseBtnClick;
        private bool mouseIn = false;
        private bool mouseOut = false;
        public event EventHandler<ChangeEventArgs<bool>> OnMouseIn;
        public event EventHandler<ChangeEventArgs<bool>> OnMouseOut;

        public virtual void Update(GameTime gametime, MouseState mouse)
        {

            bool mouseInRect = rect.Contains(new Point(mouse.X, mouse.Y));
            handleEvent(ref mouseIn, mouseInRect, (evt) => OnMouseIn?.Invoke(this, evt));
            handleEvent(ref mouseOut, !mouseInRect, (evt) => OnMouseOut?.Invoke(this, evt));

            bool currentValue = mouseInRect && mouse.LeftButton == ButtonState.Pressed;
            handleEvent(ref leftMouseBtnPressed, currentValue, (evt) => OnLeftMouseBtnPress?.Invoke(this, evt));
            if (currentValue) readyForLeftClick = true;

            currentValue = mouseInRect && mouse.LeftButton == ButtonState.Released;
            handleEvent(ref leftMouseBtnReleased, currentValue, (evt) => OnLeftMouseBtnRelease?.Invoke(this, evt));
            if (readyForLeftClick)
            {
                readyForLeftClick = false;
                OnLeftMouseBtnClick?.Invoke(this, new ChangeEventArgs<bool>(true, false));
            }

            currentValue = mouseInRect && mouse.RightButton == ButtonState.Pressed;
            handleEvent(ref rightMouseBtnPressed, currentValue, (evt) => OnRightMouseBtnPress?.Invoke(this, evt));
            if (currentValue) readyForRightClick = true;

            currentValue = mouseInRect && mouse.RightButton == ButtonState.Released;
            handleEvent(ref rightMouseBtnReleased, currentValue, (evt) => OnRightMouseBtnRelease?.Invoke(this, evt));
            if (readyForRightClick)
            {
                readyForRightClick = false;
                OnRightMouseBtnClick?.Invoke(this, new ChangeEventArgs<bool>(true, false));
            }

        }
        public virtual void Draw(SpriteBatch spriteBatch) { }
        void handleEvent(ref bool previousValue, bool currentValue, Action<ChangeEventArgs<bool>> eventRef)
        {
            if (!Equals(previousValue, currentValue))
            {
                bool _previousValue = previousValue;
                previousValue = currentValue;
                if (currentValue)
                {
                    eventRef(new ChangeEventArgs<bool>(currentValue, _previousValue));
                }
            }

        }
        public class ChangeEventArgs<T> : EventArgs
        {
            public T Prev { get; }
            public T Current { get; }

            public ChangeEventArgs(T current, T prev)
            {
                Prev = prev;
                Current = current;
            }

        }
    }
}