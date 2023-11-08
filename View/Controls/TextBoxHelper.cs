using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml;
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Controls
{
    public static class TextBoxHelper
    {
        private static Grid grid;

        public static void DefaultTextBoxBehavior(this TextBox tb, AllowFormat? format = null, bool blockSpaces = false, Grid errorGrid = null, string toolTip = null, int? tabIndex = null, bool allCaps = false, int? げんかい = null, string MSG = null)
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
            if (allCaps)
            {
                tb.CharacterCasing = CharacterCasing.Upper;
            }

            // Ordered event handlers.
            tb.PreviewKeyDown += (sender, e) => PreviewKeyDown(sender, e, げんかい);
            if(format != null)
            {
                tb.PreviewTextInput += (sender, e) => PreviewTextInputBehavior(sender, e, format, MSG);
            }
            if (blockSpaces)
            {
                tb.TextChanged += BlockSpace;
            }
            tb.PreviewMouseDoubleClick += DoubleClickBehavior;
            tb.GotFocus += GotFocusBehavior;
        }

        private static void PreviewKeyDown(object sender, KeyEventArgs e, int? げんかい)
        {
            if (げんかい != null && sender is TextBox tb && IsCharacterKey(e.Key))
            {
                if (tb.Text.Length > げんかい + 1)
                {
                    e.Handled = true;
                    ShowError(null, null, "Maximum length for this field has reached.");
                    tb.Focus();
                }
            }
            var source = e.OriginalSource as FrameworkElement;
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                source.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private static bool IsCharacterKey(Key key)
        {
            return (key >= Key.A && key <= Key.Z) || (key >= Key.D0 && key <= Key.D9) || key == Key.Space || key == Key.Oem1 || key == Key.Oem2;

        }

        private static void PreviewTextInputBehavior(object sender, TextCompositionEventArgs e, AllowFormat? format, string MSG)
        {
            if (sender is TextBox tb)
            {
                string pattern = patterns[(int) format];

                if (Regex.IsMatch(e.Text, pattern))
                {
                    e.Handled = true;
                    ShowError(format, e.Text, MSG);
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
                if(tb.SelectionLength == tb.Text.Length)
                {
                    tb.SelectionLength = 0;
                } else
                {
                    tb.SelectAll();
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
                    textBox.CaretIndex = textBox.Text.Length;
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

        /*private static void ShowError(AllowFormat? format, string input, string MSG)
        {
            if(grid != null)
            {
                if(!string.IsNullOrEmpty(MSG))
                {
                    new Toast(grid, MSG, 2.5);
                    return;
                }

                string post = !string.IsNullOrEmpty(input) ? "\n'" + input + "' is not a valid input." : "";
                string pre = "This field only accepts ";
                string msg;

                if (format == NUMBEREXPRESSIONS)
                {
                    msg = $"{pre}{patternDescription[(int)format]}";
                }
                else if (format == EMAIL)
                {
                    msg = $"' {post} ' is not a valid input for email address.";
                }
                else if (format == COMMON)
                {
                    msg = $"{pre}{patternDescription[(int)format]}";
                }
                else if (format == ADDRESS)
                {
                    msg = $"' {post} ' is not a valid input for address.";
                }
                else
                {
                    msg = pre + patternDescription[(int)format] + post;
                }
                new Toast(grid, MSG, 2.5);
            }
        }*/
        private static void ShowError(AllowFormat? format, string input, string MSG)
        {
            if (grid == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(MSG) && format == null)
            {
                new Toast(grid, MSG, 2.5);
                return;
            }

            string post = !string.IsNullOrEmpty(input) ? "\n'" + input + "' is not a valid input." : "";
            string pre = "This field only accepts ";
            string msg;

            switch (format)
            {
                case NUMBEREXPRESSIONS:
                case COMMON:
                    msg = $"{pre}{patternDescription[(int)format]}";
                    break;

                case EMAIL:
                    msg = $"' {post} ' is not a valid input for email address.";
                    break;

                case ADDRESS:
                    msg = $"' {post} ' is not a valid input for address.";
                    break;

                default:
                    msg = pre + patternDescription[(int)format] + post;
                    break;
            }

            new Toast(grid, msg, 2.5);
        }


        public enum AllowFormat
        {
            LETTER,
            LETTERDASH,
            LETTERPERIOD,
            LETTERDASHPERIOD,

            NUMBER,
            NUMBERDASH,
            NUMBERPERIOD,
            NUMBERDASHPERIOD,
            NUMBEREXPRESSIONS,

            ALPHANUMERIC,
            ALPHANUMERICDASH,
            ALPHANUMERICPERIOD,
            ALPHANUMERICDASHPERIOD,

            PHONE,
            EMAIL,
            COMMON,
            ADDRESS
        }

        private static string[] patterns = new string[]
        {
            @"[^A-Za-z]+",
            @"[^A-Za-z\-]+",
            @"[^A-Za-z.]+",
            @"[^A-Za-z\-.]+",

            @"[^0-9]+",
            @"[^0-9\-]+",
            @"[^0-9.]+",
            @"[^0-9\-.]+",
            @"[^0-9+\-*/%()^a-zA-Z]+",

            @"[^0-9A-Za-z]+",
            @"[^0-9A-Za-z\-]+",
            @"[^0-9A-Za-z.]+",
            @"[^0-9A-Za-z\-.]+",

            @"[^+\-0-9]+",
            @"[^a-zA-Z0-9@._-]+",
            @"[^A-Za-z0-9.,;:!@#$%^&*()_+=\[\]{}|'""<>/\\?~-]+",
            @"[^0-9A-Za-z.,\-/@#&()""'\\]+"
        };
        private static string[] patternDescription = new string[]
        {
            "letters.",
            "letters and dashes.",
            "letters and periods.",
            "letters, dashes, and periods.",

            "numbers.",
            "numbers and dashes.",
            "numbers and periods.",
            "numbers, dashes and periods.",
            "letters, numbers, and characters:\n〔 . % ^ * ( ) + = / - 〕",

            "letters and numbers.",
            "letters, numbers, and dashes.",
            "letters, numbers, and periods.",
            "letters, numbers, dashes and periods.",

            "numbers, dashes, and plus sign '+'.",
            "letters, numbers, and characters:\n〔 . @ _ - 〕",
            "letters, numbers, and characters:\n〔 . , ; : ! @ # $ % ^ & * ( ) _ + = [ ] { } | \" < > \\ / ? ~ - ' 〕",
            "letters, numbers, and characters:\n〔 . , # & ( ) \" \\ - ' 〕"
        };
    }
}
