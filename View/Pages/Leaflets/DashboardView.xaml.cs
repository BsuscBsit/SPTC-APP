using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SPTC_APP.View.Pages.Leaflets
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {
        public DashboardView()
        {
            InitializeComponent();
            
        }
        public async Task<Grid> Fetch()
        {
            DrawChart();
            //UpdateTableAsync();
            if (mainpanel.Parent != null)
            {
                Window currentParent = mainpanel.Parent as Window;
                if (currentParent != null)
                {
                    currentParent.Content = null;
                }
            }
            await Task.Delay(50);
            this.Close();
            return mainpanel;
        }

        private void btnGererate_Click(object sender, RoutedEventArgs e)
        {
            (new PrintPreview()).Show();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            (new Test()).Show();
        }

        private void DrawChart()
        {
            cvChart.Children.Clear();

            int currentMonthIndex = DateTime.Now.Month - 1; 
            int startMonthIndex = (currentMonthIndex + 1) % 12;

            double maxBalance = AppState.MonthlyIncome.Values.Max();
            double minBalance = AppState.MonthlyIncome.Values.Min();

            double barWidth = cvChart.Width / AppState.MonthlyIncome.Count;

            double maxMagnitude = Math.Pow(10, Math.Floor(Math.Log10(maxBalance)));

            double roundingAdjustment = maxMagnitude <= 10 ? 1 :
                                       maxMagnitude <= 100 ? 10 :
                                       maxMagnitude <= 1000 ? 100 :
                                       maxMagnitude <= 10000 ? 1000 :
                                       maxMagnitude <= 100000 ? 10000 : 100000;

            double roundedMax = Math.Ceiling(maxBalance / roundingAdjustment) * roundingAdjustment;
            double roundedMin = Math.Ceiling(minBalance / roundingAdjustment) * roundingAdjustment;

            double barHeightScale = cvChart.Height / roundedMax;

            double[] roundedValues = { roundedMin, roundedMax, (roundedMax + roundedMin) / 2 };

            double[] yPositions = new double[3];

            for (int i = 0; i < 3; i++)
            {
                yPositions[i] = cvChart.Height * (1 - (roundedValues[i] / maxBalance));

                TextBlock textBlock = new TextBlock
                {
                    Text = roundedValues[i].ToString(),
                    TextAlignment = TextAlignment.Right,
                    Opacity = 0.6,
                    Width = 60
                };
                Canvas.SetTop(textBlock, yPositions[i] - 8);
                Canvas.SetLeft(textBlock, -65);

                cvChart.Children.Add(textBlock);
            }

            foreach (double yPos in yPositions)
            {
                Line line = new Line
                {
                    X1 = 0,
                    X2 = cvChart.Width,
                    Y1 = yPos,
                    Y2 = yPos,
                    Stroke = Brushes.Gray,
                    Opacity = 0.6,
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection(new double[] { 3, 1 })
                };

                cvChart.Children.Add(line);
            }


            for (int i = 0; i < AppState.MonthlyIncome.Count; i++)
            {
                int adjustedIndex = (startMonthIndex + i) % 12;
                double barHeight = AppState.MonthlyIncome.Values.ElementAt(adjustedIndex) * barHeightScale;

                CustomRectangle customRect = new CustomRectangle(barWidth, barHeight, adjustedIndex == currentMonthIndex, adjustedIndex);
                customRect.TextBlock.Text = AppState.MonthlyIncome.Keys.ElementAt(adjustedIndex);
                customRect.Rect.Tag = AppState.MonthlyIncome.Values.ElementAt(adjustedIndex);
                customRect.SetPosition(i * barWidth + 5, 0, barHeight);

                cvChart.Children.Add(customRect.Rect);
                cvChart.Children.Add(customRect.TextBlock);
                
                if(AppState.MonthlyIncome.Keys.ElementAt(adjustedIndex) == "Dec")
                {
                    Line line = new Line
                    {
                        X1 = 2 + barWidth * (i+1),
                        X2 = 2 + barWidth * (i+1),
                        Y1 = -10,
                        Y2 = cvChart.Height,
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


                    Canvas.SetLeft(TblYear, (barWidth * (i+1)) - TblYear.Width/2);
                    Canvas.SetTop(TblYear, 0 - 25);
                    cvChart.Children.Add(TblYear);
                    cvChart.Children.Add(line);
                }

            }




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
                    Width = barWidth - 5,
                    Height = barHeight,
                    Fill = isCurrentMonth ? Brushes.Red : Brushes.Blue,
                    Opacity = (adjustedIndex % 3 == 0) ? 0.8 : 0.4,
                    RadiusX = 5,
                    RadiusY = 5,
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
                    Background = Brushes.White,
                    Foreground = Brushes.Black,
                    FontSize = 12,
                    Width = Rect.Width,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                };
            }

            public void SetPosition(double left, double bottom, double top)
            {
                Canvas.SetLeft(Rect, left);
                Canvas.SetBottom(Rect, bottom);

                Canvas.SetLeft(TextBlock, left);
                Canvas.SetBottom(TextBlock, bottom - 16);

                Canvas.SetLeft(hoverTextBox, left);
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
            DrawChart();
        }
    }
}
