using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

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

        // For the tracking data - REMOVE AT RELEASE
        //int curX = 0;
        //int curY = 0;

        PointerModule mouse;
        KeyboardModule keyboard;

        //List<KeyboardModule> keySets; // for the main keys a - z (store an upper case variant as well)
        //char selectedKey;

        public MainInterface()
        {
            // Set up the listener
            listener = new LeapControl();
            listener.FrameReady += UpdateFrame;

            //keySets.Add(new KeyboardModule(new char[] {'a', 'b', 'c'})); // EXAMPLE, use to perform a loop for all characters
            keyboard = new KeyboardModule();

            // Get the screen size
            screenWidth = SystemParameters.PrimaryScreenWidth;
            screenHeight = SystemParameters.PrimaryScreenHeight;
        }

        public PointerModule Mouse
        {
            get
            {
                if (mouse == null)
                    mouse = new PointerModule();

                return mouse;
            }
        }

        public KeyboardModule Keyboard
        {
            get
            {
                if (keyboard == null)
                    keyboard = new KeyboardModule();

                return keyboard;
            }
        }

        //public int xPos
        //{
        //    get { return curX; }
        //    set { curX = value; OnPropertyChanged("xPos"); }
        //}

        //public int yPos
        //{
        //    get { return curY; }
        //    set { curY = value; OnPropertyChanged("yPos"); }
        //}

        // Enables the controller whenever the interface is active/shown
        public void EnableController(object sender, RoutedEventArgs e)
        {
            leap = new Controller();

            // Runs the leap constantly so it can be used when the window loses focus
            if (leap.PolicyFlags != Controller.PolicyFlag.POLICYBACKGROUNDFRAMES)
            {
                leap.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);
            }

            leap.AddListener(listener);
        }

        // Disables the controller when the interface is hidden or closed
        public void DisableController(object sender, RoutedEventArgs e)
        {
            leap.RemoveListener(listener);
            leap.Dispose();
        }

        /// <summary>
        /// Triggered when the Leap Motion OnFrame event occurs, handles the cursor control and events.
        /// </summary>
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


            // Allows use of mouse normally
            if (pointable.TouchZone != Pointable.Zone.ZONENONE)
            {
                Mouse.HandleFrame(tx, ty);
            }

            // record current position
            //Win32Services.POINT pt = Win32Services.GetCursorPosition();
            //xPos = pt.x;
            //yPos = pt.y;
        }
    }
}
