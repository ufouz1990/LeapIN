using System;
using System.Windows;

namespace LeapIN.ControlPanel
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        public ControlWindow()
        {
            InitializeComponent();

            // Set this windows datacontext
            ControlPanel context = new ControlPanel();
            this.DataContext = context;

            // When this window closes make sure the interface closes with it
            Closing += context.CloseApp;
        }
    }
}
