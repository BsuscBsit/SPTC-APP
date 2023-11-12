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
    /// Interaction logic for ApplyLoan.xaml
    /// </summary>
    public partial class ApplyLoan : Window
    {
        private Franchise franchise;

        //TODO: SETUP Variables
        // palitan nalang ng actual result sa calculation mo @Diero
        //eto yumg masasave sa backend
        private string ornum;
        private string loantext;// normal or emergency , all caps please
        private double loanAmount; // magkano yung loan
        private double loanProcessingFee; // actual value to be stord, if 2%
        private double loanCbu; // also 2%
        private int loanMonthsCount; // for both longterma anbd short term
        private double loanInterest; // based on calculations
        private double loanPrincipal;
        //paymentDues or Monthly Receivable is automatically clculated in backend, loanPrincipal / loanMonthCount

        private bool isLoan;
        public ApplyLoan(Franchise fran)
        {
            /**
              * This window will be used for adding loan. LT, L, Emergency Loan.
              * Emergency loan maximum of 3 months 3k maximum.
              * Short term loan (L) 6 months 30k below.
              * Long term 12 months 31k above.
              * Add this restrictions to frontend
             **/

            InitializeComponent();

            this.franchise = fran;
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };

            //make sure the bodynumber cannot be edited
            tbBodyNum.Content = this.franchise?.BodyNumber?.ToString() ?? "N/A";
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            if(computeResult())
            { //If all inputs are filled ,
                double penalty = this.loanProcessingFee + this.loanCbu + this.loanInterest;
                if (isLoan) // if LOAN
                {
                    Ledger.Loan loan = new Ledger.Loan();
                    loan.WriteInto(this.franchise.id, DateTime.Now, this.loanAmount,this.ornum, this.loantext, this.loanProcessingFee, this.loanCbu, this.loanMonthsCount, this.loanInterest, this.loanPrincipal);
                    loan.Save();
                    PaymentDetails<Ledger.Loan> payment = new PaymentDetails<Ledger.Loan>();
                    payment.WriteInto(loan, 0, DateTime.Now, this.ornum, -loanAmount, penalty, loantext, loanPrincipal);
                    payment.isApply = true;
                    payment.ledgername = Ledger.APPLY_LOAN;
                    payment.Save();
                }
                else // if LONG TERM LOAN
                {
                    this.loantext = "LONGTERMLOAN";

                    Ledger.LongTermLoan ltloan = new Ledger.LongTermLoan();
                    ltloan.WriteInto(this.franchise.id, DateTime.Now, this.loanAmount,this.ornum, this.loantext, this.loanProcessingFee, this.loanCbu, this.loanMonthsCount, this.loanInterest, this.loanPrincipal);
                    ltloan.Save();
                    PaymentDetails<Ledger.LongTermLoan> payment = new PaymentDetails<Ledger.LongTermLoan>();
                    payment.WriteInto(ltloan, 0, DateTime.Now, this.ornum, -loanAmount, penalty, this.loantext, this.loanPrincipal);
                    payment.isApply = true;
                    payment.ledgername = Ledger.APPLY_LT_LOAN;
                    payment.Save();
                }


                this.Close();
            }
            else
            {
                ControlWindow.ShowStatic("Input Incomplete", "Some required inputs are empty", Icons.ERROR);

                this.Close();
            }
        }

        private bool computeResult()
        {
            //Retrun true if all input fields are filled
            //Actual Computation
            //From textobx to private variables
            // ex: this.loanAmount = place verification if the input is double only etc....
            // TODO: computations

            /**
                  * Processing Fee = Loan Amount * tbPF // make sure to if tbPf is 2%, transform first to decimal, 0.02 before multiplifyng to Loan Amount
                  * CBU = Loan Amount * tbCBU //same here
                  * Interest = (Loan Amount * tbInterest) * tbLoanLen //and here
                  * Principal = Loan Amount - (Processing Fee + CBU + Interest) // this is ?, outbounds, not needed to be saved
                  * 
                  * Breakdown in Monthly
                  * Loan Receivable = Principal // tbLoanLen 
                  * Interest Receivable = Loan Amount * tbInterest
                  * Total = Loan Receivable + Interest Recievable //this is the actual principal to be saved to database
                  * Overdue Penalty = Loan Amount * tbInterest // not needed to be save to the database
                  * 
                 **/
            //SAMPLE
            this.ornum = "0001";
            this.loantext = "NORMAL";// normal or emergency , all caps please
            this.loanAmount = 10000; // magkano yung loan
            this.loanProcessingFee = 200; // actual value to be stord, if 2%
            this.loanCbu = 200; // also 2%
            this.loanMonthsCount = 6; // for both longterma anbd short term
            this.loanInterest = 900; // based on calculations
            this.loanPrincipal = 10900;

            this.isLoan = false; // do calculate if loan or Long term loan,

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCompute_Click(object sender, RoutedEventArgs e)
        {
            computeResult();
            //Display Result to the user
            //ex: lblTotalAmount = this.loanAmount;
        }
    }
}
