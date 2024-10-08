using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ui.Services.Overlay.Form;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static ui.WindowUtil;

namespace ui.Services.Overlay
{
    public class Overlay : Gadget, IService
    {
        public void Load()
        {
            setRect();
            new VirtualForm() { Parent = this, Rect = new Rectangle(20, 0, 500, 200), ZIndex = 1, Resizeable = true };
            new VirtualForm() { Parent = this, Rect = new Rectangle(0, 0, 50, 50), ZIndex = 0 };
        }
        public void Update(GameTime gametime)
        {
            MouseState mouse = Mouse.GetState();
            base.Update(gametime, mouse);
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            base.Draw(spriteBatch, graphicsDevice, this);
        }
        void setRect()
        {
            int winWidth = ReadHelper.Instance.Graphics.PreferredBackBufferWidth;
            int winHeight = ReadHelper.Instance.Graphics.PreferredBackBufferHeight;
            Rect = new Rectangle(0, 0, winWidth, winHeight);
        }

    }
    public class Gadget
    {
        public Rectangle Rect = new Rectangle(0, 0, 0, 0);
        public Point RelativePosition
        {
            get
            {
                if (parent == null) return new Point(Rect.X, Rect.Y);
                return new Point(Rect.X - parent.Rect.X, Rect.Y - parent.Rect.Y);
            }
            set
            {
                if (parent == null)
                {
                    new Rectangle(value.X, value.Y, Rect.Width, Rect.Height);
                    return;
                };
                Rect = new Rectangle(value.X + parent.Rect.X, value.Y + parent.Rect.Y, Rect.Width, Rect.Height);
            }
        }
        public int ZIndex = 0;
        public bool Disabled = false;
        public Gadget Parent
        {
            get => parent;
            set
            {
                if (parent != null) parent.Children.Remove(this);
                parent = value;
                if (parent != null)
                {
                    parent.Children.Add(this);
                };
            }
        }
        private Gadget parent = null;
        // only set by Parent set except Overlay
        public readonly HashSet<Gadget> Children = new HashSet<Gadget>();
        private bool leftMouseBtnPressed = false;
        private bool rightMouseBtnPressed = false;
        private bool leftMouseBtnReleased = false;
        private bool rightMouseBtnReleased = false;
        public event EventHandler<InputEventArgs> OnLeftMouseBtnPress;
        public event EventHandler<InputEventArgs> OnRightMouseBtnPress;
        public event EventHandler<InputEventArgs> OnLeftMouseBtnRelease;
        public event EventHandler<InputEventArgs> OnRightMouseBtnRelease;
        private bool readyForLeftClick = false;
        private bool readyForRightClick = false;
        public event EventHandler<InputEventArgs> OnLeftMouseBtnClick;
        public event EventHandler<InputEventArgs> OnRightMouseBtnClick;
        private bool mouseIn = false;
        private bool mouseInChild = false;
        private bool mouseOut = false;
        public event EventHandler<InputEventArgs> OnMouseIn;
        public event EventHandler<InputEventArgs> OnMouseOut;
        public bool MouseIn => mouseIn;

        public virtual void Update(GameTime gametime, MouseState mouse)
        {
            if (Disabled) return;

            bool mouseInRect = Rect.Contains(new Point(mouse.X, mouse.Y));
            mouseInChild = false;
            if (Parent != null)
            {
                mouseInChild = Parent.mouseInChild;
                if (Parent.mouseInChild) mouseInRect = false;
                else if (mouseInRect) Parent.mouseInChild = true;
            }

            handleEvent(ref mouseIn, mouseInRect, (evt) =>
            {
                OnMouseIn?.Invoke(this, evt);
            });

            handleEvent(ref mouseOut, !mouseInRect, (evt) =>
            {
                OnMouseOut?.Invoke(this, evt);
            });
            if (mouseInRect)
            {
                bool currentValue = mouse.LeftButton == ButtonState.Pressed;
                handleEvent(ref leftMouseBtnPressed, currentValue, (evt) =>
                {
                    OnLeftMouseBtnPress?.Invoke(this, evt);
                });
                if (currentValue) readyForLeftClick = true;

                currentValue = mouse.LeftButton == ButtonState.Released;
                handleEvent(ref leftMouseBtnReleased, currentValue, (evt) =>
                {
                    OnLeftMouseBtnRelease?.Invoke(this, evt);
                });
                if (readyForLeftClick && currentValue)
                {
                    readyForLeftClick = false;
                    OnLeftMouseBtnClick?.Invoke(this, new InputEventArgs());
                }

                currentValue = mouse.RightButton == ButtonState.Pressed;
                handleEvent(ref rightMouseBtnPressed, currentValue, (evt) =>
                {
                    OnRightMouseBtnPress?.Invoke(this, evt);
                });
                if (currentValue) readyForRightClick = true;

                currentValue = mouse.RightButton == ButtonState.Released;
                handleEvent(ref rightMouseBtnReleased, currentValue, (evt) =>
                {
                    OnRightMouseBtnRelease?.Invoke(this, evt);
                });
                if (readyForRightClick && currentValue)
                {
                    readyForRightClick = false;
                    OnRightMouseBtnClick?.Invoke(this, new InputEventArgs());
                }
            }

            foreach (var child in Children.ToList().OrderByDescending(c => c.ZIndex))
            {
                child.Update(gametime, mouse);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Overlay overlay)
        {
            if (Disabled) return;
            foreach (var child in Children.ToList().OrderBy(c => c.ZIndex))
            {
                child.Draw(spriteBatch, graphicsDevice, overlay);
            }
        }
        static void handleEvent(ref bool previousValue, bool currentValue, Action<InputEventArgs> eventRef)
        {
            if (!Equals(previousValue, currentValue))
            {
                bool _previousValue = previousValue;
                previousValue = currentValue;
                if (currentValue)
                {
                    eventRef(new InputEventArgs());
                }
            }
        }
        public class InputEventArgs : EventArgs
        {
            public InputEventArgs()
            {
            }
        }
    }
}