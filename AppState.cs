using SPTC_APP.Database;
using System.Windows;
using SPTC_APP.Objects;
using SPTC_APP.View.Pages;
using SPTC_APP.View;
using System.Windows.Documents;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;

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



        //NOT SAVED EXTERNALLY
        public static List<string> Employees =new List<string> { "General Manager", "Secretary", "Treasurer", "Book Keeper" };
        public static bool IS_ADMIN = false;
        public static Employee USER = null;

        public static Window mainwindow = null;

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
                //(new PrintPreview()).Show();
                //(new Test()).Show();
                MainBody body = (new MainBody());
                AppState.mainwindow = body;
                body.Show();
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
                PRINT_AJUSTMENTS
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
                         
                }
                catch (Exception e)
                {
                    EventLogger.Post("ERR :: Exception : "+e.Message);
                }
            }
        }
    }

}
