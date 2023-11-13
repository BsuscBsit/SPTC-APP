﻿using SPTC_APP.Objects;
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
        private string loantext;// normal or emergency , all caps please
        private double loanAmount; // magkano yung loan
        private double loanProcessingFee; // actual value to be stord, if 2%
        private double loanCbu; // also 2%
        private int loanMonthsCount; // for both longterma anbd short term
        private double loanInterest; // based on calculations
        private double loanPrincipal;
        private double interestRate;

        private bool isLoan;
        public ApplyLoan(Franchise fran)
        {
            /**
              * This window will be used for adding loan. LT, L, Emergency Loan.
              * Emergency loan maximum of 3 months 3k maximum.
              * Short term loan (L) 6 months 30k below.
              * Long term 12 months 31k above.
              * Add this restrictions to frontend
             **/

            InitializeComponent();
            cbLoanType.SelectedIndex = 0;
            initTextBoxes();
            this.franchise = fran;
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };

            //make sure the bodynumber cannot be edited
            tbBodyNum.Content = this.franchise?.BodyNumber?.ToString() ?? "N/A";
            DraggingHelper.DragWindow(topBar);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            if(computeResult())
            { //If all inputs are filled ,
                double penalty = this.loanAmount * this.interestRate;
                if (isLoan) // if LOAN
                {
                    Ledger.Loan loan = new Ledger.Loan();
                    loan.WriteInto(this.franchise.id, DateTime.Now, this.loanAmount,this.ornum, this.loantext, this.loanProcessingFee, this.loanCbu, this.loanMonthsCount, this.loanInterest, this.loanPrincipal);
                    loan.Save();
                    PaymentDetails<Ledger.Loan> payment = new PaymentDetails<Ledger.Loan>();
                    payment.WriteInto(loan, 0, DateTime.Now, this.ornum, -loanAmount, penalty, loantext, loanPrincipal);
                    payment.isApply = true;
                    payment.ledgername = Ledger.APPLY_LOAN;
                    payment.Save();
                }
                else // if LONG TERM LOAN
                {
                    //this.loantext = "LONGTERMLOAN";

                    Ledger.LongTermLoan ltloan = new Ledger.LongTermLoan();
                    ltloan.WriteInto(this.franchise.id, DateTime.Now, this.loanAmount,this.ornum, this.loantext, this.loanProcessingFee, this.loanCbu, this.loanMonthsCount, this.loanInterest, this.loanPrincipal);
                    ltloan.Save();
                    PaymentDetails<Ledger.LongTermLoan> payment = new PaymentDetails<Ledger.LongTermLoan>();
                    payment.WriteInto(ltloan, 0, DateTime.Now, this.ornum, -loanAmount, penalty, this.loantext, this.loanPrincipal);
                    payment.isApply = true;
                    payment.ledgername = Ledger.APPLY_LT_LOAN;
                    payment.Save();
                }
                this.Close();
            }
            else
            {
                ControlWindow.ShowStatic("Input Incomplete", "Some required inputs are empty", Icons.ERROR);
                this.Close();
            }
        }

        private bool computeResult()
        {
            Dictionary<TextBox, bool> fillStatus = new Dictionary<TextBox, bool>
            {
                /*0*/{ tbAmount, tbAmount.Text.Length > 0 },
                /*1*/{ tbPF, tbPF.Text.Length > 0 },
                /*2*/{ tbCBU, tbCBU.Text.Length > 0 },
                /*3*/{ tbLoanLen, tbLoanLen.Text.Length > 0 },
                /*4*/{ tbInterest, tbInterest.Text.Length > 0 }
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

                    double totalFinal = 0.0;
                    double penalty = 0.0;
                    if (double.TryParse(tbPenalty.Text, out double ratio))
                    {
                        penalty = userVars[0] * (ratio / 100);
                    }

                    switch (cbLoanType.SelectedIndex)
                    {
                        case 0: // Short Term
                            pfFinal = userVars[0] * pfRatio;
                            cbuFinal = userVars[0] * cbuRatio;
                            interestFinal = (userVars[0] * interestRatio) * userVars[3];
                            deductions = pfFinal + cbuFinal + interestFinal;

                            principal = userVars[0] - deductions;

                            // Monthly Breakdown
                            loanReceivable = principal / userVars[3];
                            totalFinal = loanReceivable;
                            break;

                        case 1: // Long Term
                            pfFinal = userVars[0] * pfRatio;
                            cbuFinal = userVars[0] * cbuRatio;
                            interestFinal = (userVars[0] * interestRatio) * userVars[3];
                            deductions = pfFinal + cbuFinal;

                            principal = userVars[0] - deductions;

                            // Monthly Breakdown
                            loanReceivable = userVars[0] / userVars[3];
                            interestReceivable = userVars[0] * interestRatio;
                            totalFinal = loanReceivable + interestReceivable;
                            break;

                        case 2: // Emergency
                            pfFinal = userVars[0] * pfRatio;
                            cbuFinal = userVars[0] * cbuRatio;
                            deductions = pfFinal + cbuFinal;

                            principal = userVars[0];

                            // Monthly Breakdown
                            principal = userVars[0];
                            loanReceivable = userVars[0] / userVars[3];
                            interestReceivable = userVars[0] * interestRatio;
                            interestFinal = userVars[0] * interestRatio;
                            totalFinal = loanReceivable + interestReceivable;
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

                    double lr = Math.Round(loanReceivable, 2);
                    double tf = Math.Round(totalFinal, 2);
                    string principalStr = principal.ToString();
                    string pfFinalStr = pfFinal.ToString();
                    string cbuFinalStr = cbuFinal.ToString();
                    string interestRecStr = interestReceivable.ToString();

                    switch (cbLoanType.SelectedIndex)
                    {
                        case 0:
                            UpdateLabels(
                                userVars[0].ToString(),
                                pfFinalStr,
                                interestFinal.ToString(),
                                (SolidColorBrush)FindResource("BrushRed"),
                                "Already deducted.",
                                (SolidColorBrush)FindResource("BrushRed"),
                                cbuFinalStr,
                                principalStr,
                                lr.ToString(),
                                tf.ToString(),
                                true);
                            break;

                        case 1:
                            UpdateLabels(
                                userVars[0].ToString(),
                                pfFinalStr,
                                "Not deducted yet.",
                                (SolidColorBrush)FindResource("BrushDeepGreen"),
                                interestRecStr,
                                (SolidColorBrush)FindResource("BrushDeepGreen"),
                                cbuFinalStr,
                                principalStr,
                                lr.ToString(),
                                tf.ToString(),
                                false);
                            break;

                        case 2:
                            UpdateLabels(
                                userVars[0].ToString(),
                                $"{pfFinalStr} (To pay)",
                                "Not deducted yet.",
                                (SolidColorBrush)FindResource("BrushDeepGreen"),
                                interestRecStr,
                                (SolidColorBrush)FindResource("BrushDeepGreen"),
                                $"{cbuFinalStr} (To pay)",
                                principalStr,
                                lr.ToString(),
                                tf.ToString(),
                                true);
                            break;

                    }

                    void UpdateLabels(
                        string loanAmount,
                        string pfTotal,
                        string interestTotal,
                        SolidColorBrush interestTotalForeground,
                        string interestRecievableTotal,
                        SolidColorBrush interestRecForeground,
                        string cbuTotal,
                        string principalTotal,
                        string loanReceivableTotal,
                        string breakdownTotal,
                        bool isLoan)
                    {
                        lblLoanAmount.Content = loanAmount;
                        lblPFTotal.Content = pfTotal;
                        lblInterestTotal.Content = interestTotal;
                        lblInterestTotal.Foreground = interestTotalForeground;
                        lblCBUTotal.Content = cbuTotal;
                        lblPrincipalTotal.Content = principalTotal;
                        lblPrincipalTotal2.Content = principalTotal;
                        lblLoanRecievableTotal.Content = loanReceivableTotal;
                        lblInterestRecievableTotal.Content = interestRecievableTotal;
                        lblInterestRecievableTotal.Foreground = interestRecForeground;
                        lblBreakdownTotal.Content = breakdownTotal;
                        this.isLoan = isLoan;
                    }

                    lblPenalty.Content = penalty.ToString();
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
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCompute_Click(object sender, RoutedEventArgs e)
        {
            computeResult();
        }

        private void cbLoanType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            presetFields(cbLoanType.SelectedIndex, false);
        }


        double? minLoan, maxLoan, minMont, maxMont;
        private void initTextBoxes()
        {
            tbCVORNum.DefaultTextBoxBehavior(CVOR, true, gridToast, "Cash Voucher and/or Official Reciept Number/s.", 0, true, 50);
            tbPenalty.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 2, "Penalty amount for overdue.", 0, 100);
            tbPF.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 3, "Processing fees.", 0, 100);
            tbCBU.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 4, "CBU", 0, 100);
            tbInterest.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 6, "Interest rate.", 0, 100);

            tbAmount.NumericTextBoxBehavior(DECIMALUNSIGNED, gridToast, 1, "Loan Amount.", 1);
            tbLoanLen.NumericTextBoxBehavior(WHOLEUNSIGNED, gridToast, 5, "Term Length.", 1);
            presetFields(0);
        }

        private void presetFields(int preset, bool isReset = true)
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
                    case 0:
                        this.loantext = "SHORTTERMLOAN";
                        tbLoanLen.Text = "6";
                        break;
                    case 1:
                        this.loantext = "LONGTERMLOAN";
                        tbLoanLen.Text = "12";
                        break;
                    case 2:
                        this.loantext = "EMERGENCYLOAN";
                        tbLoanLen.Text = "3";
                        break;
                }
            }
            else
            {
                switch (preset)
                {
                    case 0:
                        this.loantext = "SHORTTERMLOAN";
                        minLoan = 1;
                        maxLoan = 30000;
                        minMont = 6;
                        maxMont = null;
                        break;
                    case 1:
                        this.loantext = "LONGTERMLOAN";
                        minLoan = 31000;
                        maxLoan = null;
                        minMont = 12;
                        maxMont = null;
                        break;
                    case 2:
                        this.loantext = "EMERGENCYLOAN";
                        minLoan = 1;
                        maxLoan = 3000;
                        minMont = 1;
                        maxMont = 3;
                        break;
                }
            }
        }
        private bool ValidSize()
        {
            if(double.TryParse(tbAmount.Text, out double minlon) && minlon < minLoan)
            {
                VisualStateManager.GoToState(tbAmount, "InputInvalidated", true);
                new Toast(gridToast, $"Value {Math.Round(minlon, 5)} is below the minimum limit of {Math.Round((double)minLoan, 3)}.");
                return false;
            }
            if (double.TryParse(tbAmount.Text, out double maxlon) && maxlon > maxLoan)
            {
                VisualStateManager.GoToState(tbAmount, "InputInvalidated", true);
                new Toast(gridToast, $"Value {Math.Round(maxlon, 5)} exceeds the maximum limit of {Math.Round((double)maxLoan, 3)}.");
                return false;
            }
            if (double.TryParse(tbLoanLen.Text, out double minmon) && minmon < minMont)
            {
                VisualStateManager.GoToState(tbLoanLen, "InputInvalidated", true);
                new Toast(gridToast, $"Value {Math.Round(minmon, 5)} is below the minimum limit of {Math.Round((double)minMont, 3)}.");
                return false;
            }
            if (double.TryParse(tbLoanLen.Text, out double maxmon) && maxmon > maxMont)
            {
                VisualStateManager.GoToState(tbLoanLen, "InputInvalidated", true);
                new Toast(gridToast, $"Value {Math.Round(maxmon, 5)} exceeds the maximum limit of {Math.Round((double)maxMont, 3)}.");
                return false;
            }

            return true;
        }
    }
}
