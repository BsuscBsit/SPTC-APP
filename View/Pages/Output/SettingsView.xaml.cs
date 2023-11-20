using AForge.Video.DirectShow;
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
        private string closingMSG;
        public SettingsView()
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            GenerateSettingsUI();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            AppState.mainwindow?.displayToast(closingMSG);
            base.OnClosing(e);
        }
        private void GenerateSettingsUI()
        {
           
            FieldInfo[] fields = typeof(AppState).GetFields(BindingFlags.Public | BindingFlags.Static);

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
                if(field.Name == "CAMERA_RESOLUTION")
                {
                    foreach (VideoCapabilities vc in AppState.GetResolutionList())
                    {
                        cbResolution.Items.Add($"{vc.FrameSize.Height}x{vc.FrameSize.Width}");
                    }
                    
                    cbResolution.SelectedItem = (string)field.GetValue(null);
                    cbResolution.Visibility = Visibility.Visible;
                    
                }
                else if(field.Name == "DEFAULT_CAMERA")
                {
                    foreach (var vc in AppState.GetCameras())
                    {
                        cbCamera.Items.Add(vc);
                    }

                    cbCamera.SelectedIndex = (int)field.GetValue(null);
                    cbCamera.Visibility = Visibility.Visible;
                }
                else if (field.FieldType == typeof(string))
                {
                    TextBox textBox = new TextBox
                    {
                        Text = (string)field.GetValue(null),
                        Width = 200,
                        Height = 31,
                        Style = (Style)Application.Current.FindResource("CommonTextBoxStyle"),
                    };

                    textBox.TextChanged += (sender, e) =>
                    {
                        field.SetValue(null, textBox.Text);
                    };

                    stackPanel.Children.Add(textBox);

                    SettingsPanel.Children.Add(stackPanel);
                }
                else if (field.FieldType == typeof(bool))
                {
                    CheckBox checkBox = new CheckBox
                    {
                        IsChecked = (bool)field.GetValue(null),
                        Width = 31,
                        Height = 31,
                        //Style = (Style)Application.Current.FindResource("CommonTextBoxStyle"),
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
                        Width = 200,
                        Height = 31,
                        Style = (Style)Application.Current.FindResource("CommonTextBoxStyle"),
                    };

                    textBox.TextChanged += (sender, e) =>
                    {
                        if (textBox.Text.ToString().Length > 0)
                        {
                            if (field.FieldType == typeof(double))
                                field.SetValue(null, double.Parse(textBox.Text));
                            else if (field.FieldType == typeof(int))
                                field.SetValue(null, int.Parse(textBox.Text));
                        }
                    };

                    stackPanel.Children.Add(textBox);

                    SettingsPanel.Children.Add(stackPanel);
                }
            }
        }
        private bool ShouldExcludeField(FieldInfo field)
        {
            string[] excludedFieldNames = { "ALL_EMPLOYEES", "Employees", "IS_ADMIN", "USER", "MonthlyIncome", "ThisMonthsChart", "isDeployment", "isDeployment_IDGeneration", "mainwindow", "isDesigner", "employees_list", "", "", "" };
            return excludedFieldNames.Contains(field.Name);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            AppState.DEFAULT_CAMERA = cbCamera.SelectedIndex;
            AppState.CAMERA_RESOLUTION = cbResolution.Text;
            AppState.SaveToJson();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbResolution.Items.Clear();
            foreach (VideoCapabilities vc in AppState.GetResolutionList())
            {
                cbResolution.Items.Add($"{vc.FrameSize.Height}x{vc.FrameSize.Width}");
            }
        }
    }
}
