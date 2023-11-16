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

            // dat depende sa total dun sa applyloan eh.
            penalty = ltloan.amountLoaned * (0.02);

            /* pano kunin to?
            lblBodyNum.Content =
            lblRemainingBalance.Content =
            */

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
            if(ControlWindow.ShowTwoway("Adding new record", "Confirm?", Icons.NOTIFY))
            {

                return true;
            } else
            {
                return false;
            }
        }

        private void initTextBoxes()
        {
            tboxAmount.DefaultTextBoxBehavior(DECIMALUNSIGNED, true, gridToast, "Enter amount of payment.", 0);
            tboxRefNo.DefaultTextBoxBehavior(WHOLEUNSIGNED, true, gridToast, "CV/OR number.", 1);
            tbPenalty.DefaultTextBoxBehavior(WHOLEUNSIGNED, true, gridToast, "Enter number of overdue in months.", 2);
        }
        /**
         * Diero dapat etong tboxAmount ay galing sa monthly , yung nakalagay sa amortizations, di nato editable
         * tapos yung sa principal daw, ay naka round up sa 100, ex. principa 3333 dapat 3400
         * Ok na yung computation tas yung design, yung sa penalty, ilagay mo nalang sa ilalim ng tbPenalty
         * **/
        private void tboxAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            double val;
            if(double.TryParse(tboxAmount.Text + e.Text, out val))
            {
                lblCurrentPay.Content = "₱" + val.ToString("N2");

                /**
                 * pano kunin yung balance?
                
                lblRemainingBalance.Content = remaining - val

                 * inuupdate ng part na to kung magkano mababawas
                 * sa existing loan.
                 * Pag di pwede/kailangan pwede naman tanggalin
                 * dinagdag ko lang to para may live update sa effect
                 * ng payment.
                 * 
                 * pag pwede ng kunin yung current balance, pwede yun
                 * mailagay sa loan details.
                 **/
                lblRemainingBalance.Content = franchise.LongTermLoanBalance - val;
            }
        }

        private void tbPenalty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            double val;
            if (double.TryParse(tbPenalty.Text + e.Text, out val))
            {
                lblPenalty.Content = "₱" + (penalty * val).ToString("N2");
                lblTotalBreak.Content = "₱" + (breakdown + (penalty * val)).ToString("N2");
            }
        }
    }
}
