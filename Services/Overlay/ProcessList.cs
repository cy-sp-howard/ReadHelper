using Microsoft.Xna.Framework;
using ReadHelper.Services.Overlay.Form;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace ReadHelper.Services.Overlay
{
    public class ProcessList : VirtualForm
    {
        private Block Selector;
        private int SelectedID = -1;
        public ProcessList()
        {
            head.Title = "Process List";
            Rect = new Rectangle(70, 70, 500, 500);
            BuildCloseBtn();
            BuildConfirmBtn();
            BuildSelector();
            OnEnable += delegate
            {
                CreateSelectorItems();
            };
            OnDisable += delegate
            {
                ClearSelectorItems();
            };
        }
        private void BuildCloseBtn()
        {
            TextButton btn = new TextButton("Close", this);
            btn.Width = 130;
            btn.Right = 0;
            btn.Top = btn.Size.Y + 10;
            btn.OnLeftMouseBtnClick += delegate
            {
                Close();
            };
        }
        private void BuildConfirmBtn()
        {
            TextButton btn = new TextButton("Confirm", this);
            btn.Width = 130;
            btn.Right = 0;
            btn.Top = 0;
            btn.OnLeftMouseBtnClick += delegate
            {
                ReadHelper.Overlay.TargetProcess.Process = Process.GetProcessById(SelectedID);
                Close();
            };
        }
        private void BuildSelector()
        {

            Selector = new Block(this);
            Selector.Height = InnerRect.Height;
            Selector.Width = 330;
            Selector.Left = 0;
            Selector.Top = 0;
        }
        private void CreateSelectorItems()
        {
            Selector.ScrollTop = 0;
            Process[] processes = Process.GetProcesses();
            foreach (Process item in processes)
            {
                TextButton selectorItem = new TextButton(item.ProcessName, Selector);
                if(item.Id == ReadHelper.Overlay.TargetProcess.Process?.Id)
                {
                    // prev selected
                    selectorItem.bg = Color.CadetBlue;
                }
                selectorItem.Width = Selector.Width;
                selectorItem.OnLeftMouseBtnClick += delegate
                {
                    SelectHandler(selectorItem, item.Id);
                };
            }
        }
        private void SelectHandler(TextButton btn, int id)
        {
            foreach (TextButton item in Selector.Children)
            {
                item.bg = new(47, 47, 47);
            }

            btn.bg = Color.CadetBlue;
            SelectedID = id;
        }
        private void ClearSelectorItems()
        {
            foreach (var item in Selector.Children)
            {
                item.Parent = null;
            }
        }
        private void Close()
        {
            SelectedID = -1;
            ReadHelper.Overlay.ProcessListForm.Disabled = true;
        }
    }

}
