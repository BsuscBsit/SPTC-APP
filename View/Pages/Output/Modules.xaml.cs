using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using System;
using System.Collections.Generic;
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
                    new ColumnConfiguration("referenceNo", "OR#", minWidth: 60, isNumeric:true),
                    new ColumnConfiguration("monthyear", "MONTH/YEAR", minWidth: 60, isNumeric : true),
                    new ColumnConfiguration("deposit", "SHARE CAPITAL", minWidth: 100),
                    new ColumnConfiguration("deposit", "AMOUNT", minWidth: 100, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("penalties", "CBU", minWidth: 100, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("balance", "TOTAL", minWidth: 100, isCenter: true, isNumeric: true),
                };
                DataGridHelper<PaymentDetails<Ledger.ShareCapital>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.ShareCapital>>(dgLedger, columnConfigurations);


                foreach(PaymentDetails<Ledger.ShareCapital> tmp in franchise.GetShareCapitalLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                lblTotalLedger.Content = franchise.GetTotalShareCapital().ToString("0.00");
            } 
            else if(strmod == LOAN_APPLY)
            {
                dgLedger.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {

                    new ColumnConfiguration("displayDate", "DATE", minWidth: 80, isNumeric : true),
                    new ColumnConfiguration("details", "DESCRIPTION", minWidth: 120),
                    new ColumnConfiguration("amountLoaned", "AMOUNT", minWidth: 100, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("cv_or", "OR#", minWidth: 60, isNumeric: true),
                    new ColumnConfiguration("processingFee", "FEE", minWidth: 60, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("cbu", "CBU", minWidth: 60, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("termsofpayment", "MONTHS", minWidth: 50, isNumeric: true),
                    new ColumnConfiguration("interest", "INT", minWidth: 60, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("principal", "PRINCIPAL", minWidth: 120, isCenter: true, isNumeric: true),
                };
                DataGridHelper<object> dataGridHelper = new DataGridHelper<object>(dgLedger, columnConfigurations);


                foreach (object tmp in franchise.GetLoanAndLTLoan())
                {
                    dgLedger.Items.Add(tmp);
                }
                if ((franchise.LoanBalance + franchise.LongTermLoanBalance) > 0)
                {
                    lblTotal.Content = "TOTAL :";
                    lblTotalLedger.Content = (franchise.LoanBalance + franchise.LongTermLoanBalance).ToString("0.00");
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
                    new ColumnConfiguration("referenceNo", "OR NO.", minWidth: 60, isNumeric:true),
                    new ColumnConfiguration("monthyear", "MONTH/YEAR", minWidth: 60, isNumeric : true),
                    new ColumnConfiguration("interest", "INTEREST", minWidth: 100, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("deposit", "AMOUNT", minWidth: 100, isCenter: true, isNumeric: true),
                };
                DataGridHelper<PaymentDetails<Ledger.Loan>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.Loan>>(dgLedger, columnConfigurations);


                foreach (PaymentDetails<Ledger.Loan> tmp in franchise.GetLoanLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                if (franchise.LoanBalance > 0)
                {
                    lblTotal.Content = "BALANCE :";
                    lblTotalLedger.Content = franchise.LoanBalance.ToString("0.00");
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
                    new ColumnConfiguration("referenceNo", "REF NO.", minWidth: 60, isNumeric : true),
                    new ColumnConfiguration("monthyear", "MONTH/YEAR", minWidth: 60, isNumeric:true),
                    new ColumnConfiguration("interest", "INTEREST", minWidth: 100, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("deposit", "AMOUNT", minWidth: 100, isCenter: true, isNumeric: true),
                };
                DataGridHelper<PaymentDetails<Ledger.LongTermLoan>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.LongTermLoan>>(dgLedger, columnConfigurations);


                foreach (PaymentDetails<Ledger.LongTermLoan> tmp in franchise.GetlTLoanLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                if (franchise.LongTermLoanBalance > 0)
                {
                    lblTotal.Content = "BALANCE :";
                    lblTotalLedger.Content = franchise.LongTermLoanBalance.ToString("0.00");
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
                    new ColumnConfiguration("referenceNo", "REFERENCE NO.", minWidth : 60, isNumeric : true),
                    new ColumnConfiguration("balance", "BALANCE", minWidth: 100, isCenter: true, isNumeric: true),
                    new ColumnConfiguration("payment", "PAYMENT", minWidth: 100, isCenter: true, isNumeric: true),
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

                    new ColumnConfiguration("violationLevelCount", "NO.", minWidth: 60, isNumeric : true),
                    new ColumnConfiguration("violationType", "VIOLATION", minWidth: 80),
                    new ColumnConfiguration("dDate", "DATE", minWidth : 60, isNumeric : true),
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

                    new ColumnConfiguration("Operator.name.legalName", "OWNER", minWidth: 100),
                    new ColumnConfiguration("displayBuyingDate", "DATE", minWidth: 80),
                    new ColumnConfiguration("ShareCapital", "SHARE CAPITAL", minWidth: 100),
                };
                new DataGridHelper<PaymentHistory>(dgTransfer, columnConfigurations);
                List<Franchise> franchises = Retrieve.GetDataUsingQuery<Franchise>(RequestQuery.GET_ALL_FRANCHISE_WITH_BODYNUM(franchise.BodyNumber));
                foreach (var fran in franchises)
                {
                    dgTransfer.Items.Add(fran);
                }
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
                    l.delete();
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE, true);
                }
                else if (selectedPayment is PaymentDetails<Ledger.LongTermLoan> ltl)
                {
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
    }
}
