using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages.Output;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        static Recap cashonhand;
        double total;
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

        public void displayToast(string message, double duration = 3)
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

        
        private void btnPieBackward_Click(object sender, RoutedEventArgs e)
        {
            if (currentmonth == 1)
            {
                currentmonth = 12;
                currentyear--;
            }
            else
            {
                currentmonth--;
            }
            UpdateRecap();
        }
        private void btnPieForward_Click(object sender, RoutedEventArgs e)
        {
            if (currentmonth >= 12)
            {
                currentmonth = 1;
                currentyear++;
            }
            else
            {
                currentmonth++;
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

            recapgrid.Children.Clear();
            total = 0;

            foreach (Recap r in recaps)
            {
                if (r.text == Field.CASH_ON_HAND)
                {
                    cashonhand = r;
                }
                else
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(40);
                    bool ch = check(r);
                    RecapDisplay recapDisplay = new RecapDisplay(this, r, ch);
                    if (ch)
                    {
                        recapgrid.Children.Add(recapDisplay.AddSelf());
                    } else
                    {
                        recapgrid1.Children.Add(recapDisplay.AddSelf());
                    }
                    UpdateTotal(r.content);
                }
            }
            tbTotal.Content = "\u20B1" + total.ToString("N2");
            cashonhand.content = total;
            cashonhand.Save();
        }

        private bool check(Recap r)
        {
            switch(r.text){
                case "Share Capital":
                    r.content = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH("SHARECAPITAL", DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault();
                    return true;
                /*case "Monthly Dues":
                    r.content = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH($"LOAN\" || {Field.LEDGER_TYPE} = \"LONGTERMLOAN", DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault();
                    return true;*/
                case "Loan Receivable - Pastdue":
                    r.content = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_LOAN_PASTDUE_IN_MONTH(DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault();
                    return true;
                case "--------------- - Current":
                    r.content = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_LOAN_CURRENT_IN_MONTH(DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault();
                    return true;
                case "Penalties":
                    r.content = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_LOAN_PENALTY_IN_MONTH(DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault();
                    return true;
                case "Interest Receivable":
                    r.content = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_LOAN_INTEREST_IN_MONTH(DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault();
                    return true;
                case "Transfer Fees":
                    r.content = Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_ALL_PAYMENT_IN_MONTH("TRANSFER_FEE", DateTime.Now.Month, DateTime.Now.Year)).FirstOrDefault();
                    return true;
                default:
                    return false;
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
            public Label labelBox;
            private Grid grid;
            private bool isReadOnly;

            public RecapDisplay(Recapitulations rptc, Recap r, bool isread = false)
            {
                this.recapmain = rptc;
                this.recap = r;
                this.grid = new Grid();
                this.isReadOnly = isread;

                recap.Save();

                ColumnDefinition col1 = new ColumnDefinition();
                col1.Width = new GridLength(250);
                ColumnDefinition col2 = new ColumnDefinition();
                col2.Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions.Add(col1);
                grid.ColumnDefinitions.Add(col2);

                this.label = new Label();
                label.Content = $"{recap.text}: ";
                label.SetValue(Grid.ColumnProperty, 0);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.Width = 180;
                label.VerticalAlignment = VerticalAlignment.Center;
                grid.Children.Add(label);
                if (!isReadOnly)
                {
                    this.textBox = new TextBox();
                    textBox.Text = "\u20B1" + recap.content.ToString("N2");
                    textBox.SetValue(Grid.ColumnProperty, 1);
                    textBox.Height = 30;
                    textBox.Style = Application.Current.FindResource("CommonTextBoxStyle") as Style;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.Margin = new Thickness(20, 0, 10, 0);
                    textBox.GotFocus += SelectAll;
                    textBox.LostFocus += Save;
                    textBox.DefaultTextBoxBehavior(NUMBERPERIOD, true, recapmain.gridToast, null);
                    
                    grid.Children.Add(textBox);
                } else
                {
                    this.labelBox = new Label();
                    labelBox.Content = "\u20B1" + recap.content.ToString("N2");
                    labelBox.SetValue(Grid.ColumnProperty, 1);
                    labelBox.Height = 30;
                    labelBox.FontSize = 16;
                    labelBox.Style = Application.Current.FindResource("SubTitlePreset") as Style;
                    labelBox.VerticalAlignment = VerticalAlignment.Center;
                    labelBox.Margin = new Thickness(20, 0, 10, 0);
                    grid.Children.Add(labelBox);
                }
            }

            private void Save(object sender, RoutedEventArgs e)
            {
                string textWithoutCurrencySymbol = textBox.Text.Replace("\u20B1", "");

                if (double.TryParse(textWithoutCurrencySymbol, out double newval))
                {
                    if (recap.content != newval)
                    {
                        recap.content = newval;
                        recap.Save();
                        recapmain.displayToast($"{recap.text} Updated!", 1);
                        recapmain.UpdateTotal(newval);
                    }
                }

                textBox.Text = "\u20B1" + textWithoutCurrencySymbol;
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

        private void UpdateTotal(double newVal)
        {
            total += newVal;
            tbTotal.Content = "\u20B1" + total.ToString("N2");
            cashonhand.content = total;
            cashonhand.Save();
        }

        private void tbTotal_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateRecap();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ControlWindow.ShowTwoway("Printing Report", "Are you sure you want to print Recapitulations?", Icons.NOTIFY))
            {
                Reports report = new Reports();
                List<Recap> recaps = AppState.LoadRecapitulations(currentmonth, currentyear);
                report.Populate(recaps, currentmonth, currentyear);
                report.StartPrint();
            }
        }
    }
}
