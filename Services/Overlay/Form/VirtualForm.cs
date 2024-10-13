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
    public class VirtualForm : Gadget
    {

        public static readonly HashSet<VirtualForm> AllForms = new HashSet<VirtualForm>();
        Color bg = new Color(33, 33, 33);
        FormHead head;
        ResizeCorner resizeCorner;
        public bool Moving { get => head.Moving; }
        public bool Resizing { get => resizeCorner.Resizing; }
        public bool Resizeable { get => !resizeCorner.Disabled; set => resizeCorner.Disabled = !value; }
        public VirtualForm()
        {
            AllForms.Add(this);
            head = new FormHead() { Parent = this };
            resizeCorner = new ResizeCorner() { Parent = this, Disabled = true };
            OnLeftMouseBtnPress += toTop;

        }
        public override void Update(GameTime gametime, MouseEventArgs mouseEvt)
        {
            base.Update(gametime, mouseEvt);
        }
        public override void Draw(SpriteBatch spriteBatch, Overlay overlay)
        {
            spriteBatch.Draw(ReadHelper.Texture.PixelTexture, Rect, bg);
            base.Draw(spriteBatch, overlay);
        }
        void toTop(object sender, MouseEventArgs e)
        {
            Gadget topGadget =  Parent.Children.MaxBy(child => child.ZIndex);
            if (Equals(topGadget, this)) return;
            ZIndex = topGadget.ZIndex + 1;
        }
    }
}
