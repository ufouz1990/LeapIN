using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LeapIN.Interface
{
    public static class Win32Services
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        // Flags for outgoing mouse events
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        /* DLL IMPORTS */

        // Required functions for controlling the cursor
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpMousePoint);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        /* DLL Method Calls */

        public static POINT GetCursorPosition()
        {
            POINT currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new POINT(0, 0); }

            return currentMousePoint;
        }

        public static bool MoveCursor(int x, int y)
        {
            return SetCursorPos(x, y);
        }

        public static bool MoveCursor(POINT point)
        {
            return SetCursorPos(point.x, point.y);
        }

        public static void MouseClick(MouseEventFlags val)
        {
            POINT pos = GetCursorPosition();
            mouse_event((uint)val, (uint)pos.x, (uint)pos.y, 0, 0);
        }
    }
}
