using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages.Input;
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

        public void displayToast(string message, double duration = 5)
        {
            if(!(string.IsNullOrEmpty(message) && string.IsNullOrWhiteSpace(message)))
            {
                System.Windows.Media.Brush bg = message.Contains("success") ?
                FindResource("BrushDeepGreen") as System.Windows.Media.Brush :
                FindResource("BrushRed") as System.Windows.Media.Brush;

                new Toast(gridToast, message, duration, true, 0.2, System.Windows.Media.Brushes.White, bg);
            }
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
                selectedButton.Background = System.Windows.Media.Brushes.White;
            }
            
            button.Background = FindResource("BrushYellow") as System.Windows.Media.Brush;
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
                        if (selectedButton == FranchiseButton && TablePanelSwap.Children.Count > 1)
                        {
                            GetFranchiseInList();
                        }
                        else if ((TableView.displayedTable) != null)
                        {
                            DataGrid datagrid = TableView.displayedTable;
                            ICollectionView collectionView = CollectionViewSource.GetDefaultView(datagrid.Items);

                            if (selectedButton == FranchiseButton)
                            {
                                collectionView.Filter = item =>
                                {
                                    if (item is Franchise franchise)
                                    {
                                        return franchise.BodyNumber.ToString().Contains(searchText) ||
                                            (franchise.Operator?.ToString()?.ToLower()?.Contains(searchText) ?? false) ||
                                            (franchise.Driver?.ToString()?.ToLower()?.Contains(searchText) ?? false);
                                    }
                                    return false;
                                };
                            }
                            else if (selectedButton == OperatorButton)
                            {
                                collectionView.Filter = item =>
                                {
                                    if (item is Franchise fran)
                                    {
                                        return fran?.Operator?.ToString()?.ToLower()?.Contains(searchText) ?? false;
                                    }
                                    return false;
                                };
                            }
                            else if (selectedButton == DriverButton)
                            {
                                collectionView.Filter = item =>
                                {
                                    if (item is Driver driver)
                                    {
                                        return driver.ToString()?.ToLower()?.Contains(searchText) ?? false;
                                    }
                                    return false;
                                };
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
            else if (cbSearch.Text.Length == 0) 
            {
                if (selectedButton != DashBoard)
                {
                    if (selectedButton == FranchiseButton && TablePanelSwap.Children.Count > 1)
                    {
                        SearchBarResize(true);
                    }
                    else if((TableView.displayedTable) != null)
                    {
                        DataGrid datagrid = TableView.displayedTable;
                        ICollectionView collectionView = CollectionViewSource.GetDefaultView(datagrid.Items);
                        collectionView.Filter = null;
                    }
                } else
                {
                    SearchBarResize(true);
                }
            } else
            {
                SearchBarResize(true);
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
                    lblSBHint.FadeOut(0.3);
                    sbBorder.AnimateHeight(272, 0.3);
                    epektos.IsEnabled = true;
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
                SearchBarResize(true);
            }
        }

        private void cbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            cbSearch.Text = "";
            SearchBarResize(true);
        }

        private void paneExpander_Click(object sender, RoutedEventArgs e)
        {
            if (profileActionCanvas.Visibility == Visibility.Visible || effectCanvas.Visibility == Visibility.Visible)
            {
                profileActionCanvas.FadeOut(0.3);
                effectCanvas.FadeOut(0.3);
            }
            paneExpander.IsEnabled = false;
            if (paneExpander.IsChecked == false)
            {
                username.FadeOut(0.2, () =>
                {
                    userImageHolder.FadeOut(0.2, () =>
                    {
                        userImageHolder.Width = 50;
                        userImageHolder.Height = 50;
                        username.Visibility = Visibility.Collapsed;
                        userImageHolder.FadeIn(0.2);
                    });
                    sidePanel.AnimateWidth(82, 0.2, () =>
                    {
                        paneExpander.IsEnabled = true;
                    });
                });
            }
            else
            {
                userImageHolder.FadeOut(0.2, () =>
                {
                    userImageHolder.Width = 100;
                    userImageHolder.Height = 100;
                    username.FadeIn(0.1);
                    userImageHolder.FadeIn(0.2);

                    sidePanel.AnimateWidth(260, 0.2, () =>
                    {
                        paneExpander.IsEnabled = true;
                    });
                });
            }
        }
        private void SearchBarResize(bool isMinimize = false)
        {

            /*if(sbBorder.Height != double.NaN)
            {
                if(sbBorder.Height == 40 && !isMinimize)
                {
                    sbBorder.AnimateHeight(272, 0.3);
                    epektos.IsEnabled = true;
                } 
                else
                {
                    sbBorder.AnimateHeight(40, 0.3);
                    epektos.IsEnabled = false;
                }
            }*/
            if (!double.IsNaN(sbBorder.Height))
            {
                if (sbBorder.Height == 40 && !isMinimize)
                {
                    sbBorder.AnimateHeight(272, 0.3);
                    epektos.IsEnabled = true;
                }
                else
                {
                    sbBorder.AnimateHeight(40, 0.3);
                    epektos.IsEnabled = false;
                }
            }

        }

        private void cbSearch_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if(!(cbSearch.Text.Length > 0))
            {
                lblSBHint.FadeIn(0.3);
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (cbSearch.IsFocused)
            {
                SearchBarResize(true);
            }
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            string defaultname = "N/A";
            int result = ControlWindow.ShowThreway("Profile", $"Employee name: {AppState.USER?.name?.legalName ?? defaultname}", "CHANGE PASSWORD", "EDIT PROFILE", Icons.NOTIFY);
            if(result == 1)
            {
                (new EditEmployee(AppState.USER, true, false)).Show();
            } else if(result == 0)
            {
                (new ChangePassword()).Show();
            }

            
        }

        private void userImageHolder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(paneExpander.IsChecked == true)
            {
                effectCanvas.FadeIn(0.1, null, () =>
                {
                    profileActionCanvas.FadeIn(0.2);
                });
            }
        }

        private void gridProfActBounding_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (profileActionCanvas.Visibility == Visibility.Visible || effectCanvas.Visibility == Visibility.Visible)
            {
                profileActionCanvas.FadeOut(0.3);
                effectCanvas.FadeOut(0.3);
            }
        }

        private void btnEditPass_Click(object sender, RoutedEventArgs e)
        {
            (new ChangePassword()).Show();
        }

        private void btnEditProf_Click(object sender, RoutedEventArgs e)
        {
            (new EditEmployee(AppState.USER, true, false)).Show();
        }

        private async void lsSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((DataGrid)sender).SelectedItem is Franchise fran)
            {
                cbSearch.Text = "";
                SearchBarResize();
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
            lblSBHint.FadeOut(0.3);
            cbSearch_TextChanged(sender, null);
        }

        private void gridReceipt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (gridReceipt.Visibility == Visibility.Visible)
                gridReceipt.FadeOut(0.2);
        }

        // Assumes all args are already tallied, not percentage.
        public void ShowReceipt(
            string termType,
            double amount,
            double processingFee,
            double cbu,
            double interest,
            double termLength)
        {
            double principal, loanRec, breakTot;
            string type = "";
            type = termType?.Trim().ToLower();

            if (type.Contains("short"))
            {
                principal = amount - (processingFee + cbu + interest);
                loanRec = principal / termLength;
                breakTot = loanRec;

                UpdateLabels(
                    "₱" + amount.ToString("N2"),
                    (processingFee > 0 ? "- " : "") + "₱" + processingFee.ToString("N2"),
                    (interest > 0 ? "- " : "") + "₱" + interest.ToString("N2"),
                    (SolidColorBrush)FindResource("BrushRed"),
                    "Already deducted.",
                    (SolidColorBrush)FindResource("BrushDeepGreen"),
                    (cbu > 0 ? "- " : "") + "₱" + cbu.ToString("N2"),
                    "₱" + principal.ToString("N2"),
                    "₱" + loanRec.ToString("N2"),
                    "₱" + loanRec.ToString("N2"));
            }
            else if (type.Contains("long"))
            {
                interest /= termLength;
                principal = amount - (processingFee + cbu);
                loanRec = principal / termLength;
                breakTot = loanRec + interest;

                UpdateLabels(
                    "₱" + amount.ToString("N2"),
                    (processingFee > 0 ? "- " : "") + "₱" + processingFee.ToString("N2"),
                    "Not deducted yet.",
                    (SolidColorBrush)FindResource("BrushDeepGreen"),
                    "₱" + interest.ToString("N2"),
                    (SolidColorBrush)FindResource("BrushDeepBlue"),
                    (cbu > 0 ? "- " : "") + "₱" + cbu.ToString("N2"),
                    "₱" + principal.ToString("N2"),
                    "₱" + loanRec.ToString("N2"),
                    "₱" + breakTot.ToString("N2"));
            }
            else if (type.Contains("emergency"))
            {
                interest /= termLength;
                principal = amount;
                loanRec = principal / termLength;
                breakTot = loanRec + interest;

                UpdateLabels(
                    "₱" + amount.ToString("N2"),
                    (processingFee > 0 ? "- " : "") + "₱" + processingFee.ToString("N2") + " (To Pay)",
                    "Not deducted yet.",
                    (SolidColorBrush)FindResource("BrushDeepGreen"),
                    "₱" + interest.ToString("N2"),
                    (SolidColorBrush)FindResource("BrushDeepBlue"),
                    (cbu > 0 ? "- " : "") + "₱" + cbu.ToString("N2") + " (To Pay)",
                    "₱" + principal.ToString("N2"),
                    "₱" + loanRec.ToString("N2"),
                    "₱" + breakTot.ToString("N2"));
            }
            else
            {
                if (gridReceipt.Visibility == Visibility.Visible)
                    gridReceipt.FadeOut(0.2);

                return;
            }

            gridReceipt.FadeIn(0.2);
            gridReceipt.Focus();

            void UpdateLabels(
                string loanAmount,
                string pfTotal,
                string interestTotal,
                SolidColorBrush interestTotalForeground,
                string interestRecievableTotal,
                SolidColorBrush interestRecForeground,
                string cbuTotal,
                string principalTotal,
                string loanReceivableTotal,
                string breakdownTotal)
            {
                lblLoanAmount.Content = loanAmount;
                lblPFTotal.Content = pfTotal;
                lblInterestTotal.Content = interestTotal;
                lblInterestTotal.Foreground = interestTotalForeground;
                lblCBUTotal.Content = cbuTotal;
                lblPrincipalTotal.Content = principalTotal;
                lblLoanRecievableTotal.Content = loanReceivableTotal;
                lblInterestRecievableTotal.Content = interestRecievableTotal;
                lblInterestRecievableTotal.Foreground = interestRecForeground;
                lblBreakdownTotal.Content = breakdownTotal;
                //lblPenalty.Content = "₱" + penalty.ToString("N2");

                if (amount > 1 && termLength >= 2)
                {
                    lblInTot.Content = $"* ₱ {(breakTot * termLength).ToString("N2")} in {termLength.ToString("N0")} mos.";
                    lblInTot.Visibility = Visibility.Visible;
                }
                else
                {
                    lblInTot.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void btnReceiptClose_Click(object sender, RoutedEventArgs e)
        {
            gridReceipt.FadeOut(0.2);
        }
    }
}