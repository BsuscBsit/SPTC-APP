using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
    /// Interaction logic for ApplyLoan.xaml
    /// </summary>
    public partial class ApplyLoan : Window
    {
        private Franchise franchise;

        private string ornum;
        private string loantext;
        private double loanAmount; 
        private double loanProcessingFee; 
        private double loanCbu; 
        private int loanMonthsCount; 
        private double loanInterest; 
        private double loanPrincipal;
        private double interestRate;
        private double penaltyPercent;

        private string closingMSG;

        private bool isLoan;
        public ApplyLoan(Franchise fran)
        {
            InitializeComponent();
            cbLoanType.SelectedIndex = 0;
            initTextBoxes();
            this.franchise = fran;
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide();
                if (cbLoanType.Items.Count == 0)
                {
                    closingMSG = "This account already have existing Short and Long term loans.";
                    this.Close();
                }
            };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };

            tbBodyNum.Content = this.franchise?.BodyNumber?.ToString() ?? "N/A";
            DraggingHelper.DragWindow(topBar);



            dpDate.SelectedDate = DateTime.Now;
            dpDate.DisplayDate = DateTime.Now;

            tbCVORNum.Text = (AppState.CV_OR_LAST + 1).ToString();
            tbBodyNum.Content = franchise.BodyNumber;

            if (this.franchise.LoanBalance <= 0)
            {
                cbLoanType.Items.Add("Emergency");
                cbLoanType.Items.Add("Short Term");
            }
            if (this.franchise.LongTermLoanBalance <= 0)
            {
                cbLoanType.Items.Add("Long Term");
            }
            tbCVORNum.Focus();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            AppState.mainwindow?.displayToast(closingMSG);
            base.OnClosing(e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            if(computeResult())
            {
                if (!string.IsNullOrEmpty(tbCVORNum.Text))
                {
                    string displaydate = dpDate.DisplayDate.ToString("MMM dd, yyyy");
                    if (ControlWindow.ShowTwoway("Confirm Details", $"CV/OR Number: {tbCVORNum.Text}\nAmount Loaned: {tbAmount.Text}\nType: {loantext}\nDate Selected: {displaydate}", Icons.NOTIFY))
                    {
                        double penalty = this.loanAmount * this.interestRate;
                        if (isLoan)
                        {
                            Ledger.Loan loan = new Ledger.Loan();
                            loan.WriteInto(this.franchise.id, dpDate.SelectedDate ?? DateTime.Now, this.loanAmount, this.ornum, this.loantext, this.loanProcessingFee, this.loanCbu, this.loanMonthsCount, this.loanInterest, this.loanPrincipal, this.penaltyPercent);

                            PaymentDetails<Ledger.Loan> payment = new PaymentDetails<Ledger.Loan>();
                            payment.WriteInto(loan, 0, dpDate.SelectedDate ?? DateTime.Now, this.ornum, -loanAmount, penalty, loantext, loanAmount);
                            payment.isApply = true;
                            payment.ledgername = Ledger.APPLY_LOAN;
                            payment.Save();
                        }
                        else
                        {
                            Ledger.LongTermLoan ltloan = new Ledger.LongTermLoan();
                            ltloan.WriteInto(this.franchise.id, dpDate.SelectedDate ?? DateTime.Now, this.loanAmount, this.ornum, this.loantext, this.loanProcessingFee, this.loanCbu, this.loanMonthsCount, this.loanInterest, this.loanPrincipal, this.penaltyPercent);

                            PaymentDetails<Ledger.LongTermLoan> payment = new PaymentDetails<Ledger.LongTermLoan>();
                            payment.WriteInto(ltloan, 0, dpDate.SelectedDate??DateTime.Now, this.ornum, -loanAmount, penalty, this.loantext, loanAmount + loanInterest);
                            payment.isApply = true; 
                            payment.ledgername = Ledger.APPLY_LT_LOAN;
                            payment.Save();
                        }
                        if (cbLoanType.SelectedItem.ToString() != "Emergency")
                        {
                            PaymentDetails<Ledger.ShareCapital> capital = new PaymentDetails<Ledger.ShareCapital>();
                            Ledger.ShareCapital share = new Ledger.ShareCapital();
                            if (franchise.GetShareCapitals()?.FirstOrDefault() != null)
                            {
                                share = franchise.GetShareCapitals().FirstOrDefault();
                            }
                            else
                            {
                                share.WriteInto(franchise.id, DateTime.Now, 0, 0);
                            }
                            AppState.CV_OR_LAST = Int32.Parse(ornum);
                            AppState.SaveToJson();
                            share.lastBalance = share.lastBalance + loanCbu;
                            capital.WriteInto(share, 0, dpDate.SelectedDate ?? DateTime.Now, ornum, loanCbu, 0, "FROM " + loantext, share.lastBalance);
                            capital.Save();

                        }
                        closingMSG = "Adding loan history successful.\nPlease refresh the view to see changes.";
                        this.Close();
                    }
                }
                else
                {
                    VisualStateManager.GoToState(tbCVORNum, "InputInvalidated", true);
                    new Toast(gridToast, "Cash Voucher/Official Receipt cannot be empty.");
                }
            }
            else
            {
                ControlWindow.ShowStatic("Found Empty Fields", "Some required fields are empty.\n Please fill them up before continuing.", Icons.ERROR);
                this.Close();
            }
        }

        private bool computeResult()
        {
            Dictionary<TextBox, bool> fillStatus = new Dictionary<TextBox, bool>
            {
                { tbAmount, tbAmount.Text.Length > 0 },
                { tbPF, tbPF.Text.Length > 0 },
                { tbCBU, tbCBU.Text.Length > 0 },
                { tbLoanLen, tbLoanLen.Text.Length > 0 },
                { tbInterest, tbInterest.Text.Length > 0 }
            };
            if (!fillStatus.Any(kvp => kvp.Value == false))
            {
                if (ValidSize())
                {
                    int i = 0;
                    double[] userVars = { 0.0, 0.0, 0.0, 0.0, 0.0 };
                    foreach (var kvp in fillStatus)
                    {
                        if (double.TryParse(kvp.Key.Text, out double parsedValue))
                        {
                            userVars[i] = parsedValue;
                        }
                        i++;
                    }
                    double pfRatio = userVars[1] / 100;
                    double cbuRatio = userVars[2] / 100;
                    double interestRatio = userVars[4] / 100;
                    this.interestRate = interestRatio;

                    #region Key Formula

                    double pfFinal = 0.0;
                    double cbuFinal = 0.0;
                    double interestFinal = 0.0;
                    double deductions = 0.0;

                    double principal = 0.0;
                    double loanReceivable = 0.0;
                    double interestReceivable = 0.0;
                    double monthlyAmortization = 0.0;
                    double totalFinal = 0.0;
                    double penalty = 0.0;
                    if (double.TryParse(tbPenalty.Text, out double ratio))
                    {
                        penalty = userVars[0] * (ratio / 100);
                        this.penaltyPercent = ratio;
                    }

                    switch (cbLoanType.SelectedItem)
                    {
                        case "Short Term":
                            isLoan = true;
                            pfFinal = userVars[0] * pfRatio;
                            cbuFinal = userVars[0] * cbuRatio;
                            interestFinal = (userVars[0] * interestRatio) * userVars[3];
                            deductions = pfFinal + cbuFinal + interestFinal;

                            principal = userVars[0] - deductions;
                            loanReceivable = userVars[0];
                            totalFinal = loanReceivable;

                            // Monthly Breakdown
                            monthlyAmortization = totalFinal / userVars[3];
                            break;

                        case "Long Term":
                            isLoan = false;
                            pfFinal = userVars[0] * pfRatio;
                            cbuFinal = userVars[0] * cbuRatio;
                            interestFinal = (userVars[0] * interestRatio) * userVars[3];
                            deductions = pfFinal + cbuFinal;

                            principal = userVars[0] - deductions;
                            loanReceivable = userVars[0];
                            interestReceivable = interestFinal;
                            totalFinal = loanReceivable + interestReceivable;

                            // Monthly Breakdown
                            monthlyAmortization = totalFinal / userVars[3];
                            break;

                        case "Emergency":
                            isLoan = true;
                            principal = userVars[0];
                            interestFinal = (userVars[0] * interestRatio) * userVars[3];

                            loanReceivable = userVars[0];
                            interestReceivable = interestFinal;
                            totalFinal = loanReceivable + interestReceivable;

                            // Monthly Breakdown
                            monthlyAmortization = totalFinal / userVars[3];
                            break;
                    }

                    // Parsing to global variables.
                    this.ornum = tbCVORNum.Text ?? string.Empty;
                    this.loanAmount = userVars[0];
                    this.loanProcessingFee = pfFinal;
                    this.loanCbu = cbuFinal;
                    this.loanMonthsCount = (int)userVars[3];
                    this.loanInterest = interestFinal;
                    this.loanPrincipal = principal;


                    #endregion

                    switch (cbLoanType.SelectedItem)
                    {
                        case "Short Term":
                            UpdateLabelVal(
                                userVars[0],
                                pfFinal,
                                cbuFinal,
                                interestFinal,
                                principal,
                                loanReceivable,
                                null,
                                totalFinal,
                                monthlyAmortization);
                            LBL4.Content = $"Interest on Loan {userVars[3]} {(userVars[3] > 1 ? "Months" : "Month")}:";
                            LBL8.Content = $"Payment in Total of {userVars[3]} {(userVars[3] > 1 ? "Months" : "Month")}:";
                            break;

                        case "Long Term":
                            UpdateLabelVal(
                                userVars[0],
                                pfFinal,
                                cbuFinal,
                                null,
                                principal,
                                loanReceivable,
                                interestReceivable,
                                totalFinal,
                                monthlyAmortization,
                                false);
                            LBL7.Content = $"Interest Receivables {userVars[3]} {(userVars[3] > 1 ? "mos." : "mo.")}:";
                            LBL8.Content = $"Payment in Total of {userVars[3]} {(userVars[3] > 1 ? "Months" : "Month")}:";
                            break;

                        case "Emergency":
                            UpdateLabelVal(
                                userVars[0],
                                null,
                                null,
                                null,
                                principal,
                                loanReceivable,
                                interestReceivable,
                                totalFinal,
                                null);
                            LBL7.Content = $"Interest Receivables {userVars[3]} {(userVars[3] > 1 ? "mos." : "mo.")}:";
                            LBL8.Content = $"One-Time Payment of {userVars[3]} {(userVars[3] > 1 ? "Months" : "Month")} in Total:";
                            break;

                    }
                    
                    void UpdateLabelVal(
                        double? amountLoan,
                        double? processingFee,
                        double? cbu,
                        double? interest,
                        double? cobPrincipal,
                        double? loanRec,
                        double? inteRec,
                        double? totaRec,
                        double? montAmo,
                        bool isLoan = true)
                    {
                        this.isLoan = isLoan;
                        Label[] label = { LBL1, LBL2, LBL3, LBL4, LBL5, LBL6, LBL7, LBL8, LBL9 };
                        Dictionary<Label, double?> labelVals = new Dictionary<Label, double?>
                        {
                            { lblLoanAmount, amountLoan },
                            { lblPFTotal, processingFee },
                            { lblCBUTotal, cbu },
                            { lblInterestTotal, interest },
                            { lblPrincipalTotal, cobPrincipal },
                            { lblLoanRecievableTotal, loanRec },
                            { lblInterestRecievableTotal, inteRec },
                            { lblInTot, totaRec },
                            { lblMonthlyAmort, montAmo }
                        };

                        int index = 0;
                        foreach (var kvp in labelVals)
                        {
                            if(kvp.Value == null)
                            {
                                label[index].Visibility = Visibility.Collapsed;
                                kvp.Key.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                label[index].Visibility = Visibility.Visible;
                                kvp.Key.Visibility = Visibility.Visible;
                                kvp.Key.Content = $"\u20B1 {kvp.Value?.ToString("N2")}";
                            }
                            index++;
                        }
                    }
                        
                }
            }
            else
            {
                foreach (var kvp in fillStatus)
                {
                    if (!kvp.Value)
                    {
                        VisualStateManager.GoToState(kvp.Key, "InputInvalidated", true);
                    }
                }
                new Toast(gridToast, "Fill in all required fields before proceeding.", 2);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.closingMSG = "Adding loan was cancelled.";
            this.Close();
        }

        private void btnCompute_Click(object sender, RoutedEventArgs e)
        {
            computeResult();
        }

        private void cbLoanType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            labelValsReset();
            presetFields(cbLoanType.SelectedItem.ToString(), false);
        }


        double? minLoan, maxLoan, minMont, maxMont;

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if(cbLoanType.Items.Count > 0)
                presetFields(cbLoanType.SelectedItem.ToString(), false);
        }

        private void initTextBoxes()
        {
            tbCVORNum.DefaultTextBoxBehavior(CVOR, true, gridToast, "Cash Voucher and/or Official Reciept Number/s.", 0, true, 50);
            tbPenalty.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 2, "Penalty amount for overdue.", 0, 100);
            tbPF.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 3, "Processing fees.", 0, 100);
            tbCBU.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 4, "CBU", 0, 100);
            tbInterest.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 6, "Interest rate.", 0, 100);

            tbAmount.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 1, "Loan Amount.", 1);
            tbLoanLen.NumericTextBoxBehavior(WHOLEUNSIGNED, gridToast, 5, "Term Length.", 1);
            presetFields("Short Term");
        }

        private void presetFields(string preset, bool isReset = true)
        {
            if(isReset)
            {
                tbAmount.Text = "0";
                tbPenalty.Text = "2";
                tbPF.Text = "2";
                tbCBU.Text = "2";
                tbInterest.Text = "1.5";

                switch (preset)
                {
                    case "Short Term":
                        this.loantext = "SHORT TERM";
                        tbLoanLen.Text = "6";
                        break;
                    case "Long Term":
                        this.loantext = "LONG TERM";
                        tbLoanLen.Text = "12";
                        break;
                    case "Emergency":
                        this.loantext = "EMERGENCY";
                        tbLoanLen.Text = "2";
                        break;
                }
            }
            else
            {
                LBL2.Visibility = Visibility.Collapsed;
                LBL3.Visibility = Visibility.Collapsed;
                LBL4.Visibility = Visibility.Collapsed;
                LBL9.Visibility = Visibility.Collapsed;
                lblPFTotal.Visibility = Visibility.Collapsed;
                lblCBUTotal.Visibility = Visibility.Collapsed;
                lblInterestTotal.Visibility = Visibility.Collapsed;
                lblMonthlyAmort.Visibility = Visibility.Collapsed;
                lblInterestRecievableTotal.Visibility = Visibility.Collapsed;

                switch (preset)
                {
                    case "Short Term":
                        this.loantext = "SHORT TERM";
                        minLoan = 1;
                        maxLoan = 30000;
                        minMont = null;
                        maxMont = 6;

                        LBL2.Visibility = Visibility.Visible;
                        LBL3.Visibility = Visibility.Visible;
                        LBL4.Visibility = Visibility.Visible;
                        LBL9.Visibility = Visibility.Visible;
                        lblPFTotal.Visibility = Visibility.Visible;
                        lblCBUTotal.Visibility = Visibility.Visible;
                        lblMonthlyAmort.Visibility = Visibility.Visible;

                        LBL4.Content = "Interest on Loan:";
                        lblInterestTotal.Visibility = Visibility.Visible;
                        break;

                    case "Long Term":
                        this.loantext = "LONG TERM";
                        minLoan = 31000;
                        maxLoan = null;
                        minMont = 12;
                        maxMont = null;

                        LBL2.Visibility = Visibility.Visible;
                        LBL3.Visibility = Visibility.Visible;
                        LBL4.Visibility = Visibility.Collapsed;
                        LBL9.Visibility = Visibility.Visible;
                        lblPFTotal.Visibility = Visibility.Visible;
                        lblCBUTotal.Visibility = Visibility.Visible;
                        lblInterestRecievableTotal.Visibility = Visibility.Visible;
                        break;

                    case "Emergency":
                        this.loantext = "EMERGENCY";
                        minLoan = 1;
                        maxLoan = 3000;
                        minMont = 1;
                        maxMont = 2;

                        LBL8.Content = "One-Time Payment:";
                        break;
                }


                constrain(ref tbAmount, double.TryParse(tbAmount?.Text, out double newAmount) ? newAmount : 0, minLoan, maxLoan);
                constrain(ref tbLoanLen, double.TryParse(tbLoanLen?.Text, out double newLoanLen) ? newLoanLen : 0, minMont, maxMont);

                void constrain(ref TextBox textBox, double? newValue, double? minValue, double? maxValue)
                {
                    textBox.Text = (newValue > maxValue) ? maxValue.ToString() : (newValue < minValue) ? minValue.ToString() : textBox.Text;
                }

            }
        }
        private void labelValsReset()
        {
            Label[] labelVal =
            {
                lblLoanAmount,
                lblPFTotal,
                lblCBUTotal,
                lblInterestTotal,
                lblPrincipalTotal,
                lblLoanRecievableTotal,
                lblInterestRecievableTotal,
                lblInTot,
                lblMonthlyAmort
            };
            foreach (var val in labelVal)
            {
                val.Content = "N/A";
            }
        }

        private bool ValidSize()
        {
            bool validAmount = double.TryParse(tbAmount.Text, out double amount) && isValid(amount, minLoan, maxLoan, tbAmount, minLoan, maxLoan);
            bool validLen = double.TryParse(tbLoanLen.Text, out double loanLen) && isValid(loanLen, minMont, maxMont, tbLoanLen, minMont, maxMont);

            return (validAmount && validLen);

            bool isValid(double? value, double? minValue, double? maxValue, TextBox textBox, double? minLimit, double? maxLimit)
            {
                if (value.HasValue && (value < minValue || value > maxValue))
                {
                    VisualStateManager.GoToState(textBox, "InputInvalidated", true);
                    new Toast(gridToast, $"Value {Math.Round(value.Value, 5)} is {(value < minValue ? "below" : "exceeds")} the {(value < minValue ? "minimum" : "maximum")} limit of {Math.Round((double)(value < minValue ? minLimit : maxLimit), 3)}.", 3);
                    return false;
                }
                return true;
            }
        }
    }
}
