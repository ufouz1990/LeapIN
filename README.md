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

#### UPDATE 19th March
---

- A keyboard now shows on one side of the screen. Currently it does not work, no commands have been hooked up yet.
- Buttons along the bottom of the screen to change between various mouse modes (Left, Right, Double, Drag, Scolling)
- Mouse events are handled using a hover system, keeping the pointer stable within a threshold allows you to perform a click. Required steadiness can be change (better for the disabled!). Also there is an exit threshold to prevent accidental extra mouse events if hovering in the same location for too long. This can be exploited to enhance the scrolling function, higher exit thresh means you can continuously scroll without needing to be super steady.
- An on screen virtual mouse to show you when a click has occurred to improve the experience. Currently all clicks are shown as a left click, extra properties and multi-triggers will be needed to differentiate the modes.

Upcoming
--

- Keyboard by default on one side of the screen. When the user types something it will be displayed on the overlay in case the keyboard happens to be covering up the text box or where they are typing. This also enables larger text to be used when typing. A word is output when a space is reached to reduce the amount of app switching. This also means errors can be corrected in advance.
- Magnification Area, this uses the win32 Magnification.dll. Another possible development would be to integrate a more precise movement mode when inside the magnified area to achieve greater control. Example [HERE](http://www.stareat.it/sp.aspx?g=9ce0bbee351b4f5a988b23593ba93e9d).
- Better theme.
- Control Panel options, layouts (too much work for a prototype?)

Issues
--

- Possible Windows 7 Garbage Collection error (Not confirmed), services that stop running on close cause the app to enter a 'stopped working' state. Issue not present on 8 or 8.1 (My PC)

