using System;
using System.ComponentModel;
using System.Windows;

namespace LeapIN.Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Win32Services.Begin(this);
            InitializeComponent();

            // Set this windows datacontext
            MainInterface context = new MainInterface();
            this.DataContext = context;

            this.Loaded += context.EnableListener;
            this.Unloaded += context.DisableListener;
            this.Closing += Window_Closing;
        }

        // Clean up the window data context
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.Closing -= Window_Closing;
            this.DataContext = null;
            GC.Collect();
        }
    }
}
