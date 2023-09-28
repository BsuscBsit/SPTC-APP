using System;
using System.Windows;
using MySql.Data.MySqlClient;
using SPTC_APP.Objects;

namespace SPTC_APP.View
{
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            Controller.StartInitialization(this, pbLoading, tbDebugLog);
            Activated += (sender, e) => { AppState.WindowsCounter(true); };
            Closing += (sender, e) => { AppState.WindowsCounter(false); };
        }

        
    }
}
