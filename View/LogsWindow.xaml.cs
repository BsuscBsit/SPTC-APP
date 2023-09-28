using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for LogsWindow.xaml
    /// </summary>
    public partial class LogsWindow : Window
    {
        public LogsWindow()
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            ReloadLog();
        }


        private void ReloadLog()
        {
            string path = AppState.LOGS;

            try
            {
                string[] lines = File.ReadAllLines(path);
                List<LogEntry> logEntries = new List<LogEntry>();
                foreach (string line in lines)
                {
                    string[] columns = line.Split(new string[] { " :: " }, StringSplitOptions.None);

                    if (columns.Length >= 3)
                    {
                        string[] dateParts = columns[0].Split(' ');

                        if (dateParts.Length >= 2)
                        {
                            logEntries.Add(new LogEntry
                            {
                                DayOfTheWeek = dateParts[0],
                                DateTime = dateParts[1] + " " + dateParts[2],
                                LogType = columns[1],
                                Message = columns[2]
                            });
                        }
                    }
                }

                dgLogs.ItemsSource = logEntries;
            }
            catch (Exception ex)
            {
                ControlWindow.Show("Error: ", ex.Message, Icons.ERROR);
            }
        }



        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            ReloadLog();
        }
        public class LogEntry
        {
            public string DayOfTheWeek { get; set; }
            public string DateTime { get; set; }
            public string LogType { get; set; }
            public string Message { get; set; }
        }

    }
}
