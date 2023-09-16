using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;

namespace SPTC_APP.Objects
{
    public class IDHistory
    {
        public int id { get; private set; }
        public DateTime date { get; set; }
        private int Owner_id { get; set; }
        public General entityType { get; set; }
        public int name { get; set; }
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
            this.Owner_id = Retrieve.GetValueOrDefault<int>(reader, Field.OWNER_ID);
            this.entityType = (Retrieve.GetValueOrDefault<string>(reader, Field.ENTITY_TYPE).Equals("OPERATOR"))? General.OPERATOR : General.DRIVER;
            this.name = Retrieve.GetValueOrDefault<int>(reader, Field.NAME_ID);
        }

        public void WriteInto(int id, General entity_type, int name)
        {
            this.date = DateTime.Now;
            this.Owner_id = id;
            this.entityType = entity_type;
            this.name = name;
        }

        public int Save()
        {
            if (idHistory == null)
            {
                idHistory = new Upsert(Table.IDHISTORY, id);
            }
            idHistory.Insert(Field.DATE, date);
            idHistory.Insert(Field.OWNER_ID, Owner_id);
            idHistory.Insert(Field.ENTITY_TYPE, (entityType == General.OPERATOR) ? "OPERATOR": "DRIVER");
            idHistory.Insert(Field.NAME_ID, name);
            idHistory.Save();
            id = idHistory.id;

            return id;
        }

        public T GetOwner<T>(string table)
        {
            return Retrieve.GetData<T>(table, Select.ALL, Where.ID_NOTDELETED, new MySqlParameter(Field.ID, Owner_id)).FirstOrDefault();
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


