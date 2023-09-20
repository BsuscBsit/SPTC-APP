using System.Linq;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;

namespace SPTC_APP.Objects
{
    public class Franchise
    {
        public int id { get; private set; }
        public string BodyNumber { get; set; }
        public Operator Operator { get; set; }
        public string LicenseNO { get; set; }
        public Driver Driver { get; set; }
        public Name owner { get; set; }
        public Franchise lastFranchiseId { get; set; }

        public string MTOPNo { get; set; }
        public double ShareCapital { get; set; }
        public double MonthlyDues { get; set; }

        private Upsert franchise;

        public Franchise()
        {
            franchise = new Upsert(Table.FRANCHISE, -1);
            Operator = null;
            Driver = null;
            owner = null;
            lastFranchiseId = null;
        }



        public Franchise(MySqlDataReader reader)
        {
            Operator = null;
            Driver = null;
            owner = null;
            lastFranchiseId = null;
            franchise = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.BodyNumber = Retrieve.GetValueOrDefault<string>(reader, Field.BODY_NUMBER);
            this.MTOPNo = Retrieve.GetValueOrDefault<string>(reader, Field.MTOP_NUMBER);
            this.LicenseNO = Retrieve.GetValueOrDefault<string>(reader, Field.LICENSE_NO);

            Populate(Retrieve.GetValueOrDefault<int>(reader, Field.OPERATOR_ID), Retrieve.GetValueOrDefault<int>(reader, Field.DRIVER_ID), Retrieve.GetValueOrDefault<int>(reader, Field.OWNER_ID), Retrieve.GetValueOrDefault<int>(reader, Field.LAST_FRANCHISE_ID));

        }

        private void Populate(int operatorID, int driverID, int nameID, int lastFranchiseID)
        {
            if (operatorID >= 0)
                this.Operator = (Retrieve.GetData<Operator>(Table.OPERATOR, Select.ALL, Where.ID_, new MySqlParameter("id", operatorID))).FirstOrDefault();
            if (driverID >= 0)
                this.Driver = (Retrieve.GetData<Driver>(Table.DRIVER, Select.ALL, Where.ID_, new MySqlParameter("id", driverID))).FirstOrDefault();
            if (nameID >= 0)
                this.owner = (Retrieve.GetData<Name>(Table.NAME, Select.ALL, Where.ID_, new MySqlParameter("id", nameID))).FirstOrDefault();
            if (lastFranchiseID >= 0)
                this.lastFranchiseId = (Retrieve.GetData<Franchise>(Table.FRANCHISE, Select.ALL, Where.ID_, new MySqlParameter("id", lastFranchiseID))).FirstOrDefault();
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
            if (this.Operator != null)
            {
                franchise.Insert(Field.OPERATOR_ID, this.Operator.Save());
            }
            if (this.Driver != null)
            {
                franchise.Insert(Field.DRIVER_ID, this.Driver.Save());
            }
            if (this.owner != null)
            {
                franchise.Insert(Field.OWNER_ID, this.owner.Save());
            }
            if (this.lastFranchiseId != null)
            {
                franchise.Insert(Field.LAST_FRANCHISE_ID, this.lastFranchiseId.Save());
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

    }
}
