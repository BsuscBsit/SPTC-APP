using SPTC_APP.Database;
using SPTC_APP.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for ViolationInput.xaml
    /// </summary>
    public partial class ViolationInput : Window
    {
        ViolationType selectedViolationType;
        Franchise franchise;
        public ViolationInput(Franchise franchise)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            AppState.mainwindow?.Hide();
            this.franchise = franchise;
            Populate();
        }

        private void Populate()
        {
            cbViolations.Items.Clear();
            List<ViolationType> listViolation = Retrieve.GetData<ViolationType>(Table.VIOLATION_TYPE, Select.ALL, Where.ALL_NOTDELETED);
            
            foreach (ViolationType type in listViolation)
            {
                cbViolations.Items.Add(type);
            }
            selectedViolationType = listViolation[0];
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (selectedViolationType != null)
            {
                if (ControlWindow.ShowDialogStatic("Adding Violation.", "Confirm?.", Icons.NOTIFY))
                {
                    DateTime violationDate = (dpViolationDate.SelectedDate != null)? (DateTime)dpViolationDate.SelectedDate: DateTime.Today;
                    DateTime startDate = calendarSelect.SelectedDates.FirstOrDefault();
                    DateTime endDate = calendarSelect.SelectedDates.LastOrDefault();

                    Violation violation = new Violation();
                    violation.WriteInto(franchise.id, selectedViolationType.id, violationDate , startDate, endDate, tboxRemarks.Text, (selectedViolationType.entityType == General.DRIVER)? franchise.Driver.name.id:franchise.Operator.name.id);
                    violation.Save();
                }
                this.Close();
            
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void cbViolations_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbViolations.SelectedItem is ViolationType selectedViolation)
            {
                int numOfDays = selectedViolation.numOfDays;

                calendarSelect.SelectedDates.Clear();

                DateTime startDate = DateTime.Today;
                calendarSelect.SelectedDate = startDate;

                for (int i = 0; i < numOfDays; i++)
                {
                    calendarSelect.SelectedDates.Add(startDate.AddDays(i));
                }
                selectedViolationType = selectedViolation;
            }
        }
        private void calendarSelect_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (selectedViolationType != null)
            {
                int numOfDays = selectedViolationType.numOfDays;

                // Disconnect the event handler temporarily
                calendarSelect.SelectedDatesChanged -= calendarSelect_SelectedDatesChanged;


                DateTime startDate = (calendarSelect.SelectedDate != null) ? (DateTime)calendarSelect.SelectedDate : DateTime.Today;

                calendarSelect.SelectedDates.Clear();

                calendarSelect.SelectedDate = startDate;

                for (int i = 0; i < numOfDays; i++)
                {
                    calendarSelect.SelectedDates.Add(startDate.AddDays(i));
                }

                // Reconnect the event handler
                calendarSelect.SelectedDatesChanged += calendarSelect_SelectedDatesChanged;
            }
        }


    }
}
