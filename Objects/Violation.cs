using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using System;
using System.Linq;

namespace SPTC_APP.Objects
{
    public class Violation
    {
        public int id { get; private set; }
        public int franchiseId { get; set; }
        public int violationLevelCount { get; set; }
        public int violationTypeId { get; set; }
        public DateTime violationDate { get; set; }
        public DateTime? suspensionStart { get; set; }
        public DateTime? suspensionEnd { get; set; }
        public string remarks { get; set; }
        public int name_id { get; set; }

        public string dDate
        {
            get { return violationDate.ToString("MM/dd/yyyy"); }
        }
        public string dDateStart
        {
            get { return suspensionStart?.ToString("MMM/dd"); }
        }
        public string dDateEnd
        {
            get { return suspensionEnd?.ToString("MMM/dd"); }
        }

        public string violationType
        {
            get
            {
                return Retrieve.GetData<ViolationType>(Table.VIOLATION_TYPE, Select.ALL, Where.ID_NOTDELETED, new MySqlParameter(Field.ID, violationTypeId)).FirstOrDefault().title.ToString();
            }
        }

        private Upsert violation;

        public Violation()
        {
            violation = new Upsert(Table.VIOLATION, -1);
            
        }

        public Violation(MySqlDataReader reader)
        {
            this.id = Retrieve.GetValueOrDefault<int>(reader, Field.ID);
            this.franchiseId = Retrieve.GetValueOrDefault<int>(reader, Field.FRANCHISE_ID);
            this.violationLevelCount = Retrieve.GetValueOrDefault<int>(reader, Field.VIOLATION_LEVEL_COUNT);
            this.violationTypeId = Retrieve.GetValueOrDefault<int>(reader, Field.VIOLATION_TYPE_ID);
            this.violationDate = Retrieve.GetValueOrDefault<DateTime>(reader, Field.DATE);
            this.suspensionStart = Retrieve.GetValueOrDefault<DateTime?>(reader, Field.SUSPENSION_START);
            this.suspensionEnd = Retrieve.GetValueOrDefault<DateTime?>(reader, Field.SUSPENSION_END);
            this.remarks = Retrieve.GetValueOrDefault<string>(reader, Field.REMARKS);
            this.name_id = Retrieve.GetValueOrDefault<int>(reader, Field.NAME_ID);
        }

        public void WriteInto(int franchise_id, int violation_type_id, DateTime violation_date, DateTime start, DateTime end, string remarks, int name_id)
        {
            this.franchiseId = franchise_id;
            this.violationLevelCount = Retrieve.GetDataUsingQuery<int>(RequestQuery.GET_VIOLATION_COUNT_OF(franchise_id)).FirstOrDefault();
            this.violationTypeId = violation_type_id;
            this.violationDate = violation_date;
            this.suspensionStart = start;
            this.suspensionEnd = end;
            this.remarks = remarks;
            this.name_id = name_id;
        }

        public int Save()
        {
            if (violation == null)
            {
                violation = new Upsert(Table.VIOLATION, id);
            }
            violation.Insert(Field.FRANCHISE_ID, franchiseId);
            violation.Insert(Field.VIOLATION_LEVEL_COUNT, violationLevelCount);
            violation.Insert(Field.VIOLATION_TYPE_ID, violationTypeId);
            violation.Insert(Field.DATE, violationDate);
            violation.Insert(Field.SUSPENSION_START, suspensionStart);
            violation.Insert(Field.SUSPENSION_END, suspensionEnd);
            violation.Insert(Field.REMARKS, remarks);
            violation.Insert(Field.NAME_ID, name_id);
            violation.Save();
            id = violation.id;

            return id;
        }

        public bool Delete()
        {
            if (violation == null)
            {
                violation = new Upsert(Table.VIOLATION, id);
                violation.Insert(Field.ISDELETED, true);
                violation.Save();
                return true;
            }
            return false;
        }
    }
}
