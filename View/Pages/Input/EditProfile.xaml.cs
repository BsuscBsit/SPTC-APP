using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.IDGenerator.Extra;
using SPTC_APP.View.Styling;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;
using Image = SPTC_APP.Objects.Image;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for ChangeOperator.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        BitmapSource lastCapturedImage = null;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private bool isCameraRunning = false;


        private string canceledMSG;
        private string savedMSG;
        private string closingMSG;

        private Driver dholder;
        private Operator oholder;
        Franchise franchise;
        General type;

        bool hasPhoto = false;
        bool hasSign = false;
        public EditProfile(Franchise franchise, General type, Driver dholder = null)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            
            this.franchise = franchise;
            this.type = type;
            bDay.SelectedDate = DateTime.Now;
            bDay.DisplayDate = DateTime.Now;
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                EventLogger.Post("ERR :: No video devices found.");
                btnStartCam.IsEnabled = false;
                return;
            }
            videoSource = new VideoCaptureDevice(videoDevices[AppState.DEFAULT_CAMERA].MonikerString);
            if (AppState.CAMERA_RESOLUTION.Split('x').Length == 2)
            {
                int height = Int32.Parse(AppState.CAMERA_RESOLUTION.Split('x')[0]);
                int width = Int32.Parse(AppState.CAMERA_RESOLUTION.Split('x')[1]);
                bool hasRes = false;
                foreach (VideoCapabilities res in AppState.GetResolutionList())
                {
                    if (res.FrameSize.Height == height && res.FrameSize.Width == width)
                    {
                        videoSource.VideoResolution = res;
                        hasRes = true;
                    }
                }
                if (!hasRes)
                {
                    videoSource.VideoResolution = AppState.GetResolutionList().FirstOrDefault();
                }
            }
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
            if (type == General.DRIVER)
            {
                Driver drv = franchise?.Driver ?? dholder;
                tbFname.Text = drv.name?.firstname;
                tbMname.Text = drv.name?.middlename;
                tbLname.Text = drv.name?.lastname;
                bDay.SelectedDate = drv.birthday;
                bDay.DisplayDate = drv.birthday;
                cbGender.SelectedIndex = drv.name?.sex ?? false ? 1: 0;
                /*tboxsContactNum.Text = drv.emergencyContact;
                tboxsCountry.Text = drv.address?.country;
                tboxsProvince.Text = drv.address?.province;
                tboxsCity.Text = drv.address?.city;*/
                lblDriverLicense.Visibility = Visibility.Visible;
                tbLicense.Visibility = Visibility.Visible;
                tbLicense.Text = drv.licenseNo?.ToString();
                tbAddressLine1.Text = drv.address?.addressline1;
                tbAddressLine2.Text = drv.address?.addressline2;
                if ((drv.image?.GetSource() ?? null) != null)
                {
                    imgIDPic.Source = drv.image.GetSource();
                    hasPhoto = true;
                }
                if ((drv.signature?.GetSource() ?? null) != null)
                {
                    imgSignPic.Source = drv.signature.GetSource();
                    vbSignPH.Visibility = Visibility.Hidden;
                    hasSign = true;
                }
                this.dholder = drv;
            } 
            else if (type == General.OPERATOR)
            {
                Operator optr =franchise.Operator as Operator;
                tbFname.Text = optr.name?.firstname;
                tbMname.Text = optr.name?.middlename;
                tbLname.Text = optr.name?.lastname;
                bDay.SelectedDate = optr.birthday;
                bDay.DisplayDate = optr.birthday;
                cbGender.SelectedIndex = optr.name?.sex ?? false ? 1 : 0;
                /*tboxsContactNum.Text = optr.emergencyContact;
                tboxsCountry.Text = optr.address?.country;
                tboxsProvince.Text = optr.address?.province;
                tboxsCity.Text = optr.address?.city;*/
                tbAddressLine1.Text = optr.address?.addressline1;
                tbAddressLine2.Text = optr.address?.addressline2;

                if ((optr.image?.GetSource() ?? null) != null)
                {
                    imgIDPic.Source = optr.image.GetSource();
                    hasPhoto = true;
                }
                if ((optr.signature?.GetSource() ?? null) != null)
                {
                    imgSignPic.Source = optr.signature.GetSource();
                    vbSignPH.Visibility = Visibility.Hidden;
                    hasSign = true;
                }

                this.oholder = optr;
            }
            else if(type == General.NEW_DRIVER)
            {
                lblDriverLicense.Visibility = Visibility.Visible;
                tbLicense.Visibility = Visibility.Visible;
            }
            DraggingHelper.DragWindow(topBar);
            tbFname.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string pos = tbLname.ToString();
            if (!string.IsNullOrEmpty(pos))
            {
                pos = pos.Trim();
                string capitalized = string.Join(" ", pos.Split(' ').Select(word => char.ToUpper(word[0]) + word.Substring(1)));
                this.canceledMSG = string.IsNullOrWhiteSpace(capitalized) ?
                    $"Changes to {capitalized} was not saved.\nAction was canceled." : "Changes to profile was not saved.\nAction was canceled.";
                this.savedMSG = string.IsNullOrWhiteSpace(capitalized) ?
                    $"Changes to {capitalized} was saved successfully." : "The changes was saved successfully.";
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.StopCamera();
            AppState.mainwindow?.Show();
            AppState.mainwindow?.displayToast(closingMSG);
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            closingMSG = "Changes was not saved.\nAction was canceled.";
            this.Close();
        }


        private async void BtnStartCam_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource != null)
            {
                if (!videoSource.IsRunning)
                {
                    try
                    {
                        groupCameraButtons.FadeOut(0.2);
                        gridCameraCapture.FadeIn(0.3);
                        hasPhoto = false;
                        btnStartCam.IsEnabled = false;
                        //((System.Windows.Controls.Button)sender).Content = "Capture";
                        btnCaptureeee.Content = "Capture";
                        btnStartCam.Content = "Capture";
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
                        ControlWindow.ShowStatic("Camera Error", $"An has error occurred: {ex.Message}", Icons.ERROR);
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
                    //((System.Windows.Controls.Button)sender).Content = "Retake Photo";
                    btnCaptureeee.Content = "Retake Photo";
                    btnStartCam.Content = "Retake Photo";
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
                    imgSignPic.Height = sp.signatureBitmapResult.Height;
                    imgSignPic.Width = sp.signatureBitmapResult.Width;
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
            }
            else
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
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (ControlWindow.ShowTwoway("Updating Fields.", "Confirm to save changes.", Icons.NOTIFY))
            {
                if (type == General.NEW_DRIVER)
                {
                    Driver drv = new Driver();
                    drv.name = new Name();
                    drv.address = new Address();
                    drv.name.firstname = tbFname.Text;
                    drv.name.middlename = tbMname.Text;
                    drv.name.lastname = tbLname.Text;
                    drv.birthday = (DateTime)bDay.SelectedDate;
                    drv.name.sex = cbGender.SelectedIndex == 1;
                    drv.address.addressline1 = tbAddressLine1.Text;
                    drv.address.addressline2 = tbAddressLine2.Text;
                    drv.licenseNo = tbLicense.Text;
                    drv.address.UpdateAddressLines();
                    if (franchise != null)
                    {
                        franchise.Driver = drv;
                        franchise.Save();
                    }
                    else
                    {
                        drv.Save();
                    }
                    (AppState.mainwindow as MainBody).ResetWindow(General.DRIVER);
                }
                else if (type == General.DRIVER)
                {
                    Driver drv = franchise?.Driver  ?? dholder;
                    if (drv.name == null)
                        drv.name = new Name();
                    if (drv.address == null)
                        drv.address = new Address();
                    drv.name.firstname = tbFname.Text;
                    drv.name.middlename = tbMname.Text;
                    drv.name.lastname = tbLname.Text;
                    drv.name.sex = cbGender.SelectedIndex == 1;
                    /*drv.birthday = (DateTime)bDay.SelectedDate;
                    drv.emergencyContact = tboxsContactNum.Text;
                    drv.address.country = tboxsCountry.Text;
                    drv.address.province = tboxsProvince.Text;
                    drv.address.city = tboxsCity.Text;*/
                    drv.address.addressline1 = tbAddressLine1.Text;
                    drv.address.addressline2 = tbAddressLine2.Text;
                    drv.licenseNo = tbLicense.Text;
                    drv.address.UpdateAddressLines();
                    if (hasPhoto)
                    {
                        drv.image = new Image(imgIDPic.Source, $"Image {drv.name.ToString()}");
                    }
                    if (hasSign)
                    {
                        drv.signature = new Image(imgSignPic.Source, $"Signature {drv.name.ToString()}");
                    }
                    if (franchise != null)
                    {
                        franchise.Save();
                    }
                    else
                    {
                        drv.Save();

                    }
                    (AppState.mainwindow as MainBody).ResetWindow(General.DRIVER);
                }
                else if (type == General.OPERATOR)
                {
                    Operator optr = (franchise?.Operator == null) ? oholder : franchise.Operator;
                    if (optr.name == null)
                        optr.name = new Name();
                    if (optr.address == null)
                        optr.address = new Address();
                    optr.name.firstname = tbFname.Text;
                    optr.name.firstname = tbFname.Text;
                    optr.name.middlename = tbMname.Text;
                    optr.name.lastname = tbLname.Text;
                    optr.birthday = (DateTime)bDay.SelectedDate;
                    optr.name.sex = cbGender.SelectedIndex == 1;
                    /*optr.emergencyContact = tboxsContactNum.Text;
                    optr.name.sex = cbGender.SelectedIndex == 1;
                    optr.address.country = tboxsCountry.Text;
                    optr.address.province = tboxsProvince.Text;
                    optr.address.city = tboxsCity.Text;*/
                    optr.address.addressline1 = tbAddressLine1.Text;
                    optr.address.addressline2 = tbAddressLine2.Text;
                    optr.address.UpdateAddressLines();

                    if (hasPhoto)
                    {
                        optr.image = new Image(imgIDPic.Source, $"Image {optr.name.ToString()}");
                    }
                    if (hasSign)
                    {
                        optr.signature = new Image(imgSignPic.Source, $"Signature {optr.name.ToString()}");
                    }
                    if (franchise != null)
                    {
                        optr.Save();
                        franchise.Save();
                    }
                    else
                    {
                        optr.Save();
                    }
                    (AppState.mainwindow as MainBody).ResetWindow(General.OPERATOR);
                }
                closingMSG = "Changes was saved successfully.\nPlease refesh the view to see changes.";
                this.Close();
            }
        
        }

        public void StopCamera()
        {
            isCameraRunning = false;
            pbCameraOpen.Visibility = Visibility.Hidden;
            try
            {
                videoSource?.SignalToStop();
                videoSource?.WaitForStop();
                if (videoSource != null)
                {
                    videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                }
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
                    System.Drawing.Bitmap frame = (System.Drawing.Bitmap)eventArgs.Frame.Clone();

                    double frameWidth = frame.Width;
                    double frameHeight = frame.Height;
                    double aspectRatio = 1.0;
                    double croppedWidth = Math.Min(frameWidth, frameHeight * aspectRatio);
                    double left = (frameWidth - croppedWidth) / 2;

                    System.Drawing.Bitmap croppedFrame = frame.Clone(new System.Drawing.RectangleF(
                        (float)left,
                        0,
                        (float)croppedWidth,
                        (float)frameHeight), frame.PixelFormat);

                    var hBitmap = croppedFrame.GetHbitmap();
                    BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    imgIDPic.Source = bitmapSource;
                    lastCapturedImage = bitmapSource;

                    frame.Dispose();
                    croppedFrame.Dispose();

                    bitmapSource.Freeze();
                    DeleteObject(hBitmap);
                });
            }
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);
        private void gridCamera_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isCameraRunning)
            {
                gridCameraCapture.FadeIn(0.2);
            }
            else
            {
                groupCameraButtons.FadeIn(0.2);
            }
        }

        private void gridCamera_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            groupCameraButtons.FadeOut(0.2);
        }

        private void parentCanvasCam_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            gridCameraCapture.FadeOut(0.2);
        }

        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            groupPadButtons.FadeIn(0.2);
        }

        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            groupPadButtons.FadeOut(0.2);
        }
        private void initTextBoxes()
        {
            tbFname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "First name.", 0);
            tbMname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "Middle name.", 0);
            tbLname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "Last name.", 0);
            tbPosTitle.DefaultTextBoxBehavior(ALPHANUMERICDASHPERIOD, false, gridToast, "Position of this person.", 0);
            tbAddressLine1.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "Street Address.", 0);
            tbAddressLine2.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "Additional Address Information.", 0);

            /*tboxsProvince.DefaultTextBoxBehavior(ALPHANUMERICDASH, false, gridToast, "First Name", 0);
            tboxsCity.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "First Name", 0);
            tboxsBarangay.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "First Name", 0);
            tboxStreetName.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "First Name", 0);*/

        }
    }
}
