using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages.Output;

namespace SPTC_APP.View.Pages
{
    /// <summary>
    /// Interaction logic for MainBody.xaml
    /// </summary>
    public partial class MainBody : Window
    {
        private Button selectedButton = null;
        public static Franchise selectedFranchise = null;
        public MainBody()
        {
            InitializeComponent();
            username.Content = AppState.USER?.position?.title.ToString();
            DashBoard_Click(DashBoard, null);
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
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
            (new SettingsView()).ShowDialog();
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
            selectedFranchise = null;
            if (selectedButton != null)
            {
                selectedButton.Background = Brushes.White;
            }

            button.Background = Brushes.Yellow;
            selectedButton = button;
        }


        //SEARCH functionality
        private async void cbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //SEARCH
            if (cbSearch.Text.Length > 0)
            {
                if (selectedButton != null)
                {
                    if (selectedButton  == FranchiseButton)
                    {

                    } 
                    else if(selectedButton == OperatorButton)
                    {

                    }
                    else if (selectedButton == DriverButton)
                    {

                    }
                    else
                    {
                        GetFranchiseInList();
                    }
                } else
                {
                    GetFranchiseInList();
                }
            }
            else
            {
                lsSuggestion.Visibility = Visibility.Collapsed;
            }
            await Task.Delay(50);
        }

        private void GetFranchiseInList()
        {
            List<Franchise> tmp = Retrieve.GetDataUsingQuery<Franchise>(RequestQuery.SEARCH(cbSearch.Text));
            if (tmp != null)
            {
                if (tmp.Count > 0)
                {
                    lsSuggestion.Visibility = Visibility.Visible;
                    List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                        {

                            new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
                            new ColumnConfiguration("Operator.name.wholename", "OPERATOR NAME", width: 140),
                            new ColumnConfiguration("Driver.name.wholename", "Driver NAME", width: 140),
                        };
                    DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(lsSuggestion, columnConfigurations);

                    lsSuggestion.ItemsSource = tmp;
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
            cbSearch.Text = "";
        }

        private async void lsSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((DataGrid)sender).SelectedItem is Franchise fran)
            {
                if (fran.BodyNumber != null)
                {
                    TablePanelSwap.Children.Clear();
                    ClickColorControl(FranchiseButton);
                    TablePanelSwap.Children.Add(await (new TableView(Table.FRANCHISE)).Fetch());
                    MainBody.selectedFranchise = fran;
                    TablePanelSwap.Children.Add((new FranchiseInformationView()).Fetch());
                }
            }
        }

        private void cbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            cbSearch_TextChanged(sender, null);
        }
    }
}