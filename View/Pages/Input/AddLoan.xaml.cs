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
    /// Interaction logic for AddLoan.xaml
    /// </summary>
    public partial class AddLoan : Window
    {
        Franchise franchise;
        private string closingMSG;
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

            loan = this.franchise.GetLoans().FirstOrDefault();
            
            tboxInterest.Text = loan.interest.ToString();

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
            if (ControlWindow.ShowTwoway("Adding Loan Payment", "Do you confirm this transaction?", Icons.NOTIFY))
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
            }
        }

        private void initTextBoxes()
        {
            // Check tooltips kung tama ba description. (Optional)
            tboxAmount.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Enter amount to loan.", 0);
            tboxRefNo.DefaultTextBoxBehavior(NUMBER, true, gridToast, "Reference number.", 1);
            tbPenalty.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Penalty amount.", 2);
            tboxInterest.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Enter loan interest monthly.", 3);
        }
    }
}
