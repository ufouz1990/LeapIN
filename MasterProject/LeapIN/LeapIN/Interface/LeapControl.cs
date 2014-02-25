using System;
using Leap;

namespace LeapIN.Interface
{
    /// <summary>
    /// This class could be used to implement a listener to call methods
    /// such as OnConnect, OnFrame, OnExit and so on
    /// </summary>
    class LeapControl : Listener
    {
        public override void OnInit(Controller con)
        {
            base.OnInit(con);
        }

        public override void OnConnect(Controller con)
        {
            base.OnConnect(con);
        }

        public override void OnDisconnect(Controller con)
        {
            base.OnDisconnect(con);
        }

        public override void OnExit(Controller con)
        {
            base.OnExit(con);
        }

        // This FrameHandler allows you to set a method outside this class to run on an event
        // Delegate is a reference to a type of method
        public delegate void FrameHandler(Controller con);

        // FrameReady is the event that calls a FrameHandler once triggered
        public event FrameHandler FrameReady;

        public override void OnFrame(Controller con)
        {
            // If a method is set to the event, fire it
            if (FrameReady != null)
            {
                FrameReady(con);
            }
        }

        public override void OnFocusLost(Controller arg0)
        {
            base.OnFocusLost(arg0);
        }
    }
}
