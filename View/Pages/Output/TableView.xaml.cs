using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages.Input;

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

        public TableView(string table)
        {
            InitializeComponent();
            this.table = table;
            if (table == Table.FRANCHISE)
            {
                btnAdd.Visibility = Visibility.Visible;
                btnAdd.Content = "ADD FRANCHISE";
            }
            else if (table == Table.DRIVER)
            {
                btnAdd.Visibility = Visibility.Visible;
                btnAdd.Content = "ADD DRIVER";
            }
            else
            {
                btnAdd.Visibility = Visibility.Collapsed;
            }
            UpdateDefaultSidePanel();
        }

        private void UpdateDefaultSidePanel()
        {

            franchiseInformation.Visibility = Visibility.Collapsed;
            if (table == Table.FRANCHISE)
            {
                btnManage.Visibility = Visibility.Visible;
            }
            if (table == Table.OPERATOR)
            {
                btnEditProfile.Visibility = Visibility.Visible;
                btnGenerateid.Visibility = Visibility.Visible;
            }
            if (table == Table.DRIVER)
            {
                btnEditProfile.Visibility = Visibility.Visible;
                btnGenerateid.Visibility = Visibility.Visible;
                btnAddViolation.Visibility = Visibility.Visible;

            }
        }

        private async Task UpdateTableAsync()
        {
            TableGrid.Items.Clear();
            TableGrid.Items.Clear();
            if (table == Table.FRANCHISE)
            {
                int batchSize = AppState.TABLE_BATCH_SIZE;
                int pageIndex = 0;
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("Operator.name.legalName", "OPERATOR NAME", width: 140),
                    new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
                    new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", width: 100),
                    new ColumnConfiguration("MTOPNo", "MTOP NO.", width: 100),
                    new ColumnConfiguration("MonthlyDues", "MONTHLY DUE", width: 100),
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
                    new ColumnConfiguration("name.legalName", "NAME", width: 140),
                    new ColumnConfiguration("franchise.BodyNumber", "BODY NO.", width: 80),
                    new ColumnConfiguration("franchise.LicenseNO", "PLATE NO.", width: 100),
                    new ColumnConfiguration("franchise.ShareCapital", "SHARE CAPITAL", width: 100),
                    new ColumnConfiguration("franchise.MonthlyDues", "MONTHLY DUE", width: 100),
                };
                DataGridHelper<Operator> dataGridHelper = new DataGridHelper<Operator>(TableGrid, columnConfigurations);


                while (true)
                {
                    List<Operator> batch = await Task.Run(() =>
                    {
                        return (new TableObject<Operator>(Table.OPERATOR, Select.ALL, Where.ALL_NOTDELETED, pageIndex * batchSize, batchSize)).data;
                    });

                    if (batch.Count == 0)
                        break;
                    foreach (var obj in batch)
                    {
                        obj.UpdateFranchise();
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
                    new ColumnConfiguration("name.legalName", "NAME", width: 140),
                    new ColumnConfiguration("franchise.BodyNumber", "BODY NO.", width: 80),
                    new ColumnConfiguration("franchise.LicenseNO", "PLATE NO.", width: 100),
                    new ColumnConfiguration("franchise.Operator", "OPERATOR", width: 100),
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
                    ValuePairFI(lblF2, "Date Of Membership: ", lblI2, MainBody.selectedFranchise?.Operator?.dateOfMembership.ToString("MMMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Body No: ", lblI3, MainBody.selectedFranchise?.BodyNumber.ToString() ?? "");
                    ValuePairFI(lblF4, "Loan Balance: ", lblI4, MainBody.selectedFranchise?.LoanBalance.ToString() ?? "");
                    ValuePairFI(lblF5, "LT Loan Balance: ", lblI5, MainBody.selectedFranchise?.LongTermLoanBalance.ToString() ?? "");
                    ValuePairFI(lblF6, "Reference: ", lblI6, MainBody.selectedFranchise?.Owner?.ToString() ?? "");
                    ValuePairFI(lblF7, "Driver: ", lblI7, MainBody.selectedFranchise?.Driver?.ToString() ?? "");
                    imgUserProfilePic.ImageSource = MainBody.selectedFranchise?.Operator?.image?.GetSource();
                }
                else if (table == Table.OPERATOR)
                {
                    MainBody.selectedFranchise = ((Operator)grid.SelectedItem).franchise;
                    oholder = (Operator)grid.SelectedItem;
                    ValuePairFI(lblF1, "Operator Name: ", lblI1, oholder?.name?.legalName?.ToString());
                    ValuePairFI(lblF2, "Date Of Membership: ", lblI2, MainBody.selectedFranchise?.Operator?.dateOfMembership.ToString("MMMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Body No: ", lblI3, MainBody.selectedFranchise?.BodyNumber.ToString() ?? "");
                    ValuePairFI(lblF4, "Loan Balance: ", lblI4, MainBody.selectedFranchise?.LoanBalance.ToString() ?? "");
                    ValuePairFI(lblF5, "LT Loan Balance: ", lblI5, MainBody.selectedFranchise?.LongTermLoanBalance.ToString() ?? "");
                    ValuePairFI(lblF6, "TIN No.: ", lblI6, MainBody.selectedFranchise?.Operator?.tinNumber.ToString() ?? "");
                    ValuePairFI(lblF6, "VOTERS ID No.: ", lblI6, MainBody.selectedFranchise?.Operator?.votersNumbewr.ToString() ?? "");
                    imgUserProfilePic.ImageSource = MainBody.selectedFranchise?.Operator?.image?.GetSource();
                }
                else if (table == Table.DRIVER)
                {
                    MainBody.selectedFranchise = ((Driver)grid.SelectedItem).franchise;
                    dholder = (Driver)grid.SelectedItem;
                    ValuePairFI(lblF1, "Driver's Name: ", lblI1, dholder?.name?.legalName?.ToString());
                    ValuePairFI(lblF2, "Date of Membership: ", lblI2, MainBody.selectedFranchise?.Driver?.dateOfMembership.ToString("MMMM dd, yyyy") ?? "");
                    ValuePairFI(lblF3, "Address: ", lblI3, MainBody.selectedFranchise?.Driver?.address?.ToString() ?? "");
                    ValuePairFI(lblF4, "Body No.: ", lblI4, MainBody.selectedFranchise?.BodyNumber.ToString() ?? "");
                    ValuePairFI(lblF5, "License No: ", lblI5, MainBody.selectedFranchise?.LicenseNO.ToString() ?? "");
                    ValuePairFI(lblF6, "Violation:  ", lblI6, MainBody.selectedFranchise?.Driver?.violationCount.ToString() ?? "");
                    imgUserProfilePic.ImageSource = MainBody.selectedFranchise?.Driver?.image?.GetSource();
                    if (MainBody.selectedFranchise?.Driver?.isSuspended ?? false)
                    {
                        ValuePairFI(lblF7, "Is Suspended:", lblI7, "YES");
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
            }


            franchiseInformation.Visibility = Visibility.Visible;
        }

        private void ValuePairFI(Label lblF, string name, Label lblI, string value)
        {
            lblF.Content = name;
            lblI.Content = (string.IsNullOrEmpty(value)) ? "N/A" : value;
        }

        public async Task<Grid> Fetch()
        {
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

        public async void BackUpdate()
        {
            Task task = UpdateTableAsync();
            await Task.Delay(1);
        }

        private async void btnManage_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.FRANCHISE)
                {
                    franchisePanel.Children.Add((new FranchiseInformationView(this)).Fetch());
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
                ControlWindow.ShowStatic("No Franchise detected!", "Cannot proceed. Dreate new Franchise first", Icons.ERROR);
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
    }
}
