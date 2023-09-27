using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
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
            connection.ClearAllPoolsAsync();
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


            public bool Create()
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    try
                    {
                        DatabaseConnection connection = new DatabaseConnection(connectionString);

                        using (MySqlConnection mySqlConnection = DatabaseConnection.GetConnection())
                        {
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();

                            Task<bool> openTask = Task.Run(() =>
                            {
                                try
                                {
                                    mySqlConnection.Open();
                                    return true;
                                }
                                catch
                                {
                                    return false;
                                }
                            });

                            int timeoutMilliseconds = 10000; // Adjust as needed

                            if (Task.WaitAll(new[] { openTask }, timeoutMilliseconds))
                            {
                                if (openTask.Result)
                                {
                                    mySqlConnection.Close();
                                    Log = ConnectionLogs.ESTABLISHED;
                                    return true;
                                }
                            }
                            else
                            {
                                mySqlConnection.Close(); // Close the connection if it times out
                                Log = ConnectionLogs.TIMEOUT;
                                return false;
                            }
                        } return false;
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
                    catch (Exception)
                    {
                        Log = ConnectionLogs.EXCEPTION_OCCURED;
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

        [Description("Host Timeout")]
        TIMEOUT,
    }
}
