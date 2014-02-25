using System;
using System.Windows;

using Leap;
using LeapIN.Extras;


namespace LeapIN.Interface
{
    /// <summary>
    /// This class represents all of the interface of the application. 
    /// All button handling functions and so on will come through here, 
    /// it should implement all the other classes.
    /// </summary>
    class MainInterface : PropertyChange
    {
        LeapControl listener;
        Controller leap;
        double screenWidth;
        double screenHeight;
        bool touching = false;

        int curX = 0;
        int curY = 0;

        public int xPos
        {
            get { return curX; }
            set { curX = value; OnPropertyChanged("xPos"); }
        }

        public int yPos
        {
            get { return curY; }
            set { curY = value; OnPropertyChanged("yPos"); }
        }

        public MainInterface()
        {
            // Set up the controller
            leap = new Controller();
            listener = new LeapControl();
            listener.FrameReady += UpdateFrame;

            // Runs the leap constantly so it can be used when the window loses focus
            if (leap.PolicyFlags != Controller.PolicyFlag.POLICYBACKGROUNDFRAMES) {
                leap.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);
            }

            // Get the screen size
            screenWidth = SystemParameters.PrimaryScreenWidth;
            screenHeight = SystemParameters.PrimaryScreenHeight;
        }

        public void EnableListener(object sender, RoutedEventArgs e)
        {
            leap.AddListener(listener);
        }

        public void DisableListener(object sender, RoutedEventArgs e)
        {
            leap.RemoveListener(listener);
        }

        protected void UpdateFrame(Controller con)
        {
            // Grab the frame and the nearest pointer
            Frame frame = con.Frame();
            Pointable pointable = frame.Pointables.Frontmost;
            Leap.Vector stabilizedPos = pointable.StabilizedTipPosition;

            // Calculate the position in screen space
            InteractionBox iBox = con.Frame().InteractionBox;
            Leap.Vector normalizedPosition = iBox.NormalizePoint(stabilizedPos);
            double tx = normalizedPosition.x * screenWidth;
            double ty = screenHeight - normalizedPosition.y * screenHeight;

            /* The following section is definitely subject to change
             * Other methods may get called to handle switching click modes and all kinds of stuff
             */

            // Allows use of mouse normally
            if (pointable.TouchZone != Pointable.Zone.ZONENONE)
            {
                Win32Services.MoveCursor((int)tx, (int)ty);

                if (pointable.TouchDistance <= 0 && !touching) // Inside the 'touch zone'
                {
                    // perform a single mouse click - this will change once on screen buttons are developed for making double clicks and right clicks
                    Win32Services.MouseClick(Win32Services.MouseEventFlags.LEFTDOWN | Win32Services.MouseEventFlags.LEFTUP);
                    touching = true;
                }
                else if (pointable.TouchDistance > 0)
                {
                    // Adding this and removing the LEFTUP from earlier allows for selection etc
                    //Win32Services.MouseClick(Win32Services.MouseEventFlags.LEFTUP);
                    touching = false;
                }
            }

            Win32Services.POINT pt = Win32Services.GetCursorPosition();
            xPos = pt.x;
            yPos = pt.y;
        }
    }
}
