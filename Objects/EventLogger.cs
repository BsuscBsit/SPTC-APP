using System;
using System.IO;
using System.Linq;
using SPTC_APP.View;

namespace SPTC_APP.Objects
{
    public class EventLogger
    {
        private static readonly int MaxLines = 10000;

        public static void Post(string message)
        {
            string logEntry = $"{DateTime.Now:ddd MMM-dd HH:mm} :: {message}{Environment.NewLine}";

            try
            {
                EnsureLogFileExists();

                string currentLogContents = File.ReadAllText(AppState.LOGS);
                string updatedLogContents = logEntry + currentLogContents;

                if (updatedLogContents.CountLines() > MaxLines)
                {
                    string[] lines = updatedLogContents.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    updatedLogContents = string.Join(Environment.NewLine, lines.Take(MaxLines));
                }

                File.WriteAllText(AppState.LOGS, updatedLogContents);

            }
            catch (Exception ex)
            {
                ControlWindow.Show("Error writing to log file", ex.Message);
            }
        }

        private static void EnsureLogFileExists()
        {
            if (!File.Exists(AppState.LOGS))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(AppState.LOGS));
                    File.Create(AppState.LOGS).Close();
                }
                catch (Exception ex)
                {
                    ControlWindow.Show("Error creating log file", ex.Message);
                }
            }
        }


    }

    public static class StringExtensions
    {
        public static int CountLines(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int count = 1;
            int position = 0;
            while ((position = text.IndexOf(Environment.NewLine, position)) != -1)
            {
                count++;
                position += Environment.NewLine.Length;
            }

            return count;
        }
    }


}
