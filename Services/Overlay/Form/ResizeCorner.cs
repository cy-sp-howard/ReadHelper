using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ui.Services.Overlay.Form
{
    public class ResizeCorner : Gadget
    {

        Color color = Color.Transparent;
        Point resizeStartPos = new Point(-1, -1);
        Point resizeStartSize = new Point(-1, -1);
        public bool Resizing = false;
        public ResizeCorner()
        {
            Rect = new Rectangle(0, 0, 20, 20);
            OnMouseIn += mouseInHandler;
            OnMouseOut += mouseOutHandlerr;
            OnLeftMouseBtnPress += mousePressHandler;
            OnLeftMouseBtnRelease += mouseReleaseHandler;
        }
        void mouseInHandler(object sender, InputEventArgs e)
        {
            color = new Color(30, 30, 30, 0);
        }
        void mouseOutHandlerr(object sender, InputEventArgs e)
        {
            color = Color.Transparent;
        }
        void mousePressHandler(object sender, InputEventArgs e)
        {
            Resizing = true;
        }
        void mouseReleaseHandler(object sender, InputEventArgs e)
        {
            Resizing = false;
            resizeStartPos = new Point(-1, -1);
        }
        void mouseMoveHandler(MouseState mouse)
        {
            if (!Resizing) return;
            if (resizeStartPos.X < 0)
            {
                resizeStartPos = mouse.Position;
                resizeStartSize = new Point(Parent.Rect.Width, Parent.Rect.Height);
            }
            int moveX = mouse.Position.X - resizeStartPos.X;
            int moveY = mouse.Position.Y - resizeStartPos.Y;
            Parent.Rect = new Rectangle(Parent.Rect.X, Parent.Rect.Y, resizeStartSize.X + moveX, resizeStartSize.Y + moveY);
        }
        void checkResizingForm()
        {
            if (MouseIn) return;
            foreach (var form in VirtualForm.AllForms)
            {
                if (Equals(form, Parent) || !form.Resizing) continue;
                Resizing = false;
            }
        }
        public override void Update(GameTime gametime, MouseState mouse)
        {
            RelativePosition = new Point(Parent.Rect.Width - Rect.Width, Parent.Rect.Height - Rect.Height);
            checkResizingForm();
            mouseMoveHandler(mouse);

            base.Update(gametime, mouse);
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Overlay overlay)
        {
            spriteBatch.Draw(ReadHelper.PixelTexture, Rect, color);
        }
    }
}
