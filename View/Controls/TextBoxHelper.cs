using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Controls
{
    public static class TextBoxHelper
    {
        private static Grid grid;
        public static void DefaultTextBoxBehavior(this TextBox tb, AllowFormat? format = null, bool blockSpaces = false, Grid errorGrid = null, string toolTip = null, int? tabIndex = null)
        {
            if(errorGrid != null)
            {
                grid = errorGrid;
            }
            if(tabIndex != null)
            {
                tb.TabIndex = (int)tabIndex;
            }
            if(toolTip != null)
            {
                tb.ToolTip = toolTip;
            }
            tb.PreviewKeyDown += PreviewKeyDown;
            tb.GotFocus += GotFocusBehavior;
            tb.MouseDoubleClick += DoubleClickBehavior;
            if(format != null)
            {
                tb.PreviewTextInput += (sender, e) => PreviewTextInputBehavior(sender, e, format);
            }
            if (blockSpaces)
            {
                tb.TextChanged += BlockSpace;
            }
        }

        private static void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                source.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private static void PreviewTextInputBehavior(object sender, TextCompositionEventArgs e, AllowFormat? format)
        {
            if (sender is TextBox tb)
            {
                string pattern = GetPatternForFormat(format);

                if (Regex.IsMatch(e.Text, pattern))
                {
                    e.Handled = true;
                    ShowError(format, e.Text);
                    tb.Focus();
                }
            }
        }

        private static void BlockSpace(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (tb.Text.Contains(" "))
                {
                    e.Handled = true;
                    tb.Text = Regex.Replace(tb.Text, " ", "");
                    if(grid != null)
                    {
                        new Toast(grid, "Spaces not allowed here.");
                    }
                    tb.CaretIndex = tb.Text.Length;
                    tb.Focus();
                }
            }
        }

        private static void DoubleClickBehavior(object sender, MouseButtonEventArgs e)
        {
            if(sender is TextBox tb)
            {
                if(tb.IsSelectionActive)
                {
                    tb.SelectionLength = 0;
                    tb.CaretIndex = tb.Text.Length;
                }
            }
        }

        private static void GotFocusBehavior(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Text.Length > 0)
                {
                    textBox.SelectAll();
                }
            }
        }

        private static void FocusAction(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                List<Key> allowedKeys = new List<Key>
                {
                    Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J,
                    Key.K, Key.L, Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T,
                    Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z,
                    Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9
                };
                if (textBox.Text.Length > 0 && 
                    textBox.SelectionLength == textBox.Text.Length && 
                    (allowedKeys.Contains(e.Key)))
                {
                    textBox.Text = "";
                }
                textBox.PreviewKeyDown -= FocusAction;
            }
        }

        private static string GetPatternForFormat(AllowFormat? format)
        {
            switch (format)
            {
                case ALPHABETS:
                    return @"[^A-Za-z.]+";

                case SIGNEDDIGIT:
                    return @"[^0-9.-]+";

                case UNSIGNEDDIGIT:
                    return @"[^0-9.]+";

                case ALPHANUMERIC:
                    return @"[^0-9.A-Za-z+\-*/()\s]+";

                case PHONENUMBER:
                    return @"[^+\-0-9]+";

                case EMAIL:
                    return @"[^a-zA-Z0-9@._-]+";

                case COMMON:
                    return @"[^A-Za-z0-9.,;:!@#$%^&*()_+=\[\]{}|'""<>/\\?~-]+";

                case ADDRESS:
                    return @"[^0-9A-Za-z.\-/@#()""'\\]+";

                default:
                    return string.Empty;
            }
        }
        
        private static void ShowError(AllowFormat? format, string input = null)
        {
            if(grid != null)
            {
                string x = !string.IsNullOrEmpty(input) ? "\n'" + input + "' is not a valid input." : "";
                string msg = "This field only accepts ";
                switch (format)
                {
                    case ALPHABETS:
                        msg += "letters and periods." + x;
                        break;

                    case SIGNEDDIGIT:
                        msg += "numbers, dashes, and periods." + x;
                        break;

                    case UNSIGNEDDIGIT:
                        msg += "numbers and periods." + x;
                        break;

                    case ALPHANUMERIC:
                        msg += "letters, numbers, and characters '* / + - ( ) .'" + x;
                        break;

                    case PHONENUMBER:
                        msg += "numbers, dashes, and plus sign '+'." + x;
                        break;

                    case EMAIL:
                        msg = input != null ? "'" + input + "' is not a valid email character." : "";
                        break;

                    case COMMON:
                        msg += "letters, numbers and characters:\n" +
                            "' . , ; : ! @ # $ % ^ & * ( ) _ + = [ ] { } |' \" < > \\ / ? ~ -'";
                        break;

                    case ADDRESS:
                        msg = !string.IsNullOrEmpty(x) ? x.Substring(1, x.Length - 2) + " for this field." : "";
                        break;
                }
                if(!((msg == "This field only accepts " || string.IsNullOrEmpty(msg)) && string.IsNullOrEmpty(x)))
                {
                    new Toast(grid, msg, 2.5);
                }
            }
        }

        public enum AllowFormat
        {
            ALPHABETS,
            SIGNEDDIGIT,
            UNSIGNEDDIGIT,
            ALPHANUMERIC,
            PHONENUMBER,
            EMAIL,
            COMMON,
            ADDRESS
        }
    }
}
