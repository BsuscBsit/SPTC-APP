using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages.Input;
using SPTC_APP.View.Styling;

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for TableView.xaml
    /// </summary>
    public partial class TableView : Window
    {
        private string table;
        private Operator oholder;
        private Driver dholder;
        public static DataGrid displayedTable = null;

        public TableView(string table)
        {
            InitializeComponent();
            this.table = table;
            if (table == Table.FRANCHISE && (AppState.USER?.position?.accesses[2] ?? false))
            {
                lblAddBtn.Content = "Add Franchise";
                btnAdd.Visibility = Visibility.Visible;
                
            }
            else if (table == Table.DRIVER && (AppState.USER?.position?.accesses[0] ?? false))
            {
                lblAddBtn.Content = "Add Driver";
                btnAdd.Visibility = Visibility.Visible;
            }
            else
            {
                btnAdd.Visibility = Visibility.Collapsed;
            }

           
            UpdateDefaultSidePanel();
        }

        private void UpdateDefaultSidePanel()
        {

            franchiseInformation.AnimateWidth(0, 0.3);
            if (table == Table.FRANCHISE)
            {
                btnManage.Visibility = Visibility.Visible;
            }
            if (table == Table.OPERATOR)
            {
                if ((AppState.USER?.position?.accesses[4] ?? false))
                {
                    btnEditProfile.Visibility = Visibility.Visible;
                }
                if ((AppState.USER?.position?.accesses[9] ?? false))
                {
                    btnGenerateid.Visibility = Visibility.Visible;
                }
            }
            if (table == Table.DRIVER)
            {
                
                if ((AppState.USER?.position?.accesses[3] ?? false))
                {
                    btnEditProfile.Visibility = Visibility.Visible;
                }
                if ((AppState.USER?.position?.accesses[9] ?? false))
                {
                    btnGenerateid.Visibility = Visibility.Visible;
                }
                if ((AppState.USER?.position?.accesses[12] ?? false))
                {
                    btnAddViolation.Visibility = Visibility.Visible;
                }

            }
        }

        private async Task UpdateTableAsync()
        {
            TableGrid.Items.Clear();
            TableGrid.Items.Clear();

            TableGrid.SelectedCellsChanged += TableSelectedChanged;
            if (table == Table.FRANCHISE)
            {
                int batchSize = AppState.TABLE_BATCH_SIZE;
                int pageIndex = 0;
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("Operator.name.legalName", "OPERATOR NAME", minWidth: 140, backgroundColor: Brushes.LightBlue),
                    new ColumnConfiguration("BodyNumber", "BODY NO.", minWidth: 80),
                    new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", minWidth: 100),
                    new ColumnConfiguration("MTOPNo", "MTOP NO.", minWidth: 100),
                    new ColumnConfiguration("MonthlyDues", "MONTHLY DUE", minWidth: 100),
                };
                DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(TableGrid, columnConfigurations);



                while (true)
                {
                    List<Franchise> batch = await Task.Run(() =>
                    {
                        return (new TableObject<Franchise>(Table.FRANCHISE_S, Select.F, Where.LATEST_FRANCHISE, pageIndex * batchSize, batchSize)).data;
                    });

                    if (batch.Count == 0)
                        break;

                    foreach (var obj in batch)
                    {
                        TableGrid.Items.Add(obj);
                        //await Task.Delay(2000);
                    }

                    //await Task.Delay(200);

                    pageIndex++;
                }

                TableGrid.SelectedCellsChanged += TableSelectedChanged;
            }
            else if (table == Table.OPERATOR)
            {
                int batchSize = AppState.TABLE_BATCH_SIZE;
                int pageIndex = 0;
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("Operator.name.legalName", "NAME", minWidth: 140),
                    new ColumnConfiguration("BodyNumber", "BODY NO.", minWidth: 80),
                    new ColumnConfiguration("LicenseNO", "PLATE NO.", minWidth: 100),
                    new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", minWidth: 100),
                    new ColumnConfiguration("MonthlyDues", "MONTHLY DUE", minWidth: 100),
                };
                DataGridHelper<Operator> dataGridHelper = new DataGridHelper<Operator>(TableGrid, columnConfigurations);


                while (true)
                {
                    List<Franchise> batch = await Task.Run(() =>
                    {
                        return Retrieve.GetDataUsingQuery<Franchise>(RequestQuery.GET_ALL_OPERATOR_FOR_DISPLAY(pageIndex*batchSize, batchSize));
                    });

                    if (batch.Count == 0)
                        break;

                    foreach (var obj in batch)
                    {
                        TableGrid.Items.Add(obj);
                        //await Task.Delay(200);
                    }

                    // await Task.Delay(200);

                    pageIndex++;
                }

                TableGrid.SelectedCellsChanged += TableSelectedChanged;
            }
            else if (table == Table.DRIVER)
            {
                int batchSize = AppState.TABLE_BATCH_SIZE;
                int pageIndex = 0;

                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("name.legalName", "NAME", minWidth: 140),
                    new ColumnConfiguration("address", "ADDRESS", minWidth: 100),
                    new ColumnConfiguration("franchise.BodyNumber", "BODY NO.", minWidth: 80),
                    new ColumnConfiguration("franchise.LicenseNO", "PLATE NO.", minWidth: 80),
                    new ColumnConfiguration("franchise.Operator", "OPERATOR", minWidth: 120),
                };
                DataGridHelper<Driver> dataGridHelper = new DataGridHelper<Driver>(TableGrid, columnConfigurations);



                while (true)
                {
                    List<Driver> batch = await Task.Run(() =>
                    {

                        return (new TableObject<Driver>(Table.DRIVER, Select.ALL, Where.ALL_NOTDELETED, pageIndex * batchSize, batchSize)).data;
                    });

                    if (batch.Count == 0)
                        break;

                    foreach (var obj in batch)
                    {
                        obj.UpdateFranchise();
                        TableGrid.Items.Add(obj);
                        //await Task.Delay(200);
                    }

                    //await Task.Delay(200);

                    pageIndex++;
                }

                TableGrid.SelectedCellsChanged += TableSelectedChanged;
            }
        }

        private void TableSelectedChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            if (grid.SelectedItems.Count > 0)
            {
                if (table == Table.FRANCHISE)
                {
                    MainBody.selectedFranchise = (Franchise)grid.SelectedItem;
                    ValuePairFI(lblF1, "Operator Name: ", lblI1, MainBody.selectedFranchise?.Operator?.ToString() ?? "");
                    ValuePairFI(lblF2, "Membership: ", lblI2, MainBody.selectedFranchise?.Operator?.dateOfMembership.ToString("MMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Body No: ", lblI3, MainBody.selectedFranchise?.BodyNumber.ToString() ?? "");

                    if (AppState.USER?.position?.title == AppState.Employees[0])
                    {
                        ValuePairFI(lblF4, "Loan Balance: ", lblI4, "P "+MainBody.selectedFranchise?.LoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF5, "LT Loan Balance: ", lblI5, "P "+MainBody.selectedFranchise?.LongTermLoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF6, "Reference: ", lblI6, MainBody.selectedFranchise?.Owner?.ToString() ?? "");
                        ValuePairFI(lblF7, "Driver: ", lblI7, MainBody.selectedFranchise?.Driver?.ToString() ?? "");
                        
                    } 
                    else if(AppState.USER?.position?.title == AppState.Employees[1])
                    {
                        ValuePairFI(lblF4, "TIN No.: ", lblI4, MainBody.selectedFranchise?.Operator?.tinNumber?.ToString() ?? "");
                        ValuePairFI(lblF5, "VOTERS ID No.: ", lblI5, MainBody.selectedFranchise?.Operator?.votersNumbewr?.ToString() ?? "");
                        ValuePairFI(lblF6, "Reference: ", lblI6, MainBody.selectedFranchise?.Owner?.ToString() ?? "");
                        ValuePairFI(lblF7, "Driver: ", lblI7, MainBody.selectedFranchise?.Driver?.ToString() ?? "");
                    }
                    else if(AppState.USER?.position?.title == AppState.Employees[2])
                    {
                        ValuePairFI(lblF4, "Share Capital: ", lblI4, "P " + MainBody.selectedFranchise?.ShareCapital.ToString("0.00") ?? "0");
                        ValuePairFI(lblF5, "Loan Balance: ", lblI5, "P " + MainBody.selectedFranchise?.LoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF6, "LT Loan Balance: ", lblI6, "P " + MainBody.selectedFranchise?.LongTermLoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF7, "Total Due for "+DateTime.Now.ToString("MMM")+": ", lblI7, "P " + MainBody.selectedFranchise?.MonthlyDues.ToString("0.00") ?? "");
                    } 
                    else if(AppState.USER?.position?.title == AppState.Employees[3])
                    {
                        ValuePairFI(lblF4, "PLATE No.: ", lblI4, MainBody.selectedFranchise?.LicenseNO ?? "");
                        ValuePairFI(lblF5, "MTOP No.: ", lblI5, MainBody.selectedFranchise?.MTOPNo ?? "");
                        ValuePairFI(lblF6, "TIN No.: ", lblI6, MainBody.selectedFranchise?.Operator?.tinNumber?.ToString() ?? "");
                        ValuePairFI(lblF7, "VOTERS ID No.: ", lblI7, MainBody.selectedFranchise?.Operator?.votersNumbewr?.ToString() ?? "");
                    }
                    if (MainBody.selectedFranchise?.Operator?.image?.GetSource() != null)
                    {
                        imgUserProfilePic.ImageSource = MainBody.selectedFranchise?.Operator?.image?.GetSource();
                    } 
                    else
                    {
                        imgUserProfilePic.ImageSource = new BitmapImage(new Uri("pack://application:,,,/SPTC APP;component/View/Images/icons/person.png"));
                    }
                }
                else if (table == Table.OPERATOR)
                {
                    MainBody.selectedFranchise = ((Franchise)grid.SelectedItem);
                    oholder = MainBody.selectedFranchise.Operator;
                    ValuePairFI(lblF1, "Operator Name: ", lblI1, oholder?.name?.legalName?.ToString());
                    ValuePairFI(lblF2, "Membership: ", lblI2, MainBody.selectedFranchise?.Operator?.dateOfMembership.ToString("MMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Body No: ", lblI3, MainBody.selectedFranchise?.BodyNumber?.ToString() ?? "");

                    if (AppState.USER?.position?.title == AppState.Employees[0])
                    {
                        ValuePairFI(lblF4, "Loan Balance: ", lblI4, "P " + MainBody.selectedFranchise?.LoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF5, "LT Loan Balance: ", lblI5, "P " + MainBody.selectedFranchise?.LongTermLoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF6, "TIN No.: ", lblI6, MainBody.selectedFranchise?.Operator?.tinNumber?.ToString() ?? "");
                        ValuePairFI(lblF7, "VOTERS ID No.: ", lblI7, MainBody.selectedFranchise?.Operator?.votersNumbewr?.ToString() ?? "");
                    }
                    else if (AppState.USER?.position?.title == AppState.Employees[1])
                    {
                        ValuePairFI(lblF4, "TIN No.: ", lblI4, MainBody.selectedFranchise?.Operator?.tinNumber?.ToString() ?? "");
                        ValuePairFI(lblF5, "VOTERS ID No.: ", lblI5, MainBody.selectedFranchise?.Operator?.votersNumbewr?.ToString() ?? "");
                        ValuePairFI(lblF6, "Reference: ", lblI6, MainBody.selectedFranchise?.Owner?.ToString() ?? "");
                        ValuePairFI(lblF7, "Driver: ", lblI7, MainBody.selectedFranchise?.Driver?.ToString() ?? "");
                    }
                    else if (AppState.USER?.position?.title == AppState.Employees[2])
                    {
                        ValuePairFI(lblF4, "Share Capital: ", lblI4, "P " + MainBody.selectedFranchise?.ShareCapital.ToString("0.00") ?? "0");
                        ValuePairFI(lblF5, "Loan Balance: ", lblI5, "P " + MainBody.selectedFranchise?.LoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF6, "LT Loan Balance: ", lblI6, "P " + MainBody.selectedFranchise?.LongTermLoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF7, "Due for " + DateTime.Now.ToString("MMM") + ": ", lblI7, "P " + MainBody.selectedFranchise?.MonthlyDues.ToString("0.00") ?? "");
                    }
                    else if (AppState.USER?.position?.title == AppState.Employees[3])
                    {
                        ValuePairFI(lblF4, "PLATE No.: ", lblI4, MainBody.selectedFranchise?.LicenseNO ?? "");
                        ValuePairFI(lblF5, "MTOP No.: ", lblI5, MainBody.selectedFranchise?.MTOPNo ?? "");
                        ValuePairFI(lblF6, "TIN No.: ", lblI6, MainBody.selectedFranchise?.Operator?.tinNumber?.ToString() ?? "");
                        ValuePairFI(lblF7, "VOTERS ID No.: ", lblI7, MainBody.selectedFranchise?.Operator?.votersNumbewr?.ToString() ?? "");
                    }
                    if (MainBody.selectedFranchise?.Operator?.image?.GetSource() != null)
                    {
                        imgUserProfilePic.ImageSource = MainBody.selectedFranchise?.Operator?.image?.GetSource();
                    }
                    else
                    {
                        imgUserProfilePic.ImageSource = new BitmapImage(new Uri("pack://application:,,,/SPTC APP;component/View/Images/icons/person.png"));
                    }
                }
                else if (table == Table.DRIVER)
                {
                    MainBody.selectedFranchise = ((Driver)grid.SelectedItem).franchise;
                    dholder = (Driver)grid.SelectedItem;
                    ValuePairFI(lblF1, "Driver's Name: ", lblI1, dholder?.name?.legalName?.ToString());
                    ValuePairFI(lblF2, "Membership: ", lblI2, MainBody.selectedFranchise?.Driver?.dateOfMembership.ToString("MMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Address: ", lblI3, MainBody.selectedFranchise?.Driver?.address?.ToString() ?? "");
                    ValuePairFI(lblF4, "Body No.: ", lblI4, MainBody.selectedFranchise?.BodyNumber?.ToString() ?? "");
                    ValuePairFI(lblF5, "License No: ", lblI5, MainBody.selectedFranchise?.LicenseNO?.ToString() ?? "");
                    ValuePairFI(lblF6, "Violation:  ", lblI6, MainBody.selectedFranchise?.Driver?.violationCount.ToString() ?? "");
                    if (MainBody.selectedFranchise?.Driver?.image?.GetSource() != null)
                    {
                        imgUserProfilePic.ImageSource = MainBody.selectedFranchise?.Driver?.image?.GetSource();
                    }
                    else
                    {
                        imgUserProfilePic.ImageSource = new BitmapImage(new Uri("pack://application:,,,/SPTC APP;component/View/Images/icons/person.png"));
                    }
                    if (MainBody.selectedFranchise?.Driver?.isSuspended ?? false)
                    {
                        ValuePairFI(lblF7, "Is Suspended:", lblI7, $"YES ({MainBody.selectedFranchise.Driver.violation.dDateEnd})");
                        lblI7.Foreground = Brushes.Red;
                        lblI7.FontWeight = FontWeights.Black;
                    }
                    else
                    {
                        ValuePairFI(lblF7, "Is Suspended:", lblI7, "NO");
                        lblI7.Foreground = Brushes.Green;
                        lblI7.FontWeight = FontWeights.Black;
                    }
                }
                
                if (MainBody.selectedFranchise == null)
                {
                    btnAddViolation.IsEnabled = false;
                    btnGenerateid.IsEnabled = false;
                }
                else
                {
                    btnAddViolation.IsEnabled = true;
                    btnGenerateid.IsEnabled = true;
                }
            }
            if (MainBody.selectedFranchise != null && table == Table.OPERATOR && AppState.USER?.position.title == AppState.Employees[2])
            {
                if ((AppState.USER?.position?.accesses[13] ?? false))
                {
                    btnAddShareCapital.Visibility = Visibility.Visible;
                }
                if ((AppState.USER?.position?.accesses[14] ?? false))
                {
                    if (MainBody.selectedFranchise?.GetLoans()?.Count <= 0)
                    {
                        btnAddLoan.Content = "Apply for Loan";
                    } else
                    {
                        btnAddLoan.Content = "Pay Loan";
                    }
                    btnAddLoan.Visibility = Visibility.Visible;
                }
                if ((AppState.USER?.position?.accesses[15] ?? false))
                {
                    if (MainBody.selectedFranchise?.GetLTLoans()?.Count <= 0)
                    {
                        btnAddLTLoan.Content = "Apply for LTLoan";
                    } else
                    {
                        btnAddLTLoan.Content = "Pay LTLoan";
                    }
                    btnAddLTLoan.Visibility = Visibility.Visible;
                }
            } else
            {
                btnAddShareCapital.Visibility = Visibility.Collapsed;
                btnAddLoan.Visibility = Visibility.Collapsed;
                btnAddLTLoan.Visibility = Visibility.Collapsed;
            }

            franchiseInformation.AnimateWidth(264, 0.3);
        }

        private void ValuePairFI(Label lblF, string name, Label lblI, string value)
        {
            lblF.Visibility = Visibility.Visible;
            lblI.Visibility = Visibility.Visible;
            lblF.Content = name;
            lblI.Content = (string.IsNullOrEmpty(value)) ? "N/A" : value;
        }

        public async Task<Grid> Fetch()
        {
            TableView.displayedTable = TableGrid;
            Task task = UpdateTableAsync();
            await Task.Delay(1);
            if (franchisePanel.Parent != null)
            {
                Window currentParent = franchisePanel.Parent as Window;
                if (currentParent != null)
                {
                    currentParent.Content = null;
                }
            }
            this.Close();
            return franchisePanel;
        }

        private async void btnManage_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.FRANCHISE)
                {
                    if(franchisePanel.Parent is Grid par)
                        par.Children.Add((new FranchiseInformationView(par)).Fetch());
                }
                await UpdateTableAsync();
            }
            else
            {
                ControlWindow.ShowStatic("No Franchise detected!", "Cannot proceed. Dreate new Franchise first", Icons.ERROR);
            }
        }

        private async void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (table == Table.OPERATOR)
            {
                (new EditProfile(MainBody.selectedFranchise, oholder, General.OPERATOR)).ShowDialog();
            }
            else if (table == Table.DRIVER)
            {
                (new EditProfile(MainBody.selectedFranchise, dholder, General.DRIVER)).ShowDialog();
            }

            await UpdateTableAsync();

        }
        private async void btnGenerateid_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.OPERATOR)
                {
                    (new GenerateID(MainBody.selectedFranchise, false)).ShowDialog();
                }
                else if (table == Table.DRIVER)
                {
                    (new GenerateID(MainBody.selectedFranchise, true)).ShowDialog();
                }
                await UpdateTableAsync();
            }
            else
            {
                ControlWindow.ShowStatic("No Franchise detected!", "Cannot proceed. Create new Franchise first", Icons.ERROR);
            }
        }
        private async void btnAddViolation_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.DRIVER)
                    (new ViolationInput(MainBody.selectedFranchise)).ShowDialog();
                await UpdateTableAsync();
            }
            else
            {
                ControlWindow.ShowStatic("No Franchise detected!", "Cannot proceed. Create new Franchise first", Icons.ERROR);
            }
        }
        private async void btnAddClick(object sender, RoutedEventArgs e)
        {
            if (table == Table.FRANCHISE)
                (new InputFranchiseView()).ShowDialog();
            if (table == Table.DRIVER)
                (new NewOptr_Drv(MainBody.selectedFranchise, General.DRIVER)).ShowDialog();
            await UpdateTableAsync();
        }

        private void btnAddShareCapital_Click(object sender, RoutedEventArgs e)
        {
            (new AddShareCaptital(MainBody.selectedFranchise)).Show();
        }

        private void btnAddLoan_Click(object sender, RoutedEventArgs e)
        {
            (new AddLoan(MainBody.selectedFranchise)).Show();
        }

        private void btnAddLTLoan_Click(object sender, RoutedEventArgs e)
        {
            (new AddLTLoan(MainBody.selectedFranchise)).Show();
        }
    }
}
