using SPTC_APP.View.Styling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SPTC_APP.View.Controls
{
    /// <summary>
    /// Interaction logic for Toast.xaml
    /// </summary>
    public partial class Toast : Window
    {
        public Toast(
            Grid targetGrid,
            string msg,
            double persistDuration = 1.5,
            bool doBeep = true,
            double animationDuration = 0.2,
            System.Windows.Media.Brush fg = null,
            System.Windows.Media.Brush bg = null)
        {
            if(!(string.IsNullOrEmpty(msg) || string.IsNullOrWhiteSpace(msg)))
            {
                InitializeComponent();
                if (doBeep)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                }
                message.Text = msg;
                message.Foreground = fg ?? FindResource("BrushBlack") as System.Windows.Media.Brush;
                border.Background = bg ?? System.Windows.Media.Brushes.White;

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
}
