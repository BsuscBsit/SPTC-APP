using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI.Relational;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using SPTC_APP.View.Pages.Input;
using SPTC_APP.View.Styling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

using Table = SPTC_APP.Database.Table; // ~ dinagdag ko, nag-eeror eh:
/**
 * Severity	Code	Description	Project	File	Line	Suppression State
 * Error CS0104 'Table' is an ambiguous reference between 'SPTC_APP.Database.Table' and 'MySqlX.XDevAPI.Relational.Table'
 * **/

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {

        private Path clickedPie;
        private bool pieClicked = false;
        private double total = 0;

        private string[] monthAbbreviations = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

        private int pieMonth;
        private int pieYear;

        private bool isTresur;
        private bool isSecret;
        private bool isBookke;

        private double aMenuFinalHeight = 310.4;

        public DashboardView()
        {
            InitializeComponent();
            btnPieForward.IsEnabled = false;
            pieMonth = DateTime.Now.Month;
            pieYear = DateTime.Now.Year;
            tbTotalOperator.Content = Retrieve.GetDataUsingQuery<int>(RequestQuery.GET_TOTAL(Table.OPERATOR)).FirstOrDefault().ToString();
            tbTotalDriver.Content = Retrieve.GetDataUsingQuery<int>(RequestQuery.GET_TOTAL(Table.DRIVER)).FirstOrDefault().ToString();
            tbTotalShares.Content = "\u20B1 " + Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_TOTAL_SHARES).FirstOrDefault().ToString("0.00");

            gridReportWindow.Visibility = Visibility.Hidden;
            gridRepWinChild.Visibility = Visibility.Hidden;
            dpDate.SelectedDate = DateTime.Now;
            dpDate.DisplayDate = DateTime.Now;
            if ((AppState.USER?.position?.accesses[9] ?? false))
            {
                btnPrint.Visibility = Visibility.Visible;
            }


            isSecret = AppState.USER?.position?.title == AppState.Employees[1];
            isTresur = AppState.USER?.position?.title == AppState.Employees[2];
            isBookke = AppState.USER?.position?.title == AppState.Employees[3];
        }

        private async Task UpdateLFContent(int currentMonth, int currentYear)
        {
            await Task.Run(async() =>
            {
                if (currentMonth == DateTime.Now.Month && currentYear == DateTime.Now.Year)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        btnPieForward.IsEnabled = false;
                    });

                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        btnPieForward.IsEnabled = true;
                    });

                }

                pieMonth = currentMonth;
                pieYear = currentYear;

                await AppState.LoadMothChartOf(pieMonth, pieYear);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    DrawPieChart();
                });
            });
        }


        public async Task<Grid> Fetch()
        {
            DrawBarChart();
            DrawPieChart();
            if (mainpanel.Parent != null)
            {
                Window currentParent = mainpanel.Parent as Window;
                if (currentParent != null)
                {
                    currentParent.Content = null;
                }
            }
            await Task.Delay(5);
            this.Close();
            return mainpanel;
        }

        
        private void DrawBarChart()
        {
            var list = AppState.MonthlyIncome;
            cvBarChart.Children.Clear();

            int currentMonthIndex = DateTime.Now.Month - 1; 
            int startMonthIndex = (currentMonthIndex + 1) % 12;

            double maxBalance = list.Values.Max();
            double minBalance = list.Values.Min();

            double barWidth = (cvBarChart.Width / list.Count);

            double maxMagnitude = Math.Pow(10, Math.Floor(Math.Log10(maxBalance)));

            double roundingAdjustment = maxMagnitude <= 10 ? 1 :
                                       maxMagnitude <= 100 ? 10 :
                                       maxMagnitude <= 1000 ? 100 :
                                       maxMagnitude <= 10000 ? 1000 :
                                       maxMagnitude <= 100000 ? 10000 : 100000;

            double roundedMax = Math.Ceiling(maxBalance / roundingAdjustment) * roundingAdjustment;
            double roundedMin = Math.Ceiling(minBalance / roundingAdjustment) * roundingAdjustment;

            double barHeightScale = cvBarChart.Height / roundedMax;

            double[] roundedValues = { roundedMin, roundedMax, (roundedMax + roundedMin) / 2 };

            double[] yPositions = new double[3];

            for (int i = 0; i < 3; i++)
            {
                yPositions[i] = cvBarChart.Height * (1 - (roundedValues[i] / maxBalance));

                TextBlock textBlock = new TextBlock
                {
                    Text = roundedValues[i].ToString(),
                    TextAlignment = TextAlignment.Right,
                    Opacity = 0.6,
                    Width = 60
                };
                Canvas.SetTop(textBlock, yPositions[i] - 8);
                Canvas.SetLeft(textBlock, -65);

                cvBarChart.Children.Add(textBlock);
            }

            foreach (double yPos in yPositions)
            {
                if (!double.IsNaN(yPos) && !double.IsInfinity(yPos))
                {
                    Line line = new Line
                    {
                        X1 = 0,
                        X2 = cvBarChart.Width,
                        Y1 = yPos,
                        Y2 = yPos,
                        Stroke = Brushes.Gray,
                        Opacity = 0.6,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] { 3, 1 })
                    };

                    cvBarChart.Children.Add(line);
                }
            }


            for (int i = 0; i < list.Count; i++)
            {
                int adjustedIndex = (startMonthIndex + i) % 12;
                double barHeight = list.Values.ElementAt(adjustedIndex) * barHeightScale;

                CustomRectangle customRect = new CustomRectangle(barWidth, barHeight, adjustedIndex == currentMonthIndex, adjustedIndex);
                customRect.TextBlock.Text = list.Keys.ElementAt(adjustedIndex);
                customRect.Rect.Tag = list.Values.ElementAt(adjustedIndex);
                customRect.SetPosition(i * barWidth + 15, 0, barHeight);

                cvBarChart.Children.Add(customRect.Rect);
                cvBarChart.Children.Add(customRect.TextBlock);
                
                if(list.Keys.ElementAt(adjustedIndex) == "Dec")
                {
                    Line line = new Line
                    {
                        X1 = 2 + barWidth * (i+1),
                        X2 = 2 + barWidth * (i+1),
                        Y1 = -10,
                        Y2 = cvBarChart.Height,
                        Stroke = Brushes.Gray,
                        Opacity = 0.6,
                        StrokeThickness = 1,
                    };
                    int year = DateTime.Now.Year;
                    TextBox TblYear = new TextBox
                    {
                        Text = year - 1 + " " + year,
                        IsReadOnly = true,
                        BorderThickness = new Thickness(0),
                        Background = Brushes.Transparent,
                        Foreground = Brushes.Gray,
                        FontSize = 10,
                        FontWeight = FontWeights.Bold,
                        Width = 120,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                    };


                    Canvas.SetLeft(TblYear, ((barWidth) * (i+1)) - TblYear.Width/2);
                    Canvas.SetTop(TblYear, 0 - 25);
                    cvBarChart.Children.Add(TblYear);
                    cvBarChart.Children.Add(line);
                }

            }




        }
        private void DrawPieChart()
        {
            
            lblPieChartTitle.Content = "Monthly Revenue: ";
            
            lblPercent.Content = "100%";
            
            var dictionary = AppState.ThisMonthsChart.ToDictionary(x => x.Key, x => x.Value);
            lblMonth.Content = monthAbbreviations[pieMonth-1] + ", " + pieYear;

            this.total = dictionary.Values.Sum();
            lblPieChart.Content = Scaler.NumberShorthand((total - dictionary.Values.Last()));
            lblPieChart.Tag = (total - dictionary.Values.Last()).ToString("0.00");
            double currentAngle = -90;

            double centerX = cvPieCircle.Center.X;
            double centerY = cvPieCircle.Center.Y;
            double radius = cvPieCircle.RadiusX;

            cvPieChart.Children.Clear();
            cvPieChart.Children.Add(new Path { Fill = Brushes.LightGray, Data = new EllipseGeometry { Center = new Point(centerX, centerY), RadiusX = radius, RadiusY = radius } });

            for (int i = 0; i < dictionary.Count; i++)
            {
                double sweepAngle = (dictionary.Values.ElementAt(i) / total) * 359.99;
                
                Path path = new Path
                {
                    Fill = RandomColor(i),
                    Opacity = 1,
                    Tag = dictionary.Keys.ElementAt(i),
                    Stroke = Brushes.Black,
                    StrokeThickness = 0,
                    Data = new PathGeometry
                    {
                        Figures = new PathFigureCollection
                        {
                            new PathFigure
                            {
                                StartPoint = new Point(centerX, centerY),
                                Segments = new PathSegmentCollection
                                {
                                    new LineSegment
                                    {
                                        Point = new Point(centerX + radius * Math.Cos(currentAngle * Math.PI / 180), centerY + radius * Math.Sin(currentAngle * Math.PI / 180))
                                    },
                                    new ArcSegment
                                    {
                                        Point = new Point(centerX + radius * Math.Cos((currentAngle + sweepAngle) * Math.PI / 180), centerY + radius * Math.Sin((currentAngle + sweepAngle) * Math.PI / 180)),
                                        Size = new Size(radius, radius),
                                        IsLargeArc = sweepAngle > 180,
                                        SweepDirection = SweepDirection.Clockwise
                                    },
                                    new LineSegment
                                    {
                                        Point = new Point(centerX, centerY)
                                    }
                                }
                            }
                        }
                    }
                };

                //EventLogger.Post($"OUT :: {path.Data.ToString()}{path.Fill}");

                path.MouseEnter += Path_MouseEnter;
                path.MouseLeave += Path_MouseLeave;
                path.MouseDown += Path_MouseDown;

                cvPieChart.Children.Add(path);

                currentAngle += sweepAngle;
            }
            lblPercent.Content = (total == 0)? "0%":"100%";
        }
        private void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!pieClicked)
            {
                pieClicked = true;
                clickedPie = sender as Path;
            } else
            {
                pieClicked = false;
                if(sender as Path == clickedPie)
                {
                    clickedPie.StrokeThickness = 0;
                    clickedPie = null;
                } else
                {
                    clickedPie.StrokeThickness = 0;
                    clickedPie = null;
                    pieClicked = true;
                    clickedPie = sender as Path;
                }
            }
        }
        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Path path)
            {
                if (!pieClicked)
                {
                    lblPieChartTitle.Content = "Monthly Revenue";
                    double tag = 0;
                    try
                    {
                        tag = Double.Parse(lblPieChart.Tag.ToString());
                    }
                    catch { }
                    lblPieChart.Content = Scaler.NumberShorthand(tag);
                    lblPercent.Content = (total == 0) ? "0%" : "100%";
                }
                path.StrokeThickness = 0;
                if(clickedPie != null)
                {
                    clickedPie.StrokeThickness = 1;
                }
                
            }
        }
        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            
            if (sender is Path path)
            {
                string tag = path.Tag?.ToString() ?? "No Data";
                lblPieChartTitle.Content = tag;
                double val = AppState.ThisMonthsChart.ToDictionary(x => x.Key, x => x.Value).TryGetValue(tag, out var value)? value: 0;
                lblPieChart.Content = Scaler.NumberShorthand(val);
                lblPercent.Content = Math.Round(((val / AppState.ThisMonthsChart.ToDictionary(x => x.Key, x => x.Value).Values.Sum()) * 100)) + "%";
                path.StrokeThickness = 1;
            }
        }

        private Brush RandomColor(int i)
        {
            List<Brush> brushes = new List<Brush> { Brushes.Cyan, Brushes.Green, Brushes.Yellow, Brushes.Red };
            return brushes[i%brushes.Count];
        }
        public class CustomRectangle
        {
            public Rectangle Rect { get; private set; }
            public TextBlock TextBlock { get; private set; }
            private TextBox hoverTextBox;

            public CustomRectangle(double barWidth, double barHeight, bool isCurrentMonth, int adjustedIndex)
            {
                Rect = new Rectangle
                {
                    Width = barWidth - 15,
                    Height = barHeight,
                    Fill = isCurrentMonth ? Brushes.Red : Brushes.Blue,
                    Opacity = (adjustedIndex % 3 == 0) ? 0.8 : 0.4,
                    RadiusX = 2,
                    RadiusY = 2,
                };

                TextBlock = new TextBlock
                {
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth - 5
                };

                Rect.MouseEnter += Rectangle_MouseEnter;
                Rect.MouseLeave += Rectangle_MouseLeave;

                hoverTextBox = new TextBox
                {
                    IsReadOnly = true,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    Foreground = Brushes.Black,
                    FontSize = 12,
                    Width = 200,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                };
            }

            public void SetPosition(double left, double bottom, double top)
            {
                Canvas.SetLeft(Rect, left);
                Canvas.SetBottom(Rect, bottom);

                Canvas.SetLeft(TextBlock, left);
                Canvas.SetBottom(TextBlock, bottom - 16);

                Canvas.SetLeft(hoverTextBox, left - 100 + (Rect.Width/2));
                Canvas.SetBottom(hoverTextBox, top);

            }

            private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
            {
                Rectangle rect = (Rectangle)sender;
                string info = Rect.Tag?.ToString() ?? "";
                hoverTextBox.Text = info;
                ((Canvas)rect.Parent).Children.Add(hoverTextBox);
            }

            private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
            {
                Rectangle rect = (Rectangle)sender;
                ((Canvas)rect.Parent).Children.Remove(hoverTextBox);
            }
        }
        private async void btnReload_Click(object sender, RoutedEventArgs e)
        {
            await AppState.LoadDatabase();
            DrawBarChart();
            DrawPieChart();
            lblPieChartTitle.Content = "Monthly Revenue";
            double totalSum = AppState.ThisMonthsChart.ToDictionary(x => x.Key, x => x.Value).Values.Sum();
            lblPieChart.Content = Scaler.NumberShorthand(totalSum);
            lblPercent.Content = totalSum == 0? "0%":"100%";
        }
        private async void btnPieBackward_Click(object sender, RoutedEventArgs e)
        {
            if (pieMonth == 1)
            {
                pieMonth = 12;
                pieYear--;
            }
            else
            {
                pieMonth--;
            }

            await UpdateLFContent(pieMonth, pieYear);
        }
        private async void btnPieForward_Click(object sender, RoutedEventArgs e)
        {
            if (pieMonth >= 12)
            {
                pieMonth = 1;
                pieYear++;
            }
            else
            {
                pieMonth++;
            }

            await UpdateLFContent(pieMonth, pieYear);
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            (new PrintPreview()).Show();
        }

        private void btnViewFullDetails_Click(object sender, RoutedEventArgs e)
        {
            (new Recapitulations()).Show();
        }

        private void btnActions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            menuExpander(menuExpanded.IsChecked == true);
        }

        private void actionMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            if(menuExpanded.IsChecked == true)
            {
                menuExpander(true);
            }
        }

        private void menuExpander(bool isExpanded)
        {
            if (isExpanded)
            {
                actionMenu.AnimateHeight(40, 0.2);
                epektos.IsEnabled = false;
            }
            else
            {
                actionMenu.AnimateHeight(aMenuFinalHeight, 0.2);
                epektos.IsEnabled = true;
            }
            menuExpanded.IsChecked = !isExpanded;
        }

        private void btnAddViolationType_Click(object sender, RoutedEventArgs e)
        {
            (new ViolationTypeList()).Show();
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            gridReportWindow.FadeIn(0.3);
        }

        private void btnGRClose_Click(object sender, RoutedEventArgs e)
        {
            gridReportWindow.FadeOut(0.3);
        }

        private string msg = "";
        private string qg = "";
        private string currBtn = "";
        private void generateReports(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                ListReport report = null;
                List<ColumnConfiguration> columns = null;

                string q = "";
                switch (cbCat.SelectedIndex)
                {
                    case 0: // Active loans (loans sa system na di pa tapos bayaran)
                        if (btn.Name.Equals("btnRepShort"))
                        {
                            q = "Short-Term";

                            report = new ListReport(ListReport.ACTIVE_SHORT);

                            /**
                             * Table: short term loans(active/di pa bayad)
                             * operator name, body#, date, cv/or, termlength, loan amount, loan balance
                             * **/
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("last_name", "OPERATOR LASTNAME", minWidth : 80),
                                new ColumnConfiguration("first_name", "OPERATOR FIRSTNAME", minWidth : 80),
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("terms_of_payment_month", "TERM", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("amount", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true)
                            };
                        }
                        else if (btn.Name.Equals("btnRepLong"))
                        {
                            q = "Long-Term";

                            report = new ListReport(ListReport.ACTIVE_LONG);

                            /**
                             * Table: long term loans(active/di pa bayad)
                             * operator name, body#, date, cv/or, termlength, loan amount, loan balance
                             * **/
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("last_name", "OPERATOR LASTNAME", minWidth : 80),
                                new ColumnConfiguration("first_name", "OPERATOR FIRSTNAME", minWidth : 80),
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("terms_of_payment_month", "TERM", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("amount", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true)
                            };
                        }
                        else if (btn.Name.Equals("btnRepEmer"))
                        {
                            q = "Emergency";

                            report = new ListReport(ListReport.ACTIVE_EMERGENCY);

                            /**
                             * Table: emergency term loans(active/di pa bayad).
                             * operator name, body#, date, cv/or, termlength, loan amount, loan balance
                             * **/
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("last_name", "OPERATOR LASTNAME", minWidth : 80),
                                new ColumnConfiguration("first_name", "OPERATOR FIRSTNAME", minWidth : 80),
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("terms_of_payment_month", "TERM", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("amount", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true)
                            };
                        }
                        // global yung msg variable, pwede irekta sa printing function.
                        msg = $"Are you sure you want to print Active Loans for {q} loans";

                        if (ControlWindow.ShowTwoway("Confirm Printing", msg, Icons.NOTIFY))
                        {
                            // printing confirmed, call printing function.
                            report.StartPrint($"active loans {q}", $"Active Loan Report for {q} Loans", columns);
                        }
                        break;

                    // Jump to btnPrintReport_Click for table configs
                    case 1:
                        currBtn = btn.Name;
                        lblTitleReport.Content = "Loan Record Report";
                        if (currBtn.Equals("btnRepShort"))
                        {
                            lblSubTitleReport.Content = "For Short-Term Loans";
                            qg = "Short-Term";
                        }
                        else if (currBtn.Equals("btnRepLong"))
                        {
                            lblSubTitleReport.Content = "For Long-Term Loans";
                            qg = "Long-Term";
                        }
                        else if (currBtn.Equals("btnRepEmer"))
                        {
                            lblSubTitleReport.Content = "For Emergency Loans";
                            qg = "Emergency";
                        }
                        msg = $"Are you sure you want to print {qg} loan records?";
                        gridRepWinChild.FadeIn(0.3);
                        break;

                    case 2:
                        currBtn = btn.Name;
                        lblTitleReport.Content = "Payment Report";
                        if (currBtn.Equals("btnRepShort"))
                        {
                            lblSubTitleReport.Content = "For Short-Term Loans";
                            qg = "Short-Term";
                        }
                        else if (currBtn.Equals("btnRepLong"))
                        {
                            lblSubTitleReport.Content = "For Long-Term Loans";
                            qg = "Long-Term";
                        }
                        else if (currBtn.Equals("btnRepEmer"))
                        {
                            lblSubTitleReport.Content = "For Emergency Loans";
                            qg = "Emergency";
                        }
                        else if (currBtn.Equals("btnRepShareCap"))
                        {
                            lblSubTitleReport.Content = "For Share Capital";
                            qg = "Share Capital";
                        }
                        msg = $"Are you sure you want to print {qg} payments";
                        gridRepWinChild.FadeIn(0.2);
                        break;

                    case 3:
                        currBtn = btn.Name;
                        lblTitleReport.Content = "List Report";
                        if (btn.Name.Equals("btnRepFran"))
                        {
                            lblSubTitleReport.Content = "For List of Franchise";
                            qg = "Franchise";
                        }
                        else if (btn.Name.Equals("btnRepOper"))
                        {                            
                            lblSubTitleReport.Content = "For List of Operators";
                            qg = "Operator";
                        }
                        else if (btn.Name.Equals("btnRepDriv"))
                        {                            
                            lblSubTitleReport.Content = "For List of Drivers";
                            qg = "Driver";

                        }
                        else if (btn.Name.Equals("btnRepViol"))
                        {                            
                            lblSubTitleReport.Content = "For List of Violation";
                            qg = "Violation";
                        }
                        else if (btn.Name.Equals("btnRepIDHi"))
                        {
                            lblSubTitleReport.Content = "For History of ID";
                            qg = "ID History";
                        }

                        // global yung msg variable, pwede irekta sa printing function.
                        msg = $"Are you sure you want to print Records for {qg}";
                        gridRepWinChild.FadeIn(0.2);
                        break;
                }
            }
        }

        private void btnPrintRepCancel_Click(object sender, RoutedEventArgs e)
        {
            gridRepWinChild.FadeOut(0.2);
        }

        private void btnPrintReport_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                #region
                /**
                 * Used for notifications, ignore.
                 * **/
                string m = dpDate.SelectedDate?.ToString("MMMM");
                string y = dpDate.SelectedDate?.ToString("yyyy");
                string q = printAll.IsChecked == true ? " from database?" : $" for {m} {y}?";
                string typ = "";
                string desc = "";
                #endregion

                int? mo = dpDate.SelectedDate?.Month;
                int? ye = dpDate.SelectedDate?.Year;

                ListReport report = null;
                List<ColumnConfiguration> columns = null;

                /**
                 * Get the date month and year sa dpDate for filtering.
                 * Month and Year lang naseset sa control na to.
                 * Preferrably, month and year lang kukunin.
                 * 
                 * Kapag checked yung printAll, hindi na fifilterin by date kukunin na lahat.
                 * **/

                switch (cbCat.SelectedIndex)
                {
                    case 1: //All Loans
                        typ = "loans";
                        desc = "Loan Report for ";
                        if (currBtn.Equals("btnRepShort"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.LOANS_SHORT(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.LOANS_SHORT_ALL);

                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("amount", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("last_payment_date", "LAST PAYMENT", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("isFullyPaid", "PAID", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                            };
                        }
                        else if (currBtn.Equals("btnRepLong"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.LOANS_LONG(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.LOANS_LONG_ALL);

                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("amount", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("last_payment_date", "LAST PAYMENT", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("isFullyPaid", "PAID", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                            };
                        }
                        else if (currBtn.Equals("btnRepEmer"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.LOANS_EMERGENCY(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.LOANS_EMERGENCY_ALL);

                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("amount", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("last_payment_date", "LAST PAYMENT", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("isFullyPaid", "PAID", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                            };
                        }
                        break;

                    case 2: // Payments
                        typ = "payments";
                        desc = "Payment Reports for ";
                        if (currBtn.Equals("btnRepShort"))
                        {
                            report = new ListReport(!(printAll.IsChecked??false) ? ListReport.PAYMENT_SHORT(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.PAYMENT_SHORT_ALL);

                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("balance", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("last_payment_date", "LAST PAYMENT", minWidth: 50, isNumeric: true, isCenter:true, isDate:true)
                            };
                        }
                        else if (currBtn.Equals("btnRepLong"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.PAYMENT_LONG(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.PAYMENT_LONG_ALL);
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("balance", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("last_payment_date", "LAST PAYMENT", minWidth: 50, isNumeric: true, isCenter:true, isDate:true)
                            };
                        }
                        else if (currBtn.Equals("btnRepEmer"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.PAYMENT_EMERGENCY(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.PAYMENT_EMERGENCY_ALL);
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("amount_loaned", "AMOUNT LOANED", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("balance", "LOAN BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("last_payment_date", "LAST PAYMENT", minWidth: 50, isNumeric: true, isCenter:true, isDate:true)
                            };
                        }
                        else if (currBtn.Equals("btnRepShareCap"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.PAYMENT_SHARECAPITAL(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.PAYMENT_SHARECAPITAL_ALL);
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isNumeric: true, isCenter:true, isDate:true),
                                new ColumnConfiguration("reference_no", "CV/OR", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("deposit", "DEPOSIT", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("balance", "BALANCE", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true)
                            };
                        }
                        break;

                    case 3:
                        typ = "list";
                        desc = "List Reports for ";
                        if (currBtn.Equals("btnRepFran"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.LIST_FRANCHISE(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.LIST_FRANCHISE_ALL);
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("last_name", "LASTNAME", minWidth : 80),
                                new ColumnConfiguration("first_name", "FIRSTNAME", minWidth : 80),
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("last_balance", "SHARE CAPITAL", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("mtop_no", "MTOP ", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("license_no", "PLATE NUMBER", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true)
                            };
                        }
                        else if (currBtn.Equals("btnRepOper"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.LIST_OPERATOR(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.LIST_OPERATOR_ALL);
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("last_name", "LASTNAME", minWidth : 80),
                                new ColumnConfiguration("first_name", "FIRSTNAME", minWidth : 80),
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("tin_number1", "TIN NUMBER", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("voters_id_number1", "VOTERS NUMBER", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("date_of_birth", "BIRTHDAY", minWidth: 50, isNumeric: true, isCenter:true, isDate:true)
                            };
                        }
                        else if (currBtn.Equals("btnRepDriv"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.LIST_DRIVER(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.LIST_DRIVER_ALL);
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("last_name", "LASTNAME", minWidth : 80),
                                new ColumnConfiguration("first_name", "FIRSTNAME", minWidth : 80),
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("license_no", "TIN NUMBER", minWidth: 50, isNumeric: true, isCenter:true, haspeso:true),
                                new ColumnConfiguration("date_of_birth", "BIRTHDAY", minWidth: 50, isNumeric: true, isCenter:true, isDate:true)
                            };
                        }
                        else if (currBtn.Equals("btnRepViol"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.LIST_VIOLATION(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.LIST_VIOLATION_ALL);
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("last_name", "LASTNAME", minWidth : 80),
                                new ColumnConfiguration("first_name", "FIRSTNAME", minWidth : 80),
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("title", "VIOLATION", minWidth: 50, isCenter:true),
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isCenter:true, isDate:true),
                                new ColumnConfiguration("suspension_start", "FROM:", minWidth: 50, isCenter:true, isDate:true),
                                new ColumnConfiguration("suspension_end", "TO:", minWidth: 50, isCenter:true, isDate:true),
                                new ColumnConfiguration("remarks", "REMARKS", minWidth: 50, isCenter:true),
                            };
                        }
                        else if (currBtn.Equals("btnRepIDHi"))
                        {
                            report = new ListReport(!(printAll.IsChecked ?? false) ? ListReport.LIST_IDHISTORY(mo ?? DateTime.Now.Month, ye ?? DateTime.Now.Year) : ListReport.LIST_IDHISTORY_ALL);
                            columns = new List<ColumnConfiguration>
                            {
                                new ColumnConfiguration("entity_type", "ID TYPE", minWidth : 80),
                                new ColumnConfiguration("last_name", "LASTNAME", minWidth : 80),
                                new ColumnConfiguration("first_name", "FIRSTNAME", minWidth : 80),
                                new ColumnConfiguration("body_number", "BODY NO.", minWidth: 50, isNumeric: true, isCenter:true),
                                new ColumnConfiguration("is_printed", "PRINTED", minWidth: 50, isCenter:true), // YES NO? //Checkbox
                                new ColumnConfiguration("date", "DATE", minWidth: 50, isCenter:true, isDate:true)
                            };
                        }
                        break;
                }


                if (ControlWindow.ShowTwoway("Confirm Printing", msg + q, Icons.NOTIFY))
                {
                    if(report == null)
                    {
                        ControlWindow.ShowStatic("Error", "An error has occured\nPrinting failed...", Icons.ERROR);
                        gridRepWinChild.FadeOut(0.2);
                    }
                    // printing confirmed, call printing function.
                    report.StartPrint($"{typ} {qg}", $"{desc} {qg} Loans", columns);

                    gridRepWinChild.FadeOut(0.2);
                }
            }
        }

        private void printAll_Click(object sender, RoutedEventArgs e)
        {
            if(printAll.IsChecked == true)
            {
                dpDate.IsEnabled = false;
                datesel.Opacity = 0.5;
            }
            else
            {
                dpDate.IsEnabled = true;
                datesel.Opacity = 1;
            }
        }

        private void cbCat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spList != null && spALPa != null)
            {
                switch (cbCat.SelectedIndex)
                {
                    case 0:
                    case 1:
                        spList.Visibility = Visibility.Collapsed;
                        spALPa.Visibility = Visibility.Visible;
                        btnRepShareCap.Visibility = Visibility.Collapsed;
                        break;

                    case 2:
                        spList.Visibility = Visibility.Collapsed;
                        spALPa.Visibility = Visibility.Visible;
                        btnRepShareCap.Visibility = Visibility.Visible;
                        break;

                    case 3:
                        spALPa.Visibility = Visibility.Collapsed;
                        spList.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void gridReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnRepIDHi.Visibility = Visibility.Visible;
            btnRepViol.Visibility = Visibility.Visible;
            if (isSecret)
            {
                //Secretary can only generate lists.
                cbCat.IsEnabled = false;
                cbCat.SelectedIndex = 3;
                cbCat.Opacity = 0.5;
            }
            else if (isTresur)
            {
                //Tresurer cannot generate lists.
                cbCat.Items.RemoveAt(3);
            }
            else if (isBookke)
            {
                //Bookkeeper cannot generate ID and Violation History.
                btnRepIDHi.Visibility = Visibility.Collapsed;
                btnRepViol.Visibility = Visibility.Collapsed;
            }
            else
            {
                /**
                 * Ewan ko, optional lang ata ito kasi pag nagpalit ng user, marerestart din naman diba?
                 * for safety purposes ahahaha.
                 * 
                 * **/

                //both general manager and secretary can generate all reports.
                if (cbCat.Items.Count < 4)
                    cbCat.Items.Add("List of Records");
                
                cbCat.IsEnabled = true;
                cbCat.SelectedIndex = 0;
                cbCat.Opacity = 1;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (isBookke || isTresur)
            {
                btnPrint.Visibility = Visibility.Collapsed;
                btnAddViolationType.Visibility = Visibility.Collapsed;
                aMenuFinalHeight = 211.2;
            }
            else if (isSecret)
            {
                btnViewFullDetails.Visibility = Visibility.Collapsed;
                aMenuFinalHeight = 265.6;
            }
            else
            {
                btnViewFullDetails.Visibility = Visibility.Visible;
                btnPrint.Visibility = Visibility.Visible;
                btnAddViolationType.Visibility = Visibility.Visible;
                aMenuFinalHeight = 310.4;
            }
        }
    }
}
