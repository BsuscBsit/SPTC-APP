using System;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using static SPTC_APP.Objects.Ledger;

namespace SPTC_APP.Objects
{
    public class Ledger
    {
        public const string SHARE_CAPITAL = "SHARECAPITAL";
        public const string LOAN = "LOAN";
        public const string LT_LOAN = "LONGTERMLOAN";
        public const string APPLY_LOAN = "APPLY_LOAN";
        public const string APPLY_LT_LOAN = "APPLY_LT_LOAN";

        public class Loan
        {
            public int id { get; private set; }
            public int franchiseId { get; set; }
            public DateTime? date { get; set; }
            public double amount { get; set; }
            public string details { get; set; }
            public double monthlyInterest { get; set; }
            public double termsofpayment { get; set; }
            public double paymentDues { get; set; }

            public bool isFullyPaid { get; set; } = false;

            private Upsert loan;

            public Loan()
            {
                loan = new Upsert(Table.LOAN, -1);
            }

            public Loan(MySqlDataReader reader)
            {
                loan = null;
                this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
                this.franchiseId = Retrieve.GetValueOrDefault<int>(reader, Field.FRANCHISE_ID);
                this.date = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE);
                this.amount = Retrieve.GetValueOrDefault<double>(reader, Field.AMOUNT);
                this.details = Retrieve.GetValueOrDefault<string>(reader, Field.DETAILS);
                this.monthlyInterest = Retrieve.GetValueOrDefault<double>(reader, Field.MONTHLY_INTEREST);
                this.termsofpayment = Retrieve.GetValueOrDefault<double>(reader, Field.TERMS_OF_PAYMENT_MONTH);
                this.paymentDues = Retrieve.GetValueOrDefault<double>(reader, Field.PAYMENT_DUES);
            }
            public bool WriteInto(
                    int franchiseId,
                    DateTime dateLoaned,
                    double amount,
                    string details,
                    double monthlyInterest,
                    int termsofpayment,
                    double paymentDues)
            {
                this.franchiseId = franchiseId;
                this.date = dateLoaned;
                this.amount = amount;
                this.details = details;
                this.monthlyInterest = monthlyInterest;
                this.termsofpayment = termsofpayment;
                this.paymentDues = paymentDues;
                return true;
            }


            public int Save()
            {
                if (loan == null)
                {
                    loan = new Upsert(Table.LOAN, id);
                }
                loan.Insert(Field.FRANCHISE_ID, franchiseId);
                loan.Insert(Field.DATE, date);
                loan.Insert(Field.AMOUNT, amount);
                loan.Insert(Field.DETAILS, details);
                loan.Insert(Field.MONTHLY_INTEREST, monthlyInterest);
                loan.Insert(Field.TERMS_OF_PAYMENT_MONTH, termsofpayment);
                loan.Insert(Field.PAYMENT_DUES, paymentDues);
                loan.Insert(Field.IS_FULLY_PAID, isFullyPaid);
                loan.Save();
                id = loan.id;

                return id;
            }

            public bool Delete()
            {
                if (loan == null)
                {
                    loan = new Upsert(Table.LOAN, id);
                    loan.Insert(Field.ISDELETED, true);
                    loan.Save();
                    return true;
                }
                return false;
            }
        }
        public class ShareCapital
        {
            public int id { get; private set; }
            public int franchiseId { get; set; }
            public DateTime? date { get; set; }
            public double beginningBalance { get; set; }
            public double lastBalance { get; set; }

            private Upsert shareCapital;

            public ShareCapital()
            {
                shareCapital = new Upsert(Table.SHARE_CAPITAL, -1);
            }

            public ShareCapital(MySqlDataReader reader)
            {
                shareCapital = null;
                this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
                this.franchiseId = Retrieve.GetValueOrDefault<int>(reader, Field.FRANCHISE_ID);
                this.date = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE);
                this.beginningBalance = Retrieve.GetValueOrDefault<double>(reader, Field.BEGINNING_BALANCE);
                this.lastBalance = Retrieve.GetValueOrDefault<double>(reader, Field.LAST_BALANCE);
            }
            public bool WriteInto(
                    int franchiseId,
                    DateTime date,
                    double beginningBalance,
                    double lastBalance)
            {
                this.franchiseId = franchiseId;
                this.date = date;
                this.beginningBalance = beginningBalance;
                this.lastBalance = lastBalance;
                return true;
            }


            public int Save()
            {
                if (shareCapital == null)
                {
                    shareCapital = new Upsert(Table.SHARE_CAPITAL, id);
                }
                shareCapital.Insert(Field.FRANCHISE_ID, franchiseId);
                shareCapital.Insert(Field.DATE, date);
                shareCapital.Insert(Field.BEGINNING_BALANCE, beginningBalance);
                shareCapital.Insert(Field.LAST_BALANCE, lastBalance);
                shareCapital.Save();
                id = shareCapital.id;

                return id;
            }

            public bool Delete()
            {
                if (shareCapital == null)
                {
                    shareCapital = new Upsert(Table.SHARE_CAPITAL, id);
                    shareCapital.Insert(Field.ISDELETED, true);
                    shareCapital.Save();
                    return true;
                }
                return false;
            }
        }
        public class LongTermLoan
        {
            public int id { get; private set; }
            public int franchiseId { get; set; }
            public DateTime? date { get; set; }
            public int termsOfPaymentMonth { get; set; }
            public DateTime? startDate { get; set; }
            public DateTime? endDate { get; set; }
            public double amountLoaned { get; set; }
            public string details { get; set; }
            public double processingFee { get; set; }
            public double capitalBuildup { get; set; }
            public double paymentDues { get; set; }
            public bool isFullyPaid { get; set; } = false;


            private Upsert longTermLoan;

            public LongTermLoan()
            {
                longTermLoan = new Upsert(Table.LONG_TERM_LOAN, -1);
            }

            public LongTermLoan(MySqlDataReader reader)
            {
                longTermLoan = null;
                this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
                this.franchiseId = Retrieve.GetValueOrDefault<int>(reader, Field.FRANCHISE_ID);
                this.date = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE);
                this.termsOfPaymentMonth = Retrieve.GetValueOrDefault<int>(reader, Field.TERMS_OF_PAYMENT_MONTH);
                this.startDate = Retrieve.GetValueOrDefault<DateTime?>(reader, Field.START_DATE);
                this.endDate = Retrieve.GetValueOrDefault<DateTime?>(reader, Field.END_DATE);
                this.amountLoaned = Retrieve.GetValueOrDefault<double>(reader, Field.AMOUNT_LOANED);
                this.details = Retrieve.GetValueOrDefault<string>(reader, Field.DETAILS);
                this.processingFee = Retrieve.GetValueOrDefault<double>(reader, Field.PROCESSING_FEE);
                this.capitalBuildup = Retrieve.GetValueOrDefault<double>(reader, Field.CAPITAL_BUILDUP);
                this.paymentDues = Retrieve.GetValueOrDefault<double>(reader, Field.PAYMENT_DUES);
            }
            public bool WriteInto(
                    int franchiseId,
                    DateTime dateLoaned,
                    int termsOfPaymentMonth,
                    DateTime? startDate,
                    DateTime? endDate,
                    double amountLoaned,
                    string details,
                    double processingFee,
                    double paymentDues)
            {
                this.franchiseId = franchiseId;
                this.date = dateLoaned;
                this.termsOfPaymentMonth = termsOfPaymentMonth;
                this.startDate = startDate;
                this.endDate = endDate;
                this.amountLoaned = amountLoaned;
                this.details = details;
                this.processingFee = processingFee;
                this.paymentDues = paymentDues;
                return true;
            }

            public int Save()
            {
                if (longTermLoan == null)
                {
                    longTermLoan = new Upsert(Table.LONG_TERM_LOAN, id);
                }
                longTermLoan.Insert(Field.FRANCHISE_ID, franchiseId);
                longTermLoan.Insert(Field.DATE, date);
                longTermLoan.Insert(Field.TERMS_OF_PAYMENT_MONTH, termsOfPaymentMonth);
                longTermLoan.Insert(Field.START_DATE, startDate);
                longTermLoan.Insert(Field.END_DATE, endDate);
                longTermLoan.Insert(Field.AMOUNT_LOANED, amountLoaned);
                longTermLoan.Insert(Field.DETAILS, details);
                longTermLoan.Insert(Field.PROCESSING_FEE, processingFee);
                longTermLoan.Insert(Field.CAPITAL_BUILDUP, capitalBuildup);
                longTermLoan.Insert(Field.PAYMENT_DUES, paymentDues);
                longTermLoan.Insert(Field.IS_FULLY_PAID, isFullyPaid);
                longTermLoan.Save();
                id = longTermLoan.id;

                return id;
            }

            public bool Delete()
            {
                if (longTermLoan == null)
                {
                    longTermLoan = new Upsert(Table.LONG_TERM_LOAN, id);
                    longTermLoan.Insert(Field.ISDELETED, true);
                    longTermLoan.Save();
                    return true;
                }
                return false;
            }
        }
    }
}
