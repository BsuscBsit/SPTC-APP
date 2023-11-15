﻿using SPTC_APP.Objects;
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

            tboxInterest.Text = ltloan.interest.ToString();
            tbProcessingFee.Text = ltloan.processingFee.ToString();
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
                double amount = Double.TryParse(tboxAmount.Text, out amount) ? amount : 0;
                
                double penalty = Double.TryParse(tbPenalty.Text, out penalty) ? penalty : 0;
                if (amount > 0)
                {

                    /*PaymentDetails<Ledger.LongTermLoan> loanPayment = new PaymentDetails<Ledger.LongTermLoan>();
                    ltloan.amount = ltloan.amount - amount;
                    loanPayment.WriteInto(ltloan, 0, dpBdate.DisplayDate, tboxRefNo.Text, amount, penalty, "", ltloan.amount);
                    if (ltloan.amount <= 0)
                    {
                        ltloan.isFullyPaid = true;
                    }
                    loanPayment.Save();
                    
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);*/
                    this.closingMSG = "Adding loan payment history was successful.\nPlease refresh the view to see changes.";
                    this.Close();
                } else
                {

                }
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
            // Check tooltips kung tama ba description. (Optional)
            tboxAmount.DefaultTextBoxBehavior(NUMBERDASHPERIOD, true, gridToast, "Enter amount to loan.", 0);
            tboxRefNo.DefaultTextBoxBehavior(NUMBER, true, gridToast, "Reference number.", 1);
            tbPenalty.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Penalty amount.", 2);
            tboxInterest.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Enter loan interest monthly.", 3);
            tbProcessingFee.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Enter processing fee.", 4);
        }

    }
}
