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
using Table = SPTC_APP.Database.Table;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for ViolationTypeList.xaml
    /// </summary>
    public partial class ViolationTypeList : Window
    {

        private string closingMSG;
        private ViolationType selected;
        public ViolationTypeList()
        {
            InitializeComponent();
            // Needs button for Cancel, New and Edit
            // edit is diabled at first
            // needs datatable witn selectedIndex 
            ContentRendered += (sender, e) => {
                AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide();

                Populate();
            };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
        }


        public void displayToast(string message, double duration = 3)
        {
            if (!(string.IsNullOrEmpty(message) && string.IsNullOrWhiteSpace(message)))
            {
                System.Windows.Media.Brush bg = message.Contains("success") ?
                FindResource("BrushDeepGreen") as System.Windows.Media.Brush :
                FindResource("BrushRed") as System.Windows.Media.Brush;

                new Toast(gridToast, message, duration, true, 0.2, System.Windows.Media.Brushes.White, bg);
                Populate();
            }
        }


        public void Populate()
        {
            dgViolationType.Items.Clear();
            selected = null;
            lblSelectedViolation.Content = "N/A";
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("title", "VIOLATION TITLE", minWidth:150),
                new ColumnConfiguration("details", "DESCRIPTION", minWidth:200),
                new ColumnConfiguration("numOfDays", "SUSPENSION", minWidth:80),
            };
            new DataGridHelper<ViolationType>(dgViolationType, columnConfigurations);

            foreach(ViolationType type in Retrieve.GetData<ViolationType>(Table.VIOLATION_TYPE, Select.ALL, Where.ALL_NOTDELETED))
            {
                dgViolationType.Items.Add(type);
            }
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            AppState.mainwindow?.displayToast(closingMSG);
            base.OnClosing(e);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dgViolationType.SelectedItem is ViolationType type)
            {
                selected = type;
                lblSelectedViolation.Content = selected.title;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            (new AddViolationType(this)).Show();
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if(selected != null)
            {
                (new AddViolationType(this,selected)).Show();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(selected != null)
            {
                if(ControlWindow.ShowTwoway("Deleting Violation Type", "Are you sure you want to continue?", Icons.DEFAULT))
                {
                    selected.Delete();
                    displayToast("Deletion success");
                } else
                {
                    displayToast("Deletion cancelled");
                }
            }
        }
    }
}
