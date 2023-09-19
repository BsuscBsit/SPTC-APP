using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private Button selectedButton = null;
        public MainBody()
        {
            InitializeComponent();
            username.Content = AppState.USER.position.title.ToString();
            DashBoard_Click(DashBoard, null);
        }

        private void imgClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        //Side panel buttons
        private void Btn_Logout(object sender, RoutedEventArgs e)
        {
            AppState.Logout(this);
        }

        private void Btn_Settings(object sender, RoutedEventArgs e)
        {
            ClickColorControl(sender as Button);
        }

        private async void FranchiseButton_Click(object sender, RoutedEventArgs e)
        {
            TablePanelSwap.Children.Clear();
            TablePanelSwap.Children.Add(await (new TableView(Table.FRANCHISE)).Fetch());
            ClickColorControl(sender as Button);
        }

        private async void DashBoard_Click(object sender, RoutedEventArgs e)
        {
            TablePanelSwap.Children.Clear();
            TablePanelSwap.Children.Add(await (new DashboardView()).Fetch());
            ClickColorControl(sender as Button);
        }

        private async void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            TablePanelSwap.Children.Clear();
            TablePanelSwap.Children.Add(await(new TableView(Table.OPERATOR)).Fetch());
            ClickColorControl(sender as Button);
        }

        private async void DriverButton_Click(object sender, RoutedEventArgs e)
        {
            TablePanelSwap.Children.Clear();
            TablePanelSwap.Children.Add(await (new TableView(Table.DRIVER)).Fetch());
            ClickColorControl(sender as Button);
        }

        private void BtnBoardMember_Click(object sender, RoutedEventArgs e)
        {
            TablePanelSwap.Children.Clear();
            //TablePanelSwap.Children.Add(await(new TableView(Table.DRIVER)).Fetch());
            ClickColorControl(sender as Button);
        }





        //Button control
        private void ClickColorControl(Button button)
        {
            if (selectedButton != null)
            {
                selectedButton.Background = Brushes.White;
            }

            button.Background = Brushes.Yellow;
            selectedButton = button;
        }
    }
}