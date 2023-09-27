using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages;
using Image = SPTC_APP.Objects.Image;
using SPTC_APP.View.IDGenerator.Extra;

namespace SPTC_APP.View
{
    public partial class Test : System.Windows.Window
    {

        public Test()
        {
            InitializeComponent();
            EventLogger.Post("VIEW :: TEST Window");
            tabControl.SelectionChanged += TabControl_SelectionChanged;
            imgSignatureo.Source = AppState.FetchChairmanSign();
        }

        //Test for VideoCamera
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            /*
            using (var capture = new VideoCapture(0))
            {
                Mat image = new Mat();
                while (true)
                {
                    capture.Read(image);
                    var bitmap = BitmapConverter.ToBitmap(image);
                    var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    imageRender.Source = bitmapSource;
                    Cv2.WaitKey(1);
                }
            }*/
        }

        //Test for Signature pad

        //Test for List
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                if(selectedTab.Header.ToString() == "Test VideoCapture Signature pad")
                {

                    
                }
                if (selectedTab.Header.ToString() == "Test Login")
                {
                    UpdateLogin();
                }
                if (selectedTab.Header.ToString() == "Test List")
                {
                    btnFranchise_Click(null, null);
                }
            }

        }

        private void OnStylusDown(object sender, StylusDownEventArgs e)
        {
            inkCanvas.CaptureStylus();

            DrawingAttributes drawingAttributes = new DrawingAttributes
            {
                Color = Colors.Black // You can set other properties like color, etc.
            };

            StylusPoint stylusPoint = e.GetStylusPoints(inkCanvas)[0];
            drawingAttributes.Width = stylusPoint.PressureFactor * 10; // Adjust as needed
            drawingAttributes.Height = stylusPoint.PressureFactor * 10; // Adjust as needed

            inkCanvas.DefaultDrawingAttributes = drawingAttributes;
        }

        private void OnStylusMove(object sender, StylusEventArgs e)
        {
            StylusPointCollection points = e.GetStylusPoints(inkCanvas);
            Stroke newStroke = new Stroke(points);
            inkCanvas.Strokes.Add(newStroke);
        }

        private void OnStylusUp(object sender, StylusEventArgs e)
        {
            inkCanvas.ReleaseStylusCapture();
        }


        private void UpdateLogin()
        {
            lblName.Content = $"My Name is {AppState.USER?.name.wholename}";
            lblAddress.Content = $"I am from {AppState.USER?.address}";
            lblPosition.Content = $"I am a {AppState.USER?.position}";
        }


        //SAMPLE ONLY, use DataTable on next Iterati

        /*foreach(Franchise f in fetchedData)
        {
            if(f.Driver_day != null)
            {
                (new GenerateID(f, true)).Show();
            }
        }*/

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private async void btnFranchise_Click(object sender, RoutedEventArgs e)
        {
            int batchSize = 5;
            int pageIndex = 0;

            

            DataGrid dataGrid = new DataGrid();

            DatagridList.Children.Add(dataGrid);
            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("BodyNumber", "###", width: 30),
                new ColumnConfiguration("LicenseNO", "Plate Number", width: 60, backgroundColor: Brushes.Yellow),
                new ColumnConfiguration("Operator", "Operator name"),
                new ColumnConfiguration("Driver", "Driver name", fontWeight: FontWeights.Black, width: 100, maxWidth: 150),
            };
            DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(dataGrid, columnConfigurations);


            while (true)
            {
                List<Franchise> batch = await Task.Run(() =>
                {
                    return (new TableObject<Franchise>(Table.FRANCHISE, Where.ALL_NOTDELETED, pageIndex * batchSize, batchSize)).data;
                });

                if (batch.Count == 0)
                    break;
                foreach (var obj in batch)
                {
                    dataGrid.Items.Add(obj);

                    await Task.Delay(200);
                }

                await Task.Delay(200);

                pageIndex++;
            }
        }

        private async void btnDriver_Click(object sender, RoutedEventArgs e)
        {

            int batchSize = 5;
            int pageIndex = 0;
            DataGrid dataGrid = new DataGrid();

            DatagridList.Children.Add(dataGrid);
            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("name", "FullName", width: 30),
                new ColumnConfiguration("birthday", "Date of Birth", width: 60, backgroundColor: Brushes.Yellow),
                new ColumnConfiguration("emergencyPerson", "Em Person"),
                new ColumnConfiguration("emergencyContact", "Em Contact", fontWeight: FontWeights.Black, width: 100, maxWidth: 150),
                new ColumnConfiguration("isDayShift", "Is Day Shift")
            };
            DataGridHelper<Driver> dataGridHelper = new DataGridHelper<Driver>(dataGrid, columnConfigurations);


            while (true)
            {
                List<Driver> batch = await Task.Run(() =>
                {
                    return (new TableObject<Driver>(Table.DRIVER, Where.ALL_NOTDELETED, pageIndex * batchSize, batchSize)).data;
                });

                if (batch.Count == 0)
                    break;
                foreach (var obj in batch)
                {
                    dataGrid.Items.Add(obj);

                    await Task.Delay(200);
                }

                await Task.Delay(200);

                pageIndex++;
            }
        }

        private async void btnOperator_Click(object sender, RoutedEventArgs e)
        {

            int batchSize = 5;
            int pageIndex = 0;
            DataGrid dataGrid = new DataGrid();

            DatagridList.Children.Add(dataGrid);

            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("name", "FullName", width: 30),
                new ColumnConfiguration("birthday", "Date of Birth", width: 60, backgroundColor: Brushes.Yellow),
                new ColumnConfiguration("emergencyPerson", "Em Person"),
                new ColumnConfiguration("emergencyContact", "Em Contact", fontWeight: FontWeights.Black, width: 100, maxWidth: 150),
            };
            DataGridHelper<Operator> dataGridHelper = new DataGridHelper<Operator>(dataGrid, columnConfigurations);

            while (true)
            {
                List<Operator> batch = await Task.Run(() =>
                {
                    return (new TableObject<Operator>(Table.OPERATOR, Where.ALL_NOTDELETED, pageIndex * batchSize, batchSize)).data;
                });

                if (batch.Count == 0)
                    break;
                foreach (var obj in batch)
                {
                    dataGrid.Items.Add(obj);
                    await Task.Delay(200);
                }

                await Task.Delay(200);

                pageIndex++;
            }
        }

        private async void btnEmployee_Click(object sender, RoutedEventArgs e)
        {

            int batchSize = 5;
            int pageIndex = 0;
            DataGrid dataGrid = new DataGrid();

            DatagridList.Children.Add(dataGrid);
            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("name", "FullName", width: 30),
                new ColumnConfiguration("position", "Position", width: 60, backgroundColor: Brushes.Yellow)
            };
            DataGridHelper<Employee> dataGridHelper = new DataGridHelper<Employee>(dataGrid, columnConfigurations);


            while (true)
            {
                List<Employee> batch = await Task.Run(() =>
                {
                    return (new TableObject<Employee>(Table.EMPLOYEE, Where.ALL_NOTDELETED, pageIndex * batchSize, batchSize)).data;
                });

                if (batch.Count == 0)
                    break;
                foreach (var obj in batch)
                {
                    dataGrid.Items.Add(obj);
                    await Task.Delay(200);
                }

                await Task.Delay(200);

                pageIndex++;
            }
        }


        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            MainBody body = (new MainBody());
            AppState.mainwindow = body;
            body.Show();
        }

        private void btnSaveSign_Click(object sender, RoutedEventArgs e)
        {
            // Set the background of the inkCanvas to transparent
            inkCanvas.Background = Brushes.Transparent;

            // Create a new StrokeCollection to store only the signature strokes
            StrokeCollection signatureStrokes = new StrokeCollection();

            // Iterate through each stroke in the inkCanvas
            foreach (Stroke stroke in inkCanvas.Strokes)
            {
                // Check if the stroke intersects with the bounds of the signature area
                if (stroke.GetBounds().IntersectsWith(new Rect(0, 0, inkCanvas.ActualWidth, inkCanvas.ActualHeight)))
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

                // Create a BitmapEncoder and save the RenderTargetBitmap as an image
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                byte[] signatureBytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    signatureBytes = stream.ToArray();
                }

                Employee employee = Retrieve.GetDataUsingQuery<Employee>(RequestQuery.GET_CURRENT_CHAIRMAN).FirstOrDefault();
                if(employee != null)
                {
                    if (employee.sign != null)
                        employee.sign.picture = signatureBytes;
                    else
                    {
                        employee.sign = new Objects.Image();
                        employee.sign.name = $"Signature - {employee.position.ToString()}";
                        employee.sign.picture = signatureBytes;
                    }
                    employee.Save();
                }
                
            }

            // Reset the inkCanvas by clearing the strokes and setting the background to white
            inkCanvas.Strokes.Clear();
            inkCanvas.Background = Brushes.White;

            imgSignatureo.Source = AppState.FetchChairmanSign();
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            if ((new Clean(RequestQuery.CLEAN_ADDRESS)).Start())
            {
                btnClean.Background = Brushes.Green;
            } else
            {
                btnClean.Background = Brushes.Red;
            }

            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SignPad sp = new SignPad();
            sp.Show();
        }



        //PLANS ON EDITING FIELDS
        //first create a class that modifies
        //then attach a 
    }
}