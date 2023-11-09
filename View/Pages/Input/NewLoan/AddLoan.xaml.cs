using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SPTC_APP.View.Pages.Input.NewLoan
{
    /// <summary>
    /// Interaction logic for AddLoan.xaml
    /// </summary>
    public partial class AddLoan : Window
    {
        public AddLoan()
        {
            /**
              * This window will be used for adding loan. LT, L, Emergency Loan.
              * Emergency loan maximum of 3 months 3k maximum.
              * Short term loan (L) 6 months 30k below.
              * Long term 12 months 31k above.
             **/

            InitializeComponent();
        }

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
    }
}
