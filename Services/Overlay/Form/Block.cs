using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHelper.Services.Overlay.Form
{
    public class Block : Gadget
    {
        public int Height;
        public int Width;
        private int ChildrenHeight;
        private float visibleRate => (float)Height / ChildrenHeight;
        private int scrollbarWidth = 10;
        public int ScrollTop = 0;
        public Block(Gadget parent) : base(parent)
        {
            OnMouseWheel += WheelHandler;
        }
        public override void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            int itemRenderTop = ScrollTop * -1;
            ChildrenHeight = 0;
            int width = Width;
            foreach (Gadget item in Children)
            {
                if (Width <= 0 && item.Rect.Width > width)
                {
                    width = item.Rect.Width;
                }
                item.Left = 0;
                item.Top = itemRenderTop;
                ChildrenHeight += item.Rect.Height;
                itemRenderTop += item.Rect.Height;
            }
            int height = Height > 0 ? Height : ChildrenHeight;
            Size = new Point(width, height);
            if (visibleRate < 1)
            {
                width += scrollbarWidth;
            }
            base.Update(gametime, mouseEvt);
        }
        public override void DrawInRect(SpriteBatch spriteBatch, OverlayRoot overlay)
        {
            spriteBatch.Draw(Texture.PixelTexture, Rect, new(47, 47, 47));
        }
        public override void DrawInRectAfterChildren(SpriteBatch spriteBatch, OverlayRoot overlay)
        {
            Rectangle barWrapperRect = new Rectangle(Rect.Left + Rect.Width - scrollbarWidth, Rect.Top, scrollbarWidth, Rect.Height);
            spriteBatch.Draw(Texture.PixelTexture, barWrapperRect, Color.Gray);
            int barWrapperPadding = 2;
            int barMaxHeight = barWrapperRect.Height - barWrapperPadding * 2;
            int barHieght = (int)((float)barMaxHeight * visibleRate);
            spriteBatch.Draw(Texture.PixelTexture, new Rectangle(barWrapperRect.Left + barWrapperPadding, barWrapperRect.Top + barWrapperPadding + (int)((float)ScrollTop * visibleRate), barWrapperRect.Width - barWrapperPadding * 2, barHieght), Color.DarkGray);
        }
        private void WheelHandler(object sender, MouseEventArgs e)
        {
            ScrollTop -= e.Wheel / 300000;
            int maxScrollTop = ChildrenHeight - Height;
            if (ScrollTop < 0) ScrollTop = 0;
            else if(ScrollTop > maxScrollTop) ScrollTop = maxScrollTop;


        }
    }
}
