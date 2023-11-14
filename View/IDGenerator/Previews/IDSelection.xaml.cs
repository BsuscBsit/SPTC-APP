using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SPTC_APP.View.IDGenerator.Previews
{
    /// <summary>
    /// Interaction logic for IDSelection.xaml
    /// </summary>
    public partial class IDSelection : Window
    {
        private bool isDriver = false;
        Franchise selectedFranchise = null;
        public IDSelection()
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            LoadFranchises();

            AppState.mainwindow?.Hide();
            if (PrintPreview.GetPrintIDs().Count > 0)
            {
                tbCurrentPrintQueue.Content = "";
                tbCurrentPrintQueue.FontStyle = FontStyles.Normal;
                foreach (var item in PrintPreview.GetPrintIDs())
                {
                    string res = item.Value ? "Driver" : "Operator";
                    tbCurrentPrintQueue.Content += $"\n B#: {item.Key} : {res}";
                }
            }
            DraggingHelper.DragWindow(topBar);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //AppState.mainwindow?.Show();
            base.OnClosing(e);
        }
        private void LoadFranchises()
        {
            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("BodyNumber", "BODY NO.", minWidth: 80),
                new ColumnConfiguration("Operator.name.wholename", "OPERATOR NAME", minWidth: 140),
                new ColumnConfiguration("Driver.name.wholename", "DRIVER NAME", minWidth: 140),
            };
            DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(dgListFranchise, columnConfigurations);

            List<Franchise> franchises = Retrieve.GetData<Franchise>(Table.FRANCHISE, Select.ALL, Where.DRIVER_AND_OPERATOR);
            foreach(Franchise fracnhise in franchises)
            {
                dgListFranchise.Items.Add(fracnhise);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if(selectedFranchise != null)
            {
                (new GenerateID(selectedFranchise, isDriver)).Show();
                this.Close();
            } else
            {
                ControlWindow.ShowStatic("No ID Selected", "Please select from the list then proceed.");
                btnNext.IsEnabled = false;
            }
        }

        private void dgListFranchise_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var select = (sender as DataGrid).SelectedItem;
            if (select is Franchise franchise)
            {
                if(franchise.Operator != null && franchise.Driver != null)
                {
                    MySwitch.IsEnabled = true;
                    MySwitch_Back(MySwitch, null);
                } else if(franchise.Operator != null)
                {
                    MySwitch.IsEnabled = false;
                    MySwitch_Back(MySwitch, null);
                } else if(franchise.Driver != null)
                {
                    MySwitch.IsEnabled = false;
                    MySwitch_Front(MySwitch, null);
                }
                selectedFranchise = franchise;
                tbSelectedFranchise.Text = "Selected: " + franchise.BodyNumber;
                btnNext.IsEnabled = true;
            }
        }
        private void MySwitch_Back(object sender, RoutedEventArgs e)
        {
            // Switch is in the True state
            isDriver = false;
            if (MySwitch.IsEnabled)
            {
                drvOrOprt.Content = "Create this ID for\nOperator.";
            }
            else
            {
                drvOrOprt.Content = "ID for Operator.";
            }
        }
        private void MySwitch_Front(object sender, RoutedEventArgs e)
        {
            // Switch is in the False state
            isDriver = true;
            if (MySwitch.IsEnabled)
            {
                drvOrOprt.Content = "Create this ID for\nDriver.";
            }
            else
            {
                drvOrOprt.Content = "ID for Driver.";
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            AppState.mainwindow?.Show();
            this.Close();
        }
    }
}
