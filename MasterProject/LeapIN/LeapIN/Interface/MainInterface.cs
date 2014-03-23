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

        PointerModule mouse;
        KeyboardModule keyboard;

        public MainInterface()
        {
            // Set up the listener
            listener = new LeapControl();
            listener.FrameReady += UpdateFrame;

            // Set up both modules with the various settings
            mouse = new PointerModule();
            keyboard = new KeyboardModule();

            // Get the screen size
            screenWidth = SystemParameters.PrimaryScreenWidth;
            screenHeight = SystemParameters.PrimaryScreenHeight;
        }

        public PointerModule Mouse
        {
            get { return mouse; }
        }

        public KeyboardModule Keyboard
        {
            get { return keyboard; }
        }

        // Enables/Disables the controller based on the visibility of the overlay
        public void HandleController(object sender, DependencyPropertyChangedEventArgs e)
        {
            Window overlay = sender as Window;

            if (overlay.Visibility == Visibility.Hidden)
            {
                leap.RemoveListener(listener);
                leap.Dispose();
            } 
            else if (overlay.Visibility == Visibility.Visible)
            {
                leap = new Controller();

                // Runs the leap constantly so it can be used when the window loses focus
                if (leap.PolicyFlags != Controller.PolicyFlag.POLICYBACKGROUNDFRAMES)
                {
                    leap.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);
                }

                leap.AddListener(listener);
            }
        }

        // When the window does get closed, make sure the leap service is disposed of
        public void DestroyDevice()
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
