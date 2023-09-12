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
        public ControlWindow()
        {
            InitializeComponent();
        }

        public static ControlWindow Show(string header, string content, Icons icons = Icons.DEFAULT)
        {
            ControlWindow control = new ControlWindow();
            control.SetIcon(icons);
            control.lblHeader.Content = header;
            control.lblContent.Content = content;
            control.Show();
            return control;
        }
        public static ControlWindow ShowDialog(string header, string content, Icons icons = Icons.DEFAULT)
        {
            
            ControlWindow control = new ControlWindow();
            control.SetIcon(icons);
            control.lblHeader.Content = header;
            control.lblContent.Content = content;
            control.ShowDialog();
            return control;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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
