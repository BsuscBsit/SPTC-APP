using System;
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
using SPTC_APP.View.IDGenerator.Extra;
using SPTC_APP.View.Styling;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for GenerateID.xaml
    /// </summary>
    public partial class GenerateID : Window
    {
        BitmapSource lastCapturedImage = null;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        bool hasPhoto = false;
        bool hasSign = false;

        private bool isCameraRunning;



        bool isDriver = true;
        private Franchise franchise;
        bool isUpdate = false;

        //CONSTRUCTORS
        public GenerateID()
        {
            InitializeComponent();
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
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
            GenerateDummy();
        }
        public GenerateID(Franchise franchise, bool isDriver)
        {
            InitializeComponent();
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            AppState.mainwindow?.Hide();
            bDay.SelectedDate = DateTime.Today;
            this.franchise = franchise;
            isUpdate = true;
            EventLogger.Post("VIEW :: ID GENERATE Window id="+franchise.id);
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //btnStartPad.IsEnabled = false;
            tboxBnum.IsEnabled = false;
            this.isDriver = isDriver;
            MySwitch.IsEnabled = false;
            if (isDriver)
            {
                if (franchise.Driver != null)
                {
                    MySwitch.IsChecked = false;
                    var drv = franchise.Driver;
                    if(drv.name != null)
                    {
                        cbGender.SelectedIndex = ((drv.name.sex)?1:0);
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
                        vbSignPH.Visibility = Visibility.Hidden;
                        hasSign = true;

                    }
                    if(drv.birthday != null)
                    {
                        bDay.DisplayDate = drv.birthday;
                        bDay.DataContext = drv.birthday;
                        bDay.Text = drv.birthday.ToString();
                    }
                    /*tboxEmePer.Text = drv.emergencyPerson;
                    tboxPhone.Text = drv.emergencyContact;*/
                    tboxBnum.Text = franchise.BodyNumber;
                    tboxLnum.Text = franchise.LicenseNO;
                    MySwitch_Front(MySwitch, null);
                }
            } else
            {
                if (franchise.Operator != null)
                {
                    MySwitch.IsChecked = true;
                    var drv = franchise.Operator;
                    if (drv.name != null)
                    {
                        cbGender.SelectedIndex = ((drv.name.sex) ? 1 : 0);
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
                        vbSignPH.Visibility = Visibility.Hidden;
                        hasSign = true;
                    }
                    if (drv.birthday != null)
                    {
                        bDay.DisplayDate = drv.birthday;
                        bDay.DataContext = drv.birthday;
                        bDay.Text = drv.birthday.ToString();
                    }
                    /*tboxEmePer.Text = drv.emergencyPerson;
                    tboxPhone.Text = drv.emergencyContact;*/
                    tboxBnum.Text = franchise.BodyNumber;
                    tboxLnum.Text = franchise.LicenseNO;
                    MySwitch_Back(MySwitch, null);
                }
            }

            if (videoDevices.Count == 0)
            {
                EventLogger.Post("ERR :: No video devices found.");
                btnStartCam.IsEnabled = false;
                return;
            }

            if (hasSign)
            {
                btnStartPad.Content = "Clear";
            } else
            {
                btnStartPad.Content = "Start Pad";
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);

        }

        //WINDOW EVENTS
        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            AppState.WindowsCounter(true, sender);
            await Task.Delay(TimeSpan.FromSeconds(0.2));
            tboxFn.Focus();
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            this.StopCamera();
            base.OnClosing(e);
        }


        //CLICK EVENTS
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.StopCamera();
            PrintPreview print = new PrintPreview();
            print.Show();
            this.Close();
        }
        private void MySwitch_Back(object sender, RoutedEventArgs e)
        {
            // Switch is in the True state
            isDriver = false;
            if (MySwitch.IsEnabled)
            {
                drvOrOprt.Content = "Create this ID for\nOperator.";
            }
            else
            {
                drvOrOprt.Content = "This ID will be created\nfor Operator.";
            }
        }
        private void MySwitch_Front(object sender, RoutedEventArgs e)
        {
            // Switch is in the False state
            isDriver = true;
            if (MySwitch.IsEnabled)
            {
                drvOrOprt.Content = "Create this ID for\nDriver.";
            }
            else
            {
                drvOrOprt.Content = "This ID will be created\nfor Driver.";
            }

        }
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
                        ((System.Windows.Controls.Button)sender).Content = "Capture";
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
                        ControlWindow.ShowStatic("Camera Error", $"An error occurred: {ex.Message}", Icons.ERROR);
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
                    ((System.Windows.Controls.Button)sender).Content = "Retake Photo";
                    this.StopCamera();
                    pbCameraOpen.Value = 100;
                    pbCameraOpen.IsIndeterminate = false;
                }
            }
        }
        private void btnStartPad_Click(object sender, RoutedEventArgs e)
        {
            if (!hasSign)
            {
                SignPad sp = new SignPad();
                if (sp.ShowDialog() ?? false)
                {
                    imgSignPic.Source = sp.signatureBitmapResult;
                    hasSign = true;
                    btnStartPad.Content = "Clear";
                    vbSignPH.Visibility = Visibility.Hidden;
                }
                else
                {
                    //ControlWindow.Show("Signing Failed!", "No signature is saved", Icons.DEFAULT);
                    //hasSign = false;

                    btnStartPad.Content = "Start Pad";
                    vbSignPH.Visibility = Visibility.Visible;
                }
            } else
            {
                btnStartPad.Content = "Start Pad";
                vbSignPH.Visibility = Visibility.Visible;
                imgSignPic.Source = null;
                hasSign = false;
            }
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
                vbSignPH.Visibility = Visibility.Hidden;
                hasSign = true;
            }
        }
        private bool Preview_WarnDone(RoutedEventArgs e)
        {
            string warn = "";
            bool cameraWarning = isCameraRunning && !hasPhoto;
            bool signPadWarning = false;

            if (cameraWarning)
            {
                this.StopCamera();
            }

          

            if (cameraWarning || signPadWarning)
            {
                warn = (cameraWarning ? "Camera" : "") + (cameraWarning && signPadWarning ? " and " : "") + (signPadWarning ? "Sign Pad" : "");
                //warn += " has been initiated but no " + (warn.Length == 6 ? "image was" : "") + " captured. Are you certain you want to proceed?";
                warn = !string.IsNullOrEmpty(warn) ? (warn + " has been initiated but, no" + (warn.Length < 7 ? " image was" : (warn.Length < 10 ? " input was" : " inputs were")) + " captured. \nAre you certain you want to proceed?") : warn;
                if(ControlWindow.ShowDialogStatic("Continue?", warn, Icons.NOTIFY))
                {
                    return true;
                }
                else
                {
                    if (cameraWarning)
                    {
                        imgIDPic.Source = null;
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
                if (tboxLnum.Text.Length > 0 && tboxFn.Text.Length > 0 && tboxLn.Text.Length > 0)
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
                            image = new SPTC_APP.Objects.Image(imgIDPic.Source, $"Drv - {tboxFn.Text}{tboxMn.Text}{tboxLn.Text}");
                        }
                        else
                        {
                            imgIDPic.Source = null;
                        }
                        if (hasSign)
                        {
                            sign = new SPTC_APP.Objects.Image(imgSignPic.Source, $"Sign Drv - {tboxFn.Text}{tboxMn.Text}{tboxLn.Text}");
                        }
                        else
                        {
                            imgSignPic.Source = null;
                        }

                        bool prefix = (cbGender.SelectedIndex == 0) ? false : true;
                        if (isUpdate)
                        {
                            @obj = franchise.Driver;
                            Name name = @obj.name;
                            name.sex = prefix;
                            name.firstname = tboxFn.Text;
                            name.middlename = tboxMn.Text;
                            name.lastname = tboxLn.Text;
                            Address address = @obj.address;
                            address.addressline1 = tboxAddressB.Text;
                            address.addressline2 = tboxAddressS.Text;
                            @obj.WriteInto(name, address, image, sign, "", (DateTime)bDay.SelectedDate, "", "");
                        }
                        else
                        {   
                            Name name = new Name(prefix, tboxFn.Text, tboxMn.Text, tboxLn.Text, "");
                            Address address = new Address(tboxAddressB.Text, tboxAddressS.Text);
                            @obj.WriteInto(name, address, image, sign, "", (DateTime)bDay.SelectedDate, "", "");
                        }

                        franchise.WriteInto(tboxBnum.Text, franchise?.Operator, @obj, tboxLnum.Text);

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
                            image = new SPTC_APP.Objects.Image(imgIDPic.Source, $"Oprt - {tboxFn.Text}{tboxMn.Text}{tboxLn.Text}");
                        }
                        else
                        {
                            imgIDPic.Source = null;
                        }
                        if (hasSign)
                        {
                            sign = new SPTC_APP.Objects.Image(imgSignPic.Source, $"Sign Oprt - {tboxFn.Text}{tboxMn.Text}{tboxLn.Text}");
                        }
                        else
                        {
                            imgSignPic.Source = null;
                        }
                        bool prefix = (cbGender.SelectedIndex == 0) ? false : true;
                        if (isUpdate)
                        {
                            @obj = franchise.Operator;
                            Name name = @obj.name;
                            name.sex = prefix;
                            name.firstname = tboxFn.Text;
                            name.middlename = tboxMn.Text;
                            name.lastname = tboxLn.Text;
                            Address address = @obj.address;
                            address.addressline1 = tboxAddressB.Text;
                            address.addressline2 = tboxAddressS.Text;
                            @obj.WriteInto(name, address, image, sign, "", (DateTime)bDay.SelectedDate, "", "");
                        }
                        else
                        {
                            Name name = new Name(prefix, tboxFn.Text, tboxMn.Text, tboxLn.Text, "");
                            Address address = new Address(tboxAddressB.Text, tboxAddressS.Text);
                            @obj.WriteInto(name, address, image, sign, "", (DateTime)bDay.SelectedDate, "", "");
                        }


                        franchise.WriteInto(tboxBnum.Text, @obj, franchise?.Driver, tboxLnum.Text);

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
                    ControlWindow.ShowStatic("Input Fields incomplete!", "Missing some required inputs.");
                }
            }
        }


        //CAMERA EVENTS
        public void StopCamera()
        {
            isCameraRunning = false; // Set the flag to indicate camera stopping
            pbCameraOpen.Visibility = Visibility.Hidden;
            try
            {
               

                // Ensure that the camera source is completely stopped and disposed
                videoSource?.SignalToStop();
                videoSource?.WaitForStop();
                if (videoSource != null)
                {
                    videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                }
            

                // Dispose of the last captured image if necessary
                //lastCapturedImage?.Freeze();
                //lastCapturedImage?.ClearValue();
            }
            catch (Exception e)
            {
                ControlWindow.ShowStatic("Camera Error", e.Message, Icons.ERROR);
            }
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

                    // Calculate the cropping rectangle to center the image horizontally
                    double frameWidth = frame.Width;
                    double frameHeight = frame.Height;
                    double aspectRatio = 1.0; // 1:1 aspect ratio
                    double croppedWidth = Math.Min(frameWidth, frameHeight * aspectRatio);
                    double left = (frameWidth - croppedWidth) / 2;

                    // Crop the frame
                    System.Drawing.Bitmap croppedFrame = frame.Clone(new System.Drawing.RectangleF(
                        (float)left,
                        0,
                        (float)croppedWidth,
                        (float)frameHeight), frame.PixelFormat);

                    // Convert to BitmapSource
                    var hBitmap = croppedFrame.GetHbitmap();
                    BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    // Display the cropped frame in imgIDPic
                    imgIDPic.Source = bitmapSource;
                    lastCapturedImage = bitmapSource;

                    // Dispose of the frames to release resources
                    frame.Dispose();
                    croppedFrame.Dispose();

                    // Freeze the BitmapSource to release its resources
                    bitmapSource.Freeze();
                    // Release the HBitmap
                    DeleteObject(hBitmap);
                });
            }
        }


        //UNIQUE METHODS
        private void ProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            pbCameraOpen.Value = pbCameraOpen.Minimum;
        }
        private void GenerateDummy()
        {
            tboxFn.Text = "First Name";
            tboxMn.Text = "Middle Name";
            tboxLn.Text = "Last Name";
            tboxLnum.Text = "C07-05-001168";
            tboxBnum.Text = "99999";
            tboxAddressB.Text = "Block 9999, Lot 9999, Phase 9999 Area ABC123";

        }

        // Define the DeleteObject method to release HBitmap
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);


        //INPUT EVENTS
        private void TextBoxGotFocus_Yey(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
                textBox.SelectAll();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                TextBox tb = sender as TextBox;
                tb?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void SetToolTips()
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void InitToolTip()
        {
            /*Text

            setToolTip(tboxFn, "first name");
            setToolTip(tboxMn, "middle name");
            setToolTip(tboxLn, "last name");
            setToolTip(tboxBnum, "body / SPTC number");
            setToolTip(tboxLnum, "license number");
            setToolTip(tboxLn, "last name");
            setToolTip(cbGender, "");*/
        }
    }

}
