using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReadHelper.Services.Overlay.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ReadHelper.Services.Overlay
{
    public class MainForm : VirtualForm
    {
        public MainForm()
        {
            Rect = new Rectangle(20, 0, 500, 200);
            Resizeable = true;
            BuildAttachBtn();
        }
        public override void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            base.Update(gametime, mouseEvt);
        }
        private void BuildAttachBtn()
        {
            TextButton btn = new TextButton("Attach", this) { StickyParent = Sticky.RIGHT };
        }
    }
    public class TextButton : Gadget
    {
        readonly Texture2D textTexture;
        Color color = Color.White;
        Color bg = Color.Transparent;
        public TextButton(string text, ParentGadget parent)
        {
            Parent = parent;
            textTexture = Texture.TextTexture(text, new() { FontSize = 18 });
            Size = new Point(textTexture.Width / 2, textTexture.Height / 2);
            RelativePosition = new Point(Parent.Size.X - Size.X, 100);
            OnMouseIn += MouseInHandler;
            OnMouseOut += MouseOutHandler;
        }
        void MouseInHandler(object sender, MouseEventArgs e)
        {
            bg = Color.DarkSlateGray;
        }
        void MouseOutHandler(object sender, MouseEventArgs e)
        {
            bg = Color.Transparent;
        }
        public override void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {
            spriteBatch.Draw(Texture.PixelTexture, Rect, bg);
            spriteBatch.Draw(textTexture, Rect, color);
        }
    }
}
