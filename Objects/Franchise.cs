using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using static SPTC_APP.Objects.Ledger;
using Table = SPTC_APP.Database.Table;

namespace SPTC_APP.Objects
{
    public class Franchise
    {
        public int id { get; private set; }
        public string BodyNumber { get; set; }
        public Operator Operator { get; set; }
        public string LicenseNO { get; set; }
        public Driver Driver { get; set; }
        public Name Owner { get; set; }
        public int lastFranchiseId { get; set; }
        public DateTime BuyingDate { get; set; }
        public string MTOPNo { get; set; }
        public double ShareCapital { get { return GetTotalShareCapital(); } }
        public double MonthlyDues { 
            get {
                double balance = AppState.TOTAL_SHARE_PER_MONTH;
                if(LoanBalance > 0)
                {
                    balance += GetLoans()?.FirstOrDefault()?.paymentDues ?? 0;
                }
                if(LongTermLoanBalance > 0)
                {
                    balance += (GetLTLoans()?.FirstOrDefault()?.paymentDues ?? 0);
                }
                return balance;
            } 
        }
        public double LoanBalance { get { return GetLoans()?.FirstOrDefault()?.amount ?? 0; } }
        public double LongTermLoanBalance { get { return GetLTLoans()?.FirstOrDefault()?.amount ?? 0; } }
        public string displayBuyingDate { get { return BuyingDate.ToString("MM/dd/yyyy"); } }
        public Franchise lastFranchise { get { return Retrieve.GetData<Franchise>(Table.FRANCHISE, Select.ALL, Where.ID_NOTDELETED, new MySqlParameter("id", lastFranchiseId)).FirstOrDefault(); } }

        private Upsert franchise;

        public Franchise()
        {
            franchise = new Upsert(Table.FRANCHISE, -1);
            Operator = null;
            Driver = null;
            Owner = null;
            BuyingDate = DateTime.Now;
        }



        public Franchise(MySqlDataReader reader)
        {
            Operator = null;
            Driver = null;
            Owner = null;
            franchise = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.BodyNumber = Retrieve.GetValueOrDefault<string>(reader, Field.BODY_NUMBER);
            this.MTOPNo = Retrieve.GetValueOrDefault<string>(reader, Field.MTOP_NUMBER);
            this.BuyingDate = Retrieve.GetValueOrDefault<DateTime>(reader, Field.BUYING_DATE);
            this.LicenseNO = Retrieve.GetValueOrDefault<string>(reader, Field.LICENSE_NO);
            this.lastFranchiseId = Retrieve.GetValueOrDefault<int>(reader, Field.LAST_FRANCHISE_ID);
            Populate(Retrieve.GetValueOrDefault<int>(reader, Field.OPERATOR_ID), Retrieve.GetValueOrDefault<int>(reader, Field.DRIVER_ID), Retrieve.GetValueOrDefault<int>(reader, Field.OWNER_ID));

        }

        private void Populate(int operatorID, int driverID, int nameID)
        {
            if (operatorID >= 0)
                this.Operator = (Retrieve.GetData<Operator>(Table.OPERATOR, Select.ALL, Where.ID_, new MySqlParameter("id", operatorID))).FirstOrDefault();
            if (driverID >= 0)
                this.Driver = (Retrieve.GetData<Driver>(Table.DRIVER, Select.ALL, Where.ID_, new MySqlParameter("id", driverID))).FirstOrDefault();
            if (nameID >= 0)
                this.Owner = (Retrieve.GetData<Name>(Table.NAME, Select.ALL, Where.ID_, new MySqlParameter("id", nameID))).FirstOrDefault();
        }

        public bool WriteInto(string bodynumber, Operator lOperator, Driver lDriver, string licenceNO)
        {
            this.BodyNumber = bodynumber;
            this.Operator = lOperator;
            this.Driver = lDriver;
            this.LicenseNO = licenceNO;
            return true;
        }
        public int Save()
        {
            if (franchise == null)
            {
                franchise = new Upsert(Table.FRANCHISE, id);
            }
            franchise.Insert(Field.BODY_NUMBER, BodyNumber);
            franchise.Insert(Field.MTOP_NUMBER, MTOPNo);
            franchise.Insert(Field.LICENSE_NO, LicenseNO);
            franchise.Insert(Field.BUYING_DATE, BuyingDate);
            franchise.Insert(Field.LAST_FRANCHISE_ID, lastFranchiseId);
            if (this.Operator != null)
            {
                franchise.Insert(Field.OPERATOR_ID, this.Operator.Save());
            } else
            {
                franchise.Insert(Field.OPERATOR_ID, -1);
            }
            if (this.Driver != null)
            {
                franchise.Insert(Field.DRIVER_ID, this.Driver.Save());
            } else
            {
                franchise.Insert(Field.DRIVER_ID, -1);
            }
            if (this.Owner != null)
            {
                franchise.Insert(Field.OWNER_ID, this.Owner.Save());
            } else
            {
                franchise.Insert(Field.OWNER_ID, -1);
            } 
            franchise.Save();
            id = franchise.id;

            return id;
        }
        public override string ToString()
        {
            if (BodyNumber != null)
            {
                return BodyNumber.ToString();
            }
            return "";
        }

        public bool delete()
        {
            if (franchise == null)
            {
                franchise = new Upsert(Table.FRANCHISE, id);
            }
            franchise.Insert(Field.ISDELETED, true);
            franchise.Save();
            return true;
        }

        public List<Ledger.ShareCapital> GetShareCapitals()
        {
            return Retrieve.GetDataUsingQuery<Ledger.ShareCapital>(RequestQuery.GET_SHARE_LEDGER_LIST(id));
        }
        public List<Ledger.Loan> GetLoans()
        {
            return Retrieve.GetDataUsingQuery<Ledger.Loan>(RequestQuery.GET_LEDGER_LIST(Table.LOAN, id));
        }
        public List<Ledger.LongTermLoan> GetLTLoans()
        {
            return Retrieve.GetDataUsingQuery<Ledger.LongTermLoan>(RequestQuery.GET_LEDGER_LIST(Table.LONG_TERM_LOAN, id));
        }

        public List<PaymentDetails<Ledger.ShareCapital>> GetShareCapitalLedger()
        {
            return Retrieve.GetDataUsingQuery<PaymentDetails<Ledger.ShareCapital>>(RequestQuery.GET_LEDGER_PAYMENT(Table.SHARE_CAPITAL, typeof(Ledger.ShareCapital).Name.ToUpper(), id));
        }
        public List<PaymentDetails<Ledger.Loan>> GetLoanLedger()
        {
            return Retrieve.GetDataUsingQuery<PaymentDetails<Ledger.Loan>>(RequestQuery.GET_LEDGER_PAYMENT(Table.LOAN, typeof(Ledger.Loan).Name.ToUpper(), id));
        }
        public List<PaymentDetails<Ledger.LongTermLoan>> GetlTLoanLedger()
        {
            return Retrieve.GetDataUsingQuery<PaymentDetails<Ledger.LongTermLoan>>(RequestQuery.GET_LEDGER_PAYMENT(Table.LONG_TERM_LOAN, typeof(Ledger.LongTermLoan).Name.ToUpper(), id));
        }

        

        public double GetTotalShareCapital()
        {
            return GetShareCapitalLedger().Sum(tmp => tmp.deposit);
        }

        public double GetTotalLoan()
        {
            return GetLoanLedger().Sum(tmp => tmp.deposit);
        }

        public double GetTotalLTLoan()
        {
            return GetlTLoanLedger().Sum(tmp => tmp.deposit);
        }

        public List<PaymentHistory> GetPaymentList()
        {
            List<PaymentHistory> mainlist = Retrieve.GetDataUsingQuery<PaymentHistory>(RequestQuery.GET_ALL_PAYMENTS_ID(id));
            
            return mainlist;
        }

        public List<object> GetLoanAndLTLoan()
        {
            List<object> mainlist = new List<object>();
            mainlist.AddRange(GetLoans());
            mainlist.AddRange(GetLTLoans());
            mainlist.Sort((x, y) =>
            {
                DateTime dateX = (x is Loan) ? ((Loan)x).date : ((LongTermLoan)x).date;
                DateTime dateY = (y is Loan) ? ((Loan)y).date : ((LongTermLoan)y).date;
                return dateY.CompareTo(dateX);
            });
            return mainlist;
        }

        internal double SaveShareCapital()
        {
            Ledger.ShareCapital share = GetShareCapitals()?.FirstOrDefault() ?? null;
            if(share != null)
            {
                share.lastBalance = GetTotalShareCapital();
                share.Save();
                return share.lastBalance;
            }
            return 0;
        }
    }
}
