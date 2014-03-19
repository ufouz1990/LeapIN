using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace LeapIN.Interface
{
    public static class Win32Services
    {
        static IntPtr lastHandle = IntPtr.Zero;

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

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }

            public int Height
            {
                get { return bottom - top; }
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }
        }

        //enum GetWindow_Cmd : uint
        //{
        //    GW_HWNDFIRST = 0,
        //    GW_HWNDLAST = 1,
        //    GW_HWNDNEXT = 2,
        //    GW_HWNDPREV = 3,
        //    GW_OWNER = 4,
        //    GW_CHILD = 5,
        //    GW_ENABLEDPOPUP = 6
        //}

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
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800
        }

        /* DLL IMPORTS */

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        //[DllImport("user32.dll")]
        //static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        //[DllImport("user32.dll")]
        //static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr SetForegroundWindow(IntPtr handle);

        // Required functions for controlling the cursor
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpMousePoint);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, int cButtons, uint dwExtraInfo);

        /* DLL Method Calls */

        public static void SetupWindow(IntPtr handle)
        {
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        public static void GetActiveWindow()
        {
            lastHandle = GetForegroundWindow();
        }

        public static void SetLastActive()
        {
            IntPtr curHandle = GetForegroundWindow();

            if (curHandle != lastHandle)
            {
                SetForegroundWindow(lastHandle);
            }
        }

        //public static void SetLastActive()
        //{
        //    IntPtr curHandle = GetForegroundWindow();
        //    IntPtr targetHwnd = GetWindow(curHandle, (uint)GetWindow_Cmd.GW_HWNDNEXT);
        //    while (true)
        //    {
        //        IntPtr temp = GetParent(targetHwnd);
        //        if (temp.Equals(IntPtr.Zero)) break;
        //        targetHwnd = temp;
        //    }
        //    SetForegroundWindow(targetHwnd);
        //}

        //public static void SetLastActive()
        //{
        //    IntPtr curHandle = GetForegroundWindow();

        //    if (curHandle != lastHandle)
        //    {
        //        IntPtr lastWindow = GetLastActivePopup(curHandle);
        //        lastHandle = IsWindowVisible(lastWindow) ? lastWindow : IntPtr.Zero;

        //        //Switch to the last window
        //        SetForegroundWindow(lastHandle);
        //    }
        //    else
        //    {
        //        lastHandle = IntPtr.Zero;
        //    }
        //}

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
            MouseClick(val, 0);
        }

        public static void MouseClick(MouseEventFlags val, int dwData)
        {
            POINT pos = GetCursorPosition();
            mouse_event((uint)val, (uint)pos.x, (uint)pos.y, dwData, 0);
        }
    }
}
