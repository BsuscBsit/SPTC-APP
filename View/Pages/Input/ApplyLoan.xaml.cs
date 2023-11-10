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
        public ApplyLoan(Franchise fran)
        {
            /**
              * This window will be used for adding loan. LT, L, Emergency Loan.
              * Emergency loan maximum of 3 months 3k maximum.
              * Short term loan (L) 6 months 30k below.
              * Long term 12 months 31k above.
             **/

            InitializeComponent();

            this.franchise = fran;
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();

            base.OnClosing(e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            // TODO: computations

            /**
                  * Processing Fee = Loan Amount * tbPF
                  * CBU = Loan Amount * tbCBU
                  * Interest = (Loan Amount * tbInterest) * tbLoanLen
                  * Principal = Loan Amount - (Processing Fee + CBU + Interest)
                  * 
                  * Breakdown in Monthly
                  * Loan Receivable = Principal / tbLoanLen
                  * Interest Receivable = Loan Amount * tbInterest
                  * Total = Loan Receivable + Interest Recievable
                  * Overdue Penalty = Loan Amount * tbInterest
                  * 
                 **/
            
            if(true){ //If all inputs are filled 
                this.Close();
            }
            else
            {
                ControlWindow.ShowStatic("Input Incomplete", "Some required inputs are empty", Icons.ERROR);

                this.Close()
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
