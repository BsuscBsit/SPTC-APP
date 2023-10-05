using System;
using System.Linq;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;

namespace SPTC_APP.Objects
{
    public class PaymentDetails<T>
    {
        public int id { get; private set; }
        public T ledger { get; set; }
        public bool isDownPayment { get; set; }
        public bool isDivPat { get; set; }
        public DateTime date { get; set; }
        public string referenceNo { get; set; }
        public double deposit { get; set; }
        public double penalties { get; set; }
        public string remarks { get; set; }

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

        public PaymentDetails()
        {
            paymentDetails = new Upsert(Table.PAYMENT_DETAILS, -1);
        }

        public PaymentDetails(MySqlDataReader reader)
        {
            paymentDetails = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.isDownPayment = Retrieve.GetValueOrDefault<bool>(reader, Field.IS_DOWN_PAYMENT);
            this.isDivPat = Retrieve.GetValueOrDefault<bool>(reader, Field.IS_DIV_PAT);
            this.date = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE);
            this.referenceNo = Retrieve.GetValueOrDefault<string>(reader, Field.REFERENCE_NO);
            this.deposit = Retrieve.GetValueOrDefault<double>(reader, Field.DEPOSIT);
            this.penalties = Retrieve.GetValueOrDefault<double>(reader, Field.PENALTIES);
            this.remarks = Retrieve.GetValueOrDefault<string>(reader, Field.REMARKS);

            Populate(Retrieve.GetValueOrDefault<int>(reader, Field.LEDGER_ID));
        }

        private void Populate(int ledgerID)
        {
            if (ledgerID >= 0)
            {

                this.ledger = (Retrieve.GetData<T>(getLedgerType(), Select.ALL, Where.ID_, new MySqlParameter("id", ledgerID))).FirstOrDefault();
            }
        }

        public bool WriteInto(T lledger, bool isDP, bool isDVP, DateTime ldate, string lreferenceNo, double ldeposit, double lpenalties, string lremarks)
        {
            this.ledger = lledger;
            this.isDownPayment = isDP;
            this.isDivPat = isDVP;
            this.date = ldate;
            this.referenceNo = lreferenceNo;
            this.deposit = ldeposit;
            this.penalties = lpenalties;
            this.remarks = lremarks;
            return true;
        }

        public int Save()
        {
            if (this.paymentDetails == null)
            {
                paymentDetails = new Upsert(Table.PAYMENT_DETAILS, id);
            }
            paymentDetails.Insert(Field.IS_DOWN_PAYMENT, isDownPayment);
            paymentDetails.Insert(Field.IS_DIV_PAT, isDivPat);
            paymentDetails.Insert(Field.DATE, date);
            paymentDetails.Insert(Field.REFERENCE_NO, referenceNo);
            paymentDetails.Insert(Field.DEPOSIT, deposit);
            paymentDetails.Insert(Field.PENALTIES, penalties);
            paymentDetails.Insert(Field.REMARKS, remarks);
            if (ledger != null)
            {
                paymentDetails.Insert(Field.LEDGER_ID, saveLedger());
                paymentDetails.Insert(Field.LEDGER_TYPE, typeof(T).Name.ToLower());
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
            paymentDetails.Save();
            return true;
        }
    }
}
