using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for Selection.xaml
    /// </summary>
    public partial class Selection : Window
    {

        Franchise franchise;
        General type;
        public Selection(Franchise franchise, General type)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            AppState.mainwindow?.Hide();
            this.franchise = franchise;
            this.type = type;

            DisplayDrivers();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        private void DisplayDrivers()
        {
            List<Driver> drivers = Retrieve.GetDataUsingQuery<Driver>(RequestQuery.GET_DRIVERS_WITHOUT_FRANCHISE);
            
            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("name.wholename", "NAME", width: 140),
                };
            DataGridHelper<Driver> dataGridHelper = new DataGridHelper<Driver>(dgDrivers, columnConfigurations);

            foreach (var obj in drivers)
            {
                dgDrivers.Items.Add(obj);
            }
            List<Franchise> franchise = Retrieve.GetDataUsingQuery<Franchise>(RequestQuery.GET_FRANCHISE_WITH_DRIVER);

            List<ColumnConfiguration> columnConfigurations1 = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("Driver.name.wholename", "NAME", width: 140),
                    new ColumnConfiguration("BodyNumber", "CURRENT BODY NO.", width: 80),
                };
            DataGridHelper<Franchise> dataGridHelper1 = new DataGridHelper<Franchise>(dgDrivers_franchise, columnConfigurations1);

            foreach(var obj in franchise)
            {
                dgDrivers_franchise.Items.Add(obj);
            }
        }
    }
}
