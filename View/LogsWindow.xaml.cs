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
            List<LogEntry> logEntries = new List<LogEntry>();
            try
            {
                string[] lines = File.ReadAllLines(path);

                string tmpDay = "";
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
                                Day = (tmpDay != dateParts[0] + dateParts[1] || !(cbUniqueTime.IsChecked??false))?dateParts[0]  : "",
                                Date = (tmpDay != dateParts[0] + dateParts[1] || !(cbUniqueTime.IsChecked ?? false)) ? dateParts[1] : "",
                                Time = dateParts[2],
                                Type = columns[1],
                                Message = columns[2]
                            });
                            tmpDay = dateParts[0] + dateParts[1];
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
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dgLogs.Items.Filter = (item) =>
            {
                if (item is LogEntry)
                {
                    LogEntry typedItem = (LogEntry)item;
                    return typedItem.Search(tbSearch.Text.ToLower(), cbDay.IsChecked ?? false, cbDate.IsChecked ?? false, cbTime.IsChecked ?? false, cbType.IsChecked ?? false, cbMessage.IsChecked ?? false);
                }
                return false;
            };
        }


        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            ReloadLog();
        }
        public class LogEntry
        {
            public string Day { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Type { get; set; }
            public string Message { get; set; }

            public bool Search(string search, bool searchDay, bool searchDate, bool searchTime, bool searchType, bool searchMessage)
            {
                if (searchDay && Day.ToLower().Contains(search)) return true;
                if (searchDate && Date.ToLower().Contains(search)) return true;
                if (searchTime && Time.ToLower().Contains(search)) return true;
                if (searchType && Type.ToLower().Contains(search)) return true;
                if (searchMessage && Message.ToLower().Contains(search)) return true;

                return false;
            }
        }

    }
}
