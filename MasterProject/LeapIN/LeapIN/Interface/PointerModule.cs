using System;
using System.Collections.Generic;
using System.Windows;

using LeapIN.Extras;

namespace LeapIN.Interface
{
    using MouseFlags = Win32Services.MouseEventFlags; // To reuse the name

    class PointerModule : PropertyChange
    {
        // States for the pointer
        bool touching = false; // This means that a click event has occurred, switches back to false after exitThreshold is exceeded
        bool dragging = false; // Special case - if drag mode is active then the left button has to be released on a second mouse click
        bool scrolling = false;
        bool mouseOverControl = false; // When the mouse is over the UI only single clicks are possible regardless of mode

        // Leap point tracking, hover duration and threshold values for hover touching
        Point anchor = new Point(); // Current hover point
        double progress = 0; // The current progress towards a click event
        double eventspeed = 4; // Higher values decrease hover time
        double threshold = 2.25d; // For sum of squares
        double exitThreshold = 6.0d; // For when a click has occurred exit thresh is higher to prevent extra clicks

        // To change the thresh, exitthresh and speed values
        public void AlterSettings(double s, double es, double sp)
        {
            threshold = s;
            exitThreshold = es;
            sp = eventspeed;
        }

        // Nested class for a pointer mode
        public class PointerMode
        {
            public string name;
            public MouseFlags flags;
            public int specialID;
            public int dwData;

            public PointerMode(string n, MouseFlags f, int s, int data)
            {
                name = n;
                flags = f;
                specialID = s;
                dwData = data;
            }

            public PointerMode(string n, MouseFlags f) : this(n, f, 0, 0) { }

            public string Name
            {
                get { return name; }
            }
        }

        // Set of pointer modes
        List<PointerMode> mouseModes;
        PointerMode selectedMode;

        public PointerModule()
        {
            // Add the mouse modes
            MouseModes.Add(new PointerMode("Left", MouseFlags.LEFTDOWN | MouseFlags.LEFTUP)); // index 0
            MouseModes.Add(new PointerMode("Right", MouseFlags.RIGHTDOWN | MouseFlags.RIGHTUP, 3, 0)); // index 1
            MouseModes.Add(new PointerMode("Double", MouseFlags.LEFTDOWN | MouseFlags.LEFTUP, 1, 0)); // 2
            MouseModes.Add(new PointerMode("Drag", MouseFlags.LEFTDOWN, 2, 0)); // 3
            MouseModes.Add(new PointerMode("Up", MouseFlags.WHEEL, 4, 10)); // 4
            MouseModes.Add(new PointerMode("Down", MouseFlags.WHEEL, 4, -10)); //5

            // Set to left click by default
            SelectedMode = MouseModes[0]; 
        }

        public List<PointerMode> MouseModes
        {
            get
            {
                if (mouseModes == null)
                    mouseModes = new List<PointerMode>();

                return mouseModes;
            }
        }

        public PointerMode SelectedMode
        {
            get { return selectedMode; }
            set
            {
                if (selectedMode != value)
                {
                    selectedMode = value;
                    OnPropertyChanged("SelectedMode");
                }
            }
        }

        // Global property that is true when the cursor is over any element of the UI linked to this via ChangePropertyAction
        public bool MouseOverControl
        {
            get { return mouseOverControl; }
            set
            {
                if (mouseOverControl != value)
                {
                    mouseOverControl = value;
                }
            }
        }

        public bool Touching
        {
            get { return touching; }
            set
            {
                if (touching != value)
                {
                    touching = value;
                    OnPropertyChanged("Touching");
                }
            }
        }

        /// <summary>
        /// The main function of this module, handles the coordinates sent by the frame
        /// of data and activates the various mouse events depending on the current mode.
        /// </summary>
        public void HandleFrame(double x, double y)
        {
            if (!Touching)
            {
                if (progress == 0)
                {
                    anchor = new Point(x, y);
                }

                if (SumSqrDist(x, y, anchor.X, anchor.Y) > threshold)
                {
                    progress = 0;
                }
                else
                {
                    progress += eventspeed;
                }

                Win32Services.MoveCursor((int)x, (int)y);

                if (progress >= 100)
                {
                    // Allows for selection/dragging
                    if (dragging && !MouseOverControl)
                    {
                        Win32Services.MouseClick(MouseFlags.LEFTUP);
                        dragging = false;
                    }
                    else
                    {
                        // perform a single mouse click when over a control, otherwise use the mode
                        if (MouseOverControl)
                        {
                            SpecialEvent();
                        }
                        else
                        {
                            MouseEvent();
                        }
                    }

                    Touching = true;
                }
            }
            else
            {
                if (scrolling)
                {
                    Win32Services.MouseClick(SelectedMode.flags, SelectedMode.dwData);
                }

                if (SumSqrDist(x, y, anchor.X, anchor.Y) > exitThreshold)
                {
                    progress = 0;
                    Touching = false;
                    scrolling = false;
                    exitThreshold = 6.0d;
                }

                Win32Services.MoveCursor((int)x, (int)y);
            }
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
            // Send an event
            Win32Services.MouseClick(SelectedMode.flags, SelectedMode.dwData);

            switch (SelectedMode.specialID)
            {
                case 1: // Double Click
                    Win32Services.MouseClick(SelectedMode.flags);
                    break;
                case 2: // Drag click
                    dragging = true;
                    break;
                case 3: // Right click
                    SelectedMode = MouseModes[0];
                    break;
                case 4: // Scrolling
                    scrolling = true;
                    exitThreshold *= 6; // just to stabilise the scrolling
                    break;
            }
        }

        /// <summary>
        /// Executed when the pointer is over certain UI elements,
        /// used to perform single clicks only.
        /// </summary>
        void SpecialEvent()
        {
            Win32Services.MouseClick(MouseModes[0].flags);
        }
    }
}
