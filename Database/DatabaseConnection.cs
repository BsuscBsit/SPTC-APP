using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SPTC_APP.Database
{
    public class DatabaseConnection
    {
        private static string connectionString;

        public DatabaseConnection(string connectionString)
        {
            DatabaseConnection.connectionString = connectionString;
        }

        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(DatabaseConnection.connectionString);
            return connection;
        }

        public static string GetEnumDescription(ConnectionLogs value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public class Builder
        {
            private string connectionString;

            public ConnectionLogs Log { private set; get; }

            public Builder(string host, string port, string database, string username, string password)
            {
                //connectionString = $"Server={host};Port={port};Database={database};Uid={username};Pwd={password};";
                connectionString = $"Server={host};Database={database};Uid={username};Pwd={password};";
            }


            public async Task<bool> CreateAsync()
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    try
                    {
                        DatabaseConnection connection = new DatabaseConnection(connectionString);

                        MySqlConnection mySqlConnection = DatabaseConnection.GetConnection();
                        await mySqlConnection.OpenAsync();
                        await Task.Delay(1000);
                        mySqlConnection.Close();

                        Log = ConnectionLogs.ESTABLISHED;
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        if (ex.Number == 1045)
                        {
                            Log = ConnectionLogs.WRONG_PASSWORD;
                        }
                        else if (ex.Number == 1042)
                        {
                            Log = ConnectionLogs.CANNOT_CONNECT;
                        }
                        else
                        {
                            Log = ConnectionLogs.EXCEPTION_OCCURED;
                        }
                        return false;
                    }
                }
                else
                {
                    Log = ConnectionLogs.STRING_EMPTY;
                    return false;
                }
            }

            public bool Connect()
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    try
                    {
                        DatabaseConnection connection = new DatabaseConnection(connectionString);

                        MySqlConnection mySqlConnection = DatabaseConnection.GetConnection();
                        mySqlConnection.Open();
                        mySqlConnection.Close();

                        Log = ConnectionLogs.ESTABLISHED;
                        return true;
                    }
                    catch (MySqlException ex)
                    {
                        if (ex.Number == 1045)
                        {
                            Log = ConnectionLogs.WRONG_PASSWORD;
                        }
                        else if (ex.Number == 1042)
                        {
                            Log = ConnectionLogs.CANNOT_CONNECT;
                        }
                        else
                        {
                            Log = ConnectionLogs.EXCEPTION_OCCURED;
                        }
                        return false;
                    }
                }
                else
                {
                    Log = ConnectionLogs.STRING_EMPTY;
                    return false;
                }
            }
        }
    }

    public enum ConnectionLogs
    {
        [Description("Empty Connection string")]
        STRING_EMPTY,

        [Description("Connection Established")]
        ESTABLISHED,

        [Description("Exception Occurred")]
        EXCEPTION_OCCURED,

        [Description("Wrong Password")]
        WRONG_PASSWORD,

        [Description("Cannot Connect")]
        CANNOT_CONNECT,
    }
}
