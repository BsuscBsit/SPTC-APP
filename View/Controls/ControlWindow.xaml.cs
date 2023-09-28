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
        public ControlWindow(string header = "", string content = "", Icons icons = Icons.DEFAULT)
        {
            InitializeComponent();
            this.SetIcon(icons);
            this.lblHeader.Content = header;
            this.tblockContent.Text = content;
            Activated += (sender, e) => { AppState.WindowsCounter(true); };
            Closing += (sender, e) => { AppState.WindowsCounter(false); };
        }

        public static void Show(string header, string content, Icons icons = Icons.DEFAULT)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ControlWindow control = new ControlWindow();
                control.SetIcon(icons);
                control.lblHeader.Content = header;
                control.tblockContent.Text = content;
                control.Show();
            });
            //return control;
        }
        public static bool ShowDialog(string header, string content, Icons icons = Icons.DEFAULT)
        {
            bool? res = Application.Current.Dispatcher.Invoke(() =>
            {
                ControlWindow control = new ControlWindow();
                control.SetIcon(icons);
                control.lblHeader.Content = header;
                control.tblockContent.Text = content;
                control.Cancellable();
                return (control.ShowDialog());
            });
            return res??false;
        }

        private void Cancellable()
        {
            //Set cancel visibility here
            isDialog = true;
            btnCancel.Visibility = Visibility.Visible;
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

            this.DialogResult = false;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.isDialog)
            {
                this.DialogResult = true;
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
