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
    public class FormHead : Gadget
    {
        Color defaultColor = new Color(10, 10, 10, 0);
        Color color;
        Point mouseMoveStartPos = new Point(-1, -1);
        Point formMoveStartPos = new Point(-1, -1);
        public bool Moving = false;
        public int Height = 50;
        public FormHead()
        {
            color = defaultColor;
            OnMouseIn += mouseInHandler;
            OnMouseOut += mouseOutHandlerr;
            OnLeftMouseBtnPress += mousePressHandler;
            OnLeftMouseBtnRelease += mouseReleaseHandler;
        }
        void mouseInHandler(object sender, MouseEventArgs e)
        {
            color = new Color(30, 30, 30, 0);
        }
        void mouseOutHandlerr(object sender, MouseEventArgs e)
        {
            color = defaultColor;
        }
        void mousePressHandler(object sender, MouseEventArgs e)
        {
            Moving = true;
        }
        void mouseReleaseHandler(object sender, MouseEventArgs e)
        {
            Moving = false;
            mouseMoveStartPos = new Point(-1, -1);
        }
        void mouseMoveHandler(MouseEventArgs evt)
        {
            if (!Moving) return;
            if (mouseMoveStartPos.X < 0)
            {
                mouseMoveStartPos = new Point(evt.X, evt.Y);
                formMoveStartPos = new Point(Parent.Rect.X, Parent.Rect.Y);
            }
            int moveX = evt.X - mouseMoveStartPos.X;
            int moveY = evt.Y - mouseMoveStartPos.Y;
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
        public override void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            Rect = new Rectangle(Parent.Rect.X, Parent.Rect.Y, Parent.Rect.Width, Height);
            checkMovingForm();
            mouseMoveHandler(mouseEvt);

            base.Update(gametime, mouseEvt);
        }
        public override void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {
            spriteBatch.Draw(ReadHelper.Texture.PixelTexture, Rect, color);
        }


    }
}
