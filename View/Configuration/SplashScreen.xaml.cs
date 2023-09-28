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
            AppState.LoadFromJson();

            EventLogger.Post("OUT :: <----- START UP ----->");
            ContentRendered += (sender, e) => {
                Controller.StartInitialization(this, pbLoading, tbDebugLog);
                AppState.WindowsCounter(true, sender);
            };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };

        }

        
    }
}
