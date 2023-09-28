/*using System;
using System.Windows;

namespace SPTC_APP.View.Controls
{
    internal class WindowStateHelper
    {
        private bool isMaximized;
        private Rect? actualDimensions;
        private Style roundedStyle;
        private Style normalStyle;

        public WindowStateHelper()
        {
            roundedStyle = Application.Current.Resources["Minified"] as Style;
            normalStyle = Application.Current.Resources["Maximized"] as Style;
        }

        public void MaximizeWindow(bool isFullScreen, bool useDefaultStyle = true, Style style = null)
        {
            isMaximized = true;
            if (isFullScreen)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Rect workingArea = SystemParameters.WorkArea;
                SetWindowProperties(workingArea.Left, workingArea.Top, workingArea.Width, workingArea.Height, useDefaultStyle ? normalStyle : style);
            }
        }

        public void ResizeTo(WindowPosition position, double width, double height, bool useDefaultStyle = true, Style style = null)
        {
            double left = 0, top = 0;
            double xCenter = (SystemParameters.PrimaryScreenWidth - width) / 2;
            double yCenter = (SystemParameters.PrimaryScreenHeight - height) / 2;
            double sWidth = SystemParameters.PrimaryScreenWidth;
            double sHeight = SystemParameters.PrimaryScreenHeight;
            isMaximized = false;
            switch (position)
            {
                case WindowPosition.TOP_LEFT:
                    left = 0;
                    top = 0;
                    break;
                case WindowPosition.TOP_CENTER:
                    left = xCenter;
                    top = 0;
                    break;
                case WindowPosition.TOP_RIGHT:
                    left = sWidth - width;
                    top = 0;
                    break;
                case WindowPosition.CENTER_LEFT:
                    left = 0;
                    top = yCenter;
                    break;
                case WindowPosition.CENTER:
                    left = xCenter;
                    top = yCenter;
                    break;
                case WindowPosition.CENTER_RIGHT:
                    left = sWidth - width;
                    top = yCenter;
                    break;
                case WindowPosition.BOTTOM_LEFT:
                    left = 0;
                    top = sHeight - height;
                    break;
                case WindowPosition.BOTTOM_CENTER:
                    left = xCenter;
                    top = sHeight - height;
                    break;
                case WindowPosition.BOTTOM_RIGHT:
                    left = sWidth - width;
                    top = sHeight - height;
                    break;

            }
            SetWindowProperties(left, top, width, height, useDefaultStyle ? roundedStyle : style);
        }

        private void SetWindowProperties(double left, double top, double width, double height, Style style)
        {
            // Update the properties of the current main window
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow != null)
            {
                mainWindow.Left = left;
                mainWindow.Top = top;
                mainWindow.Width = width;
                mainWindow.Height = height;

                // Apply the style only if it's not null
                if (style != null)
                {
                    mainWindow.Style = style;
                }
                else
                {
                    // If style is not provided, use the default styles based on isMaximized
                    mainWindow.Style = isMaximized ? normalStyle : roundedStyle;
                }
            }
        }


        public enum WindowPosition
        {
            TOP_LEFT,
            TOP_CENTER,
            TOP_RIGHT,
            CENTER_LEFT,
            CENTER,
            CENTER_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_CENTER,
            BOTTOM_RIGHT
        }
    }
}
*/
using SPTC_APP.View.Styling;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SPTC_APP.View.Controls
{
    internal class WindowStateHelper
    {
        private bool isMaximized = false;
        private Style roundedStyle;
        private Style normalStyle;

        public void MaximizeWindow(Window window, bool isFullScreen, bool useDefaultStyle = true, Style style = null)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            var helper = new WindowStateHelper(window);
            helper.MaximizeWindow(isFullScreen, useDefaultStyle, style);
        }

        public static void ResizeTo(Window window, WindowPosition position, double width, double height, bool useDefaultStyle = true, Style style = null)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            var helper = new WindowStateHelper(window);
            helper.ResizeTo(position, width, height, useDefaultStyle, style);
        }

        private Window window;
        public WindowStateHelper(Window window)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));
            roundedStyle = Application.Current.Resources["Minified"] as Style;
            normalStyle = Application.Current.Resources["Maximized"] as Style;
        }

        private async void Delay(double seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        public void MaximizeWindow(bool isFullScreen, bool useDefaultStyle = true, Style style = null)
        {
            if (isFullScreen)
            {
                SetWindowProperties(useDefaultStyle ? normalStyle : style);
            }
            else
            {
                Rect workingArea = SystemParameters.WorkArea;
                SetWindowProperties(workingArea.Left, workingArea.Top, workingArea.Width, workingArea.Height, useDefaultStyle ? normalStyle : style);
                isMaximized = true;
            }
        }
        public void ResizeTo(WindowPosition position, double width, double height, bool useDefaultStyle = true, Style style = null)
        {
            double left = 0, top = 0;
            double xCenter = (SystemParameters.PrimaryScreenWidth - width) / 2;
            double yCenter = (SystemParameters.PrimaryScreenHeight - height) / 2;
            double sWidth = SystemParameters.PrimaryScreenWidth;
            double sHeight = SystemParameters.WorkArea.Height;
            isMaximized = false;

            switch (position)
            {
                case WindowPosition.TOP_LEFT:
                    left = 0;
                    top = 0;
                    break;
                case WindowPosition.TOP_CENTER:
                    left = xCenter;
                    top = 0;
                    break;
                case WindowPosition.TOP_RIGHT:
                    left = sWidth - width;
                    top = 0;
                    break;
                case WindowPosition.CENTER_LEFT:
                    left = 0;
                    top = yCenter;
                    break;
                case WindowPosition.CENTER:
                    left = xCenter;
                    top = yCenter;
                    break;
                case WindowPosition.CENTER_RIGHT:
                    left = sWidth - width;
                    top = yCenter;
                    break;
                case WindowPosition.BOTTOM_LEFT:
                    left = 0;
                    top = sHeight - height;
                    break;
                case WindowPosition.BOTTOM_CENTER:
                    left = xCenter;
                    top = sHeight - height;
                    break;
                case WindowPosition.BOTTOM_RIGHT:
                    left = sWidth - width;
                    top = sHeight - height;
                    break;
            }

            SetWindowProperties(left, top, width, height, useDefaultStyle ? roundedStyle : style);
        }

        private void SetWindowProperties(double left, double top, double width, double height, Style style)
        {
            if (window != null)
            {
                window.WindowState = WindowState.Normal;
                window.Left = left;
                window.Top = top;
                window.Width = width;
                window.Height = height;

                if (style != null)
                {
                    window.Style = style;
                }
                else
                {
                    window.Style = isMaximized ? normalStyle : roundedStyle;
                }
            }
        }
        private void SetWindowProperties(Style style)
        {
            if (window != null)
            {
                window.WindowState = WindowState.Maximized;
                if (style != null)
                {
                    window.Style = style;
                }
                else
                {
                    window.Style = isMaximized ? normalStyle : roundedStyle;
                }
            }
        }

        public enum WindowPosition
        {
            TOP_LEFT,
            TOP_CENTER,
            TOP_RIGHT,
            CENTER_LEFT,
            CENTER,
            CENTER_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_CENTER,
            BOTTOM_RIGHT
        }
    }
}
