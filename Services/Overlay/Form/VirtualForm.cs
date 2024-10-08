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

namespace ui.Services.Overlay.Form
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
        }
        public override void Update(GameTime gametime, MouseState mouse)
        {
            base.Update(gametime, mouse);
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Overlay overlay)
        {
            spriteBatch.Draw(ReadHelper.PixelTexture, Rect, bg);
            base.Draw(spriteBatch, graphicsDevice, overlay);
        }
    }
}
