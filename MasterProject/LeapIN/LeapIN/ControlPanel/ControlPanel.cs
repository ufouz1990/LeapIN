using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using LeapIN.Extras;
using LeapIN.Interface;


namespace LeapIN.ControlPanel
{
    class ControlPanel : PropertyChange
    {
        MainWindow app;
        ICommand _changeState;

        double s, es, sp;

        public ControlPanel()
        {
            Sensitivity = 2.0d;
            ExitSensitivity = 6.0d;
            Speed = 4.0d;
        }

        public double Sensitivity
        {
            get { return s; }
            set
            {
                s = value;
                OnPropertyChanged("Sensitivity");
            }
        }

        public double ExitSensitivity
        {
            get { return es; }
            set
            {
                es = value;
                OnPropertyChanged("ExitSensitivity");
            }
        }

        public double Speed
        {
            get { return sp; }
            set
            {
                sp = value;
                OnPropertyChanged("Speed");
            }
        }

        public ICommand ChangeState
        {
            get
            {
                if (_changeState == null)
                {
                    _changeState = new RelayCommand(
                        param => ChangeInterface());
                }
                return _changeState;
            }
        }

        /// <summary>
        /// The interface is created here, hidden and disabled when not needed and quickly resumes when it is.
        /// </summary>
        void ChangeInterface()
        {
            // If the interface window doesn't exist yet or got closed by the user
            if (app == null)
            {
                app = new MainWindow();
                app.Closed += SetNull;
            }

            // Show or hide the window
            if (!app.IsVisible)
            {
                // get the window datacontext
                MainInterface con = app.DataContext as MainInterface;

                con.Mouse.AlterSettings(s, es, sp); // Mouse update

                app.Show();
            }
            else if (app.IsVisible)
            {
                app.Hide();
            }
        }

        // If the control panel is closed but the interface is still running close it - After a messagebox check!
        public void CloseApp(object sender, CancelEventArgs e)
        {
            MessageBoxResult result;

            // If the interface Closed event hasn't fired (it is still running)
            if (app != null)
            {
                result = MessageBox.Show("Closing the control panel closes the interface.\nAre you sure you want to exit?", "Exit Application", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    app.Close();
                }
                else if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        void SetNull(object sender, EventArgs e)
        {
            app = null;
        }
    }
}
