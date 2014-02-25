using System;
using System.Windows;
using LeapIN.ControlPanel;

namespace LeapIN
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Entry point for the application
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create the control panel and show it
            ControlWindow app = new ControlWindow();
            app.Show();
        }
    }
}
