using Microsoft.Win32;
using SPTC_APP.View.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPTC_APP.View.IDGenerator.Extra
{
    /// <summary>
    /// Interaction logic for SignPad.xaml
    /// </summary>
    public partial class SignPad : Window
    {
        bool isMaximized = true;
        private Rect? actualDimensions;
        private const double windowWidth = 640;
        private const double windowHeight = 390.4;
        private Style roundedStyle;
        private Style normalStyle;

        public SignPad()
        {
            InitializeComponent();
            InitProps();
            MaximizeWindow();
        }

        private void InitProps()
        {
            actualDimensions = new Rect(
                (SystemParameters.PrimaryScreenWidth - windowWidth) / 2,
                (SystemParameters.PrimaryScreenHeight - windowHeight) / 2,
                windowWidth, windowHeight);

            roundedStyle = Application.Current.Resources["Minified"] as Style;
            normalStyle = Application.Current.Resources["Maximized"] as Style;
        }

        private void btnWindowResizer_Click(object sender, RoutedEventArgs e)
        {
            if (isMaximized)
            {
                borderTopBar.CornerRadius = new CornerRadius(12, 12, 0, 0);

                RestoreWindow();
                isMaximized = false;
            }
            else
            {
                borderTopBar.CornerRadius = new CornerRadius(0);
                MaximizeWindow();
                isMaximized = true;
            }
        }

        private void RestoreWindow()
        {
            // Restore the window to its original dimensions and style
            this.Left = actualDimensions.Value.Left;
            this.Top = actualDimensions.Value.Top;
            this.Width = actualDimensions.Value.Width;
            this.Height = actualDimensions.Value.Height;
            this.Style = roundedStyle;
        }

        private void MaximizeWindow()
        {
            // Maximize the window to the working area dimensions and style
            Rect workingArea = SystemParameters.WorkArea;
            this.Left = workingArea.Left;
            this.Top = workingArea.Top;
            this.Width = workingArea.Width;
            this.Height = workingArea.Height;
            this.Style = normalStyle;
        }

        private void btnSaveStroke_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearStrokes_Click(object sender, RoutedEventArgs e)
        {
            inkSign.Strokes.Clear();

            textblockNotice.FadeIn(0.2);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void canvasButtons_MouseEnter(object sender, MouseEventArgs e)
        {
            canvasButtons.AnimateMargin(new Thickness(0, 5, 0, 0), 0.3);
            stackpanel.AnimateMargin(new Thickness(0,0,0,0), 0.5);
        }

        private void canvasButtons_MouseLeave(object sender, MouseEventArgs e)
        {
            canvasButtons.AnimateMargin(new Thickness(0, -35, 0, 0), 0.3);
            stackpanel.AnimateMargin(new Thickness(0,0,0,30), 0.5);
        }

        private void inkSign_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            textblockNotice.FadeOut(0.2);
        }
    }

}
