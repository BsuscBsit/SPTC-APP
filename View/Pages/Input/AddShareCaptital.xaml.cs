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
    /// Interaction logic for AddShareCaptital.xaml
    /// </summary>
    public partial class AddShareCaptital : Window
    {
        private string closingMSG;
        private Franchise franchise;

        //tanggalin yung cash or cheque dito

        public AddShareCaptital(Franchise franchise)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            
            this.franchise = franchise;

            dpBdate.SelectedDate = DateTime.Now;
            dpBdate.DisplayDate = DateTime.Now;

            tboxRefNo.Text = (AppState.CV_OR_LAST + 1).ToString();

            tboxAmount.Text = AppState.TOTAL_SHARE_PER_MONTH.ToString();
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
            this.closingMSG = "Adding share capital was cancelled.";
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string displaydate = dpBdate.DisplayDate.ToString("MMM dd, yyyy");
            if (ControlWindow.ShowTwoway("Confirm Details", $"CV/OR Number: {tboxRefNo.Text}\nShare Capital Amount: {tboxAmount.Text}\nDate Selected: {displaydate}", Icons.NOTIFY))
            {
                PaymentDetails<Ledger.ShareCapital> capital = new PaymentDetails<Ledger.ShareCapital>();
                Ledger.ShareCapital share = new Ledger.ShareCapital();
                if(franchise.GetShareCapitals()?.FirstOrDefault() != null)
                {
                    share = franchise.GetShareCapitals().FirstOrDefault();
                } else
                {
                    share.WriteInto(franchise.id, DateTime.Now, 0, 0);
                }

                share.lastBalance = share.lastBalance + Double.Parse(tboxAmount.Text);
                capital.WriteInto(share, 0, dpBdate.DisplayDate, tboxRefNo.Text, Double.Parse(tboxAmount.Text), 0, "MONTHLY", share.lastBalance);
                share.last_payment = dpBdate.DisplayDate;
                capital.Save();

                AppState.CV_OR_LAST = Int32.Parse(tboxRefNo.Text);
                AppState.SaveToJson();
                //(AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                this.closingMSG = "Adding share capital history was successful.\nPlease refresh the view to see changes.";
                this.Close();
            }
        }

        private void initTextBoxes()
        {
            // Check tooltips kung tama ba description. (Optional)
            tboxAmount.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, "Enter share capital.", 0);
            tboxRefNo.DefaultTextBoxBehavior(NUMBER, true, gridToast, "Reference number.", 1);
        }
    }
}
