using System.Windows;
using SPTC_APP.Objects;

namespace SPTC_APP.View
{
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            Controller.StartInitialization(this, pbLoading, tbDebugLog);
        }
    }
}
