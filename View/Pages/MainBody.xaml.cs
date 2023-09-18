using System.Windows;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Pages.Leaflets;

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

        private void FranchiseButton_Click(object sender, RoutedEventArgs e)
        {
            TablePanelSwap.Children.Clear();
            TablePanelSwap.Children.Add((new TableView(Table.FRANCHISE)).Fetch());
        }

        private void DashBoard_Click(object sender, RoutedEventArgs e)
        {
            DashButton.Visibility = Visibility.Visible;
        }
    }
}