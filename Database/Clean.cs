using MySql.Data.MySqlClient;

namespace SPTC_APP.Database
{
    public class Clean
    {

        private string CLEANER = "DELETE * FROM ";
        public Clean(string table)
        {
            CLEANER += table + " " + Where.ALL_DELETED;
        }

        public bool Start()
        {
            if (AppState.IS_ADMIN)
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(CLEANER, connection);
                    command.ExecuteReader();
                }
                return true;
            }
            return false;
        }

    }
}
