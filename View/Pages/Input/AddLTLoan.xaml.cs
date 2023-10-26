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
    /// Interaction logic for AddLTLoan.xaml
    /// </summary>
    public partial class AddLTLoan : Window
    {

        Franchise franchise;
        public AddLTLoan(Franchise franchise)
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
                PaymentDetails<Ledger.LongTermLoan> capital = new PaymentDetails<Ledger.LongTermLoan>();
                Ledger.LongTermLoan loan = new Ledger.LongTermLoan();
                if (franchise.GetLTLoans()?.FirstOrDefault() != null)
                {
                    loan = franchise.GetLTLoans().FirstOrDefault();
                    capital.WriteInto(loan, false, false, dpBdate.DisplayDate, tboxRefNo.Text, Double.Parse(tboxAmount.Text), Double.Parse(tboxPenalty.Text), "", loan.amountLoaned - Double.Parse(tboxAmount.Text));
                    loan.amountLoaned = loan.amountLoaned + Double.Parse(tboxAmount.Text);
                    loan.Save();
                    capital.Save();
                }
                else
                {
                    loan.WriteInto(franchise.id, DateTime.Now,12,DateTime.Now, DateTime.Now.AddYears(1), Double.Parse(tboxAmount.Text), "", Double.Parse(tbInterenst.Text), Double.Parse(tboxPenalty.Text));
                    loan.Save();
                }

                this.Close();
            }
        }
    }
}
