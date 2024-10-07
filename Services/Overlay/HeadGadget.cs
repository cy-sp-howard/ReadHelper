using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ui.Services.Overlay
{
    public class HeadGadget : Gadget
    {
        Color color = Color.Transparent;
        Texture2D texture;
        Point mouseMoveStartPos = new Point(-1,-1);
        Point overlayMoveStartPos = new Point(-1,-1);
        bool moving = false;
        public HeadGadget()
        {
            texture = new Texture2D(ReadHelper.Instance.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            OnMouseIn += mouseInHandler;
            OnMouseOut += mouseOutHandlerr;
            OnLeftMouseBtnPress += mousePressHandler;
            OnLeftMouseBtnRelease += mouseReleaseHandler;
        }
        void mouseInHandler(object sender, ChangeEventArgs<bool> e)
        {
            color = new Color(10, 10, 10, 0);
        }
        void mouseOutHandlerr(object sender, ChangeEventArgs<bool> e)
        {
            color = Color.Transparent;
        }
        void mousePressHandler(object sender, ChangeEventArgs<bool> e)
        {
            moving = true;
        }
        void mouseReleaseHandler(object sender, ChangeEventArgs<bool> e)
        {
            moving = false;
            mouseMoveStartPos = new Point(-1,-1);
        }
        void mouseMoveHandler(MouseState mouse)
        {
            if (!moving) return;
            if(mouseMoveStartPos.X < 0)
            {
                mouseMoveStartPos = mouse.Position;
                overlayMoveStartPos = new Point(ReadHelper.Overlay.Rect.X, ReadHelper.Overlay.Rect.Y);
            }
            int moveX = mouse.Position.X - mouseMoveStartPos.X;
            int moveY = mouse.Position.Y - mouseMoveStartPos.Y;
            ReadHelper.Overlay.Rect = new Rectangle(overlayMoveStartPos.X + moveX, overlayMoveStartPos.Y + moveY, ReadHelper.Overlay.Rect.Width, ReadHelper.Overlay.Rect.Height);
        }
        public override void Update(GameTime gametime, MouseState mouse)
        {
            int w = (int)(ReadHelper.Overlay.Rect.Width);
            rect = new Rectangle(ReadHelper.Overlay.Rect.X, ReadHelper.Overlay.Rect.Y, w, 20);
            mouseMoveHandler(mouse);
            base.Update(gametime, mouse);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, color);
        }


    }
}
