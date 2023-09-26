﻿using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using SPTC_APP.Objects;
using SPTC_APP.View.Styling;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for GenerateID.xaml
    /// </summary>
    public partial class GenerateID : Window
    {
        BitmapSource lastCapturedImage = null;

        bool hasPhoto = false;
        bool hasSign = false;


        private bool isCameraRunning;
        private bool isSignPadRunning;

        bool isDriver = true;
        private Franchise franchise;
        bool isUpdate = false;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        private bool hasInkBeenAdded = false;

        public GenerateID()
        {
            InitializeComponent();
            AppState.mainwindow?.Hide();
            tboxAddressS.Text = AppState.DEFAULT_ADDRESSLINE2;
            bDay.SelectedDate = DateTime.Today;
            EventLogger.Post("VIEW :: ID GENERATE Window");
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            isUpdate = false;
            franchise = new Franchise();
            if (videoDevices.Count == 0)
            {
                EventLogger.Post("ERR :: No video devices found.");
                btnStartCam.IsEnabled = false;
                return;
            }
            videoSource = new VideoCaptureDevice(videoDevices[AppState.DEFAULT_CAMERA].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);

            if(!AppState.isDeployment && !AppState.isDeployment_IDGeneration) GenerateDummy();
            inkSign.StrokeCollected += inkSign_StrokeCollected;
        }

        public GenerateID(Franchise franchise, bool isDriver)
        {
            InitializeComponent();
            AppState.mainwindow?.Hide();
            bDay.SelectedDate = DateTime.Today;
            this.franchise = franchise;
            isUpdate = true;
            EventLogger.Post("VIEW :: ID GENERATE Window id="+franchise.id);
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            btnStartPad.IsEnabled = false;
            
            this.isDriver = isDriver;
            MySwitch.Visibility = Visibility.Hidden;
            if (isDriver)
            {
                drvOrOprt.Content = "DRIVER";
                lblPhoto.Content = "Driver's Photo";
                lblsign.Content = "Driver's Signature";
                if (franchise.Driver != null)
                {
                    var drv = franchise.Driver;
                    if(drv.name != null)
                    {
                        cbGender.SelectedIndex = ((drv.name.prefix == "Mrs.") ? 1 : 0);
                        tboxFn.Text = drv.name.firstname;
                        tboxMn.Text = drv.name.middlename;
                        tboxLn.Text = drv.name.lastname;
                        //ADD suffix for Jr. Sr etc...
                    }
                    if(drv.address != null)
                    {
                        tboxAddressB.Text = drv.address.addressline1;
                        tboxAddressS.Text = drv.address.addressline2;
                    }
                    if(drv.image != null)
                    {
                        imgIDPic.Source = drv.image.GetSource();
                        hasPhoto = true;
                    }
                    if (drv.signature != null)
                    {
                        imgSignPic.Source = drv.signature.GetSource();
                        hasSign = true;
                    }
                    if(drv.birthday != null)
                    {
                        bDay.DisplayDate = drv.birthday;
                        bDay.DataContext = drv.birthday;
                        bDay.Text = drv.birthday.ToString();
                    }
                    tboxEmePer.Text = drv.emergencyPerson;
                    tboxPhone.Text = drv.emergencyContact;
                    tboxBnum.Text = franchise.BodyNumber;
                    tboxLnum.Text = franchise.LicenseNO;
                }
            } else
            {
                drvOrOprt.Content = "OPERATOR";
                lblPhoto.Content = "Operator's Photo";
                lblsign.Content = "Operator's Signature";
                if (franchise.Operator != null)
                {
                    var drv = franchise.Operator;
                    if (drv.name != null)
                    {
                        cbGender.SelectedIndex = ((drv.name.prefix == "Mrs.") ? 1 : 0);
                        tboxFn.Text = drv.name.firstname;
                        tboxMn.Text = drv.name.middlename;
                        tboxLn.Text = drv.name.lastname;
                        //ADD suffix for Jr. Sr etc...
                    }
                    if (drv.address != null)
                    {
                        tboxAddressB.Text = drv.address.addressline1;
                        tboxAddressS.Text = drv.address.addressline2;
                    }
                    if (drv.image != null)
                    {
                        imgIDPic.Source = drv.image.GetSource();
                        hasPhoto = true;
                    }
                    if (drv.signature != null)
                    {
                        imgSignPic.Source = drv.signature.GetSource();
                        hasSign = true;
                    }
                    if (drv.birthday != null)
                    {
                        bDay.DisplayDate = drv.birthday;
                        bDay.DataContext = drv.birthday;
                        bDay.Text = drv.birthday.ToString();
                    }
                    tboxEmePer.Text = drv.emergencyPerson;
                    tboxPhone.Text = drv.emergencyContact;
                    tboxBnum.Text = franchise.BodyNumber;
                    tboxLnum.Text = franchise.LicenseNO;
                }
            }

            if (videoDevices.Count == 0)
            {
                EventLogger.Post("ERR :: No video devices found.");
                btnStartCam.IsEnabled = false;
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            this.StopCamera();
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.StopCamera();
            PrintPreview print = new PrintPreview();
            print.Show();
            this.Close();
        }

        private void ProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            pbCameraOpen.Value = pbCameraOpen.Minimum;
        }

        private void MySwitch_Back(object sender, RoutedEventArgs e)
        {
                // Switch is in the True state
                isDriver = false;
                drvOrOprt.Content = "Create this ID for\nOperator.";
            
        }

        private void MySwitch_Front(object sender, RoutedEventArgs e)
        {
                // Switch is in the False state
                isDriver = true;
                drvOrOprt.Content = "Create this ID for\nDriver.";
            
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            isCameraRunning = true;
            if (isCameraRunning)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // This event is called for each new camera frame
                    // Convert the frame to a BitmapSource and display it
                    System.Drawing.Bitmap frame = (System.Drawing.Bitmap)eventArgs.Frame.Clone();

                    // Convert to BitmapSource
                    var hBitmap = frame.GetHbitmap();
                    BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    // Display the captured frame in imgIDPic
                    imgIDPic.Source = bitmapSource;
                    lastCapturedImage = bitmapSource;

                    // Dispose of the frame to release resources
                    frame.Dispose();

                    // Freeze the BitmapSource to release its resources
                    bitmapSource.Freeze();
                    // Release the HBitmap
                    DeleteObject(hBitmap);
                });
            }
        }


        // Define the DeleteObject method to release HBitmap
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);
        private async void BtnStartCam_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource != null)
            {
                if (!videoSource.IsRunning)
                {
                    try
                    {
                        hasPhoto = false;
                        btnStartCam.IsEnabled = false;
                        ((System.Windows.Controls.Button)sender).Content = "Capture Image";
                        pbCameraOpen.Visibility = Visibility.Visible;
                        pbCameraOpen.Value = pbCameraOpen.Minimum;
                        pbCameraOpen.IsIndeterminate = true;
                        videoSource.Start();

                        //Short delay. nag lalag/crash minsan pag binigla eh.
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        btnStartCam.IsEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        ControlWindow.Show("Camera Error", $"An error occurred: {ex.Message}", Icons.ERROR);
                        pbCameraOpen.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    if (lastCapturedImage != null)
                    {
                        imgIDPic.Source = lastCapturedImage;
                    }
                    hasPhoto = true;

                    this.StopCamera();
                    pbCameraOpen.Value = 100;
                    pbCameraOpen.IsIndeterminate = false;
                }
            }
        }

        private void btnStartPad_Click(object sender, RoutedEventArgs e)
        {
            if (!isSignPadRunning)
            {
                pbSignOpen.Visibility = Visibility.Visible;
                pbSignOpen.IsIndeterminate = true;
                imgSignPic.Visibility = Visibility.Collapsed;
                inkSign.Visibility = Visibility.Visible;
                btnStartPad.Content = "Cancel";
                inkSign.IsEnabled = true;
                isSignPadRunning = true;
                hasSign = false;
            }
            else
            {

                if (hasInkBeenAdded)
                {
                    pbSignOpen.IsIndeterminate = false;
                    pbSignOpen.Value = 100;

                    imgSignPic.Visibility = Visibility.Visible;
                    inkSign.Background = Brushes.Transparent;
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)inkSign.ActualWidth, (int)inkSign.ActualHeight, 96, 96, PixelFormats.Default);
                    rtb.Render(inkSign);

                    imgSignPic.Source = rtb;
                    inkSign.Visibility = Visibility.Collapsed;
                    isSignPadRunning = false;
                    inkSign.IsEnabled = false;
                    inkSign.Strokes.Clear();
                    inkSign.Background = Brushes.White;


                    hasInkBeenAdded = false;
                    btnStartPad.Content = "Start Pad";
                    btnClearInkCanvas.FadeOut(0.2);
                    hasSign = true;
                } else
                {
                    pbSignOpen.IsIndeterminate = true;
                    pbSignOpen.Visibility = Visibility.Hidden;

                    inkSign.Visibility = Visibility.Hidden;
                    imgSignPic.Visibility = Visibility.Visible;
                    imgSignPic.Source = null;
                    isSignPadRunning = false;
                    inkSign.IsEnabled = false;
                    inkSign.Strokes.Clear();
                    btnStartPad.Content = "Start Pad";
                    hasSign = false;
                }
                
            }
        }

        private void OnStylusDown(object sender, StylusDownEventArgs e)
        {
            inkSign.CaptureStylus();

            DrawingAttributes drawingAttributes = new DrawingAttributes
            {
                Color = Colors.Black // You can set other properties like color, etc.
            };

            StylusPoint stylusPoint = e.GetStylusPoints(inkSign)[0];
            drawingAttributes.Width = stylusPoint.PressureFactor * 10; // Adjust as needed
            drawingAttributes.Height = stylusPoint.PressureFactor * 10; // Adjust as needed

            inkSign.DefaultDrawingAttributes = drawingAttributes;
        }

        private void OnStylusMove(object sender, StylusEventArgs e)
        {
            StylusPointCollection points = e.GetStylusPoints(inkSign);
            Stroke newStroke = new Stroke(points);
            inkSign.Strokes.Add(newStroke);
        }

        private void OnStylusUp(object sender, StylusEventArgs e)
        {
            inkSign.ReleaseStylusCapture();
        }

        private void btnBrowseIDPic_Click(object sender, RoutedEventArgs e)
        {
            this.StopCamera();
            var openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(selectedFilePath);
                bitmapImage.EndInit();

                imgIDPic.Source = bitmapImage;
                hasPhoto = true;
            }
        }

        private void btnBrowseSignPic_Click(object sender, RoutedEventArgs e)
        {
            pbSignOpen.Visibility = Visibility.Visible;
            var openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(selectedFilePath);
                bitmapImage.EndInit();

                imgSignPic.Source = bitmapImage;
                hasSign = true;
            }
        }
       
        private bool Preview_WarnDone(RoutedEventArgs e)
        {
            string warn = "";
            bool cameraWarning = isCameraRunning && !hasPhoto;
            bool signPadWarning = isSignPadRunning && !hasSign;

            if (cameraWarning)
            {
                this.StopCamera();
                imgIDPic.Source = null;
            }

            if (signPadWarning)
            {
                isSignPadRunning = false;
                imgSignPic.Source = null;
                inkSign.Visibility = Visibility.Hidden;
                btnStartPad.Content = "Start Pad";
            }

            if (cameraWarning || signPadWarning)
            {
                warn = (cameraWarning ? "Camera" : "") + (cameraWarning && signPadWarning ? " and " : "") + (signPadWarning ? "Sign Pad" : "");
                //warn += " has been initiated but no " + (warn.Length == 6 ? "image was" : "") + " captured. Are you certain you want to proceed?";
                warn = !string.IsNullOrEmpty(warn) ? (warn + " has been initiated but, no" + (warn.Length < 7 ? " image was" : (warn.Length < 10 ? " input was" : " inputs were")) + " captured. Are you certain you want to proceed?") : warn;
                if(ControlWindow.ShowDialog("Continue?", warn, Icons.NOTIFY))
                {
                    return true;
                }
                else
                {
                    if (cameraWarning)
                    {
                        BtnStartCam_Click(btnStartCam, e);
                        return false;
                    }

                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (Preview_WarnDone(e))
            {
                this.StopCamera();
                pbSignOpen.Visibility = Visibility.Hidden;
                if (tboxLnum.Text.Length > 0 && tboxFn.Text.Length > 0 && tboxLn.Text.Length > 0 && tboxAddressB.Text.Length > 0 && tboxAddressS.Text.Length > 0 && tboxEmePer.Text.Length > 0 && tboxPhone.Text.Length > 0)
                {
                    GeneratedIDPreview print = new GeneratedIDPreview();
                    print.ReturnControl(this);
                    if (isDriver)
                    {
                        Driver @obj = new Driver();
                        SPTC_APP.Objects.Image image = null;
                        SPTC_APP.Objects.Image sign = null;
                        if (hasPhoto)
                        {
                            image = new SPTC_APP.Objects.Image(imgIDPic.Source, $"Drv - {tboxFn.Text}");
                        }
                        else
                        {
                            imgIDPic.Source = null;
                        }
                        if (hasSign)
                        {
                            sign = new SPTC_APP.Objects.Image(imgSignPic.Source, $"Sign - {tboxFn.Text}");
                        }
                        else
                        {
                            imgSignPic.Source = null;
                        }

                        string prefix = (cbGender.SelectedIndex == 0) ? "Mr." : "Mrs.";
                        if (isUpdate)
                        {
                            @obj = franchise.Driver;
                            Name name = @obj.name;
                            name.prefix = prefix;
                            name.firstname = tboxFn.Text;
                            name.middlename = tboxMn.Text;
                            name.lastname = tboxLn.Text;
                            Address address = @obj.address;
                            address.addressline1 = tboxAddressB.Text;
                            address.addressline2 = tboxAddressS.Text;
                            @obj.WriteInto(name, address, image, sign, "", (DateTime)bDay.SelectedDate, tboxEmePer.Text, tboxPhone.Text);
                        }
                        else
                        {
                            Name name = new Name(prefix, tboxFn.Text, tboxMn.Text, tboxLn.Text, "");
                            Address address = new Address(tboxAddressB.Text, tboxAddressS.Text);
                            @obj.WriteInto(name, address, image, sign, "", (DateTime)bDay.SelectedDate, tboxEmePer.Text, tboxPhone.Text);
                        }

                        franchise.WriteInto(tboxBnum.Text, null, @obj, tboxLnum.Text);

                        ID id = new ID(franchise, Objects.General.DRIVER);
                        print.Save(id);
                        print.Show();
                        this.Hide();
                    }
                    else
                    {
                        Operator @obj = new Operator();
                        SPTC_APP.Objects.Image image = null;
                        SPTC_APP.Objects.Image sign = null;
                        if (hasPhoto)
                        {
                            image = new SPTC_APP.Objects.Image(imgIDPic.Source, $"Oprt - {tboxFn.Text}");
                        }
                        else
                        {
                            imgIDPic.Source = null;
                        }
                        if (hasSign)
                        {
                            sign = new SPTC_APP.Objects.Image(imgSignPic.Source, $"Sign - {tboxFn.Text}");
                        }
                        else
                        {
                            imgSignPic.Source = null;
                        }
                        string prefix = (cbGender.SelectedIndex == 0) ? "Mr." : "Mrs.";
                        if (isUpdate)
                        {
                            @obj = franchise.Operator;
                            Name name = @obj.name;
                            name.prefix = prefix;
                            name.firstname = tboxFn.Text;
                            name.middlename = tboxMn.Text;
                            name.lastname = tboxLn.Text;
                            Address address = @obj.address;
                            address.addressline1 = tboxAddressB.Text;
                            address.addressline2 = tboxAddressS.Text;
                            @obj.WriteInto(name, address, image, sign, "", (DateTime)bDay.SelectedDate, tboxEmePer.Text, tboxPhone.Text);
                        }
                        else
                        {
                            Name name = new Name(prefix, tboxFn.Text, tboxMn.Text, tboxLn.Text, "");
                            Address address = new Address(tboxAddressB.Text, tboxAddressS.Text);
                            @obj.WriteInto(name, address, image, sign, "", (DateTime)bDay.SelectedDate, tboxEmePer.Text, tboxPhone.Text);
                        }


                        franchise.WriteInto(tboxBnum.Text, @obj, null, tboxLnum.Text);

                        ID id = new ID(franchise, Objects.General.OPERATOR);
                        print.Save(id);
                        print.Show();
                        this.Hide();
                    }

                }
                else
                {
                    if (!hasPhoto)
                    {
                        imgIDPic.Source = null;
                    }
                    if (!hasSign)
                    {
                        imgSignPic.Source = null;
                    }
                    ControlWindow.Show("Input Fields incomplete!", "Missing some required inputs.");
                }
            }
        }

        public void StopCamera()
        {
            isCameraRunning = false; // Set the flag to indicate camera stopping
            pbCameraOpen.Visibility = Visibility.Hidden;
            try
            {
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }

                // Ensure that the camera source is completely stopped and disposed
                videoSource?.Stop();
                videoSource?.SignalToStop();
                videoSource?.WaitForStop();
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
            

                // Dispose of the last captured image if necessary
                lastCapturedImage?.Freeze();
                //lastCapturedImage?.Dispose();
            }
            catch (Exception e)
            {
                ControlWindow.Show("Camera Error", e.Message, Icons.ERROR);
            }
        }


        private void GenerateDummy()
        {
            tboxFn.Text = "First Name";
            tboxMn.Text = "Middle Name";
            tboxLn.Text = "Last Name";
            tboxLnum.Text = "C07-05-001168";
            tboxBnum.Text = "99999";
            tboxAddressB.Text = "Block 9999, Lot 9999, Phase 9999 Area ABC123";
            tboxEmePer.Text = "FirstName LastName";
            tboxPhone.Text = "09123456789";

        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.2));
            tboxFn.Focus();
        }
        private void TextBoxGotFocus_Yey(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
                textBox.SelectAll();
            }
        }

        private void inkSign_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            if (!hasInkBeenAdded)
            {
                hasInkBeenAdded = true;
                btnClearInkCanvas.FadeIn(0.2);
                btnStartPad.Content = "Save Signature";
            }
        }

        private void btnClearInkCanvas_Click(object sender, RoutedEventArgs e)
        {
            btnClearInkCanvas.FadeOut(0.2);
            inkSign.Strokes.Clear();
            btnStartPad.Content = "Cancel";
            hasInkBeenAdded = false;
        }
    }

}
