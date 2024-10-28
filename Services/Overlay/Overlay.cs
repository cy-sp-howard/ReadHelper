using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReadHelper.Services.Overlay.Form;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static ReadHelper.WindowUtil;

namespace ReadHelper.Services.Overlay
{
    public class Overlay : ParentGadget, IService
    {
        HookCallbackDelegate mouseHookCB;
        VirtualForm form1;
        VirtualForm form2;
        MouseEventArgs mouseEvent; // mouseEvent never moved type ,after handle set null,so hard to overwrite before handle.
        public override void Load()
        {
            SetInputHook();
            SetRect();
            form1 = new MainForm() { Parent = this };
            form2 = new VirtualForm() { Parent = this, Rect = new Rectangle(0, 0, 50, 100) };
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
        void SetRect()
        {
            int winWidth = ReadHelper.Instance.Graphics.PreferredBackBufferWidth;
            int winHeight = ReadHelper.Instance.Graphics.PreferredBackBufferHeight;
            Rect = new Rectangle(0, 0, winWidth, winHeight);
        }
        void SetInputHook()
        {
            mouseHookCB = MouseHookCallback;
            SetWindowsHookEx(HookType.WH_MOUSE_LL, mouseHookCB, IntPtr.Zero, 0);
        }
        int MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseEventArgs evt = new((MouseEventType)wParam, Marshal.PtrToStructure<MouseLLHookStruct>(lParam).Point);

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
        public Rectangle Rect
        {
            get => rect;
            set
            {
                if (rect.Equals(value)) return;
                Rectangle old = rect;
                rect = value;
                OnRectChange?.Invoke(this, new(rect, old));
            }
        }
        private Rectangle rect = new(0, 0, 0, 0);
        public Point RelativePosition
        {
            get
            {
                if (parent == null) return Rect.Location;
                return new Point(Rect.X - parent.Rect.X, Rect.Y - parent.Rect.Y);
            }
            set
            {
                if (parent == null)
                {
                    Rect = new Rectangle(value, Rect.Size);
                    return;
                };
                Rect = new Rectangle(value.X + parent.Rect.X, value.Y + parent.Rect.Y, Rect.Width, Rect.Height);
            }
        }
        public bool FollowParentPosition = true;
        public Sticky StickyParent = Sticky.DEFAULT;
        public Point Size
        {
            get => Rect.Size;
            set
            {
                Rect = new Rectangle(Rect.Location, value);
            }
        }
        public int ZIndex = 0;
        public bool Disabled = false;
        public ParentGadget Parent
        {
            get => parent;
            set
            {
                parent?.Children.Remove(this);
                parent = value;
                parent?.Children.Add(this); ;
            }
        }
        private ParentGadget parent = null;
        public event EventHandler<ChangeEvent<Rectangle>> OnRectChange;
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

        public virtual void Load() { }
        public virtual void Update(GameTime gametime, MouseEventArgs mouseEvt) //need call first if inherit
        {
            bool mouseInRect = Rect.Contains(new Point(mouseEvt.X, mouseEvt.Y));
            mouseInChild = false;
            if (Parent != null)
            {
                mouseInChild = Parent.mouseInChild;
                if (Parent.mouseInChild) mouseInRect = false;
                else if (mouseInRect) Parent.mouseInChild = true;
            }

            HandleEvent(ref mouseIn, mouseInRect, () =>
            {
                OnMouseIn?.Invoke(this, mouseEvt);
            });

            HandleEvent(ref mouseOut, !mouseInRect, () =>
            {
                OnMouseOut?.Invoke(this, mouseEvt);
            });


            bool currentValue = mouseEvt.EventType == MouseEventType.LeftMouseButtonPressed;
            if (mouseInRect)
            {
                HandleEvent(ref leftMouseBtnPressed, currentValue, () =>
                {
                    OnLeftMouseBtnPress?.Invoke(this, mouseEvt);
                });
                if (currentValue) readyForLeftClick = true;
            }

            currentValue = mouseEvt.EventType == MouseEventType.LeftMouseButtonReleased;
            HandleEvent(ref leftMouseBtnReleased, currentValue, () =>
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
                HandleEvent(ref rightMouseBtnPressed, currentValue, () =>
                {
                    OnRightMouseBtnPress?.Invoke(this, mouseEvt);
                });
                if (currentValue) readyForRightClick = true;
            }

            currentValue = mouseEvt.EventType == MouseEventType.RightMouseButtonReleased;
            HandleEvent(ref rightMouseBtnReleased, currentValue, () =>
            {
                OnRightMouseBtnRelease?.Invoke(this, mouseEvt);
            });
            if (readyForRightClick && currentValue)
            {
                readyForRightClick = false;
                OnRightMouseBtnClick?.Invoke(this, mouseEvt);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, Overlay overlay) { }
        static void HandleEvent(ref bool previousValue, bool currentValue, Action eventRef)
        {
            if (!Equals(previousValue, currentValue))
            {
                previousValue = currentValue;
                if (currentValue)
                {
                    eventRef();
                }
            }
        }
        public class MouseEventArgs(WindowUtil.MouseEventType type, Point pos) : EventArgs
        {
            public MouseEventType EventType { get; } = type;
            public int X { get; } = pos.X;
            public int Y { get; } = pos.Y;
        }
        public class ChangeEvent<T>(T current, T old) : EventArgs
        {
            public T Current { get; } = current;
            public T Old { get; } = old;
        }
    }
    public class ParentGadget : Gadget
    {
        // only set by Parent set except Overlay
        public readonly HashSet<Gadget> Children = [];
        public ParentGadget()
        {
            OnRectChange += HandleRectChange;
        }
        public override void Load()
        {
            foreach (var child in Children.ToList().OrderBy(c => c.ZIndex))
            {
                child.Load();
            }
        }
        public override void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            base.Update(gametime, mouseEvt);
            int index = 0;
            foreach (var child in Children.ToList().OrderByDescending(c => c.ZIndex))
            {
                child.ZIndex = index * -1;
                index++;
                if (child.Disabled) continue;
                child.Update(gametime, mouseEvt);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {
            foreach (var child in Children.ToList().OrderBy(c => c.ZIndex))
            {
                if (child.Disabled) continue;
                child.Draw(spriteBatch, overlay);
            }
        }
        private void HandleRectChange(object sender, ChangeEvent<Rectangle> e)
        {
            Point diffPos = e.Current.Location - e.Old.Location;
            Point diffSize = e.Current.Size - e.Old.Size;
            foreach (var child in Children)
            {
                Point pos = child.Rect.Location;

                if (child.FollowParentPosition)
                {
                    pos = new Point(child.Rect.X + diffPos.X, child.Rect.Y + diffPos.Y);
                }
                if ((child.StickyParent & Sticky.RIGHT) > 0)
                {
                    pos = new(pos.X + diffSize.X, pos.Y);
                }
                if ((child.StickyParent & Sticky.BOTTOM) > 0)
                {
                    pos = new(pos.Y, pos.Y + diffSize.Y);
                }
                child.Rect = new(pos, child.Rect.Size);
            }
        }
    }
    public enum Sticky
    {
        DEFAULT,
        BOTTOM = 0b01,
        RIGHT = 0b10
    }
}