using SPTC_APP.Objects;
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

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for EditPositionAccess.xaml
    /// </summary>
    public partial class EditPositionAccess : Window
    {
        private Employee employee;
        private EditEmployee window;
        public EditPositionAccess(EditEmployee window, Employee employee)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); window.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.employee = employee;
            if(employee.position != null)
            {
                List<bool> arr = employee.position?.accesses ?? null;
                int i = 0;
                foreach(CheckBox check in new CheckBox[] {cbDCreate, cbOCreate, cbFCreate, cbDEdit, cbOEdit, cbFEdit, cbDDelete, cbODelete, cbFDelete, cbIDGen, cbDChange, cbOChange, cbDViolation, cbShare, cbLoan, cbLTLoan, cbDashboard, cbAdmin, cbPayment })
                {
                    if (arr != null)
                    {
                        check.IsChecked = arr[i++];
                    }
                }
                if (employee.position.title.Length > 0)
                {
                    lblTitle.Content = "Edit " + employee.position.title + " Access: ";
                } else
                {

                }
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            
            base.OnClosing(e);
        }

        private void btnContinue(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (CheckBox check in new CheckBox[] { cbDCreate, cbOCreate, cbFCreate, cbDEdit, cbOEdit, cbFEdit, cbDDelete, cbODelete, cbFDelete, cbIDGen, cbDChange, cbOChange, cbDViolation, cbShare, cbLoan, cbLTLoan, cbDashboard, cbAdmin, cbPayment })
            {
                employee.position.accesses[i] = check.IsChecked??false;
                i++;
            }
            employee.Save();
            AppState.mainwindow?.Show();
            (AppState.mainwindow as MainBody).ResetWindow(General.BOARD_MEMBER);
            this.Close();
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            window.Show();
            this.Close();
        }
    }
}
