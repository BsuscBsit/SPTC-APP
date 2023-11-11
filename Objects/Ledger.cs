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
            public DateTime date { get; set; }
            public double amountLoaned { get; set; }
            public string details { get; set; }
            public double processingFee { get; set; }
            public double cbu { get; set; }
            public int termsofpayment { get; set; }
            public double interest { get; set; }
            public double principal { get; set; }
            public string displayDate
            {
                get
                {
                    return date.ToString("MMMM dd, yyyy");
                }
            }
            public double paymentDues
            {
                get
                {
                    return principal / termsofpayment;
                }
            }
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
                this.amountLoaned = Retrieve.GetValueOrDefault<double>(reader, Field.AMOUNT_LOANED);
                this.details = Retrieve.GetValueOrDefault<string>(reader, Field.DETAILS);
                this.processingFee = Retrieve.GetValueOrDefault<double>(reader, Field.PROCESSING_FEE);
                this.cbu = Retrieve.GetValueOrDefault<double>(reader, Field.CBU);
                this.termsofpayment = Retrieve.GetValueOrDefault<int>(reader, Field.TERMS_OF_PAYMENT_MONTH);
                this.interest = Retrieve.GetValueOrDefault<double>(reader, Field.INTEREST);
                this.principal = Retrieve.GetValueOrDefault<double>(reader, Field.PRINCIPAL);
            }
            public bool WriteInto(
                    int franchiseId,
                    DateTime dateLoaned,
                    double amountLoaned, string details, double processingfee, double cbu, int termsofpayment, double interest, double principal)
            {
                this.franchiseId = franchiseId;
                this.date = dateLoaned;
                this.amountLoaned = amountLoaned;
                this.details = details;
                this.processingFee = processingfee;
                this.cbu = cbu;
                this.termsofpayment = termsofpayment;
                this.interest = interest;
                this.principal = principal;
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
                loan.Insert(Field.AMOUNT_LOANED, amountLoaned);
                loan.Insert(Field.DETAILS, details);
                loan.Insert(Field.PROCESSING_FEE, processingFee);
                loan.Insert(Field.CBU, cbu);
                loan.Insert(Field.TERMS_OF_PAYMENT_MONTH, termsofpayment);
                loan.Insert(Field.INTEREST, interest);
                loan.Insert(Field.PRINCIPAL, principal);
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
            public DateTime date { get; set; }
            public double amountLoaned { get; set; }
            public string details { get; set; }
            public double processingFee { get; set; }
            public double cbu { get; set; }
            public int termsofpayment { get; set; }
            public double interest { get; set; }
            public double principal { get; set; }

            public double paymentDues
            {
                get
                {
                    return principal / termsofpayment;  
                }   
            }

            public bool isFullyPaid { get; set; } = false;

            private Upsert loan;

            public LongTermLoan()
            {
                loan = new Upsert(Table.LONG_TERM_LOAN, -1);
            }

            public LongTermLoan(MySqlDataReader reader)
            {
                loan = null;
                this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
                this.franchiseId = Retrieve.GetValueOrDefault<int>(reader, Field.FRANCHISE_ID);
                this.date = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE);
                this.amountLoaned = Retrieve.GetValueOrDefault<double>(reader, Field.AMOUNT_LOANED);
                this.details = Retrieve.GetValueOrDefault<string>(reader, Field.DETAILS);
                this.processingFee = Retrieve.GetValueOrDefault<double>(reader, Field.PROCESSING_FEE);
                this.cbu = Retrieve.GetValueOrDefault<double>(reader, Field.CBU);
                this.termsofpayment = Retrieve.GetValueOrDefault<int>(reader, Field.TERMS_OF_PAYMENT_MONTH);
                this.interest = Retrieve.GetValueOrDefault<double>(reader, Field.INTEREST);
                this.principal = Retrieve.GetValueOrDefault<double>(reader, Field.PRINCIPAL);
            }
            public bool WriteInto(
                    int franchiseId,
                    DateTime dateLoaned,
                    double amountLoaned, string details, double processingfee, double cbu, int termsofpayment, double interest, double principal)
            {
                this.franchiseId = franchiseId;
                this.date = dateLoaned;
                this.amountLoaned = amountLoaned;
                this.details = details;
                this.processingFee = processingFee;
                this.cbu = cbu;
                this.termsofpayment = termsofpayment;
                this.interest = interest;
                this.principal = principal;
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
                loan.Insert(Field.AMOUNT_LOANED, amountLoaned);
                loan.Insert(Field.DETAILS, details);
                loan.Insert(Field.PROCESSING_FEE, processingFee);
                loan.Insert(Field.CBU, cbu);
                loan.Insert(Field.TERMS_OF_PAYMENT_MONTH, termsofpayment);
                loan.Insert(Field.INTEREST, interest);
                loan.Insert(Field.PRINCIPAL, principal);
                loan.Insert(Field.IS_FULLY_PAID, isFullyPaid);
                loan.Save();
                id = loan.id;

                return id;
            }

            public bool Delete()
            {
                if (loan == null)
                {
                    loan = new Upsert(Table.LONG_TERM_LOAN, id);
                    loan.Insert(Field.ISDELETED, true);
                    loan.Save();
                    return true;
                }
                return false;
            }
        }
    }
}
