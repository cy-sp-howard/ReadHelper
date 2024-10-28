using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReadHelper.Services.Overlay.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHelper.Services.Overlay
{
    public class MainForm : VirtualForm
    {
        readonly AttachButton btn;
        public MainForm()
        {
            Rect = new Rectangle(20, 0, 500, 200);
            Resizeable = true;
            btn = new AttachButton() { Parent = this, RelativePosition = new Point(0, 0) };
        }
        public override void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            base.Update(gametime, mouseEvt);
        }
    }
    public class AttachButton : Gadget
    {
        readonly Texture2D text = Texture.TextTexture("Attach");
        public AttachButton()
        {
            Size = new Point(text.Width, text.Height);
        }
        public override void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {
            spriteBatch.Draw(text, Rect, Color.White);
        }
    }
}
