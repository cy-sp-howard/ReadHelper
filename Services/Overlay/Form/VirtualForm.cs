using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ui.Services.Overlay.Form
{
    public class VirtualForm : Gadget
    {

        public static readonly HashSet<VirtualForm> AllForms = new HashSet<VirtualForm>();
        Color bg = new Color(33, 33, 33);
        FormHead head;
        public bool Moving { get => head.Moving; }
        public VirtualForm()
        {
            AllForms.Add(this);
            head = new FormHead() { Parent = this };
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Overlay overlay)
        {
            spriteBatch.Draw(ReadHelper.PixelTexture, Rect, bg);
            base.Draw(spriteBatch, graphicsDevice, overlay);
        }
    }
}
