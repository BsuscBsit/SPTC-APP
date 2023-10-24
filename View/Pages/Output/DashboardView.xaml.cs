using Google.Protobuf.WellKnownTypes;
using SPTC_APP.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {

        private Path clickedPie;
        private bool pieClicked = false;


        private string[] monthAbbreviations = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

        private int pieMonth;
        private int pieYear;

        public DashboardView()
        {
            InitializeComponent();
            btnPieForward.Visibility = Visibility.Collapsed;
            pieMonth = DateTime.Now.Month;
            pieYear = DateTime.Now.Year;
            tbTotalOperator.Text = Retrieve.GetDataUsingQuery<int>(RequestQuery.GET_TOTAL(Table.OPERATOR)).FirstOrDefault().ToString();
            tbTotalDriver.Text = Retrieve.GetDataUsingQuery<int>(RequestQuery.GET_TOTAL(Table.DRIVER)).FirstOrDefault().ToString();
            tbTotalShares.Text = "P " + Retrieve.GetDataUsingQuery<double>(RequestQuery.GET_TOTAL_SHARES).FirstOrDefault().ToString("0.00");
        }

        private async Task UpdateLFContent(int currentMonth, int currentYear)
        {
            await Task.Run(async() =>
            {
                if (currentMonth == DateTime.Now.Month && currentYear == DateTime.Now.Year)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        btnPieForward.Visibility = Visibility.Collapsed;
                    });

                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        btnPieForward.Visibility = Visibility.Visible;
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
                        StrokeThickness = 2,
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
                        StrokeThickness = 2,
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
            cvPieChart.Children.Clear();
            lblPieChartTitle.Content = "Total Revenue: ";
            lblPieChart.Content = ((double)AppState.ThisMonthsChart.ToDictionary(x => x.Key, x => x.Value).Values.Sum()).ToString("0.00");
            lblPercent.Content = "100%";
            var sortedlist = AppState.ThisMonthsChart.OrderByDescending(x => x.Value).ToList();
            var dictionary = sortedlist.ToDictionary(x => x.Key, x => x.Value);
            lblMonth.Content = monthAbbreviations[pieMonth-1] + ", " + pieYear;

            double total = dictionary.Values.Sum();
            double currentAngle = -90;

            double centerX = cvPieCircle.Center.X;
            double centerY = cvPieCircle.Center.Y;
            double radius = cvPieCircle.RadiusX;


            for (int i = 0; i < dictionary.Count; i++)
            {

                double sweepAngle = (dictionary.Values.ElementAt(i) / total) * 360;

                Path path = new Path
                {
                    Fill = i == 0? Brushes.Red : RandomColor(i),
                    Opacity = 0.5,
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

                path.MouseEnter += Path_MouseEnter;
                path.MouseLeave += Path_MouseLeave;
                path.MouseDown += Path_MouseDown;

                cvPieChart.Children.Add(path);

                currentAngle += sweepAngle;
            }


         

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
                    lblPieChartTitle.Content = "Total Revenue";
                    lblPieChart.Content = ((double) AppState.ThisMonthsChart.ToDictionary(x => x.Key, x => x.Value).Values.Sum()).ToString("0.00");
                    lblPercent.Content = "100%";
                }
                path.StrokeThickness = 0;
                if(clickedPie != null)
                {
                    clickedPie.StrokeThickness = 3.5;
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
                lblPieChart.Content = val.ToString(".00");
                lblPercent.Content = Math.Round(((val / AppState.ThisMonthsChart.ToDictionary(x => x.Key, x => x.Value).Values.Sum()) * 100)) + "%";
                path.StrokeThickness = 3.5;
            }
        }

        private Brush RandomColor(int i)
        {
            List<Brush> brushes = new List<Brush> { Brushes.Cyan, Brushes.Blue, Brushes.DarkBlue };
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
            lblPieChartTitle.Content = "Total Revenue";
            lblPieChart.Content = AppState.ThisMonthsChart.ToDictionary(x => x.Key, x => x.Value).Values.Sum();
            lblPercent.Content = "100%";
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

        }
    }
}
