using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;

namespace SPTC_APP.View.Pages.Leaflets
{
    /// <summary>
    /// Interaction logic for TableView.xaml
    /// </summary>
    public partial class TableView : Window
    {
        private string table;
        private Franchise selectedFranchise;
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
                        await Task.Delay(200);
                    }

                    await Task.Delay(200);

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
                        await Task.Delay(200);
                    }

                    await Task.Delay(200);

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
                        return (new TableObject<Franchise>(Table.FRANCHISE, Where.THEREIS_OPERATOR, pageIndex * batchSize, batchSize)).data;
                    });

                    if (batch.Count == 0)
                        break;

                    foreach(var obj in batch)
                    {
                        TableGrid.Items.Add(obj);
                        await Task.Delay(200);
                    }

                    // Wait a bit before fetching the next batch (optional, for visual effect)

                    await Task.Delay(200);

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
                     selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = selectedFranchise.Operator;
                    bodynum.Content = selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;
                    Reference.Content = selectedFranchise.owner;
                    DriverName.Content = selectedFranchise.Driver;

                }
                else if(table == Table.OPERATOR)
                {
                     selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = selectedFranchise.Operator;
                    bodynum.Content = selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;

                    Reference.Content = selectedFranchise.owner;
                    DriverName.Content = selectedFranchise.Driver;
                }
                else if (table == Table.DRIVER)
                {
                     selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = selectedFranchise.Driver;
                    bodynum.Content = selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;
                    Reference.Content = selectedFranchise.owner;
                    DriverName.Content = selectedFranchise.Driver;

                }

            }
        }

        public async Task<Grid> Fetch()
        {
            UpdateTableAsync();
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

        private void AddFranchiseButton_Click(object sender, RoutedEventArgs e)
        {
            (new InputFranchiseView()).Show();
        }

        private void ManageButton_Click(object sender, RoutedEventArgs e)
        {
            if(selectedFranchise != null)
            {
                franchisePanel.Children.Add((new FranchiseInformationView(selectedFranchise)).Fetch());
            }
        }
    }
}
