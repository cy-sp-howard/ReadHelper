using System;
using System.Collections.Generic;
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
        static ResizeCorner()
        {
       
        }
        void MouseInHandler(object sender, MouseEventArgs e)
        {
            color = new Color(30, 30, 30,0);
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
            if (width <= 100) width = 100;
            if (height <= 100) height = 100;
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
        public override void Load()
        {
            Rect = new Rectangle(0, 0, 20, 20);
            OnMouseIn += MouseInHandler;
            OnMouseOut += MouseOutHandler;
            OnLeftMouseBtnPress += MousePressHandler;
            OnLeftMouseBtnRelease += MouseReleaseHandler;
            base.Load();
        }
        public override void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            RelativePosition = new Point(Parent.Rect.Width - Rect.Width, Parent.Rect.Height - Rect.Height);
            CheckResizingForm();
            MouseMoveHandler(mouseEvt);

            base.Update(gametime, mouseEvt);
        }
        public override void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {

            spriteBatch.Draw(ReadHelper.Texture.CornerTexture, Rect, color);

        }
    }
}
