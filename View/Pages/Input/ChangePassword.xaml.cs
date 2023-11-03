﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        List<object[]> passwordArray;

        public ChangePassword()
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            passwordArray = new List<object[]> {
                new object[] { pbHint, pbView, pbPassword, btnViewPass },
                new object[] { pbHint2, pbView2, pbPassword2, btnViewPass2 },
                new object[] { pbHint3, pbView3, pbPassword3, btnViewPass3 }
            };

            FocusPb();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();

            base.OnClosing(e);
        }

        private async void FocusPb()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            pbPassword.Focus();
        }

        // Styling
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnSave_Click(null, null);
            }
        }


        private void pbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            foreach (var ps in passwordArray) {
                if ((sender as PasswordBox) == ps[2])
                    (ps[0] as Label).Visibility = Visibility.Hidden;
            }
        }

        private void pbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (var ps in passwordArray) 
            {
                if ((sender as PasswordBox) == ps[2]) {
                    if ((ps[2] as PasswordBox).Password.Length == 0)
                        (ps[0] as Label).Visibility = Visibility.Visible;
                }
            }
        }

        private void Button_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var ps in passwordArray)
            {
                if ((sender as Button) == (ps[3] as Button))
                {
                    if ((ps[2] as PasswordBox).Password.Length > 0)
                    {
                        (ps[1] as Label).Content = GetPasswordAsString((ps[2] as PasswordBox));
                        (ps[2] as PasswordBox).Visibility = Visibility.Hidden;
                        (ps[1] as Label).Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Button_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (var ps in passwordArray)
            {
                if ((sender as Button) == (ps[3] as Button))
                {
                    (ps[2] as PasswordBox).Visibility = Visibility.Visible;
                    (ps[1] as Label).Visibility = Visibility.Hidden;
                    if ((ps[2] as PasswordBox).Password.Length > 0)
                    {
                        (ps[2] as PasswordBox).Focus();
                    }
                }
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
            foreach (var ps in passwordArray)
            {
                if ((sender as PasswordBox) == ps[2])
                {
                    if ((ps[2] as PasswordBox).Password == string.Empty)
                    {
                        (ps[3] as Button).Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        (ps[3] as Button).Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}