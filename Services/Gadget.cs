using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ReadHelper.WindowUtil;
using System.Windows.Forms;
using ReadHelper.Services.Overlay;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Diagnostics;
using ReadHelper.Services.Overlay.Form;

namespace ReadHelper.Services
{
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
        public int Left
        {
            get
            {
                int padding = Parent == null ? 0 : Parent.Padding.Left;
                return RelativePosition.X + padding;
            }
            set
            {
                int padding = Parent == null ? 0 : Parent.Padding.Left;
                RelativePosition = new Point(value + padding, RelativePosition.Y);
            }
        }
        public int Top
        {
            get
            {
                int padding = Parent == null ? 0 : Parent.Padding.Top;
                return RelativePosition.Y + padding;
            }
            set
            {
                int padding = Parent == null ? 0 : Parent.Padding.Top;
                RelativePosition = new Point(RelativePosition.X, value + padding);
            }
        }
        public int Bottom
        {
            get
            {
                int padding = Parent == null ? 0 : Parent.Padding.Bottom;
                Point parentSize = Parent == null ? new Point(0, 0) : Parent.Size;
                return parentSize.Y - padding - Size.Y - RelativePosition.Y;
            }
            set
            {
                int padding = Parent == null ? 0 : Parent.Padding.Bottom;
                Point parentSize = Parent == null ? new Point(0, 0) : Parent.Size;
                RelativePosition = new Point(RelativePosition.X, parentSize.Y - padding - Size.Y - value);
            }
        }
        public int Right
        {
            get
            {
                int padding = Parent == null ? 0 : Parent.Padding.Right;
                Point parentSize = Parent == null ? new Point(0, 0) : Parent.Size;
                return parentSize.X - padding - Size.X - RelativePosition.X;
            }
            set
            {
                int padding = Parent == null ? 0 : Parent.Padding.Right;
                Point parentSize = Parent == null ? new Point(0, 0) : Parent.Size;
                RelativePosition = new Point(parentSize.X - padding - Size.X - value, RelativePosition.Y);
            }
        }
        public Point RelativePosition
        {
            get
            {
                if (Parent == null) return Rect.Location;
                return new Point(Rect.X - Parent.Rect.X, Rect.Y - Parent.Rect.Y);
            }
            set
            {
                if (Parent == null)
                {
                    Rect = new Rectangle(value, Rect.Size);
                    return;
                };
                Rect = new Rectangle(value.X + Parent.Rect.X, value.Y + Parent.Rect.Y, Rect.Width, Rect.Height);
            }
        }
        public Padding Padding = new() { Top = 0, Bottom = 0, Left = 0, Right = 0 };
        public Rectangle InnerRect { get => new Rectangle(Rect.Left + Padding.Left, Rect.Top + Padding.Top, Rect.Width - Padding.Left - Padding.Right, Rect.Height - Padding.Top - Padding.Bottom); }
        public Sticky ResizeStickyParent = Sticky.DEFAULT;
        public bool FollowParentPosition = true;
        public Point Size
        {
            get => Rect.Size;
            set
            {
                Rect = new Rectangle(Rect.Location, value);
            }
        }
        public int ZIndex = 0;
        public bool Disabled
        {
            get => _disabled; set
            {
                _disabled = value;
                if (_disabled) { OnDisable?.Invoke(this, new()); }
                else { OnEnable?.Invoke(this, new()); }
            }
        }
        private bool _disabled = false;
        public Gadget Parent
        {
            get => _parent; set
            {
                if (value != null)
                {
                    _parent = value;
                    _parent.Children.Add(this);
                }
                else
                {
                    _parent.Children.Remove(this);
                    _parent = value;
                }
            }
        }
        private Gadget _parent;

        public readonly HashSet<Gadget> Children = [];
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
        public event EventHandler<MouseEventArgs> OnMouseWheel;
        public event EventHandler<EventArgs> OnEnable;
        public event EventHandler<EventArgs> OnDisable;
        public bool MouseIn => mouseIn;
        public Gadget()
        {
            OnRectChange += HandleRectChange;
        }
        public Gadget(Gadget parentGadget)
        {
            Parent = parentGadget;
            OnRectChange += HandleRectChange;
        }

        public virtual void Load()
        {
            foreach (var child in Children.ToList().OrderBy(c => c.ZIndex))
            {
                child.Load();
            }
        }
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


            if (mouseInRect && mouseEvt.EventType == MouseEventType.MouseWheelScrolled)
            {
                OnMouseWheel?.Invoke(this, mouseEvt);
            }

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


            List<Gadget> children = Children.ToList();
            int index = children.Count - 1;
            int maxZIndex = children.Count == 0 ? 0 : children.Max(c => c.ZIndex);

            foreach (var child in children.OrderByDescending(c => c.ZIndex))
            {
                if (maxZIndex > children.Count - 1)
                {
                    child.ZIndex = index;
                    index--;
                }
                if (child.Disabled) continue;
                child.Update(gametime, mouseEvt);
            }

        }
        public virtual void DrawInRect(SpriteBatch spriteBatch, OverlayRoot overlay)
        {

        }
        public virtual void DrawInRectAfterChildren(SpriteBatch spriteBatch, OverlayRoot overlay)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, OverlayRoot overlay)
        {
            Rectangle b_rect = spriteBatch.GraphicsDevice.ScissorRectangle;
            Rectangle intersectRect = Rectangle.Intersect(b_rect, Rect);
            if (intersectRect.IsEmpty) return;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = intersectRect;
            spriteBatch.Begin(rasterizerState: new RasterizerState() { ScissorTestEnable = true });
            DrawInRect(spriteBatch, overlay);

            foreach (var child in Children.ToList().OrderBy(c => c.ZIndex))
            {
                if (child.Disabled) continue;
                child.Draw(spriteBatch, overlay);
            }
            DrawInRectAfterChildren(spriteBatch, overlay);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = b_rect;
            spriteBatch.Begin(rasterizerState: new RasterizerState() { ScissorTestEnable = true });

        }
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
        public class MouseEventArgs(WindowUtil.MouseEventType type, Point pos, int wheel = 0) : EventArgs
        {
            public MouseEventType EventType { get; } = type;
            public int X { get; } = pos.X;
            public int Y { get; } = pos.Y;
            public int Wheel { get; } = wheel;
        }
        public class ChangeEvent<T>(T current, T old) : EventArgs
        {
            public T Current { get; } = current;
            public T Old { get; } = old;
        }
        private void HandleRectChange(object sender, ChangeEvent<Rectangle> e)
        {
            Point diffPos = e.Current.Location - e.Old.Location;
            Point diffSize = e.Current.Size - e.Old.Size;
            foreach (var child in Children)
            {
                if (child.FollowParentPosition)
                {
                    child.Rect = new Rectangle(child.Rect.Location + diffPos, child.Rect.Size);
                }
                if ((child.ResizeStickyParent & Sticky.RIGHT) > 0)
                {
                    child.Right -= diffSize.X;
                }
                if ((child.ResizeStickyParent & Sticky.BOTTOM) > 0)
                {
                    child.Bottom -= diffSize.Y;
                }
            }
        }
    }
    public enum Sticky
    {
        NONE,
        DEFAULT = 0b1100,
        BOTTOM = 0b01,
        RIGHT = 0b10
    }
}
