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
    /// Interaction logic for EditEmployee.xaml
    /// </summary>
    public partial class EditEmployee : Window
    {
        Employee employee;
        BitmapSource lastCapturedImage = null;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        bool hasPhoto = false;
        bool hasSign = false;
        bool isEdit = false;
        bool isManage = false;
        private bool isCameraRunning;
        public EditEmployee(Employee employee, bool isEdit, bool isManage)
        {
            InitializeComponent();
            initTextBoxes();
            DraggingHelper.DragWindow(topBar);
            tbFname.Focus();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            bDay.SelectedDate = DateTime.Now;
            bDay.Text = DateTime.Now.ToString();
            this.employee = employee;
            this.isEdit = isEdit;
            this.isManage = isManage;
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                EventLogger.Post("ERR :: No video devices found.");
                btnStartCam.IsEnabled = false;
                return;
            }
            videoSource = new VideoCaptureDevice(videoDevices[AppState.DEFAULT_CAMERA].MonikerString);
            if(AppState.CAMERA_RESOLUTION.Split('x').Length == 2)
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
            if (isEdit)
            {
                fillUp();
            }
            if (isManage)
            {
                tbPosTitle.Text = employee.position?.title?.ToString() ?? "";
                tbPosTitle.IsEnabled = false;
                
               
            } else
            {
                if (isEdit)
                {
                    tbPosTitle.IsEnabled = true;
                }
            }
            if (!isEdit && !isManage) 
            {
                tbPosTitle.IsEnabled = true;
            }
            if (employee.position?.title?.Equals(AppState.ALL_EMPLOYEES[4]) ?? false) 
            {
                tbPosTitle.IsEnabled = false;
            }

        }


        private void fillUp()
        {
            if(employee != null)
            {
                if (isManage)
                {
                    lblTitle.Content = $"MODIFY {employee.position?.title.ToUpper() ?? string.Empty} PROFILE: ";
                    if (AppState.IS_ADMIN || AppState.USER.position.title == employee.position.title)
                    {
                        btnChangePass.Visibility = Visibility.Visible;
                    }
                } 

                tbPosTitle.Text = employee.position?.title?.ToString() ?? "";
                tbFname.Text = employee.name?.firstname ?? "";
                tbMname.Text = employee.name?.middlename ?? "";
                tbLname.Text = employee.name?.lastname ?? "";
                bDay.Text = employee.birthday.ToString();
                bDay.SelectedDate = employee.birthday;
                cbGender.SelectedIndex = (employee.name?.sex ?? false)? 1: 0;
                tbAddressLine1.Text = employee.address?.addressline1;
                tbAddressLine2.Text = employee.address?.addressline2;
                if ((employee.image?.GetSource() ?? null) != null )
                {
                    imgIDPic.Source = employee.image.GetSource();
                    hasPhoto = true;
                }
                if ((employee.sign?.GetSource() ?? null) != null)
                {
                    imgSignPic.Source = employee.sign.GetSource();
                    vbSignPH.Visibility = Visibility.Hidden;
                    hasSign = true;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.StopCamera();
            base.OnClosing(e);
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
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (tbFname.Text.Length > 0 && tbLname.Text.Length > 0 && tbAddressLine1.Text.Length > 0 && tbAddressLine2.Text.Length > 0) {
                if (isEdit)
                {
                    employee.name = (employee.name != null) ? employee.name : new Name();
                    employee.address = (employee.address != null) ? employee.address : new Address();
                    employee.image = (employee.image != null) ? employee.image : null;
                    employee.sign = (employee.sign != null) ? employee.sign : null;
                } else
                {
                    Position position = employee.position;
                    employee.delete();
                    employee = new Employee();
                    employee.position = position;
                    employee.name = new Name();
                    employee.address = new Address();
                    employee.image = new Image();
                }
                employee.name.firstname = tbFname.Text;
                employee.name.middlename = tbMname.Text;
                employee.name.lastname = tbLname.Text;
                employee.name.sex = cbGender.SelectedIndex == 1;

                employee.address.addressline1 = tbAddressLine1.Text;
                employee.address.addressline2 = tbAddressLine2.Text;
                if (hasPhoto)
                {
                    employee.image = new Image(imgIDPic.Source, $"Image {employee.position.title}");
                }
                if (hasSign)
                {
                    employee.sign = new Image(imgSignPic.Source, $"Signature {employee.position.title}");
                }
                if (isManage)
                {
                    if (AppState.IS_ADMIN)
                    {
                        if (employee.position.title == AppState.Employees[0])
                        {
                            employee.Save();
                            AppState.mainwindow?.Show();
                            this.Close();
                        }
                        else
                        {
                            (new EditPositionAccess(this, employee)).Show();
                        }
                    }
                    else
                    {
                        employee.Save();
                        AppState.mainwindow?.Show();
                        this.Close();
                    }
                } else
                {
                    if(employee.position == null)
                    {
                        employee.position = new Position();
                    }
                    employee.position.title = tbPosTitle.Text;
                    employee.Save();
                    AppState.mainwindow?.Show();
                    this.Close();
                }
            } else
            {
                ControlWindow.ShowStatic("Input Fields incomplete!", "Missing some required inputs.");
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            AppState.mainwindow?.Show();
            this.Close();
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

        private void tbLname_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void initTextBoxes()
        {
            tbFname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "First name.", 0);
            tbMname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "Middle name.", 0);
            tbLname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "Last name.", 0);
            tbPosTitle.DefaultTextBoxBehavior(ALPHANUMERICDASHPERIOD, false, gridToast, "Position of this person.", 0);
            tbAddressLine1.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "Street Address.", 0);
            tbAddressLine2.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "Additional Address Information.", 0);
        }

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

        private void btnChangePass_Click(object sender, RoutedEventArgs e)
        {
            if (ControlWindow.ShowTwoway("Reset Password", "Are you sure you want to reset the password", Icons.ERROR))
            {
                if (employee.resetPass())
                {
                    btnChangePass.Visibility = Visibility.Collapsed;
                }
            }

        }
    }
}
