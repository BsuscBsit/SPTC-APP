using System.Linq;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;

namespace SPTC_APP.Objects
{
    public class Franchise
    {
        public int id { get; private set; }
        public string bodynumber { get; set; }
        public Operator Operator { get; set; }
        public string licenceNO { get; set; }
        public Driver Driver_day { get; set; }
        public Driver Driver_night { get; set; }
        public Name owner { get; set; }
        public Franchise lastFranchiseId { get; set; }

        private Upsert franchise;

        public Franchise()
        {
            franchise = new Upsert(Table.FRANCHISE, -1);
            Operator = null;
            Driver_day = null;
            Driver_night = null;
            owner = null;
            lastFranchiseId = null;
        }



        public Franchise(MySqlDataReader reader)
        {
            Operator = null;
            Driver_day = null;
            Driver_night = null;
            owner = null;
            lastFranchiseId = null;
            franchise = null;
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.bodynumber = Retrieve.GetValueOrDefault<string>(reader, Field.BODY_NUMBER);
            this.licenceNO = Retrieve.GetValueOrDefault<string>(reader, Field.LICENSE_NO);

            Populate(Retrieve.GetValueOrDefault<int>(reader, Field.OPERATOR_ID), Retrieve.GetValueOrDefault<int>(reader, Field.DRIVER_DAY_ID), Retrieve.GetValueOrDefault<int>(reader, Field.DRIVER_NIGHT_ID), Retrieve.GetValueOrDefault<int>(reader, Field.OWNER_ID), Retrieve.GetValueOrDefault<int>(reader, Field.LAST_FRANCHISE_ID));

        }

        private void Populate(int operatorID, int driverDayID, int driverNightID, int nameID, int lastFranchiseID)
        {
            if (operatorID >= 0)
                this.Operator = (Retrieve.GetData<Operator>(Table.OPERATOR, Select.ALL, Where.ID_, new MySqlParameter("id", operatorID))).FirstOrDefault();
            if (driverDayID >= 0)
                this.Driver_day = (Retrieve.GetData<Driver>(Table.DRIVER, Select.ALL, Where.ID_, new MySqlParameter("id", driverDayID))).FirstOrDefault();
            if (driverNightID >= 0)
                this.Driver_night = (Retrieve.GetData<Driver>(Table.DRIVER, Select.ALL, Where.ID_, new MySqlParameter("id", driverNightID))).FirstOrDefault();
            if (nameID >= 0)
                this.owner = (Retrieve.GetData<Name>(Table.NAME, Select.ALL, Where.ID_, new MySqlParameter("id", nameID))).FirstOrDefault();
            if (lastFranchiseID >= 0)
                this.lastFranchiseId = (Retrieve.GetData<Franchise>(Table.FRANCHISE, Select.ALL, Where.ID_, new MySqlParameter("id", lastFranchiseID))).FirstOrDefault();
        }

        public bool WriteInto(string bodynumber, Operator lOperator, Driver lDriverDay, Driver lDriverNight, string licenceNO)
        {
            this.bodynumber = bodynumber;
            this.Operator = lOperator;
            this.Driver_day = lDriverDay;
            this.Driver_night = lDriverNight;
            this.licenceNO = licenceNO;
            return true;
        }
        public int Save()
        {
            if (franchise == null)
            {
                franchise = new Upsert(Table.FRANCHISE, id);
            }
            franchise.Insert(Field.BODY_NUMBER, bodynumber);
            franchise.Insert(Field.LICENSE_NO, licenceNO);
            if (this.Operator != null)
            {
                franchise.Insert(Field.OPERATOR_ID, this.Operator.Save());
            }
            if (this.Driver_day != null)
            {
                franchise.Insert(Field.DRIVER_DAY_ID, this.Driver_day.Save());
            }
            if (this.Driver_night != null)
            {
                franchise.Insert(Field.DRIVER_NIGHT_ID, this.Driver_night.Save());
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
            if (bodynumber != null)
            {
                return bodynumber.ToString();
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
