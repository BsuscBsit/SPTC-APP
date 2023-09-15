using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using SPTC_APP.View;

namespace SPTC_APP.Objects
{
    public class TableObject<T>
    {
        public List<T> data;
        public string tableName;
        public TableObject(string table)
        {
            data = new List<T>();
            tableName = table;
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();

                    List<T> objectList = new List<T>();
                    objectList.AddRange(Retrieve.GetData<T>(tableName, Select.ALL, Where.ALL_NOTDELETED));
                    data = objectList;
                }
            }
            catch (MySqlException e)
            {
                ControlWindow.Show("Database Error", "Error Loading table. \n" + e.Message, Icons.ERROR);
            }
        }
    }
}
