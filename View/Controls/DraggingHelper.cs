using System;
using System.Windows;
using System.Windows.Input;

namespace SPTC_APP.View.Controls
{
    public static class DraggingHelper
    {
        private static bool isDragging = false;
        private static Point offset;

        public static void DragWindow(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            element.MouseMove += Element_MouseMove;
        }

        public static void DisableDragWindow(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
            element.MouseLeftButtonUp -= Element_MouseLeftButtonUp;
            element.MouseMove -= Element_MouseMove;
        }

        private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            offset = e.GetPosition((UIElement)sender);
            Mouse.Capture((UIElement)sender);
        }

        private static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Mouse.Capture(null);
        }

        private static void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition((UIElement)sender);
                Window window = Window.GetWindow((UIElement)sender);

                if (window != null)
                {
                    window.Left = currentPosition.X - offset.X + window.Left;
                    window.Top = currentPosition.Y - offset.Y + window.Top;
                }
            }
        }
    }
}

