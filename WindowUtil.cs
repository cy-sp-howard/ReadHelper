using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using SharpDX.Win32;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InconsistentNaming

namespace ui
{

    public static class WindowUtil
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
        public enum HookType
        {
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }
        public delegate int HookCallbackDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType idHook, HookCallbackDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(HookType idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hook);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandleW(IntPtr fakezero);

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseLLHookStruct
        {

            public Point Point { get; }
            public int MouseData { get; }
            public int Flags { get; }
            public int Time { get; }
            public IntPtr Extra { get; }

        }
        public enum MouseEventType
        {

            /// <summary>
            /// Occurs when the mouse has moved.
            /// </summary>
            MouseMoved = 512,

            /// <summary>
            /// Occurs when the left-mouse button is pressed.
            /// </summary>
            LeftMouseButtonPressed = 513,

            /// <summary>
            /// Occurs when the left-mouse button is released.
            /// </summary>
            LeftMouseButtonReleased = 514,

            /// <summary>
            /// Occurs when the right-mouse button is pressed.
            /// </summary>
            RightMouseButtonPressed = 516,

            /// <summary>
            /// Occurs when the right-mouse button is released.
            /// </summary>
            RightMouseButtonReleased = 517,

            /// <summary>
            /// Occurs when the mouse-wheel is scrolled.
            /// </summary>
            MouseWheelScrolled = 522,

            /// <summary>
            /// Occurs when the mouse enters the bounds of the control.
            /// </summary>
            /// <remarks>
            /// Exclusive to mouse events on <see cref="Controls"/>.
            /// </remarks>
            MouseEntered,

            /// <summary>
            /// Occurs when the mouse leaves the bounds of the control.
            /// </summary>
            /// <remarks>
            /// Exclusive to mouse events on <see cref="Controls"/>.
            /// </remarks>
            MouseLeft

        }
    }
}
