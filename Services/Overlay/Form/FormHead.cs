using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ui.Services.Overlay.Form
{
    public class FormHead : Gadget
    {
        Color color = new Color(10, 10, 10, 0);
        Point mouseMoveStartPos = new Point(-1, -1);
        Point formMoveStartPos = new Point(-1, -1);
        public bool Moving = false;
        public int Height = 30;
        public FormHead()
        {
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
            color = new Color(10, 10, 10, 0);
        }
        void mousePressHandler(object sender, InputEventArgs e)
        {
            Moving = true;
        }
        void mouseReleaseHandler(object sender, InputEventArgs e)
        {
            Moving = false;
            mouseMoveStartPos = new Point(-1, -1);
        }
        void mouseMoveHandler(MouseState mouse)
        {
            if (!Moving) return;
            if (mouseMoveStartPos.X < 0)
            {
                mouseMoveStartPos = mouse.Position;
                formMoveStartPos = new Point(Parent.Rect.X, Parent.Rect.Y);
            }
            int moveX = mouse.Position.X - mouseMoveStartPos.X;
            int moveY = mouse.Position.Y - mouseMoveStartPos.Y;
            Parent.Rect = new Rectangle(formMoveStartPos.X + moveX, formMoveStartPos.Y + moveY, Parent.Rect.Width, Parent.Rect.Height);
        }
        void checkMovingForm()
        {
            if (MouseIn) return;
            foreach (var form in VirtualForm.AllForms)
            {
                if (Equals(form, Parent) || !form.Moving) continue;
                Moving = false;
            }
        }
        public override void Update(GameTime gametime, MouseState mouse)
        {
            Rect = new Rectangle(Parent.Rect.X, Parent.Rect.Y, Parent.Rect.Width, Height);
            checkMovingForm();
            mouseMoveHandler(mouse);

            base.Update(gametime, mouse);
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Overlay overlay)
        {
            spriteBatch.Draw(ReadHelper.PixelTexture, Rect, color);
        }


    }
}
