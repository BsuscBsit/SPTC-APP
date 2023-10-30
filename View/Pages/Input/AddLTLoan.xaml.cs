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
        bool isApply = false;
        public AddLTLoan(Franchise franchise)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.isApply = (franchise.GetLTLoans()?.FirstOrDefault() == null);
            this.franchise = franchise;
            dpBdate.DisplayDate = DateTime.Now;
            dpBdate.SelectedDate = DateTime.Now;
            if (isApply)
            {
                lblTerms.Content = "Term length in months: ";
            }
            else
            {
                tboxInterest.Visibility = Visibility.Collapsed;
                lblInterest.Visibility = Visibility.Collapsed;
                lblProcessingFee.Visibility = Visibility.Collapsed;
                tbProcessingFee.Visibility = Visibility.Collapsed;
                tboxAmount.Text = franchise.GetLTLoans().FirstOrDefault().paymentDues.ToString();
            }
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
                double amount = Double.TryParse(tboxAmount.Text, out amount) ? amount : 0;
                double interest = Double.TryParse(tboxInterest.Text, out interest) ? interest : 0;
                double penalty = Double.TryParse(tbPenalty.Text, out penalty) ? penalty : 0;
                if (amount > 0)
                {
                    PaymentDetails<Ledger.LongTermLoan> loanPayment = new PaymentDetails<Ledger.LongTermLoan>();
                    Ledger.LongTermLoan loan = new Ledger.LongTermLoan();
                    if (franchise.GetLTLoans()?.FirstOrDefault() != null)
                    {
                        loan = franchise.GetLTLoans().FirstOrDefault();
                        loanPayment.WriteInto(loan, false, false, dpBdate.DisplayDate, tboxRefNo.Text, amount, penalty, "", loan.amountLoaned - amount);
                        loan.amountLoaned = loan.amountLoaned - Double.Parse(tboxAmount.Text);
                        if (loan.amountLoaned <= 0)
                        {
                            loan.isFullyPaid = true;
                        }
                        loan.Save();
                        loanPayment.Save();
                    }
                    else
                    {
                        double monthlydue = (amount / penalty) + interest;
                        double totalamount = monthlydue * penalty;
                        loan.WriteInto(franchise.id, DateTime.Now, Int32.Parse(penalty.ToString()), DateTime.Now, DateTime.Now.AddMonths(Int32.Parse(penalty.ToString())), totalamount, "", Double.Parse(tbProcessingFee.Text),  monthlydue);
                        loan.Save();
                        loanPayment.WriteInto(loan, false, false, dpBdate.DisplayDate, tboxRefNo.Text, -amount, penalty, "", loan.amountLoaned);
                        loanPayment.isApply = true;
                        loanPayment.ledgername = Ledger.APPLY_LT_LOAN;
                        loanPayment.Save();
                    }

                    this.Close();
                } else
                {

                }
            }
        }

    }
}
