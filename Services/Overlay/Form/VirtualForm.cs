using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ui.Services.Overlay.Form
{
    public class VirtualForm : Gadget
    {
        Color bg = new Color(33,33,33);
        public VirtualForm() {
            new FormHead() { Parent = this };
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Overlay overlay)
        {
            spriteBatch.Draw(ReadHelper.PixelTexture, Rect, bg);
            base.Draw(spriteBatch, graphicsDevice, overlay);
        }
    }
}
