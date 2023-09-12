using System.Windows;
using SPTC_APP.Properties;

namespace SPTC_APP.View
{
    public partial class DatabaseConfigInput : Window
    {
        public string Host { get; private set; }
        public string Port { get; private set; }
        public string Database { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public bool Exit { get; private set; }

        public DatabaseConfigInput()
        {
            InitializeComponent();
            Exit = false;
            // Set the Topmost property to true
            Topmost = true;

            // Load the last user input values
            LoadLastUserInput();
        }

        private void LoadLastUserInput()
        {
            // Retrieve the last user input values from the Settings file
            Host = Settings.Default.Host;
            Port = Settings.Default.Port;
            Database = Settings.Default.Database;
            Username = Settings.Default.Username;
            Password = Settings.Default.Password;

            // Set the textbox values to the last user input
            tbHost.Text = Host;
            tbPort.Text = Port;
            tbDatabase.Text = Database;
            tbUsername.Text = Username;
            pbPassword.Password = Password;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the input values from the textboxes
            Host = tbHost.Text;
            Port = tbPort.Text;
            Database = tbDatabase.Text;
            Username = tbUsername.Text;
            Password = pbPassword.Password;

            // Save the changes to the settings
            Settings.Default.Host = Host;
            Settings.Default.Port = Port;
            Settings.Default.Database = Database;
            Settings.Default.Username = Username;
            Settings.Default.Password = Password;
            Settings.Default.Save();

            // Close the window
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Exit = true;
            Close();
        }

        private void configProceed_Click(object sender, RoutedEventArgs e)
        {
            brdrPrompt.Visibility = Visibility.Hidden;
            dciWindow.Height = 430;
            dciWindow.Width = 352;
            dciWindow.Top = (SystemParameters.PrimaryScreenHeight - 430) / 2;
            dciWindow.Left = (SystemParameters.PrimaryScreenWidth - 352) / 2;
        }
    }
}
