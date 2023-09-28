using Newtonsoft.Json;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View;
using SPTC_APP.View.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
        public static string CHAIRMAN;
        public static string REGISTRATION_NO;
        public static double PRINT_AJUSTMENTS;
        public static int DEFAULT_CAMERA;
        public static string[] ALL_EMPLOYEES;


        //NOT SAVED EXTERNALLY
        public static List<string> Employees;
        public static bool IS_ADMIN = false;
        public static Employee USER = null;
        public static Dictionary<string, double> MonthlyIncome;

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
            USER = null;
            EventLogger.Post($"USER :: Logout Success");
            (new Login()).Show();
            AppState.mainwindow = null;
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
                AppState.SaveToJson();
                string windowList = string.Join(", ", OpenedWindows.Select(w => w.ToString()));
                EventLogger.Post($"OUT :: Opened Window Count = {AppState.OpenedWindows.Count}, Opened Windows: [ {windowList} ]");
            }
        }
        public static async Task<bool> LoadDatabase()
        {
            try
            {
                MonthlyIncome = new Dictionary<string, double>
                {
                    { "Jan", RetrieveIncomeForMonth(1) },
                    { "Feb", RetrieveIncomeForMonth(2) },
                    { "Mar", RetrieveIncomeForMonth(3) },
                    { "Apr", RetrieveIncomeForMonth(4) },
                    { "May", RetrieveIncomeForMonth(5) },
                    { "Jun", RetrieveIncomeForMonth(6) },
                    { "Jul", RetrieveIncomeForMonth(7) },
                    { "Aug", RetrieveIncomeForMonth(8) },
                    { "Sep", RetrieveIncomeForMonth(9) },
                    { "Oct", RetrieveIncomeForMonth(10) },
                    { "Nov", RetrieveIncomeForMonth(11) },
                    { "Dec", RetrieveIncomeForMonth(12) }
                };
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
        public static void PopulateDefaults()
        {
            APPSTATE_PATH = "Config\\AppState.json";
            LOGS = "Logs\\log.txt";
            OUTPUT_PATH = "Output\\";
            DEFAULT_PASSWORD = "Admin1234";
            DEFAULT_ADDRESSLINE2 = "Sapang Palay San Jose Del Monte, Bulacan";
            EXPIRATION_DATE = "2023 - 2024";
            CHAIRMAN = "ROLLY M. LABINDAO";
            REGISTRATION_NO = "9520-03006397";
            PRINT_AJUSTMENTS = 0;
            DEFAULT_CAMERA = 0;
            ALL_EMPLOYEES = new string[] { "General Manager", "Secretary", "Treasurer", "Bookeeper", "Chairman" };
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
                CHAIRMAN,
                REGISTRATION_NO,
                PRINT_AJUSTMENTS,
                DEFAULT_CAMERA,
                ALL_EMPLOYEES,
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
                    ControlWindow.Show("Error creating log file", ex.Message);
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
                    CHAIRMAN = data.CHAIRMAN;
                    REGISTRATION_NO = data.REGISTRATION_NO;
                    PRINT_AJUSTMENTS = data.PRINT_AJUSTMENTS;
                    DEFAULT_CAMERA = data.DEFAULT_CAMERA;
                    ALL_EMPLOYEES = data.ALL_EMPLOYEES;

                }
                catch (Exception e)
                {
                    EventLogger.Post("ERR :: Exception in JSON : " + e.Message);
                }
            }
            else
            {
                PopulateDefaults();
                AppState.SaveToJson();
                if (File.Exists(APPSTATE_PATH))
                {
                    AppState.LoadFromJson();
                }
            }
            Employees = new List<string> { ALL_EMPLOYEES?[0], ALL_EMPLOYEES?[1], ALL_EMPLOYEES?[2], ALL_EMPLOYEES?[3] };
        }




        public static System.Windows.Media.ImageSource FetchChairmanSign()
        {
            Employee chair = Retrieve.GetDataUsingQuery<Employee>(RequestQuery.GET_CURRENT_CHAIRMAN).FirstOrDefault();
            //EventLogger.Post($"OUT :: {chair?.sign?.ToString()}");
            return chair?.sign?.GetSource() ?? null;
        }
        private static double RetrieveIncomeForMonth(int month)
        {
            int year = DateTime.Now.Year;

            // Check if the requested month is in the future (next year)
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
        
    }

}
