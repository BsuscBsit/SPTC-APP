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
            dpBdate.DisplayDate = DateTime.Now;
            dpBdate.SelectedDate = DateTime.Now;
            ltloan = this.franchise.GetLTLoans().FirstOrDefault();

            lblLoanDate.Content = ltloan.displayDate;
            lblLoanCVOR.Content = ltloan.cv_or;
            lblLoanAmount.Content = "₱" + ltloan.amountLoaned.ToString("N2");
            lblLoanInterest.Content = "₱" + ltloan.interest.ToString("N2");
            lblTotal.Content = "₱" + (ltloan.amountLoaned + ltloan.interest).ToString("N2");

            lblLoanRec.Content = "₱" + (ltloan.amountLoaned / ltloan.termsofpayment).ToString("N2");
            lblInteRec.Content = "₱" + (ltloan.interest / ltloan.termsofpayment).ToString("N2");

            breakdown = ((ltloan.amountLoaned / ltloan.termsofpayment) + (ltloan.interest / ltloan.termsofpayment));
            lblTotalBreak.Content = "₱" + breakdown.ToString("N2");

            // Eto galing sa applyloan
            penalty = ltloan.amountLoaned * (ltloan.penaltyPercent / 100);

            lblRemainingBalance.Content = "₱" + (franchise.LongTermLoanBalance - Scaler.RoundUp(breakdown)).ToString("N2");
            tboxAmount.Text = Scaler.RoundUp(breakdown).ToString();
            lblCurrentPay.Content = "₱" + Scaler.RoundUp(breakdown).ToString("N2");
            lblAmort.Content = "₱" + Scaler.RoundUp(breakdown).ToString("N2");

            // Kung kaya, i-autocompute sana.
            tbPenalty.Text = "0";


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
                /*
                double amount = Double.TryParse(tboxAmount.Text, out amount) ? amount : 0;
                
                double penalty = Double.TryParse(tbPenalty.Text, out penalty) ? penalty : 0;
                if (amount > 0)
                {

                    PaymentDetails<Ledger.LongTermLoan> loanPayment = new PaymentDetails<Ledger.LongTermLoan>();
                    ltloan.amount = ltloan.amount - amount;
                    loanPayment.WriteInto(ltloan, 0, dpBdate.DisplayDate, tboxRefNo.Text, amount, penalty, "", ltloan.amount);
                    if (ltloan.amount <= 0)
                    {
                        ltloan.isFullyPaid = true;
                    }
                    loanPayment.Save();
                    
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                    this.closingMSG = "Adding loan payment history was successful.\nPlease refresh the view to see changes.";
                    this.Close();
                } else
                {

                }*/
            }
        }

        private bool confirmPayment()
        {
            if (!string.IsNullOrEmpty(tboxCVOR.Text))
            {
                if (ControlWindow.ShowTwoway("Adding new record", "Confirm?", Icons.NOTIFY))
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
            }
        }
        
        private void compute(double overdue)
        {
            amortization = Scaler.RoundUp(breakdown);
            principal = breakdown;
            total = franchise.LongTermLoanBalance - amortization;

            penaltyTot = penalty * overdue;
            paidAmount = amortization + penaltyTot;

            // Changing labels/fields
            lblPenalty.Content = "+₱" + penaltyTot.ToString("N2");
            lblPenalty2.Content = "₱" + penaltyTot.ToString("N2");
            lblTotalBreak.Content = "₱" + paidAmount.ToString("N2");
            tboxAmount.Text = amortization.ToString();
            lblAmort.Content = "₱" + amortization.ToString("N2");
            lblCurrentPay.Content = "₱" + paidAmount.ToString("N2");
            lblRemainingBalance.Content = "₱" + total.ToString("N2");
        }
    }
}
