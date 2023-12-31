﻿using AForge.Video;
using AForge.Video.DirectShow;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View;
using SPTC_APP.View.Pages;
using SPTC_APP.View.Styling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using static SPTC_APP.Objects.Ledger;

namespace SPTC_APP
{
    public static class AppState
    {
        //SAVED EXTERNALLY
        public static string APPSTATE_PATH;
        public static string LOGS;
        public static string OUTPUT_PATH;
        public static int LAST_LOGIN;
        public static string DEFAULT_PASSWORD;
        public static string DEFAULT_ADDRESSLINE2;
        public static string EXPIRATION_DATE;
        public static string REGISTRATION_NO;
        public static double PRINT_AJUSTMENTS;
        public static bool LOG_WINDOW;
        public static int DEFAULT_CAMERA;
        public static int TABLE_BATCH_SIZE;
        public static int CV_OR_LAST;
        public static double TRANSFER_FEE;
        public static double TOTAL_SHARE_PER_MONTH;
        public static string CAMERA_RESOLUTION;

        public static string SPTC;
        public static string ADDRESS;
        public static string EMAIL;
        public static string CDA;
        public static string CIN;
        public static string TIN;

        public static Newtonsoft.Json.Linq.JArray LIST_RECAP;
        


        //NOT SAVED EXTERNALLY
        public static string[] ALL_EMPLOYEES;
        public static List<string> Employees;
        public static bool IS_ADMIN = false;
        public static Employee USER = null;
        public static Dictionary<string, double> MonthlyIncome;
        public static List<KeyValuePair<string, double>> ThisMonthsChart;
        private static List<Employee> employees_list;
        public static List<Employee> EMPLOYEE_LIST(bool isLatest) {
            if (isLatest)
            {
                employees_list = Retrieve.GetDataUsingQuery<Employee>(RequestQuery.GET_ALL_EMPLOYEES);
            }
            return employees_list;
        }
        

        //Toggle on defore deploying
        public static bool isDeployment = true;
        public static bool isDeployment_IDGeneration = false;

        public static MainBody mainwindow = null;

        private static HashSet<Window> OpenedWindows = new HashSet<Window>();


        public static void Login(string username, string password, Window window)
        {
            dynamic result = Retrieve.Login(username, password);

            if (result is View.ControlWindow)
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
                    //(new Test()).Show();
                }
                if (AppState.isDeployment_IDGeneration)
                {
                    (new PrintPreview()).Show();
                }
                if (!AppState.isDeployment_IDGeneration && !AppState.isDeployment)
                {
                    MainBody body = (new MainBody());
                    AppState.mainwindow = body;
                    body.Show();
                    //(new Test()).Show();
                }


                EventLogger.Post($"USER :: Login Success: USER({username}) NAME({employee.name?.ToString()})");
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
                        new KeyValuePair<string, double>("Share Capital", Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH("SHARECAPITAL", month, year)).FirstOrDefault()),
                        new KeyValuePair<string, double>("Loan", Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH("LOAN", month, year)).FirstOrDefault()),
                        new KeyValuePair<string, double>("Long Term Loan", Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH("LONGTERMLOAN", month, year)).FirstOrDefault())
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
            LAST_LOGIN = 0;
            DEFAULT_PASSWORD = "Admin1234";
            DEFAULT_ADDRESSLINE2 = "Sapang Palay San Jose Del Monte, Bulacan";
            EXPIRATION_DATE = "2023 - 2024";
            REGISTRATION_NO = "9520-03006397";
            PRINT_AJUSTMENTS = 0;
            DEFAULT_CAMERA = 0;
            LOG_WINDOW = false;
            TABLE_BATCH_SIZE = 2;
            TOTAL_SHARE_PER_MONTH = 30;
            TRANSFER_FEE = 6400;
            CV_OR_LAST = 0;
            CAMERA_RESOLUTION = "";
            SPTC = "SAPANG PALAY TRICYCLE SERVICE COOPERATIVE";
            ADDRESS = "Blk 1 Lot 8, Sitio Hulo, Hacienda Sapang Palay Proper, City of San Jose Del Monte, Bulacan";
            EMAIL = "Sapangpalaytricyclecooperative@gmail.com";
            CDA = "9520-03006397";
            CIN = "0106030220";
            TIN = "234-829-228";
            LIST_RECAP = new Newtonsoft.Json.Linq.JArray { Field.CASH_ON_HAND, "Share Capital", "Monthly Dues", "Mutual Funds Payable", "Certification Fee", "Loan Receivable - Pastdue", "--------------- - Current", "Change Motor - Entrance", "--------------- - Motor", "Clearance Fee", "Miscelleneous Income", "Sales Trapal", "AIR-Stricker", "Penalties", "Interest Receivable", "Cash in Bank", "Rental Fee: Franchise", "Transfer Fees", "Membership Fee", "Seminar Fee" };
        }
        public static void SaveToJson()
        {
            var data = new
            {
                APPSTATE_PATH,
                LOGS,
                OUTPUT_PATH,
                LAST_LOGIN,
                DEFAULT_PASSWORD,
                DEFAULT_ADDRESSLINE2,
                EXPIRATION_DATE,
                REGISTRATION_NO,
                PRINT_AJUSTMENTS,
                LOG_WINDOW,
                DEFAULT_CAMERA,
                TABLE_BATCH_SIZE,
                CV_OR_LAST,
                TOTAL_SHARE_PER_MONTH,
                TRANSFER_FEE,
                CAMERA_RESOLUTION,
                SPTC,
                ADDRESS,
                EMAIL,
                CDA,
                CIN,
                TIN,
                LIST_RECAP,
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
                    LAST_LOGIN = data.LAST_LOGIN;
                    DEFAULT_PASSWORD = data.DEFAULT_PASSWORD;
                    DEFAULT_ADDRESSLINE2 = data.DEFAULT_ADDRESSLINE2;
                    EXPIRATION_DATE = data.EXPIRATION_DATE;
                    REGISTRATION_NO = data.REGISTRATION_NO;
                    PRINT_AJUSTMENTS = data.PRINT_AJUSTMENTS;
                    LOG_WINDOW = data.LOG_WINDOW;
                    DEFAULT_CAMERA = data.DEFAULT_CAMERA;
                    TABLE_BATCH_SIZE = data.TABLE_BATCH_SIZE;
                    CV_OR_LAST = data.CV_OR_LAST;
                    TOTAL_SHARE_PER_MONTH = data.TOTAL_SHARE_PER_MONTH;
                    TRANSFER_FEE = data.TRANSFER_FEE;
                    CAMERA_RESOLUTION = data.CAMERA_RESOLUTION;
                    SPTC = data.SPTC;
                    ADDRESS = data.ADDRESS;
                    EMAIL = data.EMAIL;
                    CDA = data.CDA;
                    CIN = data.CIN;
                    TIN = data.TIN;
                    LIST_RECAP = data.LIST_RECAP;
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
            if((ALL_EMPLOYEES?.Length ?? 0) >= 4)
                Employees = new List<string> { ALL_EMPLOYEES?[0], ALL_EMPLOYEES?[1], ALL_EMPLOYEES?[2], ALL_EMPLOYEES?[3] };
        }


        public static Employee FetchChairman()
        {
            Employee chair = Retrieve.GetDataUsingQuery<Employee>(RequestQuery.GET_CURRENT_CHAIRMAN).FirstOrDefault();
            //EventLogger.Post($"OUT :: {chair?.sign?.ToString()} {chair?.name?.ToString()}");
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
                double income = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_CASHONHAND(month, year)).FirstOrDefault();
                if (income <= 0)
                {
                    income = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH(month, year)).FirstOrDefault();
                }
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

        public static List<VideoCapabilities> GetResolutionList()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                EventLogger.Post("ERR :: No video devices found.");
                return new List<VideoCapabilities>();
            }
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[AppState.DEFAULT_CAMERA].MonikerString);
            
            return videoSource.VideoCapabilities.OrderByDescending(c => c.FrameSize.Width * c.FrameSize.Height).ToList();
        }
        private static bool isRecapLoaded = false;
        public static void LoadRecapList(int currentmonth, int currentYear)
        {
            if (!isRecapLoaded && AppState.LIST_RECAP != null)
            {
                foreach (string title in AppState.LIST_RECAP.ToObject<string[]>())
                {
                    if ((Retrieve.GetDataUsingQuery<int>(RequestQuery.CHECK_RECAP(title, currentmonth, currentYear)).FirstOrDefault() == 0))
                    {
                        Recap recap = new Recap(title, 0);
                        recap.date = new DateTime(currentYear, currentmonth, 1);
                        recap.Save();
                    }
                }
                isRecapLoaded = true;
            }

        }
        public static List<Recap> LoadRecapitulations(int currentmonth, int currentYear)
        {
           
            LoadRecapList(currentmonth, currentYear);

            return Retrieve.GetDataUsingQuery<Recap>(RequestQuery.GET_ALL_RECAP_IN_MONTH(currentmonth, currentYear));
        }

        public static void CheckCamera(int cam)
        {
            FilterInfoCollection videoDevices;
            VideoCaptureDevice videoSource;
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    EventLogger.Post("ERR :: No video devices found.");
                    return;
                }
                videoSource = new VideoCaptureDevice(videoDevices[cam].MonikerString);
                
                List<VideoCapabilities> vcList = videoSource.VideoCapabilities.OrderByDescending(c => c.FrameSize.Width * c.FrameSize.Height).ToList();
                bool change = false;
                foreach(VideoCapabilities vc in vcList)
                {
                    change = true;
                    if(AppState.CAMERA_RESOLUTION == $"{vc.FrameSize.Height}x{vc.FrameSize.Width}")
                    {
                        change = false;
                        break;
                    }
                }
                if (change)
                {
                    VideoCapabilities vc = vcList.LastOrDefault();
                    AppState.CAMERA_RESOLUTION = $"{vc.FrameSize.Height}x{vc.FrameSize.Width}";
                    AppState.SaveToJson();
                }
            }
            catch (Exception e)
            {
                EventLogger.Post($"ERR :: {e.Message}");
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    EventLogger.Post("ERR :: No video devices found.");
                    return;
                }
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                AppState.DEFAULT_CAMERA = 0;
                
                VideoCapabilities vc = videoSource.VideoCapabilities.OrderByDescending(c => c.FrameSize.Width * c.FrameSize.Height).ToList().LastOrDefault();
                AppState.CAMERA_RESOLUTION = $"{vc.FrameSize.Height}x{vc.FrameSize.Width}";
                AppState.SaveToJson();
            }
            
        }

        public static List<string> GetCameras()
        {
            CheckCamera(AppState.DEFAULT_CAMERA);

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            List<string> cameraList = new List<string>();

            foreach (FilterInfo device in videoDevices)
            {
                cameraList.Add(device.Name);
            }

            return cameraList;
        }
    }

}
