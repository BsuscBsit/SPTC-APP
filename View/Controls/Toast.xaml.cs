using SPTC_APP.View.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPTC_APP.View.Controls
{
    /// <summary>
    /// Interaction logic for Toast.xaml
    /// </summary>
    public partial class Toast : Window
    {
        public Toast(Grid targetGrid, string msg, double persistDuration = 1.5, bool doBeep = true, double animationDuration = 0.2)
        {
            if (doBeep)
            {
                System.Media.SystemSounds.Beep.Play();
            }
            InitializeComponent();
            message.Text = msg;

            parent.Children.Remove(toast);
            targetGrid.Children.Add(toast);

            toast.FadeIn(animationDuration, async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(persistDuration));
                toast.FadeOut(animationDuration, () =>
                {
                    targetGrid.Children.Remove(toast);
                });
            });
        }
    }
}
