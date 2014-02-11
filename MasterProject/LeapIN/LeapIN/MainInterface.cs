using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Ink;

using Leap;
using LeapIN.Extras;


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
        bool eventOccurred = true;

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
            if (pointable.TouchZone != Pointable.Zone.ZONENONE)
            {
                Win32Services.MoveCursor((int)tx, (int)ty);

                if (pointable.TouchDistance <= 0 && eventOccurred) // Inside the 'touch zone'
                {
                    // perform mouse click - this will change once on screen buttons are developed for making double clicks and right clicks
                    Win32Services.MouseClick((uint)tx, (uint)ty);
                    eventOccurred = false;
                }
                else if (pointable.TouchDistance > 0)
                {
                    eventOccurred = true;
                }
            }
        }
    }
}
