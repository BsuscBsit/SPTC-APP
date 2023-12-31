﻿using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace SPTC_APP.View.Pages.Output
{
    public partial class Modules : Window
    {
        public const string HISTORY = "HistoryGrid";
        public const string CODING = "CodingGrid";
        public const string VIOLATION = "ViolationGrid";
        public const string SHARECAPITAL = "ShareCapital";
        public const string LOAN_APPLY = "LoanApply";
        public const string LOAN = "Loan";
        public const string LTLOAN = "LTLoan";
        public const string TRANSFER = "TransferGrid";

        private Grid module;
        private string strmod;
        private Franchise franchise;

        private object selectedPayment;
        private Violation selectedViolation;
        public Modules(string moduleName, Franchise franchise)
        {
            InitializeComponent();
            switch (moduleName)
            {
                case HISTORY:
                    module = HistoryGrid;
                    break;
                case CODING:
                    module = CodingGrid;
                    break;
                case VIOLATION:
                    module = ViolationGrid;
                    break;
                case SHARECAPITAL:
                    module = gridLedger;
                    break;
                case LOAN_APPLY:
                    module = gridLedger;
                    break;
                case LOAN:
                    module = gridLedger;
                    break;
                case LTLOAN:
                    module = gridLedger;
                    break;
                case TRANSFER:
                    module = TransferGrid;
                    break;
                default:
                    module = null; 
                    break;
            }
            this.franchise = franchise;
            this.strmod = moduleName;
            RenderFranchiseInformation();
        }

        private void RenderFranchiseInformation()
        {
            //TODO: Designs here

            if (strmod == SHARECAPITAL)
            { 
                dgLedger.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("displayDate", "DATE", minWidth: 80, isNumeric : true),
                    new ColumnConfiguration("referenceNo", "OR NO.", minWidth: 80, isNumeric:true),
                    new ColumnConfiguration("monthyear", "M/Y", minWidth: 60, isNumeric : true),
                    new ColumnConfiguration("remarks", "DETAILS", minWidth: 150, isNumeric : true),
                    new ColumnConfiguration("deposit", "SHARE CAPITAL", minWidth: 100, isCenter: true, isNumeric: true, haspeso: true),
                    new ColumnConfiguration("balance", "TOTAL", minWidth: 100, isCenter: true, isNumeric: true, haspeso : true),
                };
                DataGridHelper<PaymentDetails<Ledger.ShareCapital>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.ShareCapital>>(dgLedger, columnConfigurations);


                foreach(PaymentDetails<Ledger.ShareCapital> tmp in franchise.GetShareCapitalLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                lblTotalLedger.Content = "\u20B1 " + franchise.ShareCapital.ToString("0.00");
            } 
            else if(strmod == LOAN_APPLY)
            {
                dgLedger.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {

                    new ColumnConfiguration("displayDate", "DATE", minWidth: 80, isNumeric : true),
                    new ColumnConfiguration("cv_or", "CV/OR", minWidth: 60, isNumeric: true),
                    new ColumnConfiguration("details", "DESCRIPTION", minWidth: 100, isCenter: true),
                    new ColumnConfiguration("amountLoaned", "AMOUNT", minWidth: 100, isCenter: true, isNumeric: true, haspeso:true),
                    //new ColumnConfiguration("processingFee", "FEE", minWidth: 60, isCenter: true, isNumeric: true, haspeso:true),
                    //new ColumnConfiguration("cbu", "CBU", minWidth: 60, isCenter: true, isNumeric: true, haspeso:true),
                    new ColumnConfiguration("termsofpayment", "TERM", minWidth: 60, isNumeric: true, isCenter: true),
                    //new ColumnConfiguration("interest", "INT", minWidth: 60, isCenter: true, isNumeric: true, haspeso : true),
                    new ColumnConfiguration("principal", "PRINCIPAL", minWidth: 120, isCenter: true, isNumeric: true, haspeso:true),
                    new ColumnConfiguration("status", "BALANCE", minWidth: 120, isCenter: true, isNumeric: true),
                };
                DataGridHelper<object> dataGridHelper = new DataGridHelper<object>(dgLedger, columnConfigurations);


                foreach (object tmp in franchise.GetLoanAndLTLoan())
                {
                    dgLedger.Items.Add(tmp);
                }
                if ((franchise.LoanBalance + franchise.LongTermLoanBalance) > 0)
                {
                    lblTotal.Content = "TOTAL :";
                    lblTotalLedger.Content = "\u20B1 " + (franchise.LoanBalance + franchise.LongTermLoanBalance).ToString("0.00");
                }
                else
                {
                    lblTotal.Content = "LOANS :";
                    lblTotalLedger.Content = "N/A";
                }
            }
            else if (strmod == LOAN)
            {
                dgLedger.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {

                    new ColumnConfiguration("displayDate", "DATE", minWidth: 80, isNumeric : true),
                    new ColumnConfiguration("referenceNo", "CV/OR", minWidth: 60, isNumeric:true),
                    new ColumnConfiguration("monthyear", "M/Y", minWidth: 60, isNumeric : true),
                    new ColumnConfiguration("deposit", "AMOUNT", minWidth: 100, isCenter: true, isNumeric: true, haspeso:true),
                    new ColumnConfiguration("penalties", "PENALTY", minWidth: 100, isCenter: true, isNumeric: true, haspeso : true),
                };
                DataGridHelper<PaymentDetails<Ledger.Loan>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.Loan>>(dgLedger, columnConfigurations);


                foreach (PaymentDetails<Ledger.Loan> tmp in franchise.GetLoanLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                if (franchise.LoanBalance > 0)
                {
                    lblTotal.Content = "BALANCE :";
                    lblTotalLedger.Content = "\u20B1 " +franchise.LoanBalance.ToString("0.00");
                } else
                {
                    lblTotal.Content = "CURRENT LOAN :";
                    lblTotalLedger.Content = "N/A";
                }
            } 
            else if (strmod == LTLOAN)
            {
                dgLedger.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {

                    new ColumnConfiguration("displayDate", "DATE", minWidth: 80, isNumeric: true),
                    new ColumnConfiguration("referenceNo", "CV/OR", minWidth: 60, isNumeric : true),
                    new ColumnConfiguration("monthyear", "M/Y", minWidth: 60, isNumeric:true),
                    new ColumnConfiguration("deposit", "AMOUNT", minWidth: 100, isCenter: true, isNumeric: true , haspeso : true),
                    new ColumnConfiguration("penalties", "PENALTY", minWidth: 100, isCenter: true, isNumeric: true, haspeso : true),
                };
                DataGridHelper<PaymentDetails<Ledger.LongTermLoan>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.LongTermLoan>>(dgLedger, columnConfigurations);


                foreach (PaymentDetails<Ledger.LongTermLoan> tmp in franchise.GetlTLoanLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                if (franchise.LongTermLoanBalance > 0)
                {
                    lblTotal.Content = "BALANCE :";
                    lblTotalLedger.Content = "\u20B1 " + franchise.LongTermLoanBalance.ToString("0.00");
                }
                else
                {
                    lblTotal.Content = "CURRENT LTLOAN :";
                    lblTotalLedger.Content = "N/A";
                }
            }
            else if(strmod == HISTORY)
            {
                dgHistory.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    
                    new ColumnConfiguration("date", "DATE", minWidth : 60, isNumeric : true),
                    new ColumnConfiguration("ledgerType", "LEDGER TYPE", minWidth: 80),
                    new ColumnConfiguration("referenceNo", "OR NO.", minWidth : 60, isNumeric : true),
                    new ColumnConfiguration("balance", "BALANCE", minWidth: 100, isCenter: true, isNumeric: true, haspeso : true),
                    new ColumnConfiguration("payment", "PAYMENT", minWidth: 100, isCenter: true, isNumeric: true, haspeso:true),
                };
                DataGridHelper<PaymentHistory> dataGridHelper = new DataGridHelper<PaymentHistory>(dgHistory, columnConfigurations);


                foreach(var tmp in franchise.GetPaymentList())
                {
                    dgHistory.Items.Add(tmp);
                }
            }
            else if(strmod == CODING)
            {
                lblBodyNum.Content = franchise.BodyNumber;
                List<Image> imgs = new List<Image> { imgMon, imgTue, imgWed, imgThu, imgFri };

                for (int i = 0; i < imgs.Count; i++)
                { 
                    if (int.TryParse(franchise.BodyNumber, out int bodyNumber))
                    {
                        int lastDigit = bodyNumber % 10;

                        if (i % 5 == lastDigit / 2) 
                        {
                            Uri uri = new Uri("../../Images/icons/cross.png", UriKind.Relative);
                            BitmapImage bitmapImage = new BitmapImage(uri);
                            imgs[i].Source = bitmapImage;
                        }
                        else
                        {
                            Uri uri = new Uri("../../Images/icons/check.png", UriKind.Relative);
                            BitmapImage bitmapImage = new BitmapImage(uri);
                            imgs[i].Source = bitmapImage;
                        }
                    }
                }

            }
            else if(strmod == VIOLATION)
            {
                dgDriverViolation.Items.Clear();
                lblDriverName.Content = franchise.Driver?.name?.legalName?.ToString() ?? "N/A";
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {

                    //new ColumnConfiguration("violationLevelCount", "NO.", minWidth: 60, isNumeric : true),
                    new ColumnConfiguration("violationType", "VIOLATION", minWidth: 80),
                    new ColumnConfiguration("dDate", "DATE", minWidth : 80, isNumeric : true),
                    new ColumnConfiguration("dDateStart", "FROM: ", minWidth: 60, isNumeric:true),
                    new ColumnConfiguration("dDateEnd", "TO: ", minWidth: 60, isNumeric:true),
                    new ColumnConfiguration("remarks", "REMARKS", minWidth: 100),
                };
                new DataGridHelper<PaymentHistory>(dgDriverViolation, columnConfigurations);
                if (franchise.Driver != null)
                {
                    List<Violation> violationList = Retrieve.GetDataUsingQuery<Violation>(RequestQuery.GET_VIOLATION_LIST_OF(franchise.id, franchise.Driver.name?.id ?? -1));
                    foreach (var vio in violationList)
                    {
                        dgDriverViolation.Items.Add(vio);
                    }
                }
            }
            else if(strmod == TRANSFER)
            {
                dgTransfer.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    new ColumnConfiguration("lastFranchise.Operator.name.legalName", "FROM", minWidth: 100),
                    new ColumnConfiguration("Operator.name.legalName", "TO", minWidth: 100),
                    new ColumnConfiguration("displayBuyingDate", "DATE", minWidth: 80),
                    new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", minWidth: 100, haspeso:true),
                };
                new DataGridHelper<PaymentHistory>(dgTransfer, columnConfigurations);
                List<Franchise> franchises = Retrieve.GetDataUsingQuery<Franchise>(RequestQuery.GET_ALL_FRANCHISE_WITH_BODYNUM(franchise.BodyNumber));
                double totalShareCapital = 0;
                foreach (var fran in franchises)
                {
                    dgTransfer.Items.Add(fran);
                    totalShareCapital += fran.ShareCapital;
                }
                lblTransferTotal.Content = "\u20B1 " + totalShareCapital.ToString("0.00");
            }
        }

        public Grid Fetch()
        {
            if (module.Parent != null)
            {
                Grid currentParent = module.Parent as Grid;
                if (currentParent != null)
                {
                    currentParent.Children.Remove(module);
                }
            }
            this.Close();
            module.Visibility = Visibility.Visible;
            return module;
        }

        private void dgLedger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPayment = null;
            lblSelectedLedger.Content = "N/A";
            btnDeletePayment.IsEnabled = false;
            if(module == gridLedger)
            {
                if(dgLedger.SelectedItem is PaymentDetails<Ledger.ShareCapital> sc)
                {
                    lblSelectedLedger.Content = "ref no.: " + sc.referenceNo;
                    selectedPayment = sc;
                    if (AppState.USER.position?.accesses[13] ?? false)
                        btnDeletePayment.IsEnabled = true;
                }
                else if (dgLedger.SelectedItem is PaymentDetails<Ledger.Loan> l)
                {
                    lblSelectedLedger.Content = "ref no.: " + l.referenceNo;
                    selectedPayment = l;
                    if (AppState.USER.position?.accesses[14] ?? false)
                        btnDeletePayment.IsEnabled = true;
                }
                else if (dgLedger.SelectedItem is PaymentDetails<Ledger.LongTermLoan> ltl)
                {
                    lblSelectedLedger.Content = "ref no.: " + ltl.referenceNo;
                    selectedPayment = ltl;
                    if (AppState.USER.position?.accesses[15] ?? false)
                        btnDeletePayment.IsEnabled = true;
                }
                else if(dgLedger.SelectedItem is Ledger.Loan led)
                {
                    lblSelectedLedger.Content = "Loaned Amount: " + led.amountLoaned;
                    selectedPayment = led;
                    lblTotal.Content = "BALANCE :";
                    lblTotalLedger.Content = led.amount;
                    if (AppState.USER.position?.accesses[14] ?? false)
                        btnDeletePayment.IsEnabled = true;
                    //Moved to DoubleClick mouse event

                }
                else if (dgLedger.SelectedItem is Ledger.LongTermLoan lled)
                {
                    lblSelectedLedger.Content = "Loaned Amount: " + lled.amountLoaned;
                    selectedPayment = lled;
                    lblTotal.Content = "BALANCE :";
                    lblTotalLedger.Content = lled.amount;
                    if (AppState.USER.position?.accesses[15] ?? false)
                        btnDeletePayment.IsEnabled = true;

                }
            }
        }

        private void btnDeletePayment_Click(object sender, RoutedEventArgs e)
        {
            if(selectedPayment != null && ControlWindow.ShowTwoway("Deletion", $"Removing data {lblSelectedLedger.Content}"))
            {
                if (selectedPayment is PaymentDetails<Ledger.ShareCapital> sc)
                {
                    sc.delete();
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                }
                else if (selectedPayment is PaymentDetails<Ledger.Loan> l)
                {
                    Ledger.Loan loan = franchise.currentloan;
                    loan.amount = loan.amount + l.deposit;
                    loan.Save();
                    l.delete();
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                }
                else if (selectedPayment is PaymentDetails<Ledger.LongTermLoan> ltl)
                {
                    Ledger.LongTermLoan loan = franchise.currentltloan;
                    loan.amount = loan.amount + ltl.deposit;
                    loan.Save();
                    ltl.delete();
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                }
                else if(selectedPayment is Ledger.Loan led)
                {
                    led.Delete();
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                }
                else if (selectedPayment is Ledger.LongTermLoan lled)
                {
                    lled.Delete();
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                }
            }
        }

        private void dgLedger_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgLedger.SelectedItem != null)
            {
                if (dgLedger.SelectedItem is Ledger.Loan led)
                {
                    lblSelectedLedger.Content = "Loaned Amount: " + led.amountLoaned;
                    selectedPayment = led;
                    lblTotal.Content = "BALANCE :";
                    lblTotalLedger.Content = led.amount;
                    if (AppState.USER.position?.accesses[14] ?? false)
                        btnDeletePayment.IsEnabled = true;

                    AppState.mainwindow.ShowReceipt(
                        led.details,
                        led.amountLoaned,
                        led.processingFee,
                        led.cbu,
                        led.interest,
                        led.termsofpayment);

                }
                else if (dgLedger.SelectedItem is Ledger.LongTermLoan lled)
                {
                    lblSelectedLedger.Content = "Loaned Amount: " + lled.amountLoaned;
                    selectedPayment = lled;
                    lblTotal.Content = "BALANCE :";
                    lblTotalLedger.Content = lled.amount;
                    if (AppState.USER.position?.accesses[15] ?? false)
                        btnDeletePayment.IsEnabled = true;

                    AppState.mainwindow.ShowReceipt(
                        lled.details,
                        lled.amountLoaned,
                        lled.processingFee,
                        lled.cbu,
                        lled.interest,
                        lled.termsofpayment);
                }
            }
        }

        private void btnDeleteViolation_Click(object sender, RoutedEventArgs e)
        {
            if (selectedViolation != null && ControlWindow.ShowTwoway("Deletion", $"Removing data {lblSelectedViolation.Content}"))
            {
                if (selectedViolation is Violation sc)
                {
                    sc.Delete();
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                }
            }
        }

        private void dgDriverViolation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedViolation = null;
            lblSelectedViolation.Content = "N/A";
            btnDeleteViolation.IsEnabled = false;
            if (module == ViolationGrid)
            {
                if (dgDriverViolation.SelectedItem is Violation sc)
                {
                    lblSelectedViolation.Content = "Remarks : " + sc.remarks;
                    selectedViolation = sc;
                    if (AppState.USER.position?.accesses[12] ?? false)
                        btnDeleteViolation.IsEnabled = true;
                }
            }
        }
    }
}
