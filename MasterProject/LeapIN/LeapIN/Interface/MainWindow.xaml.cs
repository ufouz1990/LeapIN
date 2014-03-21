using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

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

            this.SourceInitialized += Window_SourceInitialized;
            this.Loaded += context.EnableController;
            this.Unloaded += context.DisableController;
            this.Closing += Window_Closing;
        }

        // Clean up the window data context
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.Closing -= Window_Closing;
            this.DataContext = null;
            GC.Collect();
        }

        void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            Win32Services.SetupWindow(handle);
        }
    }
}
