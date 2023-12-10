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
using static SPTC_APP.Objects.Ledger;
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for AddLoan.xaml
    /// </summary>
    public partial class AddLoan : Window
    {
        Franchise franchise;
        private string closingMSG;

        private double penalty;
        private double breakdown;

        private int seriesNo;
        private double amortization;
        private double principal;
        private double total;

        private double paidAmount;
        private double penaltyTot; // eto inistore ko sa database, 
        private string cvorNum;


        Ledger.Loan loan;
        public AddLoan(Franchise franchise)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.franchise = franchise;
            this.loan = this.franchise.currentloan;
           
            dpBdate.SelectedDate = DateTime.Now;
            dpBdate.DisplayDate = DateTime.Now;

            lblLoanDate.Content = loan.displayDate;
            lblLoanCVOR.Content = loan.cv_or;

            lblBodyNum.Content = franchise.BodyNumber;

            lblLoanAmount.Content = $"\u20B1 {loan.amountLoaned.ToString("N2")}";
            lblLoanInterest.Content = $"\u20B1 {loan.interest.ToString("N2")}";
            lblTotal.Content = $"\u20B1 {(loan.amountLoaned + loan.interest).ToString("N2")}";

            lblLoanRec.Content = $"\u20B1 {(loan.amountLoaned / loan.termsofpayment).ToString("N2")}";
            lblInteRec.Content = $"\u20B1 {(loan.interest / loan.termsofpayment).ToString("N2")}";

            int datem = DateTime.Now.Month;
            double balance = 0;
            if ((loan?.paymentDues ?? 0) > 0)
            {
                double totalloan = loan.paymentDues;
                if (loan?.balance == 0)
                    balance += totalloan * ((datem) - (loan?.last_payment.Month ?? datem));
                else
                    balance += totalloan + (((loan?.amountLoaned ?? 0) / loan?.termsofpayment ?? 0) * ((datem) - ((loan?.last_payment.Month ?? datem))));
            }

            breakdown = loan.details.Contains("EMERGENCY") ? loan.amount : balance;

            lblTotalBreak.Content = $"\u20B1 {breakdown.ToString("N2")}";

            penalty = loan.amountLoaned * (loan.penaltyPercent / 100);

            lblRemainingBalance.Content = $"\u20B1 {(franchise.LoanBalance - breakdown).ToString("N2")}";
            tboxAmount.Text = breakdown.ToString();
            lblCurrentPay.Content = $"\u20B1 {breakdown.ToString("N2")}";
            lblAmort.Content = $"\u20B1 {breakdown.ToString("N2")}";

            tbPenalty.Text = "0";

            tboxCVOR.Text = (AppState.CV_OR_LAST + 1).ToString();

            compute(0);

            DraggingHelper.DragWindow(topBar);
            tboxAmount.Focus();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            AppState.mainwindow?.displayToast(closingMSG);
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.closingMSG = "Adding loan payment history was canceled.";
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (confirmPayment())
            {
                PaymentDetails<Ledger.Loan> loanPayment = new PaymentDetails<Ledger.Loan>();
                double actualbalance = (loan.balance != 0) ? loan.balance : (loan.details.Contains("EMERGENCY") ? loan.amount : (loan.amountLoaned / loan.termsofpayment));
                double inputamount = Double.Parse(tboxAmount.Text);
                if (inputamount != actualbalance)
                {
                    loan.balance = actualbalance - inputamount;
                }
                else
                {
                    loan.balance = 0;
                }

                loan.amount = loan.amount - amortization;
                loanPayment.WriteInto(loan, 0, dpBdate.DisplayDate, tboxCVOR.Text, amortization, penaltyTot, "", loan.amount);
                if(loan.amount <= 0)
                {
                    loan.isFullyPaid = true;
                }
                loan.last_payment = dpBdate.DisplayDate;
                loanPayment.Save();
                if (inputamount != actualbalance)
                {
                    loan.last_payment = dpBdate.DisplayDate;
                    loan.Save();
                }
                AppState.CV_OR_LAST = Int32.Parse(tboxCVOR.Text);
                AppState.SaveToJson();
                //(AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                closingMSG = "Adding loan payment history was successful.\nPlease refresh the view to see changes.";
                this.Close();
            }
        }

        private bool confirmPayment()
        {
            if (!string.IsNullOrEmpty(tboxCVOR.Text))
            {
                string displaydate = dpBdate.DisplayDate.ToString("MMM dd, yyyy");
                if (ControlWindow.ShowTwoway("Confirm Details", $"CV/OR Number: {tboxCVOR.Text}\nPayment Amount: {tboxAmount.Text}\nSelected Date: {displaydate}", Icons.NOTIFY))
                {
                    compute(double.TryParse(tbPenalty.Text, out double pen) ? pen : 0);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                VisualStateManager.GoToState(tboxCVOR, "InputInvalidated", true);
                new Toast(gridToast, "Cash Voucher/Official Receipt Number cannot be empty.", 5, true, 0.2, Brushes.White, FindResource("BrushDeepRed") as Brush);
                return false;
            }
        }

        private void initTextBoxes()
        {
            tboxAmount.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Amount of payment.", 0);
            tboxCVOR.DefaultTextBoxBehavior(NUMBER, true, gridToast, "Cash Voucher/Official Receipt Number.", 1);
            tbPenalty.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Penalty amount.", 2);
        }

        private void tbPenalty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            double val;
            if (double.TryParse(tbPenalty.Text + e.Text, out val))
            {
                compute(val);
            }
        }

        private void compute(double overdue)
        {
            double inputamount = Double.Parse(tboxAmount.Text);
            if(breakdown != inputamount)
            {
                breakdown = inputamount;
            }

            amortization = breakdown;
            principal = breakdown;
            total = franchise.LoanBalance - amortization;

            penaltyTot = penalty * overdue;
            paidAmount = amortization + penaltyTot;

            // Changing labels/fields
            lblPenalty.Content = $"+\u20B1 {penaltyTot.ToString("N2")}";
            lblPenalty2.Content = $"\u20B1 {penaltyTot.ToString("N2")}";
            lblTotalBreak.Content = $"\u20B1 {paidAmount.ToString("N2")}";
            tboxAmount.Text = amortization.ToString();
            lblAmort.Content = $"\u20B1 {amortization.ToString("N2")}";
            lblCurrentPay.Content = $"\u20B1 {paidAmount.ToString("N2")}";
            lblRemainingBalance.Content = $"\u20B1 {total.ToString("N2")}";
        }
    }
}
