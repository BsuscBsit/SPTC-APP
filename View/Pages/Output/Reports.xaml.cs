using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages.Input;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Image = System.Windows.Controls.Image;

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        private string filename = "Recapitulations";

        private string[] monthAbbreviations = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

        public Reports()
        {
            InitializeComponent();
        }

        public void Populate(List<Recap> recaps, int currentmonth, int currentyear)
        {
            double cashonhand = 0;
            double total = 0;
            lblDate.Content = $"{monthAbbreviations[currentmonth - 1] + ", " + currentyear}";
            foreach (Recap r in recaps)
            {
                bool is_cash_onhand = false;
                if (r.text == Field.CASH_ON_HAND)
                {
                    is_cash_onhand = true;
                    cashonhand = r.content;
                } else
                {
                    total += r.content;
                }
                
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(40);
                RecapDisplay recapDisplay = new RecapDisplay(r, !is_cash_onhand);
                recapgrid.Children.Add(recapDisplay.AddSelf());
            }

            Border bar = new Border {
                Height = 5,
                Width = 800,
                BorderBrush = System.Windows.Media.Brushes.Black,
            };
            recapgrid.Children.Add(bar);

            Grid grid = new Grid();
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(250);
            ColumnDefinition col2 = new ColumnDefinition();
            col2.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition col3 = new ColumnDefinition();
            col3.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);
            grid.ColumnDefinitions.Add(col3);

            Label label = new Label();
            label.Content = $"TOTAL: ";
            label.SetValue(Grid.ColumnProperty, 0);
            label.HorizontalAlignment = HorizontalAlignment.Right;
            label.Width = 180;
            label.FontWeight = FontWeights.Bold;
            label.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(label);

            Label labelBox = new Label();
            labelBox.Content = "\u20B1 " + cashonhand.ToString("N2");
            labelBox.SetValue(Grid.ColumnProperty, 1);
            labelBox.Height = 30;
            labelBox.FontSize = 16;
            labelBox.FontWeight = FontWeights.Bold;
            labelBox.Style = Application.Current.FindResource("SubTitlePreset") as Style;
            labelBox.VerticalAlignment = VerticalAlignment.Center;
            labelBox.Margin = new Thickness(0, 0,0, 0);
            grid.Children.Add(labelBox);

            Label labelBox2 = new Label();
            labelBox2.Content = "\u20B1 " + total.ToString("N2");
            labelBox2.SetValue(Grid.ColumnProperty, 2);
            labelBox2.Height = 30;
            labelBox2.FontSize = 16;
            labelBox2.FontWeight = FontWeights.Bold;
            labelBox2.Style = Application.Current.FindResource("SubTitlePreset") as Style;
            labelBox2.VerticalAlignment = VerticalAlignment.Center;
            labelBox2.Margin = new Thickness(0, 0, 0, 0);
            grid.Children.Add(labelBox2);

            recapgrid.Children.Add(grid);

        }

        class RecapDisplay
        {
            private Recap recap;
            private Label label;
            public TextBox textBox;
            public Label labelBox;
            public Label labelBox2;
            private Grid grid;

            public RecapDisplay( Recap r, bool isTabbed = true)
            {
                this.recap = r;
                this.grid = new Grid();

                recap.Save();

                ColumnDefinition col1 = new ColumnDefinition();
                col1.Width = new GridLength(250);
                ColumnDefinition col2 = new ColumnDefinition();
                col2.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition col3 = new ColumnDefinition();
                col3.Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions.Add(col1);
                grid.ColumnDefinitions.Add(col2);
                grid.ColumnDefinitions.Add(col3);

                this.label = new Label();
                label.Content = $"{recap.text}: ";
                label.SetValue(Grid.ColumnProperty, 0);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.Width = 180;
                label.VerticalAlignment = VerticalAlignment.Center;
                grid.Children.Add(label);


                this.labelBox = new Label();
                labelBox.Content = (!isTabbed)? "\u20B1 " + recap.content.ToString("N2"): "----------------------------";
                labelBox.SetValue(Grid.ColumnProperty, 1);
                labelBox.Height = 30;
                labelBox.FontSize = 16;
                labelBox.Style = Application.Current.FindResource("SubTitlePreset") as Style;
                labelBox.VerticalAlignment = VerticalAlignment.Center;
                labelBox.Margin = new Thickness(0, 0, 0, 0);
                grid.Children.Add(labelBox);


                this.labelBox2 = new Label();
                labelBox2.Content = (isTabbed)?"\u20B1 " + recap.content.ToString("N2"): "";
                labelBox2.SetValue(Grid.ColumnProperty, 2);
                labelBox2.Height = 30;
                labelBox2.FontSize = 16;
                labelBox2.Style = Application.Current.FindResource("SubTitlePreset") as Style;
                labelBox2.VerticalAlignment = VerticalAlignment.Center;
                labelBox2.Margin = new Thickness(0, 0, 0, 0);
                grid.Children.Add(labelBox2);
            }

            public Grid AddSelf()
            {
                return grid;
            }
        }


        public void StartPrint()
        {
            string outputPath = AppState.OUTPUT_PATH + filename + "_page.xps";

            if (!Directory.Exists(AppState.OUTPUT_PATH))
            {
                Directory.CreateDirectory(AppState.OUTPUT_PATH);
            }

            using (XpsDocument xpsDoc = new XpsDocument(outputPath, FileAccess.Write))
            {
                XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

                PrintTicket printTicket = new PrintTicket(); 
                xpsWriter.Write(reportpage, printTicket);
            }

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                XpsDocument xpsDocToPrint = new XpsDocument(outputPath, FileAccess.Read);
                FixedDocumentSequence fixedDocumentSequence = xpsDocToPrint.GetFixedDocumentSequence();

                // Get a paginator for the document sequence
                DocumentPaginator paginator = fixedDocumentSequence.DocumentPaginator;

                // Print the paginator
                printDialog.PrintDocument(paginator, "Printing XPS Document");
                xpsDocToPrint.Close();
                ControlWindow.ShowStatic("Print Success", "Recapitulations is printed", Icons.NOTIFY);
            }
        }
    }
}
