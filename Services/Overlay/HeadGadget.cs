using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ui.Services.Overlay
{
    public class HeadGadget : Gadget
    {
        Color color = Color.Transparent;
        Texture2D texture;
        public HeadGadget()
        {
            texture = new Texture2D(ReadHelper.Instance.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            OnMouseIn += mouseInHandler;
        }
        void mouseInHandler(object sender, ChangeEventArgs<bool> e)
        {
            if (e.Current)
            {
                color = new Color(10, 10, 10, 0);
            }
            else
            {
                color = Color.Transparent;
            }
        }
        public override void Update(GameTime gametime)
        {
            int w = (int)(ReadHelper.Overlay.Rect.Width * 0.9);
            int h = (int)(w * 0.4);
            rect = new Rectangle(0, 0, w, 20);
            base.Update(gametime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, rect, color);
        }


    }
}
