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
                List<Franchise> fetchedData = await Task.Run(() =>
                {
                    return (new TableObject<Franchise>(Table.FRANCHISE)).data;
                });

                DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(TableGrid);

                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
        {
            
            new ColumnConfiguration("Operator.name.wholename", "OPERATOR NAME", width: 140),
            new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
            new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", width: 100),
            new ColumnConfiguration("MTOPNo", "MTOP NO.", width: 100),
            new ColumnConfiguration("MonthlyDues", "MONTHLY DUE", width: 100),
        };

                dataGridHelper.DesignGrid(fetchedData, columnConfigurations);
                TableGrid.SelectedCellsChanged += TableSelectedChanged;
            }
            else if (table == Table.OPERATOR)
            {
                List<Franchise> fetchedData = await Task.Run(() =>
                {
                    return (new TableObject<Franchise>(Table.FRANCHISE, Where.THEREIS_OPERATOR)).data;
                });

                DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(TableGrid);

                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
        {
            new ColumnConfiguration("Operator.name.wholename", "NAME", width: 140),
            new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
            new ColumnConfiguration("LicenseNO", "PLATE NO.", width: 100),
            new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", width: 100),
            new ColumnConfiguration("MonthlyDues", "MONTHLY DUE", width: 100),
        };

                dataGridHelper.DesignGrid(fetchedData, columnConfigurations);
                TableGrid.SelectedCellsChanged += TableSelectedChanged;
            }
            else if (table == Table.DRIVER)
            {
                List<Franchise> fetchedData = await Task.Run(() =>
                {
                    return (new TableObject<Franchise>(Table.FRANCHISE, Where.THEREIS_DRIVER)).data;
                });

                DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(TableGrid);

                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
        {
            new ColumnConfiguration("Driver.name.wholename", "NAME", width: 140),
            new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
            new ColumnConfiguration("LicenseNO", "PLATE NO.", width: 100),
            new ColumnConfiguration("Operator", "OPERATOR", width: 100),
            new ColumnConfiguration("Driver.Shift", "SHIFT", width: 100),
        };

                dataGridHelper.DesignGrid(fetchedData, columnConfigurations);
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
                    Franchise selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = selectedFranchise.Operator;
                    bodynum.Content = selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;
                    Reference.Content = selectedFranchise.owner;
                    DriverName.Content = selectedFranchise.Driver;

                }
                else if(table == Table.OPERATOR)
                {
                    Franchise selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = selectedFranchise.Operator;
                    bodynum.Content = selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;

                    Reference.Content = selectedFranchise.owner;
                    DriverName.Content = selectedFranchise.Driver;
                }
                else if (table == Table.DRIVER)
                {
                    Franchise selectedFranchise = (Franchise)grid.SelectedItems[0];
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
            
        }
    }
}
