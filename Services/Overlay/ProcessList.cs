using Microsoft.Xna.Framework;
using ReadHelper.Services.Overlay.Form;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHelper.Services.Overlay
{
    public class ProcessList : VirtualForm
    {
        public ProcessList()
        {
            Rect = new Rectangle(20, 0, 500, 500);
            BuildCloseBtn();
            BuildConfirmBtn();
        }
        private void BuildCloseBtn()
        {
            TextButton btn = new TextButton("Close", this)
            {
                ResizeStickyParent = Sticky.BOTTOM,
                
            };
            Left = 10;
            Top = 0;
            btn.OnLeftMouseBtnClick += delegate
            {
                Close();
            };
        }
        private void BuildConfirmBtn()
        {
            TextButton btn = new TextButton("Confirm", this) { ResizeStickyParent = Sticky.BOTTOM};

            btn.RelativePosition = new Point((Size.X - btn.Size.X) / 2 - 100, Size.Y + 100);
            btn.OnLeftMouseBtnClick += delegate
            {
                Close();
            };
        }
        private void Close()
        {
            ReadHelper.Overlay.ProcessListForm.Disabled = true;
        }
    }
}
