using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;

namespace SPTC_APP.Objects
{
    public class IDHistory<T>
    {
        public int id { get; private set; }
        public DateTime date { get; set; }
        public T Owner { get; set; }
        public int name { get; set; }
        public bool isPrinted { get; set; }
        public bool isDeleted { get; set; }

        private Upsert idHistory;

        public IDHistory()
        {
            idHistory = new Upsert(Table.IDHISTORY, -1);
        }

        public IDHistory(MySqlDataReader reader)
        {
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.date = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE);
            this.name = Retrieve.GetValueOrDefault<int>(reader, Field.NAME_ID);
            this.isPrinted = Retrieve.GetValueOrDefault<bool>(reader, Field.IS_PRINTED);

            Populate(Retrieve.GetValueOrDefault<int>(reader, Field.OWNER_ID));
        }

        private void Populate(int ownerID)
        {
            if (ownerID >= 0)
            {

                this.Owner = (Retrieve.GetData<T>(getOwnerType(), Select.ALL, Where.ID_, new MySqlParameter("id", ownerID))).FirstOrDefault();
            }
        }

        public void WriteInto(int id, T owner, int name, bool isPrinted)
        {
            this.date = DateTime.Now;
            this.Owner = owner;
            this.name = name;
            this.isPrinted = isPrinted;
        }
        private string getOwnerType()
        {
            if (Owner is Driver)
            {
                return Table.DRIVER;
            } else if (Owner is Operator)
            {
                return Table.OPERATOR;
            }
            return "";
        }
        private int getOwnerId()
        {
            if (Owner is Driver drv)
            {
                return drv.id;
            }
            else if (Owner is Operator optr)
            {
                return optr.id;
            } 
            return -1;
        }

        public int Save()
        {
            if (idHistory == null)
            {
                idHistory = new Upsert(Table.IDHISTORY, id);
            }
            idHistory.Insert(Field.DATE, date);
            
            idHistory.Insert(Field.NAME_ID, name);
            idHistory.Insert(Field.IS_PRINTED, isPrinted);
            if (this.Owner != null)
            {
                idHistory.Insert(Field.OWNER_ID, getOwnerId());
                idHistory.Insert(Field.ENTITY_TYPE, typeof(T).Name.ToLower());
            }
            idHistory.Save();
            id = idHistory.id;

            return id;
        }


        public bool Delete()
        {
            if (idHistory == null)
            {
                idHistory = new Upsert(Table.IDHISTORY, id);
                idHistory.Insert(Field.ISDELETED, true);
                idHistory.Save();
                return true;
            }
            return false;
        }
    }
}


