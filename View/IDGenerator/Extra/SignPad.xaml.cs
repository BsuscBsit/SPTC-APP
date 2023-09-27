using Microsoft.Win32;
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


        public RenderTargetBitmap signatureBitmapResult;

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
        }

        private void btnClearStrokes_Click(object sender, RoutedEventArgs e)
        {
            inkSign.Strokes.Clear();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
