using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using SPTC_APP.View.Styling;
using static Org.BouncyCastle.Math.EC.ECCurve;

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
            filterOptions.Visibility = Visibility.Hidden;
        }

        private void UpdateDefaultSidePanel()
        {

            franchiseInformation.AnimateWidth(0, 0.3, () =>
            {
                btnHideFI.Visibility = Visibility.Hidden;
            });
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
                    new ColumnConfiguration("Operator.name.legalName", "OPERATOR NAME", minWidth: 140, filter:franchiseFilter),
                    new ColumnConfiguration("Driver.name.legalName", "DRIVER NAME", minWidth: 140, filter:franchiseFilter),
                    new ColumnConfiguration("displayBuyingDate", "OWNERSHIP DATE", minWidth:120, filter:franchiseFilter),
                    new ColumnConfiguration("BodyNumber", "BODY NO.", minWidth: 80, isCenter:true, isNumeric: true, filter:franchiseFilter),
                    new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", minWidth: 100, isCenter:true, isNumeric: true, filter:franchiseFilter),
                    new ColumnConfiguration("MTOPNo", "MTOP NO.", minWidth: 100, isCenter:true, isNumeric: true, filter:franchiseFilter),
                    new ColumnConfiguration("MonthlyDues", "PAYMENT DUE", minWidth: 100, isCenter:true, isNumeric: true, haspeso:true, filter:franchiseFilter),
                    new ColumnConfiguration("LoanBalance", "LOAN BALANCE", minWidth: 100, isCenter:true, isNumeric: true, haspeso:true, filter:franchiseFilter),
                    new ColumnConfiguration("LongTermLoanBalance", "LTLOAN BALANCE", minWidth: 100, isCenter:true, isNumeric: true, haspeso:true, filter:franchiseFilter),
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
                    new ColumnConfiguration("Operator.name.legalName", "NAME", minWidth: 140, filter:opratorFilter),
                    new ColumnConfiguration("Driver.name.legalName", "DRIVER NAME", minWidth: 140, filter:opratorFilter),
                    new ColumnConfiguration("Operator.address", "ADDRESS", minWidth: 100, filter:opratorFilter),
                    new ColumnConfiguration("Operator.displayBirth", "BIRTHDAY", minWidth:120, filter:opratorFilter),
                    new ColumnConfiguration("displayBuyingDate", "MEMBERSHIP DATE", minWidth:120, filter:opratorFilter),
                    new ColumnConfiguration("Operator.emergencyContact", "CONTACT", minWidth: 120, isNumeric: true, filter:opratorFilter),
                    new ColumnConfiguration("Operator.tinNumber", "TIN NUMBER", minWidth:120, filter:opratorFilter),
                    new ColumnConfiguration("Operator.votersNumbewr", "VOTERS ID", minWidth:120, filter:opratorFilter),
                    new ColumnConfiguration("BodyNumber", "BODY NO.", minWidth: 80, isCenter:true, isNumeric: true, filter:opratorFilter),
                    new ColumnConfiguration("PlateNo", "PLATE NO.", minWidth: 100, isCenter: true,isNumeric: true, filter:opratorFilter),
                    new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", minWidth: 100, isCenter : true, isNumeric : true, filter:opratorFilter),
                    new ColumnConfiguration("MonthlyDues", "PAYMENT DUE", minWidth: 100, isCenter:true, isNumeric: true, haspeso:true, filter:opratorFilter),
                     new ColumnConfiguration("LoanBalance", "LOAN BALANCE", minWidth: 100, isCenter:true, isNumeric: true, haspeso:true, filter:opratorFilter),
                    new ColumnConfiguration("LongTermLoanBalance", "LTLOAN BALANCE", minWidth: 100, isCenter:true, isNumeric: true, haspeso:true, filter:opratorFilter),
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
                    new ColumnConfiguration("name.legalName", "NAME", minWidth: 140, filter:driverFilter),
                    new ColumnConfiguration("address", "ADDRESS", minWidth: 100, filter:driverFilter),
                    new ColumnConfiguration("displayBirth", "BIRTHDAY", minWidth:120, filter:driverFilter),
                    new ColumnConfiguration("Operator.emergencyContact", "CONTACT", minWidth: 120, isNumeric: true, filter:driverFilter),
                    new ColumnConfiguration("franchise.BodyNumber", "BODY NO.", minWidth: 80, isCenter : true, isNumeric : true, filter : driverFilter),
                    new ColumnConfiguration("licenseNo", "LICENSE", minWidth: 80, isCenter:true, isNumeric: true, filter:driverFilter),
                    new ColumnConfiguration("Franchise.PlateNo", "PLATE NO.", minWidth: 100, isCenter: true,isNumeric: true, filter:driverFilter),
                    new ColumnConfiguration("franchise.Operator", "OPERATOR", minWidth: 120, filter:driverFilter),
                    
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
                    ValuePairFI(lblF1, "Operator: ", lblI1, MainBody.selectedFranchise?.Operator?.ToString() ?? "");
                    ValuePairFI(lblF2, "Membership: ", lblI2, MainBody.selectedFranchise?.Operator?.dateOfMembership.ToString("MMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Body No: ", lblI3, MainBody.selectedFranchise?.BodyNumber.ToString() ?? "");

                    if (AppState.USER?.position?.title == AppState.Employees[0])
                    {
                        ValuePairFI(lblF4, "Loan Balance: ", lblI4, "\u20B1 "+ MainBody.selectedFranchise?.LoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF5, "LT Loan Balance: ", lblI5, "\u20B1 "+ MainBody.selectedFranchise?.LongTermLoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF6, "Total Shares: ", lblI6, "\u20B1 " + MainBody.selectedFranchise?.ShareCapital.ToString("0.00") ?? "");
                        ValuePairFI(lblF7, "Driver: ", lblI7, MainBody.selectedFranchise?.Driver?.ToString() ?? "");
                        
                    } 
                    else if(AppState.USER?.position?.title == AppState.Employees[1])
                    {
                        ValuePairFI(lblF4, "TIN No.: ", lblI4, MainBody.selectedFranchise?.Operator?.tinNumber?.ToString() ?? "");
                        ValuePairFI(lblF5, "VOTERS ID No.: ", lblI5, MainBody.selectedFranchise?.Operator?.votersNumbewr?.ToString() ?? "");
                        ValuePairFI(lblF6, "Total Shares: ", lblI6, "\u20B1 " + MainBody.selectedFranchise?.ShareCapital.ToString("0.00") ?? "");
                        ValuePairFI(lblF7, "Driver: ", lblI7, MainBody.selectedFranchise?.Driver?.ToString() ?? "");
                    }
                    else if(AppState.USER?.position?.title == AppState.Employees[2])
                    {
                        ValuePairFI(lblF4, "Share Capital: ", lblI4, "\u20B1 "+ MainBody.selectedFranchise?.ShareCapital.ToString("0.00") ?? "0");
                        ValuePairFI(lblF5, "Loan Balance: ", lblI5, "\u20B1 "+ MainBody.selectedFranchise?.LoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF6, "LT Loan Balance: ", lblI6, "\u20B1 "+ MainBody.selectedFranchise?.LongTermLoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF7, "Total Due for "+DateTime.Now.ToString("MMM")+": ", lblI7, "\u20B1 "+ MainBody.selectedFranchise?.MonthlyDues.ToString("0.00") ?? "");
                    } 
                    else if(AppState.USER?.position?.title == AppState.Employees[3])
                    {
                        ValuePairFI(lblF4, "PLATE No.: ", lblI4, MainBody.selectedFranchise?.PlateNo ?? "");
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
                    ValuePairFI(lblF1, "Operator: ", lblI1, oholder?.name?.legalName?.ToString());
                    ValuePairFI(lblF2, "Membership: ", lblI2, MainBody.selectedFranchise?.Operator?.dateOfMembership.ToString("MMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Body No: ", lblI3, MainBody.selectedFranchise?.BodyNumber?.ToString() ?? "");

                    if (AppState.USER?.position?.title == AppState.Employees[0])
                    {
                        ValuePairFI(lblF4, "Loan Balance: ", lblI4, "\u20B1 "+ MainBody.selectedFranchise?.LoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF5, "LT Loan Balance: ", lblI5, "\u20B1 "+ MainBody.selectedFranchise?.LongTermLoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF6, "TIN No.: ", lblI6, MainBody.selectedFranchise?.Operator?.tinNumber?.ToString() ?? "");
                        ValuePairFI(lblF7, "VOTERS ID No.: ", lblI7, MainBody.selectedFranchise?.Operator?.votersNumbewr?.ToString() ?? "");
                    }
                    else if (AppState.USER?.position?.title == AppState.Employees[1])
                    {
                        ValuePairFI(lblF4, "PLATE No.: ", lblI4, MainBody.selectedFranchise?.PlateNo ?? "");
                        ValuePairFI(lblF5, "MTOP No.: ", lblI5, MainBody.selectedFranchise?.MTOPNo ?? "");
                        ValuePairFI(lblF6, "TIN No.: ", lblI6, MainBody.selectedFranchise?.Operator?.tinNumber?.ToString() ?? "");
                        ValuePairFI(lblF7, "VOTERS ID No.: ", lblI7, MainBody.selectedFranchise?.Operator?.votersNumbewr?.ToString() ?? "");
                    }
                    else if (AppState.USER?.position?.title == AppState.Employees[2])
                    {
                        ValuePairFI(lblF4, "Share Capital: ", lblI4, "\u20B1 "+ MainBody.selectedFranchise?.ShareCapital.ToString("0.00") ?? "0");
                        ValuePairFI(lblF5, "Loan Balance: ", lblI5, "\u20B1 "+ MainBody.selectedFranchise?.LoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF6, "LT Loan Balance: ", lblI6, "\u20B1 " + MainBody.selectedFranchise?.LongTermLoanBalance.ToString("0.00") ?? "0");
                        ValuePairFI(lblF7, "Due for " + DateTime.Now.ToString("MMM") + ": ", lblI7, "\u20B1 "+ MainBody.selectedFranchise?.MonthlyDues.ToString("0.00") ?? "");
                    }
                    else if (AppState.USER?.position?.title == AppState.Employees[3])
                    {
                        ValuePairFI(lblF4, "PLATE No.: ", lblI4, MainBody.selectedFranchise?.PlateNo ?? "");
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
                    ValuePairFI(lblF1, "Driver: ", lblI1, dholder?.name?.legalName?.ToString());
                    ValuePairFI(lblF2, "Membership: ", lblI2, MainBody.selectedFranchise?.Driver?.dateOfMembership.ToString("MMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Address: ", lblI3, MainBody.selectedFranchise?.Driver?.address?.ToString() ?? "");
                    ValuePairFI(lblF4, "Body No.: ", lblI4, MainBody.selectedFranchise?.BodyNumber?.ToString() ?? "");
                    ValuePairFI(lblF5, "License No.: ", lblI5, MainBody.selectedFranchise?.Driver?.licenseNo?.ToString() ?? "");
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
                        ValuePairFI(lblF7, "Suspended:", lblI7, $"YES ( {MainBody.selectedFranchise.Driver.violation.dDateEnd} )");
                        lblI7.Foreground = FindResource("BrushRed") as Brush;
                        lblI7.FontWeight = FontWeights.SemiBold;
                    }
                    else
                    {
                        ValuePairFI(lblF7, "Suspended:", lblI7, "NO");
                        lblI7.Foreground = FindResource("BrushDeepGreen") as Brush;
                        lblI7.FontWeight = FontWeights.SemiBold;
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

            franchiseInformation.AnimateWidth(325, 0.3, () =>
            {
                if(btnHideFI.Visibility == Visibility.Hidden)
                    btnHideFI.FadeIn(0.3);
            });
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
                ControlWindow.ShowStatic("No Franchise Found", "Please create a new franchise first.", Icons.ERROR);
            }
        }
        private async void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (table == Table.OPERATOR)
            {
                (new EditProfile(MainBody.selectedFranchise, General.OPERATOR)).ShowDialog();
            }
            else if (table == Table.DRIVER)
            {
                (new EditProfile(MainBody.selectedFranchise, General.DRIVER, dholder)).ShowDialog();
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
                ControlWindow.ShowStatic("No Franchise Found", "Please create a new franchise first.", Icons.ERROR);
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
                ControlWindow.ShowStatic("No Franchise Found", "Please create a new franchise first.", Icons.ERROR);
            }
        }
        private async void btnAddClick(object sender, RoutedEventArgs e)
        {
            if (table == Table.FRANCHISE)
                (new InputFranchiseView()).ShowDialog();
            if (table == Table.DRIVER)
                (new EditProfile(MainBody.selectedFranchise, General.NEW_DRIVER)).ShowDialog();
            await UpdateTableAsync();
        }
        private void btnAddShareCapital_Click(object sender, RoutedEventArgs e)
        {
            (new AddShareCaptital(MainBody.selectedFranchise)).Show();
        }
        private void btnAddLoan_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise?.GetLoans()?.Count <= 0)
            {

                (new ApplyLoan(MainBody.selectedFranchise)).Show();
            }
            else
            {

                (new AddLoan(MainBody.selectedFranchise)).Show();
            }
        }
        private void btnAddLTLoan_Click(object sender, RoutedEventArgs e)
        {
            if(MainBody.selectedFranchise?.GetLTLoans()?.Count <= 0)
            {
                (new ApplyLoan(MainBody.selectedFranchise)).Show();
            } else
            {

                (new AddLTLoan(MainBody.selectedFranchise)).Show();
            }
        }

        //CONTROL THIS IN FILTER AS CHECKBOXES
        static Dictionary<string, bool> driverFilter = new Dictionary<string, bool>()
        {
            // DRIVER DEFAULT FILTER
            { "NAME", true },
            { "ADDRESS", true },
            { "BODY NO.", true },
            { "LICENSE", true },
            { "PAYMENT DUE", true },

            { "BIRTHDAY", false },
            { "CONTACT", false },
            { "PLATE NO.", false },
        };
        static Dictionary<string, bool> franchiseFilter = new Dictionary<string, bool>()
        {
            // FRANCHISE DEFAULT FILTER
            { "OPERATOR NAME", true },
            { "BODY NO.", true },
            { "SHARE CAPITAL", true },
            { "MTOP NO.", true },
            { "PAYMENT DUE", true },

            { "DRIVER NAME", false },
            { "LOAN BALANCE", false },
            { "LTLOAN BALANCE", false },
            { "OWNERSHIP DATE", false },
        };
        static Dictionary<string, bool> opratorFilter = new Dictionary<string, bool>()
        {
            // OPERATOR DEFAULT FILTER
            { "NAME", true },
            { "BODY NO.", true },
            { "PLATE NO.", true },
            { "SHARE CAPITAL", true },
            { "PAYMENT DUE", true },

            { "ADDRESS", false },
            { "BIRTHDAY", false },
            { "CONTACT", false },
            { "MEMBERSHIP DATE", false },
            { "TIN NUMBER", false },
            { "VOTERS ID", false },
            { "DRIVER NAME", false },
            { "LOAN BALANCE", false },
            { "LTLOAN BALANCE", false },
        };

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if(filterOptions.Visibility != Visibility.Visible)
            {
                filterOptions.FadeIn(0.2, () =>
                {
                    showFilterChoices(this.table);
                });
            }
            else
            {
                filterOptions.FadeOut(0.2);
            }
        }
        
        private void btnHideFI_Click(object sender, RoutedEventArgs e)
        {
            franchiseInformation.AnimateWidth(0, 0.3, () =>
            {
                btnHideFI.Visibility = Visibility.Hidden;
            });
        }

        private void btnApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            //Setting the filters
            DataGrid datagrid = TableGrid;
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(datagrid.Items);

            if (table == Table.FRANCHISE)
            {

                franchiseFilter["OPERATOR NAME"] = franOpName.IsChecked ?? false;
                franchiseFilter["BODY NO."] = franBnum.IsChecked ?? false;
                franchiseFilter["SHARE CAPITAL"] = franShareCap.IsChecked ?? false;
                franchiseFilter["MTOP NO."] = franMTOP.IsChecked ?? false;
                franchiseFilter["PAYMENT DUE"] = franPayDue.IsChecked ?? false;
                franchiseFilter["DRIVER NAME"] = franDrName.IsChecked ?? false;
                franchiseFilter["OWNERSHIP DATE"] = franOwnDate.IsChecked ?? false;
                franchiseFilter["LOAN BALANCE"] = cbFrWLoan.IsChecked ?? false;
                franchiseFilter["LTLOAN BALANCE"] = cbFrWLtLoan.IsChecked ?? false;

                collectionView.Filter = item =>
                {
                    if (item is Franchise franchise)
                    {
                        bool shown = true;
                        if (cbFrWLoan.IsChecked ?? false)
                        {
                            shown = franchise.LoanBalance > 0;
                        }
                        if (cbFrWLtLoan.IsChecked ?? false)
                        {
                            shown = franchise.LongTermLoanBalance > 0;
                        }
                        return shown;
                    }
                    return true;
                };
                foreach (DataGridColumn column in TableGrid.Columns)
                {
                    if (column.Header != null && franchiseFilter.TryGetValue(column.Header.ToString(), out bool isVisible))
                    {
                        column.HeaderStyle = new Style()
                        {
                            Setters = 
                            {
                                new Setter(Control.FontWeightProperty, FontWeights.Bold),
                                new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                                new Setter(Control.FontFamilyProperty, new FontFamily("Inter")),
                                new Setter(VisibilityProperty, isVisible ? Visibility.Visible : Visibility.Collapsed)
                            }
                        };
                        column.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                    }
                }


            }
            else if (table == Table.OPERATOR) 
            {
                opratorFilter["NAME"] = operName.IsChecked ?? false;
                opratorFilter["BODY NO."] = operBnum.IsChecked ?? false;
                opratorFilter["PLATE NO."] = operPlate.IsChecked ?? false;
                opratorFilter["SHARE CAPITAL"] = operShareCap.IsChecked ?? false;
                opratorFilter["PAYMENT DUE"] = operPaymentDue.IsChecked ?? false;
                opratorFilter["ADDRESS"] = operAddr.IsChecked ?? false;
                opratorFilter["BIRTHDAY"] = operBday.IsChecked ?? false;
                opratorFilter["CONTACT"] = operContact.IsChecked ?? false;
                opratorFilter["MEMBERSHIP DATE"] = operMemDate.IsChecked ?? false;
                opratorFilter["TIN NUMBER"] = operTin.IsChecked ?? false;
                opratorFilter["VOTERS ID"] = operVoters.IsChecked ?? false;
                opratorFilter["DRIVER NAME"] = operDrName.IsChecked ?? false;
                opratorFilter["LOAN BALANCE"] = cbOpWLoan.IsChecked ?? false;
                opratorFilter["LTLOAN BALANCE"] = cbOpWLtLoan.IsChecked ?? false;

                collectionView.Filter = item =>
                {
                    if (item is Franchise franchise) 
                    {
                        bool shown = true;
                        if (cbOpWLoan.IsChecked ?? false)
                        {
                            shown = franchise.LoanBalance > 0;
                        }
                        if (cbOpWLtLoan.IsChecked ?? false)
                        {
                            shown = franchise.LongTermLoanBalance > 0;
                        }
                        return shown;
                    }
                    return true;
                };
                foreach (DataGridColumn column in TableGrid.Columns)
                {
                    if (column.Header != null && opratorFilter.TryGetValue(column.Header.ToString(), out bool isVisible))
                    {
                        column.HeaderStyle = new Style()
                        {
                            Setters =
                            {
                                new Setter(Control.FontWeightProperty, FontWeights.Bold),
                                new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                                new Setter(Control.FontFamilyProperty, new FontFamily("Inter")),
                                new Setter(VisibilityProperty, isVisible ? Visibility.Visible : Visibility.Collapsed)
                            }
                        };
                        column.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
            else if (table == Table.DRIVER) 
            {
                driverFilter["NAME"] = drivName.IsChecked ?? false;
                driverFilter["ADDRESS"] = drivAddr.IsChecked ?? false;
                driverFilter["BODY NO."] = drivBnum.IsChecked ?? false;
                driverFilter["LICENSE"] = drivLicense.IsChecked ?? false;
                driverFilter["PAYMENT DUE"] = drivPaymentDue.IsChecked ?? false;
                driverFilter["BIRTHDAY"] = drivBirthday.IsChecked ?? false;
                driverFilter["CONTACT"] = drivContact.IsChecked ?? false;
                driverFilter["PLATE NO."] = drivPlateNum.IsChecked ?? false;

                collectionView.Filter = item =>
                {
                    if (item is Driver driver)
                    {
                        
                    }
                    return true;
                };
                foreach (DataGridColumn column in TableGrid.Columns)
                {
                    if (column.Header != null && driverFilter.TryGetValue(column.Header.ToString(), out bool isVisible))
                    {
                        column.HeaderStyle = new Style()
                        {
                            Setters =
                            {
                                new Setter(Control.FontWeightProperty, FontWeights.Bold),
                                new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                                new Setter(Control.FontFamilyProperty, new FontFamily("Inter")),
                                new Setter(VisibilityProperty, isVisible ? Visibility.Visible : Visibility.Collapsed)
                            }
                        };
                        column.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }



            filterOptions.FadeOut(0.2);
        }

        private void btnApplyFiltersCancel_Click(object sender, RoutedEventArgs e)
        {
            filterOptions.FadeOut(0.2);
        }

        private void showFilterChoices(string tbl)
        {
            spFranchiseBY.Visibility = Visibility.Collapsed;
            spOperatorBY.Visibility = Visibility.Collapsed;
            spDriverBY.Visibility = Visibility.Collapsed;
            spFranchiseSHOW.Visibility = Visibility.Collapsed;
            spOperatorSHOW.Visibility = Visibility.Collapsed;
            lblOnlyShow.Visibility = Visibility.Collapsed;
            recOnlyShow.Visibility = Visibility.Collapsed;

            switch (tbl)
            {
                case Table.FRANCHISE:
                    spFranchiseBY.Visibility = Visibility.Visible;
                    spFranchiseSHOW.Visibility = Visibility.Visible;
                    lblOnlyShow.Visibility = Visibility.Visible;
                    recOnlyShow.Visibility = Visibility.Visible;
                    break;

                case Table.OPERATOR:
                    spOperatorBY.Visibility = Visibility.Visible;
                    spOperatorSHOW.Visibility = Visibility.Visible;
                    lblOnlyShow.Visibility = Visibility.Visible;
                    recOnlyShow.Visibility = Visibility.Visible;
                    break;

                case Table.DRIVER:
                    spDriverBY.Visibility = Visibility.Visible;
                    break;

                default:
                    return;
            }
        }
    }
}
