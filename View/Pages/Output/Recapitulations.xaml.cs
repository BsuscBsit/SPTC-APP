using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
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

using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;
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

            tbTotal.DefaultTextBoxBehavior(NUMBERPERIOD, true, gridToast, null, 21);
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
            gridChashonhand.Children.Clear();
            recapgrid.Children.Clear();
            double total = 0;

            foreach (Recap r in recaps)
            {
                if (r.text == Field.CASH_ON_HAND)
                {
                    RecapDisplay recapDisplay = new RecapDisplay(this, r, recaps.Count - 1, check(r.text));
                    gridChashonhand.Children.Add(recapDisplay.AddSelf());
                }
                else
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(40);
                    recapgrid.RowDefinitions.Add(rowDefinition);

                    RecapDisplay recapDisplay = new RecapDisplay(this, r, recaps.IndexOf(r) -1, check(r.text));
                    recapgrid.Children.Add(recapDisplay.AddSelf());
                    total += r.content;
                }
            }
            tbTotal.Text = total.ToString("0.00");
        }

        private KeyValuePair<bool, double> check(string r)
        {
            switch(r){
                case "Share Capital":
                    return new KeyValuePair<bool, double>(true, Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH("SHARECAPITAL", DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault());
                default:
                    return  new KeyValuePair<bool, double>(false, 0);
            }
        }

        private void btn_click(object sender, MouseButtonEventArgs e)
        {
            UpdateRecap();
        }

        class RecapDisplay
        {
            private Recapitulations recapmain;
            private Recap recap;
            private Label label;
            public TextBox textBox;
            private Grid grid;
            private bool isReadOnly;
            private double autoval;

            public RecapDisplay(Recapitulations rptc, Recap r, int row, KeyValuePair<bool, double> kvp)
            {
                this.recapmain = rptc;
                this.recap = r;
                this.grid = new Grid();
                this.isReadOnly = kvp.Key;
                this.autoval = kvp.Value;
                grid.SetValue(Grid.RowProperty, row); 

                ColumnDefinition col1 = new ColumnDefinition();
                col1.Width = new GridLength(250);
                ColumnDefinition col2 = new ColumnDefinition();
                col2.Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions.Add(col1);
                grid.ColumnDefinitions.Add(col2);

                this.label = new Label();
                label.Content = $"{recap.text}: " + (isReadOnly? $"({autoval})": "");
                label.SetValue(Grid.ColumnProperty, 0);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.Width = 200;
                label.VerticalAlignment = VerticalAlignment.Center;

                this.textBox = new TextBox();
                textBox.Text = recap.content.ToString("0.00");
                textBox.SetValue(Grid.ColumnProperty, 1);
                textBox.Height = 30;
                textBox.Style = Application.Current.FindResource("CommonTextBoxStyle") as Style;
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.Margin = new Thickness(20, 0, 10, 0);
                textBox.GotFocus += SelectAll;
                textBox.LostFocus += Save;
                textBox.DefaultTextBoxBehavior(NUMBERPERIOD, true, recapmain.gridToast, null, row);
                grid.Children.Add(label);
                grid.Children.Add(textBox);
            }

            private void Save(object sender, RoutedEventArgs e)
            {
                double newval = Double.Parse(textBox.Text);
                if (recap.content != newval) {
                    recap.content = newval;
                    recap.Save();
                    recapmain.displayToast($"{recap.text} Updated!", 1);
                }
            }

            private void SelectAll(object sender, RoutedEventArgs e)
            {
                textBox.SelectAll();
            }



            public Grid AddSelf()
            {
                return grid;
            }
        }

        private void tbTotal_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateRecap();
        }

    }
}
