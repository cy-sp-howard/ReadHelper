using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ReadHelper.Services.Overlay.Form
{
    public class ResizeCorner : Gadget
    {

        Point resizeStartPos = new(-1, -1);
        Point resizeStartSize = new(-1, -1);
        Color color = Color.Transparent;
        public bool Resizing = false;
        public ResizeCorner(Gadget p):base(p)
        {
            Size = new Point(20, 20);
            Bottom = Parent.Padding.Right * -1;
            Right = Parent.Padding.Bottom * -1;
            ResizeStickyParent = Sticky.RIGHT | Sticky.BOTTOM;
            OnMouseIn += MouseInHandler;
            OnMouseOut += MouseOutHandler;
            OnLeftMouseBtnPress += MousePressHandler;
            OnLeftMouseBtnRelease += MouseReleaseHandler;
        }
        void MouseInHandler(object sender, MouseEventArgs e)
        {
            color = new Color(30, 30, 30, 0);
        }
        void MouseOutHandler(object sender, MouseEventArgs e)
        {
            color = Color.Transparent;
        }
        void MousePressHandler(object sender, MouseEventArgs e)
        {
            Resizing = true;
        }
        void MouseReleaseHandler(object sender, MouseEventArgs e)
        {
            Resizing = false;
            resizeStartPos = new Point(-1, -1);
        }
        void MouseMoveHandler(MouseEventArgs evt)
        {
            if (!Resizing) return;
            if (resizeStartPos.X < 0)
            {
                resizeStartPos = new Point(evt.X, evt.Y);
                resizeStartSize = new Point(Parent.Rect.Width, Parent.Rect.Height);
            }
            int width = evt.X - resizeStartPos.X + resizeStartSize.X;
            int height = evt.Y - resizeStartPos.Y + resizeStartSize.Y;
            if (Parent is VirtualForm parentForm )
            {
                if (width < parentForm.MinSize.X)
                {
                    width = parentForm.MinSize.X;
                } else if(parentForm.MaxSize.X > 0 && width > parentForm.MaxSize.X)
                {
                    width = parentForm.MaxSize.X;

                }
                if (height < parentForm.MinSize.Y)
                {
                    height = parentForm.MinSize.Y;
                }
                else if (parentForm.MaxSize.Y > 0 && width > parentForm.MaxSize.Y)
                {
                    height = parentForm.MaxSize.Y;

                }

            }
            Parent.Rect = new Rectangle(Parent.Rect.X, Parent.Rect.Y, width, height);
        }
        void CheckResizingForm()
        {
            if (MouseIn) return;
            foreach (var form in VirtualForm.AllForms)
            {
                if (Equals(form, Parent) || !form.Resizing) continue;
                Resizing = false;
            }
        }
        public override void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            CheckResizingForm();
            MouseMoveHandler(mouseEvt);

            base.Update(gametime, mouseEvt);
        }
        public override void Draw(SpriteBatch spriteBatch, OverlayRoot overlay)
        {

            spriteBatch.Draw(Texture.CornerTexture, Rect, color);

        }
    }
}
