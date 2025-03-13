using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using static System.Net.Mime.MediaTypeNames;

namespace ReadHelper.Services.Overlay.Form
{
    public class FormHead : Gadget
    {
        Color defaultColor = new(10, 10, 10, 0);
        Color color;
        Point mouseMoveStartPos = new(-1, -1);
        Point formMoveStartPos = new(-1, -1);
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                TitleTexture = Texture.TextTexture(value, new() { FontSize = 24 });
            }
        }
        string _title = "";
        Texture2D TitleTexture;
        public bool Moving = false;
        private int Height = 50;
        public FormHead(Gadget p) : base(p)
        {
            FollowParentPosition = false;
            Parent.Padding.Top += Height;


            color = defaultColor;
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
            color = defaultColor;
        }
        void MousePressHandler(object sender, MouseEventArgs e)
        {
            Moving = true;
        }
        void MouseReleaseHandler(object sender, MouseEventArgs e)
        {
            Moving = false;
            mouseMoveStartPos = new Point(-1, -1);
        }
        void MouseMoveHandler(MouseEventArgs evt)
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
        void CheckMovingForm()
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
            CheckMovingForm();
            MouseMoveHandler(mouseEvt);

            base.Update(gametime, mouseEvt);
        }
        public override void Draw(SpriteBatch spriteBatch, OverlayRoot overlay)
        {
            spriteBatch.Draw(Texture.PixelTexture, Rect, color);
            if (TitleTexture != null)
            {
                int textWidth = TitleTexture.Width / 2;
                int textHeight = TitleTexture.Height / 2;
                spriteBatch.Draw(TitleTexture, new Rectangle(Rect.X + 10,Rect.Y + Rect.Height / 2 - textHeight / 2, textWidth, textHeight), Color.White);
            }
        }


    }
}
