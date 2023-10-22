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

        public TableObject(string table, string select, string where, int startIndex = -1, int batchSize = -1)
        {
            
            data = new List<T>();
            tableName = table;
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();

                    List<T> objectList = new List<T>();
                    if (startIndex != -1 && batchSize != -1)
                    {
                        objectList.AddRange(Retrieve.GetPaginationData<T>(tableName, select, where, startIndex, batchSize));
                    } else
                    {
                        objectList.AddRange(Retrieve.GetData<T>(tableName, select, where));
                    }
                    data = objectList;
                }
            }
            catch (MySqlException e)
            {
                ControlWindow.ShowStatic("Database Error", "Error Loading table. \n" + e.Message, Icons.ERROR);
            }
        }
    }

}
