using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using SPTC_APP.View.Controls;

namespace SPTC_APP.Objects
{
    public class PaymentDetails<T>
    {
        public int id { get; private set; }

        private int ledgerId;
        public T ledger { get; set; }
        public double DivPat { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        public string referenceNo { get; set; }
        public double deposit { get; set; }
        public double penalties { get; set; }
        public string remarks { get; set; }
        public double balance { get; set; }
        public bool isApply { get; set; } = false;
        public string ledgername { get; set; }
        public string displayDate { 
            get
            {
                return date.ToString("MM/dd/yyyy");
            }
        }
        public string monthyear
        {
            get
            {
                return date.ToString("MMM, yyyy");
            }
        }


        public double paymentDues
        {
            get
            {
                if (ledger is Ledger.Loan)
                {
                    UpdateLedger();
                    return (ledger as Ledger.Loan).paymentDues;
                } 
                if(ledger is Ledger.LongTermLoan)
                {
                    UpdateLTLedger();
                    return (ledger as Ledger.LongTermLoan).paymentDues;
                }
                return 0;
            }
        }
        public double interest
        {
            get
            {
                if (ledger is Ledger.Loan)
                {
                    UpdateLedger();
                    return (ledger as Ledger.Loan).interest;
                }
                if (ledger is Ledger.LongTermLoan)
                {
                    UpdateLTLedger();
                    return (ledger as Ledger.LongTermLoan).interest;
                }
                return 0;
            }
        }


        public double withdraw
        {
            get
            {
                return (deposit < 0) ? deposit : 0;
            }
            set { }
        }

        private Upsert paymentDetails;

        private string getLedgerType()
        {
            if (ledger is Objects.Ledger.Loan)
            {
                return Table.LOAN;
            }
            if (ledger is Objects.Ledger.ShareCapital)
            {
                return Table.SHARE_CAPITAL;
            }
            if (ledger is Objects.Ledger.LongTermLoan)
            {
                return Table.LONG_TERM_LOAN;
            }
            
            return null;
        }

        private int saveLedger()
        {
            if (ledger is Objects.Ledger.Loan loan)
            {
                return loan.Save();
            }
            if (ledger is Objects.Ledger.ShareCapital sharecapital)
            {
                return sharecapital.Save();
            }
            if (ledger is Objects.Ledger.LongTermLoan ltloan)
            {
                return ltloan.Save();
            }
            return -1;
        }
        private void deletePayment()
        {
            if(ledger != null)
            {
                if (ledger is Objects.Ledger.Loan loan)
                {
                    loan.amount += (deposit );
                }
                if (ledger is Objects.Ledger.LongTermLoan ltloan)
                {
                    ltloan.amount += (deposit );
                }
            }
        }

        public PaymentDetails()
        {
            paymentDetails = new Upsert(Table.PAYMENT_DETAILS, -1);
        }

        public PaymentDetails(MySqlDataReader reader)
        {
            paymentDetails = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.DivPat = Retrieve.GetValueOrDefault<double>(reader, Field.DIV_PAT);
            this.date = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE);
            this.referenceNo = Retrieve.GetValueOrDefault<string>(reader, Field.REFERENCE_NO);
            this.deposit = Retrieve.GetValueOrDefault<double>(reader, Field.DEPOSIT);
            this.penalties = Retrieve.GetValueOrDefault<double>(reader, Field.PENALTIES);
            this.remarks = Retrieve.GetValueOrDefault<string>(reader, Field.REMARKS);
            this.balance = Retrieve.GetValueOrDefault<double>(reader, Field.BALANCE);
            this.ledgerId = Retrieve.GetValueOrDefault<int>(reader, Field.LEDGER_ID);
        }

        public void UpdateLedger()
        {
            if (ledgerId >= 0)
            {
                if (ledger == null)
                {
                    this.ledger = (dynamic)Retrieve.GetData<Ledger.Loan>(Table.LOAN, Select.ALL, Where.ID_, new MySqlParameter("id", ledgerId)).FirstOrDefault();
                }
            }
        }

        public void UpdateLTLedger()
        {
            if (ledgerId >= 0)
            {
                if (ledger == null)
                {
                    this.ledger = (dynamic)Retrieve.GetData<Ledger.LongTermLoan>(Table.LONG_TERM_LOAN, Select.ALL, Where.ID_, new MySqlParameter("id", ledgerId)).FirstOrDefault();
                }
            }
        }

        public bool WriteInto(T lledger, double isDVP, DateTime ldate, string lreferenceNo, double ldeposit, double lpenalties, string lremarks, double balance)
        {
            this.ledger = lledger;
            this.DivPat = isDVP;
            this.date = ldate;
            this.referenceNo = lreferenceNo;
            this.deposit = ldeposit;
            this.penalties = lpenalties;
            this.remarks = lremarks;
            this.balance = balance;
            return true;
        }

        public int Save()
        {
            if (this.paymentDetails == null)
            {
                paymentDetails = new Upsert(Table.PAYMENT_DETAILS, id);
            }
            paymentDetails.Insert(Field.DIV_PAT, DivPat);
            paymentDetails.Insert(Field.DATE, date);
            paymentDetails.Insert(Field.REFERENCE_NO, referenceNo);
            paymentDetails.Insert(Field.DEPOSIT, deposit);
            paymentDetails.Insert(Field.PENALTIES, penalties);
            paymentDetails.Insert(Field.REMARKS, remarks);
            paymentDetails.Insert(Field.BALANCE, balance);
            if (ledger != null)
            {
                paymentDetails.Insert(Field.LEDGER_ID, saveLedger());
                if (isApply)
                {
                    paymentDetails.Insert(Field.LEDGER_TYPE, ledgername);
                }
                else
                {
                    paymentDetails.Insert(Field.LEDGER_TYPE, typeof(T).Name.ToUpper());
                }
            }
            paymentDetails.Save();
            id = paymentDetails.id;
            

            return id;
        }

        public override string ToString()
        {
            return (deposit - penalties).ToString();
        }
        public bool delete()
        {
            if (paymentDetails == null)
            {
                paymentDetails = new Upsert(Table.PAYMENT_DETAILS, id);
            }
            paymentDetails.Insert(Field.ISDELETED, true);

            deletePayment();
            paymentDetails.Save();
            return true;
        }
    }

    public class PaymentHistory
    {
        public string date { get; set; }
        public string ledgerType { get; set; }
        public string referenceNo { get; set; }
        public string balance { get; set; }
        public string payment { get; set; }

        /*public PaymentHistory(string date, string obj, string referenceNo, double balance, double payment)
        {
            this.date = date;
            this.ledgerType = obj;
            this.referenceNo = referenceNo;
            this.balance = "P " + balance.ToString("0.00");
            this.payment = "P " + payment.ToString("0.00");
        }*/
        public PaymentHistory(MySqlDataReader reader)
        {
            this.date = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE).ToString("MMM dd, yyyy");
            this.ledgerType = Retrieve.GetValueOrDefault<string>(reader, Field.LEDGER_TYPE);
            this.referenceNo = Retrieve.GetValueOrDefault<string>(reader, Field.REFERENCE_NO);
            this.balance = "P " + Retrieve.GetValueOrDefault<double>(reader, Field.BALANCE);
            this.payment = "P " + Retrieve.GetValueOrDefault<double>(reader, Field.DEPOSIT);
        }
    }
}
