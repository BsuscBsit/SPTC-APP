using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for Recapitulations.xaml
    /// </summary>
    public partial class Recapitulations : Window
    {
        int currentmonth;
        int currentyear;
        private string[] monthAbbreviations = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

        public Recapitulations()
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            currentmonth = DateTime.Now.Month;
            currentyear = DateTime.Now.Year;
            UpdateRecap();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        public void displayToast(string message, double duration = 5)
        {
            if (!(string.IsNullOrEmpty(message) && string.IsNullOrWhiteSpace(message)))
            {
                System.Windows.Media.Brush bg = message.Contains("success") ?
                FindResource("BrushDeepGreen") as System.Windows.Media.Brush :
                FindResource("BrushRed") as System.Windows.Media.Brush;

                new Toast(gridToast, message, duration, true, 0.2, System.Windows.Media.Brushes.White, bg);
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPieForward_Click(object sender, RoutedEventArgs e)
        {
            if (currentmonth == 1)
            {
                currentmonth = 12;
                currentyear++;
            }
            else
            {
                currentmonth++;
            }
            UpdateRecap();

        }

        private void btnPieBackward_Click(object sender, RoutedEventArgs e)
        {
            if (currentmonth >= 12)
            {
                currentmonth = 1;
                currentyear--;
            }
            else
            {
                currentmonth--;
            }
            UpdateRecap();
        }

        public void UpdateRecap()
        {
            if (currentmonth == DateTime.Now.Month && currentyear == DateTime.Now.Year)
            {
                    btnPieForward.IsEnabled = false;
            }
            else
            {
                    btnPieForward.IsEnabled = true;
            }
            lblMonthYear.Content = $"{monthAbbreviations[currentmonth - 1] + ", " + currentyear}";

            List<Recap> recaps = AppState.LoadRecapitulations(currentmonth, currentyear);

            recapgrid.RowDefinitions.Clear();
            recapgrid.Children.Clear();
            double total = 0;
            List<Button> btnDeletes = new List<Button>();

            foreach (Recap r in recaps)
            {
                
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(40);
                recapgrid.RowDefinitions.Add(rowDefinition);

                RecapDisplay recapDisplay = new RecapDisplay(r, recaps.IndexOf(r), check(r));
                recapgrid.Children.Add(recapDisplay.AddSelf());
                Button button = recapDisplay.GetButton();
                button.MouseDown += btn_click;
                total += r.content;
            }
            tbTotal.Text = total.ToString("0.00");
        }

        private bool check(Recap r)
        {
            switch(r.text){
                case "Share Capital":
                    r.content = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH("SHARECAPITAL", DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault();
                    return true;
                default:
                    return  false;
            }
        }

        private void btn_click(object sender, MouseButtonEventArgs e)
        {
            UpdateRecap();
        }

        class RecapDisplay
        {
            private Recap recap;
            private Label label;
            private TextBox textBox;
            private Grid grid;
            private Button button;

            public RecapDisplay(Recap r, int row, bool isReadonly = false)
            {
                this.recap = r;
                this.grid = new Grid();
                grid.SetValue(Grid.RowProperty, row); 

                ColumnDefinition col1 = new ColumnDefinition();
                col1.Width = new GridLength(250);
                ColumnDefinition col2 = new ColumnDefinition();
                col2.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition col3 = new ColumnDefinition();
                col3.Width = new GridLength(100);
                grid.ColumnDefinitions.Add(col1);
                grid.ColumnDefinitions.Add(col2);
                grid.ColumnDefinitions.Add(col3);

                this.label = new Label();
                label.Content = $"{recap.text}: ";
                label.SetValue(Grid.ColumnProperty, 0);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.Width = 200;
                label.VerticalAlignment = VerticalAlignment.Center;

                this.textBox = new TextBox();
                textBox.Text = recap.content.ToString("0.00");
                textBox.SetValue(Grid.ColumnProperty, 1);
                textBox.IsReadOnly = isReadonly;
                textBox.Style = Application.Current.FindResource("CommonTextBoxStyle") as Style; 

                textBox.Margin = new Thickness(20, 5, 10, 5);

                button = new Button();
                button.Content = "SAVE";
                button.SetValue(Grid.ColumnProperty, 2);
                button.Width = 90;
                button.IsEnabled = !isReadonly;
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.Style = Application.Current.FindResource("CommonButtonStyle") as Style;
                button.Margin = new Thickness(5);
                button.Click += btn_click;


                grid.Children.Add(label);
                grid.Children.Add(textBox);
                grid.Children.Add(button);
            }

            private void btn_click(object sender, RoutedEventArgs e)
            {
                recap.content = Double.Parse(textBox.Text);
                recap.Save();
                
            }
            public Button GetButton()
            {
                return button;
            }

            public Grid AddSelf()
            {
                return grid;
            }
        }

    }
}
