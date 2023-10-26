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
    /// Interaction logic for AddLoan.xaml
    /// </summary>
    public partial class AddLoan : Window
    {
        Franchise franchise;
        public AddLoan(Franchise franchise)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            AppState.mainwindow?.Hide();
            this.franchise = franchise;
            dpBdate.DisplayDate = DateTime.Now;
            dpBdate.SelectedDate = DateTime.Now;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ControlWindow.ShowDialogStatic("Adding new record", "Confirm?", Icons.NOTIFY))
            {
                PaymentDetails<Ledger.Loan> capital = new PaymentDetails<Ledger.Loan>();
                Ledger.Loan loan = new Ledger.Loan();
                if (franchise.GetLoans()?.FirstOrDefault() != null)
                {
                    loan = franchise.GetLoans().FirstOrDefault();
                    capital.WriteInto(loan, false, false, dpBdate.DisplayDate, tboxRefNo.Text, Double.Parse(tboxAmount.Text), Double.Parse(tbPenalty.Text), "", loan.amount - Double.Parse(tboxAmount.Text));
                    loan.amount = loan.amount + Double.Parse(tboxAmount.Text);
                    loan.Save();
                    capital.Save();
                } else
                {
                    loan.WriteInto(franchise.id, DateTime.Now, Double.Parse(tboxAmount.Text), "", Double.Parse(tboxInterest.Text), 0, 0);
                    loan.Save();
                }
                
                this.Close();
            }
        }
    }
}
