#if UNITY_EDITOR_WIN

using System.Runtime.InteropServices;
using System;

namespace EditorUtils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width => Right - Left;
        public int Height => Bottom - Top;
    }

    public static class WinApi
    {
        public static bool FullscreenMode;
        public static RECT InitialRect;
        public static int InitialStyle;

        public static void MaximizeWindow(IntPtr hWnd)
        {
            FullscreenMode = true;

            GetWindowRect(hWnd, out InitialRect);
            InitialStyle = GetWindowStyle(hWnd);

            SetWindowStyle(hWnd, FullscreenWindowStyle);
            SetWindowPos(hWnd, 0, 0, ScreenX, ScreenY);
        }

        public static void RestoreWindow(IntPtr hWnd)
        {
            FullscreenMode = false;

            SetWindowStyle(hWnd, InitialStyle);
            SetWindowPos(hWnd, InitialRect.Left, InitialRect.Top, InitialRect.Width, InitialRect.Height);
        }

        private static int ScreenX => 
            GetSystemMetrics(SM_CXSCREEN);

        private static int ScreenY => 
            GetSystemMetrics(SM_CYSCREEN);

        private static int GetWindowStyle(IntPtr hWnd) =>
            GetWindowLong(hWnd, GWL_STYLE);

        private static void SetWindowStyle(IntPtr hWnd, int style) =>
            SetWindowLong(hWnd, GWL_STYLE, style);

        private static void SetWindowPos(IntPtr hWnd, int x, int y, int width, int height) =>
            SetWindowPos(hWnd, IntPtr.Zero, x, y, width, height, SWP_SHOWWINDOW);


        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        private const int GWL_STYLE = -16;
        private const int SWP_SHOWWINDOW = 0x0040;

        private const int FullscreenWindowStyle = 0x14000000;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        private static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    }
}

#endif