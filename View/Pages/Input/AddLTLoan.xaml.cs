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
    /// Interaction logic for AddLTLoan.xaml
    /// </summary>
    public partial class AddLTLoan : Window
    {

        Franchise franchise;
        private string closingMSG;

        private double penalty;
        private double breakdown;

        /**
         * Lagay dito yung mga isasave sa database, yung amount na nareceive
         * tas yung totalpenalty, penalty * months not paying ganun
         */

        // Based on table in pictures(might be redundant).
        private int seriesNo;
        private double amortization;
        private double principal;
        private double total; // remaining balance

        // Others
        private double paidAmount;
        private double penaltyTot;
        private string cvorNum;


        Ledger.LongTermLoan ltloan;
        public AddLTLoan(Franchise franchise)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.franchise = franchise;
            dpBdate.SelectedDate = DateTime.Now;
            dpBdate.DisplayDate = DateTime.Now;

            ltloan = this.franchise.currentltloan;

            lblLoanDate.Content = ltloan.displayDate;
            lblLoanCVOR.Content = ltloan.cv_or;

            lblBodyNum.Content = franchise.BodyNumber;

            lblLoanAmount.Content = $"\u20B1 {ltloan.amountLoaned.ToString("N2")}";
            lblLoanInterest.Content = $"\u20B1 {ltloan.interest.ToString("N2")}";
            lblTotal.Content = $"\u20B1 {(ltloan.amountLoaned + ltloan.interest).ToString("N2")}";

            double balanceOrReceivable = (ltloan.amount < (ltloan.amountLoaned / ltloan.termsofpayment)) ? ltloan.amount: Scaler.RoundUp(ltloan.amountLoaned / ltloan.termsofpayment);
            bool shouldRound = (ltloan.amountLoaned / ltloan.termsofpayment != Scaler.RoundUp(ltloan.amountLoaned / ltloan.termsofpayment)); // if true,it should display round
            lblLoanRec.Content = $"\u20B1 {(ltloan.amountLoaned / ltloan.termsofpayment).ToString("N2") + (shouldRound ? $"\n -> (\u20B1 {balanceOrReceivable.ToString("N2")})" : "")}";
            lblInteRec.Content = $"\u20B1 {(ltloan.interest / ltloan.termsofpayment).ToString("N2")}";
            int datem = DateTime.Now.Month;
            double balance = 0;
            if ((ltloan?.paymentDues ?? 0) > 0)
            {
                double totalloan = ltloan.paymentDues;
                if (ltloan?.balance == 0)
                    balance += totalloan * ((datem) - (ltloan?.last_payment.Month ?? datem));
                else
                    balance += totalloan + ((((ltloan?.amountLoaned ?? 0) + (ltloan?.interest ?? 0))) / ltloan?.termsofpayment ?? 0) * ((datem) - ((ltloan?.last_payment.Month ?? datem)));
            }

            breakdown = balance;
            if(ltloan.amount < breakdown)
            {
                breakdown = ltloan.amount;
            }


            lblTotalBreak.Content = $"\u20B1 {breakdown.ToString("N2")}";

            penalty = ltloan.amountLoaned * (ltloan.penaltyPercent / 100);

            lblRemainingBalance.Content = $"\u20B1 {(franchise.LongTermLoanBalance - Scaler.RoundUp(breakdown)).ToString("N2")}";
            tboxAmount.Text = Scaler.RoundUp(breakdown).ToString();
            lblCurrentPay.Content = $"\u20B1 {Scaler.RoundUp(breakdown).ToString("N2")}";
            lblAmort.Content = $"\u20B1 {Scaler.RoundUp(breakdown).ToString("N2")}";

            // Kung kaya, i-autocompute sana.
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
                PaymentDetails<Ledger.LongTermLoan> loanPayment = new PaymentDetails<Ledger.LongTermLoan>();
                double actualbalance = (ltloan.balance != 0) ? ltloan.balance : ((ltloan.amountLoaned / ltloan.termsofpayment) + (ltloan.interest / ltloan.termsofpayment));
                double inputamount = Double.Parse(tboxAmount.Text);
                if (inputamount != actualbalance)
                {
                    ltloan.balance = inputamount - actualbalance;
                } else
                {
                    ltloan.balance = 0;
                }

                ltloan.amount = ltloan.amount - amortization;
                loanPayment.WriteInto(ltloan, 0, dpBdate.DisplayDate, tboxCVOR.Text, amortization, penaltyTot, "", ltloan.amount);
                if (ltloan.amount <= 0)
                {
                    ltloan.isFullyPaid = true;
                }
                ltloan.last_payment = dpBdate.DisplayDate;
                loanPayment.Save();
                if (inputamount != actualbalance)
                {
                    ltloan.last_payment = dpBdate.DisplayDate;
                    ltloan.Save();
                }
                AppState.CV_OR_LAST = Int32.Parse(tboxCVOR.Text);
                AppState.SaveToJson();
                //(AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                this.closingMSG = "Adding loan payment history was successful.\nPlease refresh the view to see changes.";
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
            tboxAmount.DefaultTextBoxBehavior(DECIMALUNSIGNED, true, gridToast, "Enter amount of payment.", 1);
            tboxCVOR.DefaultTextBoxBehavior(NUMBER, true, gridToast, "CV/OR number.", 0);
            tbPenalty.DefaultTextBoxBehavior(WHOLEUNSIGNED, true, gridToast, "Enter number of overdue in months.", 2);
        }

        private void tbPenalty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            double val;
            if (double.TryParse(tbPenalty.Text + e.Text, out val))
            {
                compute(val);
            } else
            {
                tbPenalty.Text = "0";
                compute(0);
            }
        }
        
        private void compute(double overdue)
        {
            double inputamount = Double.Parse(tboxAmount.Text);
            if (breakdown != inputamount)
            {
                breakdown = inputamount;
            }

            amortization = Scaler.RoundUp(breakdown);
            principal = breakdown;
            total = franchise.LongTermLoanBalance - amortization;

            penaltyTot = penalty * overdue;
            paidAmount = amortization + penaltyTot;

            // Changing labels/fields
            lblPenalty.Content = $"\u20B1 {penaltyTot.ToString("N2")}";
            lblPenalty2.Content = $"\u20B1 {penaltyTot.ToString("N2")}";
            lblTotalBreak.Content = $"\u20B1 {amortization.ToString("N2")}";
            tboxAmount.Text = amortization.ToString();
            lblAmort.Content = $"\u20B1 {amortization.ToString("N2")}";
            lblCurrentPay.Content = $"\u20B1 {paidAmount.ToString("N2")}";
            lblRemainingBalance.Content = $"\u20B1 {total.ToString("N2")}";
        }
    }
}
