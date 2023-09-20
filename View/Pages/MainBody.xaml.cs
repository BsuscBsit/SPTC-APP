using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
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

        private async void cbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Search
            if (cbSearch.Text.Length > 0)
            {
                List<Franchise> tmp = Retrieve.GetDataUsingQuery<Franchise>(Where.Search(cbSearch.Text));
                if (tmp != null)
                {
                    if (tmp.Count > 0)
                    {
                        lsSuggestion.Visibility = Visibility.Visible;
                        DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(lsSuggestion);

                        List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                        {

                            new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
                            new ColumnConfiguration("Operator.name.wholename", "OPERATOR NAME", width: 140),
                            new ColumnConfiguration("Driver.name.wholename", "Driver NAME", width: 140),
                        };

                        dataGridHelper.DesignGrid(tmp, columnConfigurations);
                        lsSuggestion.ItemsSource = tmp;
                    }
                }
                else
                {
                    lsSuggestion.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                lsSuggestion.Visibility = Visibility.Collapsed;
            }
        }


        private void cbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            lsSuggestion.Visibility = Visibility.Collapsed;
        }

        private async void lsSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Franchise fran = ((DataGrid)sender).SelectedItem as Franchise;
            TablePanelSwap.Children.Clear();
            ClickColorControl(FranchiseButton);
            TablePanelSwap.Children.Add(await (new TableView(Table.FRANCHISE)).Fetch());
            TablePanelSwap.Children.Add((new FranchiseInformationView(fran)).Fetch());
        }
    }
}