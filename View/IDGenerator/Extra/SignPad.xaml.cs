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

        public RenderTargetBitmap signatureBitmapResult;

        public SignPad()
        {
            InitializeComponent();
        }

        private void btnWindowResizer_Click(object sender, RoutedEventArgs e)
        {
            if (isMaximized)
            {
                DraggingHelper.DragWindow(borderTopBar);
                borderTopBar.CornerRadius = new CornerRadius(12, 12, 0, 0);
                wsh.ResizeTo(WindowStateHelper.WindowPosition.CENTER, windowWidth, windowHeight);
                isMaximized = false;
            }
            else
            {
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

                // Create a new StrokeCollection to store only the signature strokes
                StrokeCollection signatureStrokes = new StrokeCollection();

                // Iterate through each stroke in the inkCanvas
                foreach (Stroke stroke in inkSign.Strokes)
                {
                    // Check if the stroke intersects with the bounds of the signature area
                    if (stroke.GetBounds().IntersectsWith(new Rect(0, 0, inkSign.ActualWidth, inkSign.ActualHeight)))
                    {
                        // Add the stroke to the signatureStrokes collection
                        signatureStrokes.Add(stroke);
                    }
                }

                // Create a new inkCanvas to render only the signature strokes
                InkCanvas signatureCanvas = new InkCanvas();
                signatureCanvas.Strokes = signatureStrokes;

                // Get the bounds of the signature strokes
                Rect signatureBounds = signatureCanvas.Strokes.GetBounds();

                // Check if the signature bounds have a non-zero width and height
                if (signatureBounds.Width > 0 && signatureBounds.Height > 0)
                {
                    // Create a new inkCanvas with the size of the signature bounds
                    InkCanvas croppedCanvas = new InkCanvas();
                    croppedCanvas.Background = Brushes.Transparent;
                    croppedCanvas.Width = signatureBounds.Width;
                    croppedCanvas.Height = signatureBounds.Height;

                    // Translate the signature strokes to fit within the cropped canvas
                    Matrix translationMatrix = new Matrix();
                    translationMatrix.Translate(-signatureBounds.Left, -signatureBounds.Top);
                    signatureCanvas.Strokes.Transform(translationMatrix, false);

                    // Copy the signature strokes to the cropped canvas
                    foreach (Stroke stroke in signatureCanvas.Strokes)
                    {
                        croppedCanvas.Strokes.Add(stroke.Clone());
                    }

                    // Render the cropped canvas to a RenderTargetBitmap
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
        }
    }

}
