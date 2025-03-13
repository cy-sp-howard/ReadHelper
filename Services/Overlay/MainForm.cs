using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReadHelper.Services.Overlay.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

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
            TextButton btn = new TextButton("Attach", this) { ResizeStickyParent = Sticky.RIGHT };
            btn.Right = 0;
            btn.Top = 0;
            btn.OnLeftMouseBtnClick += delegate
            {
                ReadHelper.Overlay.ProcessListForm.Disabled = false;
                ReadHelper.Overlay.ProcessListForm.ToTop();
            };
        }
    }
}
