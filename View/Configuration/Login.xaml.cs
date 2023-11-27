using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();
            AppState.LoadFromJson();
            if(!AppState.isDeployment && !AppState.isDeployment_IDGeneration) pbPassword.Password = "Admin1234";
            cbUser.ItemsSource = AppState.Employees;
            cbUser.SelectedIndex = AppState.LAST_LOGIN;
            if((AppState.Employees?.Count ?? 0) < 4)
            {
                AppState.SaveToJson();
            }
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            FocusPb();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            AppState.LAST_LOGIN = cbUser.SelectedIndex;
            AppState.SaveToJson();
            AppState.Login(cbUser.Text, pbPassword.Password, this);
            pbPassword.Focus();
        }
      
        private async void FocusPb()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            pbPassword.Focus();
            if ((AppState.Employees?.Count ?? 0) < 4)
            {
                ControlWindow.ShowStatic("Loading Error", "An error occured", Icons.ERROR);
                this.Close();
            }
        }

        // Styling
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnLogin_Click(null, null);
            }
        }
        private void pbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            pbHint.Visibility = Visibility.Hidden;
        }

        private void pbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password.Length == 0)
            {
                pbHint.Visibility = Visibility.Visible;
            }
        }

        private void Button_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (pbPassword.Password.Length > 0)
            {
                pbView.Content = GetPasswordAsString(pbPassword);
                pbPassword.Visibility = Visibility.Hidden;
                pbView.Visibility = Visibility.Visible;
            }
        }

        private void Button_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            pbPassword.Visibility = Visibility.Visible;
            pbView.Visibility = Visibility.Hidden;
            if (pbPassword.Password.Length > 0)
            {
                pbPassword.Focus();
            }
        }
        private string GetPasswordAsString(PasswordBox passwordBox)
        {
            SecureString securePassword = passwordBox.SecurePassword;
            IntPtr passwordPtr = IntPtr.Zero;
            try
            {
                passwordPtr = Marshal.SecureStringToBSTR(securePassword);
                return Marshal.PtrToStringBSTR(passwordPtr);
            }
            finally
            {
                if (passwordPtr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(passwordPtr);
                }
            }
        }

        private void pbPassword_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            /*if (pbPassword.Password.Length > 0)
            {

            }
            else
            {

            }*/
            if (pbPassword.Password == string.Empty)
            {
                btnViewPass.Visibility = Visibility.Hidden;
            }
            else
            {
                btnViewPass.Visibility = Visibility.Visible;
            }
        }

        private void imgClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("HI");
        }
    }
}
