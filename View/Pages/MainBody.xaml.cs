using System.Windows;
using SPTC_APP.Objects;

namespace SPTC_APP.View.Pages
{
    /// <summary>
    /// Interaction logic for MainBody.xaml
    /// </summary>
    public partial class MainBody : Window
    {

        public MainBody()
        {
            InitializeComponent();
            username.Content = AppState.USER.position.title.ToString();
        }

        private void imgClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            (new Test()).Show();
        }

        private void btnGererate_Click(object sender, RoutedEventArgs e)
        {

            (new PrintPreview()).Show();
        }

        private void Btn_Logout(object sender, RoutedEventArgs e)
        {
            AppState.Logout(this);
        }

        private void Btn_Settings(object sender, RoutedEventArgs e)
        {

        }
    }
}