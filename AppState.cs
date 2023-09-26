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
        public static string APPSTATE_PATH = "Config\\AppState.json";
        public static string OUTPUT_PATH = "Output\\";
        public static string DEFAULT_PASSWORD = "Admin1234";
        public static string DEFAULT_ADDRESSLINE2 = "Sapang Palay San Jose Del Monte, Bulacan";
        public static string EXPIRATION_DATE = "2023 - 2024";
        public static string CHAIRMAN = "ROLLY M. LABINDAO";
        public static string REGISTRATION_NO = "9520-03006397";
        public static double PRINT_AJUSTMENTS = 0;
        public static int DEFAULT_CAMERA = 0;


        //"Signature - CurrentChairman" signature usage in name


        //NOT SAVED EXTERNALLY
        public static List<string> Employees =new List<string> { "General Manager", "Secretary", "Treasurer", "Bookeeper" };
        public static bool IS_ADMIN = false;
        public static Employee USER = null;
        public static Dictionary<string, double> MonthlyIncome;

        //Toggle on defore deploying
        public static bool isDeployment = false;
        public static bool isDeployment_IDGeneration = false;

        public static Window mainwindow = null;

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
                if(!AppState.isDeployment_IDGeneration && !AppState.isDeployment)
                {
                    (new Test()).Show();
                }

                
                EventLogger.Post($"User :: Login Success: USER({username})");
                window.Close();
            }
        }

        public static void Logout(Window window)
        {
            IS_ADMIN = false;
            USER = null;
            EventLogger.Post($"User :: Logout Success");
            (new Login()).Show();
            AppState.mainwindow = null;
            window.Close();
        }

        public static void SaveToJson()
        {
            var data = new
            {
                APPSTATE_PATH,
                OUTPUT_PATH,
                DEFAULT_PASSWORD,
                DEFAULT_ADDRESSLINE2,
                EXPIRATION_DATE,
                CHAIRMAN,
                REGISTRATION_NO,
                PRINT_AJUSTMENTS,
                DEFAULT_CAMERA,
            };

            if (File.Exists(APPSTATE_PATH))
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(APPSTATE_PATH, json);
            } else
            {
                try
                {
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
                    OUTPUT_PATH = data.OUTPUT_PATH;
                    DEFAULT_PASSWORD = data.DEFAULT_PASSWORD;
                    DEFAULT_ADDRESSLINE2 = data.DEFAULT_ADDRESSLINE2;
                    EXPIRATION_DATE = data.EXPIRATION_DATE;
                    CHAIRMAN = data.CHAIRMAN;
                    REGISTRATION_NO = data.REGISTRATION_NO;
                    PRINT_AJUSTMENTS = data.PRINT_AJUSTMENTS;
                    DEFAULT_CAMERA = data.DEFAULT_CAMERA;
                         
                }
                catch (Exception e)
                {
                    EventLogger.Post("ERR :: Exception in JSON : "+e.Message);
                }
            }
        }

        public static System.Windows.Media.ImageSource FetchChairmanSign()
        {
            Image presImage = Retrieve.GetDataUsingQuery<Image>(RequestQuery.GET_CURRENT_CHAIRMAN_SIGN).FirstOrDefault();
            return presImage.GetSource();
        }
    }

}
