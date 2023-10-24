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
        public const string LOAN = "Loan";
        public const string LTLOAN = "LTLoan";
        public const string TRANSFER = "TransferGrid";

        private Grid module;
        private string strmod;
        private Franchise franchise;
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
            if (strmod == SHARECAPITAL)
            { 
                dgLedger.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    
                    new ColumnConfiguration("displayDate", "DATE", width: 100),
                    new ColumnConfiguration("referenceNo", "REF NO.", width: 80),
                    new ColumnConfiguration("deposit", "DEPOSIT", width: 100),
                    new ColumnConfiguration("withdraw", "WITHDRAW", width: 100),
                    new ColumnConfiguration("deposit", "AMOUNT", width: 100),
                };
                DataGridHelper<PaymentDetails<Ledger.ShareCapital>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.ShareCapital>>(dgLedger, columnConfigurations);


                foreach(PaymentDetails<Ledger.ShareCapital> tmp in franchise.GetShareCapitalLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                lblTotalLedger.Content = franchise.GetTotalShareCapital().ToString("0.00");
            } 
            else if (strmod == LOAN)
            {
                dgLedger.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {

                    new ColumnConfiguration("displayDate", "DATE", width: 100),
                    new ColumnConfiguration("referenceNo", "REF NO.", width: 80),
                    new ColumnConfiguration("principal", "PRINCIPAL", width: 100),
                    new ColumnConfiguration("interest", "INTEREST", width: 100),
                    new ColumnConfiguration("penalties", "PENALTIES", width: 100),
                    new ColumnConfiguration("deposit", "AMOUNT", width: 100),
                };
                DataGridHelper<PaymentDetails<Ledger.Loan>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.Loan>>(dgLedger, columnConfigurations);


                foreach (PaymentDetails<Ledger.Loan> tmp in franchise.GetLoanLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                lblTotalLedger.Content = franchise.GetTotalLoan().ToString("0.00");
            } 
            else if (strmod == LTLOAN)
            {
                dgLedger.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {

                    new ColumnConfiguration("displayDate", "DATE", width: 100),
                    new ColumnConfiguration("referenceNo", "REF NO.", width: 80),
                    new ColumnConfiguration("principal", "CAPITAL BUILDUP", width: 100),
                    new ColumnConfiguration("interest", "FEES", width: 100),
                    new ColumnConfiguration("penalties", "PENALTIES", width: 100),
                    new ColumnConfiguration("deposit", "AMOUNT", width: 100),
                };
                DataGridHelper<PaymentDetails<Ledger.LongTermLoan>> dataGridHelper = new DataGridHelper<PaymentDetails<Ledger.LongTermLoan>>(dgLedger, columnConfigurations);


                foreach (PaymentDetails<Ledger.LongTermLoan> tmp in franchise.GetlTLoanLedger())
                {
                    dgLedger.Items.Add(tmp);
                }
                lblTotalLedger.Content = franchise.GetTotalLTLoan().ToString("0.00");
            }
            else if(strmod == HISTORY)
            {
                dgHistory.Items.Clear();
                List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
                {
                    
                    new ColumnConfiguration("date", "DATE", width: 100),
                    new ColumnConfiguration("ledgerType", "LEDGER TYPE", width: 80),
                    new ColumnConfiguration("referenceNo", "REFERENCE NO.", width: 100),
                    new ColumnConfiguration("balance", "BALANCE", width: 100),
                    new ColumnConfiguration("payment", "PAYMENT", width: 100),
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

                    new ColumnConfiguration("violationLevelCount", "NO.", width: 100),
                    new ColumnConfiguration("violationType", "VIOLATION", width: 80),
                    new ColumnConfiguration("dDate", "DATE", width: 100),
                    new ColumnConfiguration("dDateStart", "FROM: ", width: 100),
                    new ColumnConfiguration("dDateEnd", "TO: ", width: 100),
                    new ColumnConfiguration("remarks", "REMARKS", width: 100),
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

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            TestButton.Background = Brushes.Green;
        }
    }

    
}
