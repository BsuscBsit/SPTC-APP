using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        private bool isDialog = false;
        private bool isThreeway = false;
        public int ResultingCode = -1;
        public ControlWindow(string header = "", string content = "", Icons icons = Icons.DEFAULT)
        {
            InitializeComponent();
            this.SetIcon(icons);
            this.lblHeader.Content = header;
            this.tblockContent.Text = content;
            DraggingHelper.DragWindow(topBar);
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
        }

        public ControlWindow(string header, string content, string middlebtn, string rightbtn, Icons icons = Icons.DEFAULT)
        {
            InitializeComponent();
            this.SetIcon(icons);
            this.lblHeader.Content = header;
            this.tblockContent.Text = content;
            btnOK.Content = rightbtn;
            btnMiddle.Content = middlebtn;
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
        }

        public static void ShowStatic(string header, string content, Icons icons = Icons.DEFAULT)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ControlWindow control = new ControlWindow();
                control.SetIcon(icons);
                control.lblHeader.Content = header;
                control.tblockContent.Text = content;
                control.ShowDialog();
            });
            //return control;
        }
        public static int ShowDialogStatic(string header, string content, string middlebtn, string rightbtn, Icons icons = Icons.DEFAULT)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                ControlWindow control = new ControlWindow(header, content, middlebtn, rightbtn, icons);
                control.Cancellable();
                control.Threeway();
                control.ShowDialog();
                return control.ResultingCode;
            });
        }

        

        public static bool ShowDialogStatic(string header, string content, Icons icons = Icons.DEFAULT)
        {
            bool? res = Application.Current.Dispatcher.Invoke(() =>
            {
                ControlWindow control = new ControlWindow(header, content, icons);
                control.Cancellable();
                return (control.ShowDialog());
            });
            return res ?? false;
        }

        private void Cancellable()
        {
            isDialog = true;
            btnCancel.Visibility = Visibility.Visible;
            btnOK.Width = 120;
        }

        private void Threeway()
        {
            isThreeway = true;
            btnMiddle.Visibility = Visibility.Visible;
            btnOK.Width = 120;
        }

        private void SetIcon(Icons icon)
        {
            if (icon != Icons.DEFAULT)
            {
                switch (icon)
                {
                    case Icons.NOTIFY:
                        Viewbox circleNotificationIcon = (Viewbox)this.FindResource("CircleInform");
                        messageicon.Content = circleNotificationIcon;
                        break;
                    case Icons.ERROR:
                        Viewbox OctagonErr = (Viewbox)this.FindResource("OctagonError");
                        messageicon.Content = OctagonErr;
                        break;
                    default: break;
                }
            }
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

            if(isThreeway && isDialog)
            {
                this.DialogResult = null;
                this.ResultingCode = -1;
            }
            else
            {
                this.DialogResult = false;
            }
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.isDialog)
            {
                this.DialogResult = true;
                this.ResultingCode = 1;
            }
            this.Close();
        }

        private void btnMiddle_Click(object sender, RoutedEventArgs e)
        {
            if (this.isDialog)
            {
                this.DialogResult = false;
                this.ResultingCode = 0;
            }
            this.Close();
        }

    }

    public enum Icons
    {
        DEFAULT,
        NOTIFY,
        ERROR,
        //ADD OTHER ICON
    }
}
