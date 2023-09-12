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
            try
            {
                Controller.StartInitialization(this, pbLoading, tbDebugLog);
            }
            catch (MySqlException ex)
            {
                EventLogger.Post("DTB :: MySqlException : " + ex.Message);
            }
            catch (Exception e)
            {
                EventLogger.Post("ERR :: Exception : " + e.Message);
            }
        }
    }
}
