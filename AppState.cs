using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View;
using SPTC_APP.View.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static SPTC_APP.Objects.Ledger;

namespace SPTC_APP
{
    public static class AppState
    {
        //SAVED EXTERNALLY
        public static string APPSTATE_PATH;
        public static string LOGS;
        public static string OUTPUT_PATH;
        public static string DEFAULT_PASSWORD;
        public static string DEFAULT_ADDRESSLINE2;
        public static string EXPIRATION_DATE;
        public static string REGISTRATION_NO;
        public static double PRINT_AJUSTMENTS;
        public static bool LOG_WINDOW;
        public static int DEFAULT_CAMERA;
        public static int TABLE_BATCH_SIZE;



        //NOT SAVED EXTERNALLY
        public static string[] ALL_EMPLOYEES;
        public static List<string> Employees;
        public static bool IS_ADMIN = false;
        public static Employee USER = null;
        public static Dictionary<string, double> MonthlyIncome;
        public static List<KeyValuePair<string, double>> ThisMonthsChart;

        //Toggle on defore deploying
        public static bool isDeployment = false;
        public static bool isDeployment_IDGeneration = false;

        public static Window mainwindow = null;

        private static HashSet<Window> OpenedWindows = new HashSet<Window>();


        public static void Login(string username, string password, Window window)
        {
            dynamic result = Retrieve.Login(username, password);

            if (result is View.ControlWindow controlWindow)
            {
                result.Show();
                EventLogger.Post($"User :: Login Failed: USER({username})");
                //DEBUG THIS ON OTHER PC
                //CreateEmployee(AppState.Employees.IndexOf(username)); //result in password :: 751cb3f4aa17c36186f4856c8982bf27
            }
            else if (result is Employee employee)
            {

                USER = employee;
                if (employee.position.ToString() == Employees[0])
                {
                    IS_ADMIN = true;
                }

                if (AppState.isDeployment)
                {
                    MainBody body = (new MainBody());
                    AppState.mainwindow = body;
                    body.Show();
                }
                if (AppState.isDeployment_IDGeneration)
                {
                    (new PrintPreview()).Show();
                }
                if (!AppState.isDeployment_IDGeneration && !AppState.isDeployment)
                {
                    (new Test()).Show();
                }


                EventLogger.Post($"USER :: Login Success: USER({username})");
                window.Close();
            }
        }
        public static void Logout(Window window)
        {
            IS_ADMIN = false;
            EventLogger.Post($"USER :: Logout Success");
            (new Login()).Show();
            AppState.mainwindow = null;
            USER = null;
            window.Close();
        }




        public static void WindowsCounter(bool isOpen, object sender)
        {
            if (isOpen)
            {
                if (sender is SPTC_APP.View.Login)
                {
                    // Close all other windows
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != sender)
                        {
                            window.Close();
                        }
                    }
                }

                // Add the opened window to the HashSet
                OpenedWindows.Add(sender as Window);
            }
            else
            {

                // Remove the closed window from the HashSet
                OpenedWindows.Remove(sender as Window);
            }
            if (AppState.OpenedWindows.Count > 0)
            {
                string windowList = string.Join(", ", OpenedWindows.Select(w => w.ToString()));
                if (LOG_WINDOW)
                {
                    EventLogger.Post($"OUT :: Opened Window Count = {AppState.OpenedWindows.Count}, Opened Windows: [ {windowList} ]");
                }
            }
        }
        public static async Task<bool> LoadDatabase()
        {
            try
            {
                await LoadMonthlyIncome(DateTime.Now.Year);

                await LoadMonthChart(DateTime.Now.Month, DateTime.Now.Year);

                await Task.Delay(50);
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                EventLogger.Post($"ERR :: Exception in Loading Database{ex.Message}");
                return false;
            }
        }
        private static Task<bool> LoadMonthlyIncome(int year)
        {
            try
            {
                MonthlyIncome = new Dictionary<string, double>
                {
                    { "Jan", RetrieveIncomeForMonth(1, year) },
                    { "Feb", RetrieveIncomeForMonth(2, year) },
                    { "Mar", RetrieveIncomeForMonth(3, year) },
                    { "Apr", RetrieveIncomeForMonth(4, year) },
                    { "May", RetrieveIncomeForMonth(5, year) },
                    { "Jun", RetrieveIncomeForMonth(6, year) },
                    { "Jul", RetrieveIncomeForMonth(7, year) },
                    { "Aug", RetrieveIncomeForMonth(8, year) },
                    { "Sep", RetrieveIncomeForMonth(9, year) },
                    { "Oct", RetrieveIncomeForMonth(10, year) },
                    { "Nov", RetrieveIncomeForMonth(11, year) },
                    { "Dec", RetrieveIncomeForMonth(12, year) },
                };
            } catch(MySqlException e)
            {
                EventLogger.Post($"DTB :: MySQLException in Loading Database{e.Message}");
            }
            return Task.FromResult(true);
        }
        private static Task<bool> LoadMonthChart(int month, int year)
        {
            try
            {
                ThisMonthsChart = new List<KeyValuePair<string, double>> {
                        new KeyValuePair<string, double>("Share Capital", Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH(typeof(ShareCapital).Name.ToLower(), month, year)).FirstOrDefault()),
                        new KeyValuePair <string, double>("Loan", Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH(typeof(Loan).Name.ToLower(), month, year)).FirstOrDefault()),
                        new KeyValuePair <string, double>("Long Term Loan", Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH(typeof(LongTermLoan).Name.ToLower(), month, year)).FirstOrDefault())
                };
            }
            catch(MySqlException e)
            {
                EventLogger.Post($"DTB :: MySQLException in Loading Database{e.Message}");
            }

            return Task.FromResult(true);
        }
        
        
        public static void PopulateDefaults()
        {
            APPSTATE_PATH = "Config\\AppState.json";
            LOGS = "Logs\\log.txt";
            OUTPUT_PATH = "Output\\";
            DEFAULT_PASSWORD = "Admin1234";
            DEFAULT_ADDRESSLINE2 = "Sapang Palay San Jose Del Monte, Bulacan";
            EXPIRATION_DATE = "2023 - 2024";
            REGISTRATION_NO = "9520-03006397";
            PRINT_AJUSTMENTS = 0;
            DEFAULT_CAMERA = 0;
            LOG_WINDOW = false;
            TABLE_BATCH_SIZE = 2;
        }
        public static void SaveToJson()
        {
            var data = new
            {
                APPSTATE_PATH,
                LOGS,
                OUTPUT_PATH,
                DEFAULT_PASSWORD,
                DEFAULT_ADDRESSLINE2,
                EXPIRATION_DATE,
                REGISTRATION_NO,
                PRINT_AJUSTMENTS,
                LOG_WINDOW,
                DEFAULT_CAMERA,
                TABLE_BATCH_SIZE,
            };

            if (File.Exists(APPSTATE_PATH))
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(APPSTATE_PATH, json);
            }
            else
            {
                try
                {
                    PopulateDefaults();
                    Directory.CreateDirectory(Path.GetDirectoryName(APPSTATE_PATH));
                    File.Create(APPSTATE_PATH).Close();
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    File.WriteAllText(APPSTATE_PATH, json);
                }
                catch (Exception ex)
                {
                    ControlWindow.ShowStatic("Error creating log file", ex.Message);
                }
            }
        }
        public static void LoadFromJson()
        {
            if (File.Exists(APPSTATE_PATH))
            {
                string json = File.ReadAllText(APPSTATE_PATH);
                try
                {
                    dynamic data = JsonConvert.DeserializeObject(json);
                    APPSTATE_PATH = data.APPSTATE_PATH;
                    LOGS = data.LOGS;
                    OUTPUT_PATH = data.OUTPUT_PATH;
                    DEFAULT_PASSWORD = data.DEFAULT_PASSWORD;
                    DEFAULT_ADDRESSLINE2 = data.DEFAULT_ADDRESSLINE2;
                    EXPIRATION_DATE = data.EXPIRATION_DATE;
                    REGISTRATION_NO = data.REGISTRATION_NO;
                    PRINT_AJUSTMENTS = data.PRINT_AJUSTMENTS;
                    LOG_WINDOW = data.LOG_WINDOW;
                    DEFAULT_CAMERA = data.DEFAULT_CAMERA;
                    TABLE_BATCH_SIZE = data.TABLE_BATCH_SIZE;
                    if (DatabaseConnection.HasConnection())
                    {
                        ALL_EMPLOYEES = Retrieve.GetDataUsingQuery<string>(RequestQuery.GET_LIST_OF_POSITION).ToArray();
                    }
                }
                catch (MySqlException ex)
                {
                    EventLogger.Post("ERR :: MySqlException in JSON : " + ex.Message);
                }
                catch (Exception e)
                {
                    EventLogger.Post("ERR :: Exception in JSON : " + e.Message);
                }
                
            }
            else
            {
                PopulateDefaults();
                if (File.Exists(APPSTATE_PATH))
                {
                    AppState.LoadFromJson();
                } else
                {
                    AppState.SaveToJson();
                    AppState.LoadFromJson();
                }
            }
            if(ALL_EMPLOYEES.Length >= 4)
                Employees = new List<string> { ALL_EMPLOYEES?[0], ALL_EMPLOYEES?[1], ALL_EMPLOYEES?[2], ALL_EMPLOYEES?[3] };
        }




        public static Employee FetchChairman()
        {
            Employee chair = Retrieve.GetDataUsingQuery<Employee>(RequestQuery.GET_CURRENT_CHAIRMAN).FirstOrDefault();
            //EventLogger.Post($"OUT :: {chair?.sign?.ToString()}");
            return chair;
        }
        private static double RetrieveIncomeForMonth(int month, int year)
        {
            if (month > DateTime.Now.Month)
            {
                year--;
            }
            try
            {
                var income = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH(month, year)).FirstOrDefault();
                return income;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                EventLogger.Post($"ERR :: Exception in Retrieveing Income{ex.Message}");
                return 0.0;
            }
        }
        public static async Task<bool> Test()
        {
            await Task.Delay(10000);
            return true;
        }

        public static async Task<bool> LoadMonthlyIncomeOfYear(int year)
        {
            return await LoadMonthlyIncome(year);
        }
        public static async Task<bool> LoadMothChartOf(int month, int year)
        {
            return await LoadMonthChart(month, year);
        }

        public static string GetEnumDescription(General value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }

}
