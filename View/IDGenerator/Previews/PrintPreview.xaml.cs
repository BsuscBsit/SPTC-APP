﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SPTC_APP.Objects;
using SPTC_APP.View.IDGenerator.Hidden;
using SPTC_APP.View.Styling;
using Image = System.Windows.Controls.Image;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        private bool isFront;
        private static ID mGrid1 = null;
        private static ID mGrid2 = null;
        private static ID mGrid3 = null;
        private static ID mGrid4 = null;
        private static int idcount = 0;

        //Zoom Variables
        private double zoomScale = 1.0;
        private Point panOrigin;
        private bool isPanning = false;
        private bool isReset = true;
        private int offsetAfterSpace = 50;

        private ID zoomedIn = null;

        public PrintPreview()
        {
            InitializeComponent();
            AppState.mainwindow?.Hide();
            isFront = true;
            RenderIDs();
            checkIdCount();
            if(idcount > 4)
            {
                btnAddNew.IsEnabled = false;
            } else
            {
                btnAddNew.IsEnabled = true;
            }

            //Zoom Constructors
            InitializePanning();
            zoomSlider.Value = zoomScale;
            UpdateZoomText();
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
        }

        //ROUTED XAML EVENTS
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CenterOffset();
            AppState.WindowsCounter(true, sender);
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            int idnotsavedcount = 0;
            int idnotprintedcunt = 0;
            foreach (ID id in new ID[] { PrintPreview.mGrid1, PrintPreview.mGrid2, PrintPreview.mGrid3, PrintPreview.mGrid4 })
            {
                if (id != null)
                {
                    if (!id.isSaved)
                    {
                        idnotsavedcount = idnotsavedcount + 1;
                    }
                    if (id.FrontPrint == 0 || id.BackPrint == 0)
                    {
                        idnotprintedcunt = idnotprintedcunt + 1;
                    }
                }
            }

            if (idnotprintedcunt > 0)
            {
                if (ControlWindow.ShowDialog("Confirm Exit?", $"{numtext(idnotsavedcount)} ID{(idnotprintedcunt > 1 ? "s were" : " was")} not printed!", Icons.NOTIFY))
                {
                    ResetPrintData();
                    (new PrintPreview()).Show();
                    this.Close();
                    return;
                }
                else
                {
                    return;
                }
            }

            if (idnotsavedcount > 0)
            {
                if (ControlWindow.ShowDialog("Confirm Exit?", $"{numtext(idnotsavedcount)} ID{(idnotsavedcount > 1 ? "s were" : " was")} not saved!", Icons.NOTIFY))
                {
                    ResetPrintData();
                    (new PrintPreview()).Show();
                    this.Close();
                    return;
                }
                else
                {
                    return;
                }
            }



            ResetPrintData();
            (new PrintPreview()).Show();
            this.Close();
        }
        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (idcount < 4)
            {
                (new GenerateID()).Show();
                this.Close();
            }
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

            if (idcount >= 1)
            {
                if (isFront)
                {
                    PrintPaper printpaper = new PrintPaper();
                    if (printpaper.StartPrint(new ID[] { mGrid1, mGrid2, mGrid3, mGrid4 }, true))
                    {
                        EventLogger.Post($"OUT :: Print Front page");
                    }
                    else
                    {
                        EventLogger.Post($"OUT :: Print Front page Failed");
                    }
                }
                else
                {
                    PrintPaper printpaper = new PrintPaper();
                    if (printpaper.StartPrint(new ID[] { mGrid2, mGrid1, mGrid4, mGrid3 }, false))
                    {
                        EventLogger.Post($"OUT :: Print Back page");
                    }
                    else
                    {
                        EventLogger.Post($"OUT :: Print Back page Failed");
                    }
                }
            }

            checkIdCount();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            AppState.mainwindow?.Show();
            //App.Current.Shutdown();
        }
        private void btnPage1_Click(object sender, RoutedEventArgs e)
        {
            lblPageNum.Content = "Page 1 of 2";
            btnPage1.IsEnabled = false;
            btnPage2.IsEnabled = true;
            g1Border.BorderBrush = Brushes.Black;
            g2Border.BorderBrush = Brushes.Black;
            g3Border.BorderBrush = Brushes.Black;
            g4Border.BorderBrush = Brushes.Black;

            // Switch is in the False state
            btnPrintCurrent.Content = "PRINT PAGE 1";
            isFront = true;
            RenderIDs();
        }
        private void btnPage2_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33000000"));
            lblPageNum.Content = "Page 2 of 2";
            btnPage1.IsEnabled = true;
            btnPage2.IsEnabled = false;
            g1Border.BorderBrush = brush;
            g2Border.BorderBrush = brush;
            g3Border.BorderBrush = brush;
            g4Border.BorderBrush = brush;

            // Switch is in the True state
            btnPrintCurrent.Content = "PRINT PAGE 2";
            isFront = false;
            RenderIDs();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveAndClearID();
        }
        private void btnPrintBoth_Click(object sender, RoutedEventArgs e)
        {
            if (idcount >= 1)
            {
                PrintPaper printpaper = new PrintPaper();
                if (printpaper.StartPrint(new ID[] { mGrid1, mGrid2, mGrid3, mGrid4 }, true))
                {
                    EventLogger.Post($"OUT :: Print Front page");
                    if (printpaper.StartPrint(new ID[] { mGrid2, mGrid1, mGrid4, mGrid3 }, false))
                    {
                        EventLogger.Post($"OUT :: Print Back page");
                        checkIdCount();
                    }
                    else
                    {
                        EventLogger.Post($"OUT :: Print Back page FAILED");
                    }

                }
                else
                {
                    EventLogger.Post($"OUT :: Print Front and Back page FAILED");
                }

                printpaper.Show();
                printpaper.Close();
                RenderIDs();
            }
        }
        private void btnSaveAnID_Click(object sender, RoutedEventArgs e)
        {
            if (zoomedIn != null)
            {
                zoomedIn.SaveInfo();
                EventLogger.Post("OUT :: ID : " + zoomedIn.franchise.BodyNumber + " FRONT: " + zoomedIn.FrontPrint + " BACK: " + zoomedIn.BackPrint);
            }
        }



        //ID EVENTS
        private void RenderIDs()
        {
            if (isFront)
            {
                if (mGrid1 != null)
                {
                    grid1.Children.Clear();
                    grid1.Children.Add(mGrid1.RenderFrontID());
                }
                else
                {
                    grid1.Children.Clear();
                    grid1.Children.Add(CreateNoIDImage());
                }

                if (mGrid2 != null)
                {
                    grid2.Children.Clear();
                    grid2.Children.Add(mGrid2.RenderFrontID());
                }
                else
                {
                    grid2.Children.Clear();
                    grid2.Children.Add(CreateNoIDImage());
                }

                if (mGrid3 != null)
                {
                    grid3.Children.Clear();
                    grid3.Children.Add(mGrid3.RenderFrontID());
                }
                else
                {
                    grid3.Children.Clear();
                    grid3.Children.Add(CreateNoIDImage());
                }

                if (mGrid4 != null)
                {
                    grid4.Children.Clear();
                    grid4.Children.Add(mGrid4.RenderFrontID());
                }
                else
                {
                    grid4.Children.Clear();
                    grid4.Children.Add(CreateNoIDImage());
                }
            }
            else
            {
                if (mGrid2 != null)
                {
                    grid1.Children.Clear();
                    grid1.Children.Add(mGrid2.RenderBackID());
                }
                else
                {
                    grid1.Children.Clear();
                    grid1.Children.Add(CreateNoIDImage());
                }

                if (mGrid1 != null)
                {
                    grid2.Children.Clear();
                    grid2.Children.Add(mGrid1.RenderBackID());
                }
                else
                {
                    grid2.Children.Clear();
                    grid2.Children.Add(CreateNoIDImage());
                }

                if (mGrid4 != null)
                {
                    grid3.Children.Clear();
                    grid3.Children.Add(mGrid4.RenderBackID());
                }
                else
                {
                    grid3.Children.Clear();
                    grid3.Children.Add(CreateNoIDImage());
                }

                if (mGrid3 != null)
                {
                    grid4.Children.Clear();
                    grid4.Children.Add(mGrid3.RenderBackID());
                }
                else
                {
                    grid4.Children.Clear();
                    grid4.Children.Add(CreateNoIDImage());
                }
            }
        }
        private System.Windows.Controls.Image CreateNoIDImage()
        {
            return new System.Windows.Controls.Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                Source = new BitmapImage(new Uri("/View/Images/no_id.png", UriKind.Relative)),
                Tag = "NullImage",
                Opacity = 0.2
            };
        }
        public void NewID(ID id)
        {

            switch (++PrintPreview.idcount)
            {
                case 1:
                    PrintPreview.mGrid1 = id;
                    break;
                case 2:
                    PrintPreview.mGrid2 = id;
                    break;
                case 3:
                    PrintPreview.mGrid3 = id;
                    break;
                case 4:
                    PrintPreview.mGrid4 = id;
                    break;
                default:
                    break;
            }
            RenderIDs();
            checkIdCount();
        }


        //HELPER FUNCTIONS
        private string numtext(int i)
        {
            switch(i)
            {
                case 1: return "One";
                case 2: return "Two";
                case 3: return "Three";
                case 4: return "Four";
                default: return "No";
            }
        }
        private void SaveAndClearID()
        {
            //Save to database and clear print paper
            PrintPreview.mGrid1?.SaveInfo();
            PrintPreview.mGrid2?.SaveInfo();
            PrintPreview.mGrid3?.SaveInfo();
            PrintPreview.mGrid4?.SaveInfo();

            foreach (ID id in new ID[] { PrintPreview.mGrid1, PrintPreview.mGrid2, PrintPreview.mGrid3, PrintPreview.mGrid4 }) {
                if(id != null)
                {
                    EventLogger.Post("OUT :: ID : " + id.franchise.BodyNumber + " FRONT: " + id.FrontPrint  + " BACK: " +  id.BackPrint);

                }
            }
            ResetPrintData();
            checkIdCount();
        }
        private void checkIdCount()
        {
            if (idcount == 0)
            {
                Grid0Content.Visibility = Visibility.Visible;
                btnPrint.Visibility = Visibility.Hidden;
            }
            else
            {
                Grid0Content.Visibility = Visibility.Collapsed;
                btnPrint.Visibility = Visibility.Visible;
                if (mGrid1 != null) {
                    if (mGrid1.FrontPrint >= 1 && mGrid1.BackPrint >= 1)
                    {
                        btnClear.IsEnabled = true;
                        btnClear.Visibility = Visibility.Visible;
                    }
                } else
                {
                    btnClear.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void ResetPrintData()
        {
            PrintPreview.mGrid1 = null;
            PrintPreview.mGrid2 = null;
            PrintPreview.mGrid3 = null;
            PrintPreview.mGrid4 = null;
            PrintPreview.idcount = 0;
        }
        private void ZoomToImage(Image image)
        {
            if (zoomScale == 1)
            {
                int tho = 0;
                int tvo = 0;
                bool pan = false;
                switch (((Grid)image.Parent).Name)
                {
                    case "grid1":
                        tho = 85;
                        tvo = 90;
                        pan = (image.Tag?.ToString() != "NullImage");
                        zoomedIn = (isFront) ? mGrid1 : mGrid2;
                        break;
                    case "grid2":
                        tho = 420;
                        tvo = 90;
                        pan = (image.Tag?.ToString() != "NullImage");
                        zoomedIn = (!isFront) ? mGrid1 : mGrid2;
                        break;
                    case "grid3":
                        tho = 85;
                        tvo = 545;
                        pan = (image.Tag?.ToString() != "NullImage");
                        zoomedIn = (isFront) ? mGrid3 : mGrid3;
                        break;
                    case "grid4":
                        tho = 420;
                        tvo = 545;
                        pan = (image.Tag?.ToString() != "NullImage");
                        zoomedIn = (!isFront) ? mGrid3 : mGrid3;
                        break;
                    default:
                        break;
                }
                if (pan)
                {
                    UpdateZoom(2.44);
                    scrollViewer.ScrollToHorizontalOffset(tho + (offsetAfterSpace * 2));
                    scrollViewer.ScrollToVerticalOffset(tvo + (offsetAfterSpace * 2));

                    canvasPageCtrl.FadeOut(1);
                    canvasSaveAnID.FadeIn(1);

                }
                else
                {
                    //ControlWindow.ShowDialog("Note: ", "Pwede rito maglagay ng functionality", Icons.NOTIFY);
                }
            }
        }



        //ZOOM EVENTS
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var contentArea = new Rect(0, 0, scrollViewer.ViewportWidth, scrollViewer.ViewportHeight);

                if (contentArea.Contains(e.GetPosition(scrollViewer)))
                {
                    isPanning = true;
                    panOrigin = e.GetPosition(scrollViewer);
                    Mouse.OverrideCursor = Cursors.Hand;
                    scrollViewer.CaptureMouse();
                }
            }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning && scrollViewer.IsMouseCaptured)
            {
                Point currentPos = e.GetPosition(scrollViewer);
                double deltaX = currentPos.X - panOrigin.X;
                double deltaY = currentPos.Y - panOrigin.Y;

                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - deltaX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - deltaY);

                panOrigin = currentPos;


                canvasPageCtrl.FadeIn(1);
                canvasSaveAnID.FadeOut(1);
            }
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isPanning)
            {
                isPanning = false;
                Mouse.OverrideCursor = Cursors.Arrow;
                scrollViewer.ReleaseMouseCapture();
            }
        }
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoom(zoomScale + 0.1);
        }
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoom(zoomScale - 0.1);
        }
        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            isReset = true;
            if (canvasSaveAnID.Visibility == Visibility.Visible)
            {
                canvasSaveAnID.FadeOut(1);
                canvasPageCtrl.FadeIn(1);
            }
            UpdateZoom(1.0);
            CenterOffset();
        }
        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateZoom(zoomSlider.Value);
        }
        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (zoomScale == 1)
            {
                if (e.OriginalSource is Image image)
                {
                    ZoomToImage(image);
                    e.Handled = true;
                }
            }
        }

        
        
        //ZOOM FUNCTIONS
        private void InitializePanning()
        {
            scrollViewer.PreviewMouseDown += OnMouseDown;
            scrollViewer.PreviewMouseMove += OnMouseMove;
            scrollViewer.PreviewMouseUp += OnMouseUp;
        }
        private void CenterOffset()
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight / 2);
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.ScrollableWidth / 2);
        }
        private void UpdateZoom(double newZoom)
        {
            zoomScale = Math.Max(0.1, newZoom);
            zoomSlider.Value = zoomScale;
            UpdateZoomTransform();
        }
        private void UpdateZoomTransform()
        {
            ScaleTransform scaleTransform = new ScaleTransform(zoomScale, zoomScale);
            zoomableGrid.LayoutTransform = scaleTransform;
            UpdateZoomText();
        }
        private void UpdateZoomText()
        {
            if (zoomTextBlock != null)
            {
                zoomTextBlock.Content = $"Zoom: {Math.Round(zoomScale * 100)}%";
            }
        }
        private void frontPage_Loaded(object sender, RoutedEventArgs e)
        {
            CenterOffset();
        }
        private void frontPage_Loaded(object sender, SizeChangedEventArgs e)
        {
            CenterOffset();
        }
        private void scrollViewer_LayoutUpdated(object sender, EventArgs e)
        {
            if (isReset)
            {
                CenterOffset();
                isReset = false;
            }
        }
        private void scrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (isShiftPressed)
            {
                double horizontalChange = e.Delta;
                double horizontalOffset = scrollViewer.HorizontalOffset - horizontalChange;
                scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
            }
            else if (isCtrlPressed)
            {
                double zoomChange = e.Delta > 0 ? 0.1 : -0.1;
                double newZoom = zoomScale + zoomChange;
                UpdateZoom(newZoom);
            }
            else
            {
                double verticalChange = e.Delta;
                double verticalOffset = scrollViewer.VerticalOffset - verticalChange;
                scrollViewer.ScrollToVerticalOffset(verticalOffset);
            }
            e.Handled = true;
        }
        
    }
}
