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
        public TableView(string table)
        {
            InitializeComponent();
            this.table = table;
            if (table == Table.FRANCHISE)
            {
                btnAdd.Visibility = Visibility.Visible;
                btnAdd.Content = "ADD FRANCHISE";
            } else if (table == Table.DRIVER) 
            {
                btnAdd.Visibility = Visibility.Visible;
                btnAdd.Content = "ADD DRIVER";
            } else
            {
                btnAdd.Visibility = Visibility.Collapsed;
            }
            UpdateDefaultSidePanel();
        }

        private void UpdateDefaultSidePanel()
        {
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
            if (table == Table.FRANCHISE)
            {
                int batchSize = 5;
                int pageIndex = 0;
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("Operator.name.wholename", "OPERATOR NAME", width: 140),
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
                        return (new TableObject<Franchise>(Table.FRANCHISE, Where.ALL_NOTDELETED, pageIndex * batchSize, batchSize)).data;
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
                int batchSize = 5;
                int pageIndex = 0;
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("Operator.name.wholename", "NAME", width: 140),
                    new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
                    new ColumnConfiguration("LicenseNO", "PLATE NO.", width: 100),
                    new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", width: 100),
                    new ColumnConfiguration("MonthlyDues", "MONTHLY DUE", width: 100),
                };
                DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(TableGrid, columnConfigurations);


                while (true)
                {
                    List<Franchise> batch = await Task.Run(() =>
                    {
                        return (new TableObject<Franchise>(Table.FRANCHISE, Where.THEREIS_OPERATOR, pageIndex * batchSize, batchSize)).data;
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
                int batchSize = 5;
                int pageIndex = 0;

                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("Driver.name.wholename", "NAME", width: 140),
                    new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
                    new ColumnConfiguration("LicenseNO", "PLATE NO.", width: 100),
                    new ColumnConfiguration("Operator", "OPERATOR", width: 100),
                };
                DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(TableGrid, columnConfigurations);



                while (true)
                {
                    List<Franchise> batch = await Task.Run(() =>
                    {
                        return (new TableObject<Franchise>(Table.FRANCHISE, Where.THEREIS_DRIVER, pageIndex * batchSize, batchSize)).data;
                    });

                    if (batch.Count == 0)
                        break;

                    foreach(var obj in batch)
                    {
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
                    MainBody.selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = MainBody.selectedFranchise.Operator;
                    bodynum.Content = MainBody.selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;
                    Reference.Content = MainBody.selectedFranchise.owner;
                    DriverName.Content = MainBody.selectedFranchise.Driver;
                    imgUserProfilePic.ImageSource = MainBody.selectedFranchise.Operator?.image?.GetSource();
                }
                else if(table == Table.OPERATOR)
                {
                    MainBody.selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = MainBody.selectedFranchise.Operator;
                    bodynum.Content = MainBody.selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;

                    Reference.Content = MainBody.selectedFranchise.owner;
                    DriverName.Content = MainBody.selectedFranchise.Driver;
                    imgUserProfilePic.ImageSource = MainBody.selectedFranchise.Operator?.image?.GetSource();
                    if (MainBody.selectedFranchise.Operator.isSuspended)
                    {
                        lblIsSuspended.Content = "YES";
                        lblIsSuspended.Foreground = Brushes.Red;
                    } else
                    {
                        lblIsSuspended.Content = "NO";
                        lblIsSuspended.Foreground = Brushes.Green;
                    }
                }
                else if (table == Table.DRIVER)
                {
                    MainBody.selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = MainBody.selectedFranchise.Driver;
                    bodynum.Content = MainBody.selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;
                    Reference.Content = MainBody.selectedFranchise.owner;
                    DriverName.Content = MainBody.selectedFranchise.Driver;
                    imgUserProfilePic.ImageSource = MainBody.selectedFranchise.Driver?.image?.GetSource();
                    if (MainBody.selectedFranchise.Driver.isSuspended)
                    {
                        lblIsSuspended.Content = "YES";
                        lblIsSuspended.Foreground = Brushes.Red;
                    } else
                    {
                        lblIsSuspended.Content = "NO";
                        lblIsSuspended.Foreground = Brushes.Green;
                    }

                }

            }

            franchiseInformation.Visibility = Visibility.Visible;
        }

        public async Task<Grid> Fetch()
        {
            Task task = UpdateTableAsync();
            await Task.Delay(5);
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
            await Task.Delay(5);
        }

        private void btnManage_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.FRANCHISE)
                {
                    franchisePanel.Children.Add((new FranchiseInformationView(this)).Fetch());
                }
            }
        }

        private async void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.OPERATOR)
                {
                    (new EditProfile(MainBody.selectedFranchise, General.OPERATOR)).ShowDialog();
                }
                else if (table == Table.DRIVER)
                {
                    (new EditProfile(MainBody.selectedFranchise, General.DRIVER)).ShowDialog();
                }
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
            }
            await UpdateTableAsync();
        }

        private async void btnAddViolation_Click(object sender, RoutedEventArgs e)
        {
            if(MainBody.selectedFranchise != null)
            {
                if(table == Table.DRIVER)
                    (new ViolationInput(MainBody.selectedFranchise)).ShowDialog();
            }
            await UpdateTableAsync();
        }

        private async void btnAddClick(object sender, RoutedEventArgs e)
        {
            if(table == Table.FRANCHISE)
                (new InputFranchiseView()).ShowDialog();
            //if(table == Table.DRIVER)
            await UpdateTableAsync();
        }
    }
}
