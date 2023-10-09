using SPTC_APP.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
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

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
            AppState.mainwindow?.Hide();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); GenerateSettingsUI(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }
        private void GenerateSettingsUI()
        {
            SettingsPanel.Children.Clear();
            FieldInfo[] fields = typeof(AppState).GetFields(BindingFlags.Public | BindingFlags.Static);

            //make a way to excclude some values

            foreach (FieldInfo field in fields)
            {
                if (ShouldExcludeField(field))
                {
                    continue; 
                }
                StackPanel stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(10, 0, 0, 10)
                };
                TextBlock label = new TextBlock
                {
                    Text = field.Name,
                    Width = 150,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                stackPanel.Children.Add(label);

                if (field.FieldType == typeof(string))
                {
                    TextBox textBox = new TextBox
                    {
                        Text = (string)field.GetValue(null),
                        Width = 200
                    };

                    textBox.TextChanged += (sender, e) =>
                    {
                        field.SetValue(null, textBox.Text);
                    };

                    stackPanel.Children.Add(textBox);

                    SettingsPanel.Children.Add(stackPanel);
                }
                if (field.FieldType == typeof(bool))
                {
                    CheckBox checkBox = new CheckBox
                    {
                        IsChecked = (bool)field.GetValue(null)
                    };

                    checkBox.Checked += (sender, e) =>
                    {
                        field.SetValue(null, true);
                    };

                    checkBox.Unchecked += (sender, e) =>
                    {
                        field.SetValue(null, false);
                    };

                    stackPanel.Children.Add(checkBox);
                    SettingsPanel.Children.Add(stackPanel);
                }
                else if (field.FieldType == typeof(double) || field.FieldType == typeof(int))
                {
                    TextBox textBox = new TextBox
                    {
                        Text = field.GetValue(null).ToString(),
                        Width = 200
                    };

                    textBox.TextChanged += (sender, e) =>
                    {
                        if (field.FieldType == typeof(double))
                            field.SetValue(null, double.Parse(textBox.Text));
                        else if (field.FieldType == typeof(int))
                            field.SetValue(null, int.Parse(textBox.Text));
                    };

                    stackPanel.Children.Add(textBox);

                    SettingsPanel.Children.Add(stackPanel);
                }
                else if (field.FieldType == typeof(string[]))
                {
                    ListBox listBox = new ListBox
                    {
                        ItemsSource = (string[])field.GetValue(null),
                        Width = 200,
                        MaxHeight = 200
                    };

                    stackPanel.Children.Add(listBox);

                    SettingsPanel.Children.Add(stackPanel);
                }
            }
        }
        private bool ShouldExcludeField(FieldInfo field)
        {
            // Add your exclusion criteria here based on field names or any other conditions
            string[] excludedFieldNames = { "Employees", "IS_ADMIN", "USER", "MonthlyIncome", "ThisMonthsChart", "isDeployment", "isDeployment_IDGeneration", "mainwindow", "", "", "", "", "", "" };
            return excludedFieldNames.Contains(field.Name);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            AppState.SaveToJson();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
