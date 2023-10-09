using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            if(table == Table.FRANCHISE)
            {
                AddFranchiseButton.Visibility = Visibility.Visible;
            } else
            {
                AddFranchiseButton.Visibility = Visibility.Collapsed;
            }
            UpdateDefaultSidePanel();
        }

        private void UpdateDefaultSidePanel()
        {
            if (table == Table.FRANCHISE)
            {
                //USAGE:: CHange side panel design
                btnManage.Visibility = Visibility.Visible;
            }
            if (table == Table.OPERATOR)
            {
                //USAGE:: CHange side panel design
                btnEditProfile.Visibility = Visibility.Visible;
                btnGenerateid.Visibility = Visibility.Visible;
            }
            if (table == Table.DRIVER)
            {
                //USAGE:: CHange side panel design
                btnEditProfile.Visibility = Visibility.Visible;
                btnGenerateid.Visibility = Visibility.Visible;
                btnAddViolation.Visibility = Visibility.Visible;
            }
        }

        private async Task UpdateTableAsync()
        {
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
                        //await Task.Delay(200);
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
                    new ColumnConfiguration("Driver.Shift", "SHIFT", width: 100),
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

                }

            }

            franchiseInformation.Visibility = Visibility.Visible;
        }

        public async Task<Grid> Fetch()
        {
            await UpdateTableAsync();
            if (franchisePanel.Parent != null)
            {
                Window currentParent = franchisePanel.Parent as Window;
                if (currentParent != null)
                {
                    currentParent.Content = null;
                }
            }
            await Task.Delay(50);
            this.Close();
            return franchisePanel;
        }

        private void AddFranchiseButton_Click(object sender, RoutedEventArgs e)
        {
            (new InputFranchiseView()).Show();
        }


        private void btnManage_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.FRANCHISE)
                {
                    franchisePanel.Children.Add((new FranchiseInformationView()).Fetch());
                }
            }
        }

        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.OPERATOR)
                {
                    franchisePanel.Children.Add((new FranchiseInformationView()).Fetch());
                }
                else if (table == Table.DRIVER)
                {
                    franchisePanel.Children.Add((new DriverInformationView()).Fetch());
                }
            }
        }

        private void btnGenerateid_Click(object sender, RoutedEventArgs e)
        {
            if (MainBody.selectedFranchise != null)
            {
                if (table == Table.OPERATOR)
                {
                    (new GenerateID(MainBody.selectedFranchise, false)).Show();
                }
                else if (table == Table.DRIVER)
                {
                    (new GenerateID(MainBody.selectedFranchise, true)).Show();
                }
            }
        }

        private void btnAddViolation_Click(object sender, RoutedEventArgs e)
        {
            if(MainBody.selectedFranchise != null)
            {
                if(table == Table.DRIVER) 
                {
                    (new ViolationInput()).Show();
                }
            }
        }
    }
}
