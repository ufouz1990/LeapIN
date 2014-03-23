using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace LeapIN.Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainInterface context;

        public MainWindow()
        {
            InitializeComponent();

            // Set this windows datacontext
            context = new MainInterface();
            this.DataContext = context;

            this.SourceInitialized += Window_SourceInitialized;
            this.IsVisibleChanged += context.HandleController;
            this.Closing += Window_Closing;
        }

        // Clean up the window data context
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.Closing -= Window_Closing;

            if (this.Visibility == Visibility.Visible)
            {
                context.DestroyDevice();
            }

            this.DataContext = null;
            GC.Collect();
        }

        void Window_SourceInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            Win32Services.SetupWindow(helper);
        }
    }
}
