using System;
using System.Collections.Generic;
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
            UpdateTable();
        }

        private void UpdateTable()
        {
            if(table == Table.FRANCHISE)
            {
                List<Franchise> fetchedData = (new TableObject<Franchise>(Table.FRANCHISE)).data;

                DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(TableGrid);

                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("id", "id", visibility: Visibility.Hidden, width: 0),
                new ColumnConfiguration("Operator", "NAME", width: 140),
                new ColumnConfiguration("BodyNumber", "BODY NO.", width: 80),
                new ColumnConfiguration("LicenseNO", "PLATE NO.", width: 100),
                new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", width: 100),
                 new ColumnConfiguration("MonthlyDues", "MONTHLY DUE", width: 100),
            };
                dataGridHelper.DesignGrid(fetchedData, columnConfigurations);
                TableGrid.SelectedCellsChanged += TableSelectedChanged;
            }
        }

        private void TableSelectedChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            DataGrid grid = (DataGrid)sender;
            if (table == Table.FRANCHISE)
            {
                if (grid.SelectedItems.Count > 0)
                {
                    Franchise selectedFranchise = (Franchise)grid.SelectedItems[0];
                    OperatorName.Content = selectedFranchise.Operator;
                    bodynum.Content = selectedFranchise.BodyNumber;
                    LoanBalance.Content = 0;
                    LTLBalance.Content = 0;
                    Reference.Content = selectedFranchise.owner;
                    DriverName.Content = selectedFranchise.Driver;
                }
            }
        }

        public Grid Fetch()
        {
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

    }
}
