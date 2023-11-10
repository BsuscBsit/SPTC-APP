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
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for AddLTLoan.xaml
    /// </summary>
    public partial class AddLTLoan : Window
    {

        Franchise franchise;
        public AddLTLoan(Franchise franchise)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.franchise = franchise;
            dpBdate.DisplayDate = DateTime.Now;
            dpBdate.SelectedDate = DateTime.Now;
            
            DraggingHelper.DragWindow(topBar);
            tboxAmount.Focus();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ControlWindow.ShowTwoway("Adding new record", "Confirm?", Icons.NOTIFY))
            {
                double amount = Double.TryParse(tboxAmount.Text, out amount) ? amount : 0;
                double interest = Double.TryParse(tboxInterest.Text, out interest) ? interest : 0;
                double penalty = Double.TryParse(tbPenalty.Text, out penalty) ? penalty : 0;
                if (amount > 0)
                {
                    PaymentDetails<Ledger.LongTermLoan> loanPayment = new PaymentDetails<Ledger.LongTermLoan>();
                    
                    loanPayment.WriteInto(null, false, false, dpBdate.DisplayDate, tboxRefNo.Text, amount, penalty, "",  amount);
                    //loan.amountLoaned = loan.amountLoaned - Double.Parse(tboxAmount.Text);
                    /*if (loan.amountLoaned <= 0)
                    {
                        loan.isFullyPaid = true;
                    }
                    loan.Save();*/
                    loanPayment.Save();
                    
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                    this.Close();
                } else
                {

                }
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
