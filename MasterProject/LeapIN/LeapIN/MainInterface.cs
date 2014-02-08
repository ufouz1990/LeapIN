using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Ink;

using Leap;
using System.Runtime.InteropServices;


namespace LeapIN
{
    /// <summary>
    /// This class represents all of the interface of the application. 
    /// All button handling functions and so on will come through here, 
    /// it should implement all the other classes.
    /// </summary>
    class MainInterface
    {
        Controller leap;
        double windowWidth;
        double windowHeight;

        public MainInterface()
        {
            // Set up the cursor
            leap = new Controller();

            // Get the window size
            windowWidth = SystemParameters.PrimaryScreenWidth;
            windowHeight = SystemParameters.PrimaryScreenHeight;

            // Add event to update frame with pointer position
            CompositionTarget.Rendering += UpdateFrame;
        }

        // Required system win32 API call to move the cursor
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        protected void UpdateFrame(object sender, EventArgs e)
        {
            // Grab the frame and the nearest pointer
            Leap.Frame frame = leap.Frame();
            Pointable pointable = frame.Pointables.Frontmost;
            Leap.Vector stabilizedPos = pointable.StabilizedTipPosition;

            // Calculate the position in screen space
            InteractionBox iBox = leap.Frame().InteractionBox;
            Leap.Vector normalizedPosition = iBox.NormalizePoint(stabilizedPos);
            double tx = normalizedPosition.x * windowWidth;
            double ty = windowHeight - normalizedPosition.y * windowHeight;

            // Allows use of mouse normally
            if (pointable.TouchDistance > 0 && pointable.TouchZone != Pointable.Zone.ZONENONE)
            {
                SetCursorPos((int)tx, (int)ty);
            }
        }
    }
}
