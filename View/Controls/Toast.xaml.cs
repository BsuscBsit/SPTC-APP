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
        public Toast(Grid targetGrid, string message, double animationDuration = 0.3, double persistDuration = 2)
        {
            InitializeComponent();
            lblContent.Content = message;
            parent.Children.Remove(gridToast);
            targetGrid.Children.Add(gridToast);

            gridToast.FadeIn(animationDuration, async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(persistDuration));
                gridToast.FadeOut(animationDuration, () =>
                {
                    targetGrid.Children.Remove(gridToast);
                });
            });
        }
    }
}
