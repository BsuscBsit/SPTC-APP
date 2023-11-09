using MySql.Data.MySqlClient;
using SPTC_APP.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPieForward_Click(object sender, RoutedEventArgs e)
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

        private void btnPieBackward_Click(object sender, RoutedEventArgs e)
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
                btnAdd.IsEnabled = true;
            }
            else
            {
                btnPieForward.IsEnabled = true;
                btnAdd.IsEnabled = false;
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

                RecapDisplay recapDisplay = new RecapDisplay(r, recaps.IndexOf(r));
                recapgrid.Children.Add(recapDisplay.AddSelf());

                Button btn = recapDisplay.getDelete();
                btn.MouseLeave += click_remove;
                total += r.content;
            }
            tbTotal.Text = total.ToString("0.00");
        }

        private void click_remove(object sender, MouseEventArgs e)
        {
            if(sender is Button btn){
                if (btn.Tag?.ToString().Equals("DELETED") ?? false)
                {
                    
                }

                UpdateRecap();
            }
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            (new AddRecap(this)).Show();
        }

        class RecapDisplay
        {
            private Recap recap;
            private Label label;
            private TextBox textBox;
            private Button btnRemove;
            private Grid grid;

            public RecapDisplay(Recap r, int row)
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
                ColumnDefinition col4 = new ColumnDefinition();
                col4.Width = new GridLength(100);
                grid.ColumnDefinitions.Add(col1);
                grid.ColumnDefinitions.Add(col2);
                grid.ColumnDefinitions.Add(col3);
                grid.ColumnDefinitions.Add(col4);

                this.label = new Label();
                label.Content = $"{recap.text}: ";
                label.SetValue(Grid.ColumnProperty, 0);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.Width = 200;
                label.VerticalAlignment = VerticalAlignment.Center;

                this.textBox = new TextBox();
                textBox.Text = recap.content.ToString("0.00");
                textBox.SetValue(Grid.ColumnProperty, 1);
                textBox.Style = Application.Current.FindResource("CommonTextBoxStyle") as Style; 

                textBox.Margin = new Thickness(20, 5, 10, 5);

                Button button = new Button();
                button.Content = "SYNC";
                button.SetValue(Grid.ColumnProperty, 2);
                button.Width = 90;
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.Style = Application.Current.FindResource("CommonButtonStyle") as Style;
                button.Margin = new Thickness(5);
                button.Click += btn_click;

                btnRemove = new Button();
                btnRemove.Content = "REMOVE";
                btnRemove.Width = 90;
                btnRemove.SetValue(Grid.ColumnProperty, 3);
                btnRemove.HorizontalAlignment = HorizontalAlignment.Center;
                btnRemove.VerticalAlignment = VerticalAlignment.Center;
                btnRemove.Style = Application.Current.FindResource("CommonButtonStyle") as Style;
                btnRemove.Margin = new Thickness(5);
                btnRemove.Click += btnRemove_click;

                grid.Children.Add(label);
                grid.Children.Add(textBox);
                grid.Children.Add(button);
                grid.Children.Add(btnRemove);
            }

            private void btnRemove_click(object sender, RoutedEventArgs e)
            {
                recap.Delete();
                btnRemove.Style = Application.Current.FindResource("RedButtonStyle") as Style;
                btnRemove.Tag = "DELETED";
            }

            public Button getDelete()
            {
                return btnRemove;
            }

            private void btn_click(object sender, RoutedEventArgs e)
            {
                recap.content = Double.Parse(textBox.Text);
                recap.Save();
            }

            public Grid AddSelf()
            {
                return grid;
            }
        }

    }
}
