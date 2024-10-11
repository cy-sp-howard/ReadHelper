using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ui.Services.Overlay.Form;
using static ui.WindowUtil;

namespace ui.Services.Overlay
{
    public class Overlay : Gadget, IService
    {
        HookCallbackDelegate mouseHookCB;
        MouseEventArgs mouseEvent; // mouseEvent never type moved ,after handle set null,so hard to overwrite before handle.
        public override void Load()
        {
            setInputHook();
            setRect();
            new VirtualForm() { Parent = this, Rect = new Rectangle(20, 0, 500, 200), Resizeable = true };
            new VirtualForm() { Parent = this, Rect = new Rectangle(0, 0, 50, 100) };
            base.Load();
        }
        public void Update(GameTime gametime)
        {
            MouseState mouseState = Mouse.GetState();
            mouseEvent = new MouseEventArgs(mouseEvent == null ? MouseEventType.MouseMoved : mouseEvent.EventType, mouseState.Position);
            base.Update(gametime, mouseEvent);
            mouseEvent = null;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch, this);
        }
        void setRect()
        {
            int winWidth = ReadHelper.Instance.Graphics.PreferredBackBufferWidth;
            int winHeight = ReadHelper.Instance.Graphics.PreferredBackBufferHeight;
            Rect = new Rectangle(0, 0, winWidth, winHeight);
        }
        void setInputHook()
        {
            mouseHookCB = mouseHookCallback;
            SetWindowsHookEx(HookType.WH_MOUSE_LL, mouseHookCB, IntPtr.Zero, 0);
        }
        int mouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseEventArgs evt = new MouseEventArgs((MouseEventType)wParam, Marshal.PtrToStructure<MouseLLHookStruct>(lParam).Point);

            if (nCode != 0 || evt.EventType == MouseEventType.MouseMoved) return CallNextHookEx(HookType.WH_MOUSE_LL, nCode, wParam, lParam);

            bool skip = evt.EventType == MouseEventType.LeftMouseButtonReleased || evt.EventType == MouseEventType.RightMouseButtonReleased;

            mouseEvent = evt;
            if (!skip && mouseInChild)
            {
                return 1;
            };
            return CallNextHookEx(HookType.WH_MOUSE_LL, nCode, wParam, lParam);

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
        public event EventHandler<MouseEventArgs> OnLeftMouseBtnPress;
        public event EventHandler<MouseEventArgs> OnRightMouseBtnPress;
        public event EventHandler<MouseEventArgs> OnLeftMouseBtnRelease;
        public event EventHandler<MouseEventArgs> OnRightMouseBtnRelease;
        private bool readyForLeftClick = false;
        private bool readyForRightClick = false;
        public event EventHandler<MouseEventArgs> OnLeftMouseBtnClick;
        public event EventHandler<MouseEventArgs> OnRightMouseBtnClick;
        private bool mouseIn = false;
        protected bool mouseInChild = false;
        private bool mouseOut = false;
        public event EventHandler<MouseEventArgs> OnMouseIn;
        public event EventHandler<MouseEventArgs> OnMouseOut;
        public bool MouseIn => mouseIn;

        public virtual void Load()
        {
            foreach (var child in Children.ToList().OrderBy(c => c.ZIndex))
            {
                child.Load();
            }
        }
        public virtual void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            bool mouseInRect = Rect.Contains(new Point(mouseEvt.X, mouseEvt.Y));
            mouseInChild = false;
            if (Parent != null)
            {
                mouseInChild = Parent.mouseInChild;
                if (Parent.mouseInChild) mouseInRect = false;
                else if (mouseInRect) Parent.mouseInChild = true;
            }

            handleEvent(ref mouseIn, mouseInRect, () =>
            {
                OnMouseIn?.Invoke(this, mouseEvt);
            });

            handleEvent(ref mouseOut, !mouseInRect, () =>
            {
                OnMouseOut?.Invoke(this, mouseEvt);
            });


            bool currentValue = mouseEvt.EventType == MouseEventType.LeftMouseButtonPressed;
            if (mouseInRect)
            {
                handleEvent(ref leftMouseBtnPressed, currentValue, () =>
                {
                    OnLeftMouseBtnPress?.Invoke(this, mouseEvt);
                });
                if (currentValue) readyForLeftClick = true;
            }

            currentValue = mouseEvt.EventType == MouseEventType.LeftMouseButtonReleased;
            handleEvent(ref leftMouseBtnReleased, currentValue, () =>
            {
                OnLeftMouseBtnRelease?.Invoke(this, mouseEvt);
            });
            if (readyForLeftClick && currentValue)
            {
                readyForLeftClick = false;
                OnLeftMouseBtnClick?.Invoke(this, mouseEvt);
            }

            currentValue = mouseEvt.EventType == MouseEventType.RightMouseButtonPressed;
            if (mouseInRect)
            {
                handleEvent(ref rightMouseBtnPressed, currentValue, () =>
                {
                    OnRightMouseBtnPress?.Invoke(this, mouseEvt);
                });
                if (currentValue) readyForRightClick = true;
            }

            currentValue = mouseEvt.EventType == MouseEventType.RightMouseButtonReleased;
            handleEvent(ref rightMouseBtnReleased, currentValue, () =>
            {
                OnRightMouseBtnRelease?.Invoke(this, mouseEvt);
            });
            if (readyForRightClick && currentValue)
            {
                readyForRightClick = false;
                OnRightMouseBtnClick?.Invoke(this, mouseEvt);
            }

            int index = 0;
            foreach (var child in Children.ToList().OrderByDescending(c => c.ZIndex))
            {
                child.ZIndex = index * -1;
                index++;
                if (child.Disabled) continue;
                child.Update(gametime, mouseEvt);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {
            foreach (var child in Children.ToList().OrderBy(c => c.ZIndex))
            {
                if (child.Disabled) continue;
                child.Draw(spriteBatch, overlay);
            }
        }
        static void handleEvent(ref bool previousValue, bool currentValue, Action eventRef)
        {
            if (!Equals(previousValue, currentValue))
            {
                bool _previousValue = previousValue;
                previousValue = currentValue;
                if (currentValue)
                {
                    eventRef();
                }
            }
        }
        public class MouseEventArgs : EventArgs
        {
            public MouseEventType EventType { get; }
            public int X { get; }
            public int Y { get; }
            public MouseEventArgs(MouseEventType type, Point pos)
            {
                X = pos.X;
                Y = pos.Y;
                EventType = type;
            }
        }
    }
}