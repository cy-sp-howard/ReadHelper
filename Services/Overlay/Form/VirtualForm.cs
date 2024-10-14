using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.Xna.Framework.Input;

namespace ReadHelper.Services.Overlay.Form
{
    public class VirtualForm : ParentGadget
    {

        public static readonly HashSet<VirtualForm> AllForms = [];
        Color bg = new(33, 33, 33);
        readonly FormHead head;
        readonly ResizeCorner resizeCorner;
        public bool Moving { get => head.Moving; }
        public bool Resizing { get => resizeCorner.Resizing; }
        public bool Resizeable { get => !resizeCorner.Disabled; set => resizeCorner.Disabled = !value; }
        public VirtualForm()
        {
            AllForms.Add(this);
            head = new FormHead() { Parent = this };
            resizeCorner = new ResizeCorner() { Parent = this, Disabled = true };
            OnLeftMouseBtnPress += ToTop;

        }
        public override void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {
            spriteBatch.Draw(ReadHelper.Texture.PixelTexture, Rect, bg);
            base.Draw(spriteBatch, overlay); // draw children
        }
        private void ToTop(object sender, MouseEventArgs e)
        {
            Gadget topGadget = Parent.Children.MaxBy(child => child.ZIndex);
            if (Equals(topGadget, this)) return;
            ZIndex = topGadget.ZIndex + 1;
        }
    }
}
