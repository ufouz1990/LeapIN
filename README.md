LeapIN
======

Leap Motion basic interface for the disabled, requires use of only a single finger or tool for input.

Current Features
--

- Transparent window with green border to show window (will be removed at release)
- Mouse cursor can be moved.
- Mouse clicks are always handled now due to the Leap Service running in the background at start. To offer greater control over the application the listener which handles these events is turned on and off based on the activity (whether it is hidden or not) of the overlay window. This prevents the confusion of the pointer still moving when the overlay is disabled.
- A small control panel window used to start or hide (disable) the service. Settings items will be added here so the overlay can be customised before or during use. Depending on the needs, some of these settings will be housed in the overlay window instead.
- Handling for application exit. The overlay is hidden rather than closed when it is disabled, however if it does get closed (Alt-F4), then it can be reopened. If the control panel attempts to close when the overlay has been started it prompts the user that the overlay will close as well, this currently happens even if it is hidden, but that will probably be altered.

Upcoming
--

- Keyboard by default on one side of the screen. When the user types something it will be displayed on the overlay in case the keyboard happens to be covering up the text box or where they are typing. This also enables larger text to be used when typing.
- Additional buttons to change the click mode from single to double, right and text selection/click and hold.
- Magnification Area, this uses the win32 Magnification.dll. Another possible development would be to integrate a more precise movement mode when inside the magnified area to achieve greater control. Example [HERE](http://www.stareat.it/sp.aspx?g=9ce0bbee351b4f5a988b23593ba93e9d).
- Better support for pointers, increased accuracy, decent theme etc.


