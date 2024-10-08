using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming

namespace ui
{

    internal static class WindowUtil
    {
        public const uint WS_EX_TOPMOST = 0x00000008;
        public const uint WS_EX_TRANSPARENT = 0x00000020;
        public const uint WS_EX_TOOLWINDOW = 0x00000080;
        public const uint WS_EX_CONTROLPARENT = 0x00010000;
        public const uint WS_EX_APPWINDOW = 0x00040000;
        public const uint WS_EX_LAYERED = 0x00080000;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        public const uint CS_VREDRAW = 0x0001;
        public const uint CS_HREDRAW = 0x0002;

        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;

        public const int MINIMIZED_POS = -32000;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);
        
        [DllImport("Dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, UIntPtr dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct Margins
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex) => IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
        public static int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong) => IntPtr.Size != 8 ? SetWindowLong32(hWnd, nIndex, dwNewLong) : (int)SetWindowLongPtr64(hWnd, nIndex, new UIntPtr(dwNewLong));
        public static void SetWindowParam(IntPtr winHandle, bool showInTaskbar = false)
        {
            uint windowParam = WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_CONTROLPARENT | WS_EX_LAYERED;

            if (showInTaskbar)
            {
                ShowWindow(winHandle, SW_HIDE);
                windowParam |= WS_EX_APPWINDOW;
            }
            else
            {
                windowParam |= WS_EX_TOOLWINDOW;
            }

            SetWindowLong(winHandle, GWL_EXSTYLE, windowParam);

            if (showInTaskbar)
            {
                ShowWindow(winHandle, SW_SHOW);
            }
        }
    }
}
