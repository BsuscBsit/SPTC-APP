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
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SPTC_APP.View.Controls;

namespace SPTC_APP.View.IDGenerator.Extra
{
    /// <summary>
    /// Interaction logic for SignPad.xaml
    /// </summary>
    public partial class SignPad : Window
    {
        bool isMaximized = true;
        bool noticeHasChanged = false;
        private const double windowWidth = 640;
        private const double windowHeight = 390.4;
        private WindowStateHelper wsh;

        private const int minheight = 50, minwidth=100;
        public RenderTargetBitmap signatureBitmapResult;

        public SignPad()
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
        }

        private void btnWindowResizer_Click(object sender, RoutedEventArgs e)
        {
            if (isMaximized)
            {
                resizerContent.Content = this.FindResource("Expand");
                DraggingHelper.DragWindow(borderTopBar);
                borderTopBar.CornerRadius = new CornerRadius(12, 12, 0, 0);
                wsh.ResizeTo(WindowStateHelper.WindowPosition.CENTER, windowWidth, windowHeight);
                isMaximized = false;
            }
            else
            {
                resizerContent.Content = this.FindResource("Shrink");
                DraggingHelper.DisableDragWindow(borderTopBar);
                borderTopBar.CornerRadius = new CornerRadius(0);
                wsh.MaximizeWindow(false);
                isMaximized = true;
            }
        }

        private async void btnSaveStroke_Click(object sender, RoutedEventArgs e)
        {
            if (inkSign.Strokes.Count > 0)
            {
                inkSign.Background = Brushes.Transparent;

                StrokeCollection signatureStrokes = new StrokeCollection();

                foreach (Stroke stroke in inkSign.Strokes)
                {
                    if (stroke.GetBounds().IntersectsWith(new Rect(0, 0, inkSign.ActualWidth, inkSign.ActualHeight)))
                    {
                        signatureStrokes.Add(stroke);
                    }
                }

                InkCanvas signatureCanvas = new InkCanvas();
                signatureCanvas.Strokes = signatureStrokes;

                Rect signatureBounds = signatureCanvas.Strokes.GetBounds();

                if (signatureBounds.Width > 0 && signatureBounds.Height > 0)
                {
                    InkCanvas croppedCanvas = new InkCanvas();
                    croppedCanvas.Background = Brushes.Transparent;
                    croppedCanvas.MinHeight = minheight;
                    croppedCanvas.MinWidth = minwidth;
                    croppedCanvas.Width = (signatureBounds.Width > minwidth)? signatureBounds.Width: minwidth;
                    croppedCanvas.Height = (signatureBounds.Height > minheight)? signatureBounds.Height: minheight;

                    Matrix translationMatrix = new Matrix();
                    translationMatrix.Translate(-signatureBounds.Left, -signatureBounds.Top);
                    signatureCanvas.Strokes.Transform(translationMatrix, false);

                    foreach (Stroke stroke in signatureCanvas.Strokes)
                    {
                        croppedCanvas.Strokes.Add(stroke.Clone());
                    }

                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)croppedCanvas.Width, (int)croppedCanvas.Height, 96, 96, PixelFormats.Default);
                    rtb.Render(croppedCanvas);
                    signatureBitmapResult = rtb;


                    DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                textblockNotice.FadeIn(0.2);
                textblockNotice.Text = "Please draw your signature before saving.";
                await Task.Delay(TimeSpan.FromSeconds(2));
                if(inkSign.Strokes.Count < 1)
                {
                    textblockNotice.FadeIn(0.2);
                    textblockNotice.Text = "Use your stylus to draw your signature inside the box. Keep your signature within the provided area.";
                }
            }
        }

        private void btnClearStrokes_Click(object sender, RoutedEventArgs e)
        {
            inkSign.Strokes.Clear();
            if (noticeHasChanged)
            {
                textblockNotice.Text = "Use your stylus to draw your signature inside the box. Keep your signature within the provided area.";
            }
            textblockNotice.FadeIn(0.2);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void canvasButtons_MouseEnter(object sender, MouseEventArgs e)
        {
            borderOfButtons.AnimateMargin(new Thickness(0, 5, 0, 0), 0.3);
            stackpanel.AnimateMargin(new Thickness(0,0,0,0), 0.5);
        }

        private void canvasButtons_MouseLeave(object sender, MouseEventArgs e)
        {
            borderOfButtons.AnimateMargin(new Thickness(0, -35, 0, 0), 0.3);
            stackpanel.AnimateMargin(new Thickness(0,0,0,30), 0.5);
        }

        private void inkSign_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            textblockNotice.FadeOut(0.2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            wsh = new WindowStateHelper(this);
            wsh.MaximizeWindow(false);
            resizerContent.Content = this.FindResource("Shrink");
        }
    }

}
