using System;
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

        // States for the pointer
        bool touching = false;
        bool dragging = false;
        bool mouseOverControl = false;

        // For the tracking data - REMOVE AT RELEASE
        int curX = 0;
        int curY = 0;
        
        // Leap point tracking, hover duration and threshold values for hover touching
        Point anchor = new Point(); // Current hover point
        double progress = 0; // The current progress towards a click event
        double eventspeed = 4; // Higher values decrease hover time
        double threshold = 2.25d; // For sum of squares
        double exitThreshold = 6.0d; // For when a click has occurred exit thresh is higher to prevent extra clicks

        ICommand _changeClick;
        string _curMode;

        enum ClickMode
        {
            Single,
            Double,
            Drag,
            Right
        }

        ClickMode currentMode;

        public MainInterface()
        {
            // Set up the controller
            leap = new Controller();
            listener = new LeapControl();
            listener.FrameReady += UpdateFrame;

            // Set the default mode
            currentMode = ClickMode.Single;
            Mode = "Single Click";

            // Runs the leap constantly so it can be used when the window loses focus
            if (leap.PolicyFlags != Controller.PolicyFlag.POLICYBACKGROUNDFRAMES)
            {
                leap.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);
            }

            // Get the screen size
            screenWidth = SystemParameters.PrimaryScreenWidth;
            screenHeight = SystemParameters.PrimaryScreenHeight;
        }

        public ICommand ChangeClick
        {
            get
            {
                if (_changeClick == null)
                {
                    _changeClick = new RelayCommand(
                        param => SetClickMode());
                }
                return _changeClick;
            }
        }

        public string Mode
        {
            get { return _curMode; }
            set
            {
                if (_curMode != value)
                {
                    _curMode = value;
                    OnPropertyChanged("Mode");
                }
            }
        }

        public bool MouseOverControl
        {
            get { return mouseOverControl; }
            set
            {
                if (mouseOverControl != value)
                {
                    mouseOverControl = value;
                    OnPropertyChanged("MouseOverControl");
                }
            }
        }

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

        protected void SetClickMode()
        {
            switch (currentMode)
            {
                case ClickMode.Single:
                    currentMode = ClickMode.Double;
                    Mode = "Double Click";
                    break;
                case ClickMode.Double:
                    currentMode = ClickMode.Drag;
                    Mode = "Drag Click";
                    break;
                case ClickMode.Drag:
                    currentMode = ClickMode.Right;
                    Mode = "Right Click";
                    break;
                case ClickMode.Right:
                    currentMode = ClickMode.Single;
                    Mode = "Single Click";
                    break;
                default:
                    break;
            }
        }

        // Enables the listener whenever the interface is active/shown
        public void EnableListener(object sender, RoutedEventArgs e)
        {
            leap.AddListener(listener);
        }

        // Disables the listener when the interface is hidden or closed
        public void DisableListener(object sender, RoutedEventArgs e)
        {
            leap.RemoveListener(listener);
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

            /* The following section is definitely subject to change
             * Other methods may get called to handle switching click modes and all kinds of stuff
             */

            // Allows use of mouse normally
            if (pointable.TouchZone != Pointable.Zone.ZONENONE)
            {
                if (!touching)
                {
                    if (progress == 0)
                    {
                        anchor = new Point(tx, ty);
                    }

                    if (SumSqrDist(tx, ty, anchor.X, anchor.Y) > threshold)
                    {
                        progress = 0;
                    }
                    else
                    {
                        progress += eventspeed;
                    }

                    Win32Services.MoveCursor((int)tx, (int)ty);

                    if (progress >= 100)
                    {
                        // Allows for selection/dragging
                        if (dragging && !MouseOverControl)
                        {
                            Win32Services.MouseClick(Win32Services.MouseEventFlags.LEFTUP);
                            dragging = false;
                        }
                        else
                        {
                            // perform a single mouse click when over a control, otherwise use the mode
                            if (MouseOverControl)
                            {
                                Win32Services.MouseClick(Win32Services.MouseEventFlags.LEFTDOWN | Win32Services.MouseEventFlags.LEFTUP);
                            }
                            else
                            {
                                MouseEvent();
                            }

                            touching = true;
                        }
                    }
                }
                else
                {
                    if (SumSqrDist(tx, ty, anchor.X, anchor.Y) > exitThreshold)
                    {
                        progress = 0;
                        touching = false;
                    }

                    Win32Services.MoveCursor((int)tx, (int)ty);
                }
            }

            Win32Services.POINT pt = Win32Services.GetCursorPosition();
            xPos = pt.x;
            yPos = pt.y;
        }

        /// <summary>
        /// Calculates a returns a sum of the squared distances
        /// to provide a score showing how far from the anchor
        /// the cursor currently is.
        /// </summary>
        double SumSqrDist(double ax, double ay, double bx, double by)
        {
            double dx = ax - bx;
            double dy = ay - by;

            return dx * dx + dy * dy;
        }

        /// <summary>
        /// Handles the different types of mouse event based on
        /// what mode the module is currently in.
        /// </summary>
        void MouseEvent()
        {
            switch (currentMode)
            {
                case ClickMode.Single:
                    Win32Services.MouseClick(Win32Services.MouseEventFlags.LEFTDOWN | Win32Services.MouseEventFlags.LEFTUP);
                    break;
                case ClickMode.Double:
                    Win32Services.MouseClick(Win32Services.MouseEventFlags.LEFTDOWN | Win32Services.MouseEventFlags.LEFTUP);
                    Win32Services.MouseClick(Win32Services.MouseEventFlags.LEFTDOWN | Win32Services.MouseEventFlags.LEFTUP);
                    break;
                case ClickMode.Drag:
                    Win32Services.MouseClick(Win32Services.MouseEventFlags.LEFTDOWN);
                    dragging = true;
                    break;
                case ClickMode.Right:
                    Win32Services.MouseClick(Win32Services.MouseEventFlags.RIGHTDOWN | Win32Services.MouseEventFlags.RIGHTUP);
                    break;
            }
        }
    }
}
