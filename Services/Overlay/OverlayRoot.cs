using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReadHelper.Services.Overlay.Form;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static ReadHelper.WindowUtil;

namespace ReadHelper.Services.Overlay
{
    public class OverlayRoot : Gadget, IService
    {
        public ProcessList ProcessListForm { get => _ProcessListForm; }
        private MainForm _MainForm;
        private ProcessList _ProcessListForm;
        HookCallbackDelegate mouseHookCB;
        MouseEventArgs mouseEvent; // mouseEvent never moved type ,after handle set null,so hard to overwrite before handle.
        public override void Load()
        {
            SetInputHook();
            SetRect();
            _MainForm = new MainForm();
            _ProcessListForm = new ProcessList() { Disabled = true};
            base.Load();
        }
        public void Update(GameTime gametime)
        {
            MouseState mouseState = Mouse.GetState();
            mouseEvent = new MouseEventArgs(mouseEvent == null ? MouseEventType.MouseMoved : mouseEvent.EventType, mouseState.Position);
            base.Update(gametime, mouseEvent);
            mouseEvent = null;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch, this);
        }
        void SetRect()
        {
            int winWidth = ReadHelper.Instance.Graphics.PreferredBackBufferWidth;
            int winHeight = ReadHelper.Instance.Graphics.PreferredBackBufferHeight;
            Rect = new Rectangle(0, 0, winWidth, winHeight);
        }
        void SetInputHook()
        {
            mouseHookCB = MouseHookCallback;
            SetWindowsHookEx(HookType.WH_MOUSE_LL, mouseHookCB, IntPtr.Zero, 0);
        }
        int MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseEventArgs evt = new((MouseEventType)wParam, Marshal.PtrToStructure<MouseLLHookStruct>(lParam).Point);

            if (nCode != 0 || evt.EventType == MouseEventType.MouseMoved) return CallNextHookEx(HookType.WH_MOUSE_LL, nCode, wParam, lParam);

            bool skip = evt.EventType == MouseEventType.LeftMouseButtonReleased || evt.EventType == MouseEventType.RightMouseButtonReleased;

            mouseEvent = evt;
            if (!skip && mouseInChild)
            {
                return 1;
            };
            return CallNextHookEx(HookType.WH_MOUSE_LL, nCode, wParam, lParam);

        }

    }

}