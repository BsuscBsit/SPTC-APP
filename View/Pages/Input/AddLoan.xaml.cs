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
        private double penaltyTot;
        private string cvorNum;


        Ledger.Loan loan;
        public AddLoan(Franchise franchise)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.franchise = franchise;
            dpBdate.DisplayDate = DateTime.Now;
            dpBdate.SelectedDate = DateTime.Now;

            lblLoanDate.Content = loan.displayDate;
            lblLoanCVOR.Content = loan.cv_or;
            lblLoanAmount.Content = "₱" + loan.amountLoaned.ToString("N2");
            lblLoanInterest.Content = "₱" + loan.interest.ToString("N2");
            lblTotal.Content = "₱" + (loan.amountLoaned + loan.interest).ToString("N2");

            lblLoanRec.Content = "₱" + (loan.amountLoaned / loan.termsofpayment).ToString("N2");
            lblInteRec.Content = "₱" + (loan.interest / loan.termsofpayment).ToString("N2");

            breakdown = ((loan.amountLoaned / loan.termsofpayment) + (loan.interest / loan.termsofpayment));
            lblTotalBreak.Content = "₱" + breakdown.ToString("N2");

            penalty = loan.amountLoaned * (loan.penaltyPercent / 100);

            lblRemainingBalance.Content = "₱" + (franchise.LongTermLoanBalance - Scaler.RoundUp(breakdown)).ToString("N2");
            tboxAmount.Text = Scaler.RoundUp(breakdown).ToString();
            lblCurrentPay.Content = "₱" + Scaler.RoundUp(breakdown).ToString("N2");
            lblAmort.Content = "₱" + Scaler.RoundUp(breakdown).ToString("N2");

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
                /*if (ControlWindow.ShowTwoway("Adding Loan Payment", "Do you confirm this transaction?", Icons.NOTIFY))
                {
                    double amount = Double.TryParse(tboxAmount.Text, out amount) ? amount : 0;
                    double interest = Double.TryParse(tboxInterest.Text, out interest) ? interest : 0;
                    double penalty = Double.TryParse(tbPenalty.Text, out penalty) ? penalty : 0;
                    if (amount > 0)
                    {
                        PaymentDetails<Ledger.Loan> loanPayment = new PaymentDetails<Ledger.Loan>();
                        loan.amount = loan.amount - amount;
                        loanPayment.WriteInto(loan, 0, dpBdate.DisplayDate, tboxRefNo.Text, amount, penalty, "", loan.amount);
                        if(loan.amount <= 0)
                        {
                            loan.isFullyPaid = true;
                        }
                        loanPayment.Save();
                        (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                        closingMSG = "Adding loan payment history was successful.\nPlease refresh the view to see changes.";
                        this.Close();
                    }
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
