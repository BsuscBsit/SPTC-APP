using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages.Output;
using SPTC_APP.View.Styling;

namespace SPTC_APP.View.Pages
{
    /// <summary>
    /// Interaction logic for MainBody.xaml
    /// </summary>
    public partial class MainBody : Window
    {
        private Button selectedButton = null;

        public static Franchise selectedFranchise = null;
        private TableView Ftable, Otable, Dtable;




        public MainBody()
        {
            InitializeComponent();
            
            DashBoard_Click(DashBoard, null);
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            username.Content = AppState.USER?.position?.title.ToString();
            userImage.ImageSource = AppState.USER?.image?.GetSource() ?? new BitmapImage(new Uri("pack://application:,,,/SPTC APP;component/View/Images/icons/person.png"));
        }

        private void imgClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public async void ResetWindow(General where, bool hasSelection = false)
        {
            switch (where)
            {
                case General.FRANCHISE:
                    TablePanelSwap.Children.Clear();
                    Franchise fran = MainBody.selectedFranchise;
                    ClickColorControl(FranchiseButton);
                    MainBody.selectedFranchise = fran;
                    Ftable = (new TableView(Table.FRANCHISE));
                    TablePanelSwap.Children.Add(await Ftable.Fetch());
                    if (hasSelection && MainBody.selectedFranchise != null)
                    {
                        TablePanelSwap.Children.Add((new FranchiseInformationView(TablePanelSwap)).Fetch());
                    }
                    break;
                case General.OPERATOR: OperatorButton_Click(OperatorButton, null); break;
                case General.DRIVER: DriverButton_Click(DriverButton, null); break;
                case General.BOARD_MEMBER: BtnBoardMember_Click(BtnBoardMember, null); break;
                default: DashBoard_Click(DashBoard, null); break;
            }
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
            ClickColorControl(sender as Button);
            TablePanelSwap.Children.Clear();
            Ftable = (new TableView(Table.FRANCHISE));
            TablePanelSwap.Children.Add(await Ftable.Fetch());
            
        }

        private async void DashBoard_Click(object sender, RoutedEventArgs e)
        {
            ClickColorControl(sender as Button);
            TablePanelSwap.Children.Clear();
            TablePanelSwap.Children.Add(await (new DashboardView()).Fetch());
            
        }

        private async void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            ClickColorControl(sender as Button);
            TablePanelSwap.Children.Clear();
            Otable = (new TableView(Table.OPERATOR));
            TablePanelSwap.Children.Add(await Otable.Fetch());
           
        }

        private async void DriverButton_Click(object sender, RoutedEventArgs e)
        {
            ClickColorControl(sender as Button);
            TablePanelSwap.Children.Clear();
            Dtable = (new TableView(Table.DRIVER));
            TablePanelSwap.Children.Add(await Dtable.Fetch());
            
        }

        private async void BtnBoardMember_Click(object sender, RoutedEventArgs e)
        {
            ClickColorControl(sender as Button);
            TablePanelSwap.Children.Clear();
            TablePanelSwap.Children.Add(await (new BoardMembers()).Fetch());
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
            if (selectedButton == BtnBoardMember)
            {
                cbSearch.Visibility = Visibility.Collapsed;
            }
            else
            {
                cbSearch.Visibility = Visibility.Visible;
            }
        }


        //SEARCH functionality
        private async void cbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

            //SEARCH
            if (cbSearch.Text.Length > 0)
            {
                string searchText = cbSearch.Text.ToLower();
                if (selectedButton != null)
                {
                    if (selectedButton != DashBoard)
                    {
                        if (TablePanelSwap.Children[0] is Grid grid)
                        {
                            if (grid.Children[0] is DataGrid datagrid)
                            {
                                if (selectedButton == FranchiseButton)
                                {
                                    List<Franchise> franchises = new List<Franchise>();
                                    foreach (Franchise f in datagrid.Items)
                                    {
                                        franchises.Add(f);
                                    }

                                    franchises = franchises.OrderByDescending(f =>
                                        f.BodyNumber.ToString().Contains(searchText) ||
                                        (f.Operator?.ToString()?.ToLower()?.Contains(searchText) ?? false) ||
                                        (f.Driver?.ToString()?.ToLower()?.Contains(searchText) ?? false)
                                        ).ThenBy(f => f.BodyNumber).ToList();
                                    datagrid.Items.Clear();
                                    foreach (Franchise f in franchises)
                                    {
                                        datagrid.Items.Add(f);
                                    }
                                }
                                else if (selectedButton == OperatorButton)
                                {
                                    List<Operator> operators = new List<Operator>();
                                    foreach (Operator o in datagrid.Items)
                                    {
                                        operators.Add(o);
                                    }

                                    operators = operators.OrderByDescending(o =>
                                        (o.ToString()?.ToLower()?.Contains(searchText) ?? false)
                                        ).ThenBy(o => o.ToString()).ToList();
                                    datagrid.Items.Clear();
                                    foreach (Operator o in operators)
                                    {
                                        datagrid.Items.Add(o);
                                    }
                                }
                                else if (selectedButton == DriverButton)
                                {
                                    List<Driver> drivers = new List<Driver>();
                                    foreach (Driver d in datagrid.Items)
                                    {
                                        drivers.Add(d);
                                    }

                                    drivers = drivers.OrderByDescending(d =>
                                        (d.ToString()?.ToLower()?.Contains(searchText) ?? false)
                                        ).ThenBy(o => o.ToString()).ToList();
                                    datagrid.Items.Clear();
                                    foreach (Driver d in drivers)
                                    {
                                        datagrid.Items.Add(d);
                                    }
                                }
                            }
                        }
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
            await Task.Delay(5);
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

                            new ColumnConfiguration("BodyNumber", "BODY NO.", minWidth: 80),
                            new ColumnConfiguration("Operator.name.wholename", "OPERATOR NAME", minWidth: 140),
                            new ColumnConfiguration("Driver.name.wholename", "DRIVER NAME", minWidth: 140),
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

        private void paneExpander_Click(object sender, RoutedEventArgs e)
        {
            paneExpander.IsEnabled = false;
            if (paneExpander.IsChecked == false)
            {
                username.FadeOut(0.2, () =>
                {
                    AnimationHelper.FadeOut(userImageHolder, 0.2, () =>
                    {
                        userImageHolder.Width = 50;
                        userImageHolder.Height = 50;
                        username.Visibility = Visibility.Collapsed;
                        AnimationHelper.FadeIn(userImageHolder, 0.2);
                    });
                    sidePanel.AnimateWidth(82, 0.2, async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        paneExpander.IsEnabled = true;
                    });
                });
            }
            else
            {
                AnimationHelper.FadeOut(userImageHolder, 0.2, () =>
                {
                    userImageHolder.Width = 100;
                    userImageHolder.Height = 100;
                    username.FadeIn(0.1);
                    AnimationHelper.FadeIn(userImageHolder, 0.2);

                    sidePanel.AnimateWidth(225, 0.2, async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        paneExpander.IsEnabled = true;
                    });
                });
            }
        }

        private async void lsSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((DataGrid)sender).SelectedItem is Franchise fran)
            {
                lsSuggestion.Visibility = Visibility.Collapsed;
                cbSearch.Text = "";
                if (fran.BodyNumber != null)
                {
                    TablePanelSwap.Children.Clear();
                    ClickColorControl(FranchiseButton);
                    Ftable = (new TableView(Table.FRANCHISE));
                    TablePanelSwap.Children.Add(await Ftable.Fetch());
                    MainBody.selectedFranchise = fran;
                    TablePanelSwap.Children.Add((new FranchiseInformationView(TablePanelSwap)).Fetch());
                    
                }
            }
        }

        private void cbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            cbSearch_TextChanged(sender, null);
        }
    }
}