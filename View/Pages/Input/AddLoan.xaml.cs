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

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for AddLoan.xaml
    /// </summary>
    public partial class AddLoan : Window
    {
        Franchise franchise;
        bool isApply = false;
        public AddLoan(Franchise franchise)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.franchise = franchise;
            dpBdate.DisplayDate = DateTime.Now;
            dpBdate.SelectedDate = DateTime.Now;
            this.isApply = (franchise.GetLoans()?.FirstOrDefault() == null);
            if (isApply)
            {
                lblTerms.Content = "Term length in months: ";
                tbPenalty.Text = "12";
                lblTitle.Content = "APPLY FOR LONG TERM LOAN";
            } else
            {
                tboxInterest.Visibility = Visibility.Collapsed;
                lblInterest.Visibility = Visibility.Collapsed;
                double payment = franchise.GetLoans().FirstOrDefault().paymentDues;
                double balance = franchise.LoanBalance;
                if (payment < balance)
                {
                    tboxAmount.Text = payment.ToString("0.00");
                } else
                {
                    tboxAmount.Text = balance.ToString("0.00");
                }
            }
            DraggingHelper.DragWindow(topBar);
            tboxAmount.Focus();
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
            if (ControlWindow.ShowTwoway("Adding new record", "Confirm?", Icons.NOTIFY))
            {
                double amount = Double.TryParse(tboxAmount.Text, out amount) ? amount : 0;
                double interest = Double.TryParse(tboxInterest.Text, out interest) ? interest : 0;
                double penalty = Double.TryParse(tbPenalty.Text, out penalty) ? penalty : 0;
                if (amount > 0)
                {
                    PaymentDetails<Ledger.Loan> loanPayment = new PaymentDetails<Ledger.Loan>();
                    Ledger.Loan loan = new Ledger.Loan();
                    if (!isApply)
                    {
                        loan = franchise.GetLoans().FirstOrDefault();
                        loanPayment.WriteInto(loan, false, false, dpBdate.DisplayDate, tboxRefNo.Text, amount, penalty, "", loan.amount - amount);
                        loan.amount = loan.amount - amount;
                        if(loan.amount <= 0)
                        {
                            loan.isFullyPaid = true;
                        }
                        loan.Save();
                        loanPayment.Save();
                    } else
                    {

                        double monthlydue = (amount / penalty) + interest;
                        double totalamount = monthlydue * penalty;
                        loan.WriteInto(franchise.id, DateTime.Now, totalamount, "",interest, Int32.Parse(penalty.ToString()), monthlydue);
                        loan.Save();
                        loanPayment.WriteInto(loan, false, false, dpBdate.DisplayDate, tboxRefNo.Text, -amount, penalty, "", loan.amount);
                        loanPayment.isApply = true;
                        loanPayment.ledgername = Ledger.APPLY_LOAN;
                        loanPayment.Save();
                    }
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                    this.Close();
                } else
                {

                }
            }
        }

        private void TextBox_GotFucus(object sender, RoutedEventArgs e)
        {
            if(sender is TextBox tbox)
            {
                if(tbox.Text.Length > 0)
                {
                    tbox.SelectAll();
                }
            }
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Define a regular expression that allows only letters and numbers
            string pattern = @"[^0-9]+";

            // Check if the input text matches the pattern
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, pattern))
            {
                e.Handled = true; // Cancel the input event
            }
        }

        private void tboxAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
