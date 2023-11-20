using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using SPTC_APP.Properties;
using SPTC_APP.View;
using Table = SPTC_APP.Database.Table;

namespace SPTC_APP.Objects
{
    public static class Controller
    {

        // START UP INITIALIZATION
        public static async void StartInitialization(Window window, ProgressBar progressBar, TextBox log)
        {
            string host = Settings.Default.Host;
            string port = Settings.Default.Port;
            string database = Settings.Default.Database;
            string username = Settings.Default.Username;
            string password = Settings.Default.Password;
            try
            {
                DatabaseConnection.Builder builder = CreateDatabaseConnectionBuilder(host, port, database, username, password);
            
                bool isConnected = builder.Create();

                int maxAttempts = 3;
                int attemptCount = 1;

                while (!IsConnectionSuccessful(isConnected, builder.Log) && attemptCount <= maxAttempts)
                {
                    EventLogger.Post($"DTB :: Database Connnection attemp ({attemptCount}) :{builder.Log.ToString()}");
                    UpdateUIForRetry(progressBar, log, builder.Log);
                    UpdateSettingsFromDefault(ref host, ref port, ref database, ref username, ref password);

                    DatabaseConfigInput inputWindow = GetDatabaseConfigInputWindow(host, port, database, username, password);
                    if (inputWindow.Exit)
                    {
                        window.Close();
                        return;
                    }
                    UpdateSettingsFromDefault(ref host, ref port, ref database, ref username, ref password);

                    builder = CreateDatabaseConnectionBuilder(host, port, database, username, password);
                    isConnected = builder.Create();

                    attemptCount++;
                }
                if (!isConnected && attemptCount > maxAttempts)
                {
                    window.Close();
                    return;
                }

                if (isConnected && builder.Log == ConnectionLogs.ESTABLISHED)
                {
                    if(await PerformDatabaseTasks(progressBar, log))
                    {
                        log.Text = DatabaseConnection.GetEnumDescription(builder.Log);
                        await Task.Delay(100);

                        if (progressBar.Value == 100)
                        {
                            ShowLoginWindowAndCloseCurrent(window);
                        }
                        else
                        {
                            HandleConnectionFailure(builder.Log);
                            ShowSplashScreenAndCloseCurrent(window);
                        }
                    } else
                    {
                        progressBar.Value = 10;
                        if(ControlWindow.ShowTwoway("Somethings Wrong!", "Database takes too long to respond. Retry?", Icons.ERROR))
                        {
                            
                            ShowSplashScreenAndCloseCurrent(window);
                        } else
                        {
                            window.Close();
                        }
                    }
                }
                else
                {
                    progressBar.Value = 10;
                    HandleConnectionFailure(builder.Log);
                    ShowSplashScreenAndCloseCurrent(window);
                }
            }
            catch (MySqlException ex)
            {
                EventLogger.Post("DTB :: MySqlException : " + ex.Message);
            }
            catch (Exception e)
            {
                EventLogger.Post("ERR :: Exception : " + e.Message);
            }
        }
        private static DatabaseConnection.Builder CreateDatabaseConnectionBuilder(string host, string port, string database, string username, string password)
        {
            return new DatabaseConnection.Builder(host, port, database, username, password);
        }
        private static bool IsConnectionSuccessful(bool isConnected, ConnectionLogs log)
        {
            return isConnected && log == ConnectionLogs.ESTABLISHED;
        }
        private static void UpdateUIForRetry(ProgressBar progressBar, TextBox log, ConnectionLogs logType)
        {
            progressBar.IsIndeterminate = true;
            log.Text = DatabaseConnection.GetEnumDescription(logType);
            Settings.Default.Reload();
        }
        private static void UpdateSettingsFromDefault(ref string host, ref string port, ref string database, ref string username, ref string password)
        {
            host = Settings.Default.Host;
            port = Settings.Default.Port;
            database = Settings.Default.Database;
            username = Settings.Default.Username;
            password = Settings.Default.Password;
        }
        private static DatabaseConfigInput GetDatabaseConfigInputWindow(string host, string port, string database, string username, string password)
        {
            var inputWindow = new DatabaseConfigInput();
            inputWindow.tbHost.Text = host;
            inputWindow.tbPort.Text = port;
            inputWindow.tbDatabase.Text = database;
            inputWindow.tbUsername.Text = username;
            inputWindow.pbPassword.Password = password;
            inputWindow.ShowDialog();
            return inputWindow;
        }
        private static async Task<bool> PerformDatabaseTasks(ProgressBar progressBar, TextBox log)
        {
            Task<bool> loadDatabaseTask = Task.Run(() => AppState.LoadDatabase());
            AppState.LoadRecapList(DateTime.Now.Month, DateTime.Now.Year);
            AppState.CheckCamera(AppState.DEFAULT_CAMERA);
            AppState.EMPLOYEE_LIST(true);
            for (int i = 0; i < 100; i++)
            {
                progressBar.Dispatcher.Invoke(() =>
                {
                    progressBar.IsIndeterminate = false;
                    progressBar.Value = i;
                });

                if (loadDatabaseTask.IsCompleted)
                {
                    break; 
                }

                await Task.Delay(50);
                log.Dispatcher.Invoke(() => log.Text = "Loading . . .");
            }

            if (loadDatabaseTask.IsCompleted)
            {
                log.Dispatcher.Invoke(() => log.Text = "Loading completed.");

                await Task.Delay(500);
                progressBar.Value = 100;
                return true;
            } else
            {
                log.Dispatcher.Invoke(() => log.Text = "Loading resources Incomplete.");
                await Task.Delay(500);
                EventLogger.Post($"ERR :: Server timeout");
                return false;
            }
        }
        private static void ShowLoginWindowAndCloseCurrent(Window window)
        {
            (new Login()).Show();
            window.Close();
        }
        private static void HandleConnectionFailure(ConnectionLogs logType)
        {
            if (logType == ConnectionLogs.CANNOT_CONNECT)
            {
                ControlWindow.ShowStatic(DatabaseConnection.GetEnumDescription(logType), "Check if database is online");
            }
            else if (logType == ConnectionLogs.WRONG_PASSWORD)
            {
                ControlWindow.ShowStatic(DatabaseConnection.GetEnumDescription(logType), "Input the correct password and try again");
            } else if(logType == ConnectionLogs.EXCEPTION_OCCURED)
            {
                ControlWindow.ShowStatic(DatabaseConnection.GetEnumDescription(logType), "Something wentwrong. try again");
            }
            EventLogger.Post($"LOG :: {DatabaseConnection.GetEnumDescription(logType)}");
        }
        private static void ShowSplashScreenAndCloseCurrent(Window window)
        {
            (new View.SplashScreen()).Show();
            window.Close();
        }
        

        //FOR DEBUG PURPOSE
        public static void CreateEmployee(int userindex)
        {
            Upsert employee = new Upsert(Table.EMPLOYEE, -1);
            employee.Insert(Field.PASSWORD, RequestQuery.Protect(AppState.DEFAULT_PASSWORD));
            employee.Insert(Field.POSITION_ID, userindex);
            employee.Save();
        }

    }
}
