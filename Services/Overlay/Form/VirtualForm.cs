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
using System.Diagnostics;

namespace ReadHelper.Services.Overlay.Form
{
    public class VirtualForm : Gadget
    {

        public static readonly HashSet<VirtualForm> AllForms = [];
        Color bg = new(33, 33, 33);
        readonly FormHead head;
        readonly ResizeCorner resizeCorner;
        public bool Moving { get => head.Moving; }
        public bool Resizing { get => resizeCorner.Resizing; }
        public Point MinSize = new Point(300,100);
        public Point MaxSize = new Point(-1, -1);
        public bool Resizeable { get => !resizeCorner.Disabled; set => resizeCorner.Disabled = !value; }
        public VirtualForm():base(ReadHelper.Overlay)
        {
            AllForms.Add(this);
            Padding = new() { Top = 10, Bottom = 10, Left = 10, Right = 10 };
            head = new FormHead(this);
            resizeCorner = new ResizeCorner(this) {  Disabled = true };
            OnLeftMouseBtnPress += delegate { ToTop(); };

        }
        public override void Draw(SpriteBatch spriteBatch, OverlayRoot overlay)
        {
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = Rect;
            spriteBatch.Begin(rasterizerState: new RasterizerState() { ScissorTestEnable = true });
            spriteBatch.Draw(Texture.PixelTexture, Rect, bg);
            base.Draw(spriteBatch, overlay); // draw children
        }
        public void ToTop()
        {
            Gadget topGadget = Parent.Children.MaxBy(child => child.ZIndex);
            if (Equals(topGadget, this)) return;
            ZIndex = topGadget.ZIndex + 1;
        }

    }
}
