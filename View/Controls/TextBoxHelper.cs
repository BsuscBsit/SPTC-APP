using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SPTC_APP.View.Controls.TextBoxHelper.EventSubscription;
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Controls
{
    public static class TextBoxHelper
    {

        #region Public Methods

        public static void DefaultTextBoxBehavior(
            this TextBox tb,
            AllowFormat? format = null,
            bool blockSpaces = false,
            Grid errorGrid = null,
            string toolTip = null,
            int? tabIndex = null,
            bool allCaps = false,
            int? げんかい = null,
            bool acceptReturn = false,
            bool acceptTab = false,
            string MSG = null)
        {
            if (tabIndex != null)
                tb.TabIndex = (int)tabIndex;

            if (toolTip != null)
                tb.ToolTip = toolTip;

            if (allCaps)
                tb.CharacterCasing = CharacterCasing.Upper;

            tb.AcceptsReturn = acceptReturn;
            tb.AcceptsTab = acceptTab;

            tb.Loaded += (sender, e) => Loaded_InitializeLength(sender, e, げんかい);
            tb.GotFocus += GotFocusBehavior;
            tb.PreviewKeyDown += (sender, e) => PreviewKeyDown_LengthLimiter(sender, e, げんかい, errorGrid);

            if (format != null)
                tb.PreviewTextInput += (sender, e) => PreviewTextInput_Formatter(sender, e, format, MSG, errorGrid);

            if (blockSpaces)
                tb.TextChanged += (sender, e) => TextChanged_SpaceBlocker(sender, e, errorGrid);

            tb.PreviewMouseDoubleClick += PreviewMouseDoubleClick_TextSelection;
        }

        public static void NumericTextBoxBehavior(
            this TextBox tb,
            AllowFormat format,
            Grid errorGrid = null,
            int? tabIndex = null,
            string toolTip = null,
            double? min = null,
            double? max = null)
        {
            if (tabIndex != null && tb != null)
                tb.TabIndex = (int)tabIndex;

            if (toolTip != null && tb != null)
                tb.ToolTip = toolTip;
            tb.Loaded += (sender, e) => Loaded_InitializeSize(sender, e, format, min, max);
            tb.PreviewMouseDoubleClick += PreviewMouseDoubleClick_TextSelection;
            tb.GotFocus += GotFocusBehavior;
            tb.TextChanged += (sender, e) => TextChanged_SpaceBlocker(sender, e, null);
            tb.PreviewTextInput += (sender, e) => TextInput_NumFormatter(sender, e, format, min, max, errorGrid);
        }

        #endregion

        #region Event Handlers

        private static void Loaded_InitializeLength(object sender, RoutedEventArgs e, int? げんかい)
        {
            if (sender is TextBox tb && げんかい != null && tb.Text.Length > げんかい + 1)
            {
                tb.Text = tb.Text.Substring(0, (int)げんかい);
            }
        }

        private static void Loaded_InitializeSize(object sender, RoutedEventArgs e, AllowFormat? format, double? min, double? max)
        {
            if(sender is TextBox tb)
            {
                double inp;
                tb.Text = tb.Text.Replace(patterns[(int)format], "");

                int decimals = tb.Text.IndexOf('.');
                int negative = tb.Text.IndexOf('-');

                if (decimals != -1)
                    tb.Text = tb.Text.Substring(0, decimals + 1) + tb.Text.Substring(decimals + 1).Replace(".", "");

                if (negative != -1)
                    tb.Text = (negative == 0) ? tb.Text.Substring(0, negative + 1)
                        + tb.Text.Substring(negative + 1).Replace("-", "") : tb.Text.Replace("-", "");


                if (double.TryParse(tb.Text, out inp) && min != null)
                {
                    double? result = (format == WHOLEUNSIGNED || format == DECIMALUNSIGNED) && min < 0 ? 0 : min;
                    tb.Text = inp < result ? result.ToString() : inp.ToString();
                }

                if (double.TryParse(tb.Text, out inp) && max != null)
                {
                    tb.Text = inp > max ? max.ToString() : inp.ToString();
                }
            }
        }

        private static void PreviewKeyDown_LengthLimiter(object sender, KeyEventArgs e, int? げんかい, Grid grid)
        {
            if (げんかい != null && sender is TextBox tb && IsCharacterKey(e.Key) && tb.Text.Length > げんかい - 1 && tb.SelectionLength != tb.Text.Length)
            {
                e.Handled = true;
                ShowError(null, grid, null, "Maximum length for this field has reached.");
                tb.Focus();
            }

            var source = e.OriginalSource as FrameworkElement;
            if (sender is TextBox t && e.Key == Key.Enter)
            {
                e.Handled = true;
                source.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private static void PreviewTextInput_Formatter(object sender, TextCompositionEventArgs e, AllowFormat? format, string MSG, Grid grid)
        {
            if (sender is TextBox tb && Regex.IsMatch(e.Text, patterns[(int)format]) && !Regex.IsMatch(e.Text, @"\r\n"))
            {
                e.Handled = true;
                ShowError(format, grid, e.Text, MSG);
                tb.Focus();
            }
        }
        private static void TextInput_NumFormatter(object sender, TextCompositionEventArgs e, AllowFormat? format, double? min, double? max, Grid grid)
        {
            if (sender is TextBox tb)
            {
                if (Regex.IsMatch(e.Text, patterns[(int)format]))
                {
                    e.Handled = true;
                    tb.Focus();
                }
                if (e.Text == "." && tb.Text.Count(c => c == '.') > 0)
                {
                    e.Handled = true;
                    tb.Focus();
                }
                if (e.Text == "-")
                {
                    if(tb.Text.Count(c => c == '-') > 0)
                    {
                        e.Handled = true;
                        tb.Focus();
                    }
                    else
                    {
                        int caret = tb.CaretIndex;
                        string txt = tb.Text.Insert(caret, e.Text);

                        if (txt.IndexOf("-") == 0)
                        {
                            e.Handled = true;
                            tb.Text = txt;
                            tb.CaretIndex = caret + 1;
                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }
                }

                double input = 0;
                string teksto = tb.Text.Insert(tb.CaretIndex, e.Text);
                double.TryParse(teksto, out input);

                if(min != null && input < min && tb.SelectedText.Length != tb.Text.Length)
                {
                    e.Handled = true;
                    ShowError(null, grid, null, $"Value {Math.Round(input, 5)} is below the minimum limit of {Math.Round((double)min, 3)}.");
                }
                if (max != null && input > max && tb.SelectedText.Length != tb.Text.Length)
                {
                    e.Handled = true;
                    ShowError(null, grid, null, $"Value {Math.Round(input, 5)} exceeds the maximum limit of {Math.Round((double)max, 3)}.");
                }
            }
        }

        private static void TextChanged_SpaceBlocker(object sender, TextChangedEventArgs e, Grid grid)
        {
            if (sender is TextBox tb && tb.Text.Contains(" "))
            {
                int x = tb.CaretIndex - 1;
                e.Handled = true;
                tb.Text = Regex.Replace(tb.Text, " ", "");
                if (grid != null)
                    new Toast(grid, "Spaces not allowed here.");

                tb.CaretIndex = x;
            }
        }

        private static void PreviewMouseDoubleClick_TextSelection(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (tb.SelectionLength == tb.Text.Length)
                {
                    tb.SelectionLength = 0;
                    tb.CaretIndex = tb.Text.Length;
                }
                else
                {
                    tb.SelectAll();
                }
            }
        }

        private static void GotFocusBehavior(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        #endregion

        #region Private Methods

        private static bool IsCharacterKey(Key key)
        {
            return (key >= Key.A && key <= Key.Z) || (key >= Key.D0 && key <= Key.D9) || key == Key.Space || key == Key.Oem1 || key == Key.Oem2;
        }
        private static void ShowError(AllowFormat? format, Grid grid, string input, string MSG)
        {
            if (grid == null)
                return;

            if (!string.IsNullOrEmpty(MSG))
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
                    msg = pre + patternDescription[(int)format];
                    break;

                case EMAIL:
                    msg = $"' {input} ' is not a valid input for an email address.";
                    break;

                case ADDRESS:
                    msg = $"' {input} ' is not a valid input for an address.";
                    break;

                case CVOR:
                    msg = $"' {input} ' is not a valid input for this field.";
                    break;

                case WHOLESIGNED:
                case WHOLEUNSIGNED:
                case DECIMALSIGNED:
                case DECIMALUNSIGNED:
                    msg = null;
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
            ADDRESS,

            WHOLESIGNED,
            WHOLEUNSIGNED,

            DECIMALSIGNED,
            DECIMALUNSIGNED,

            CVOR
        }

        public enum EventSubscription
        {
            DefaultBehavior,
            NumberFormat
        }

        #endregion

        #region Patterns

        private static readonly string[] patterns = new string[]
        {
            "[^A-Za-z]+",
            @"[^A-Za-z\-]+",
            "[^A-Za-z.]+",
            @"[^A-Za-z\-.]+",

            "[^0-9]+",
            @"[^0-9\-]+",
            "[^0-9.]+",
            @"[^0-9\-.]+",
            @"[^0-9+\-*/%()^a-zA-Z]+",

            "[^0-9A-Za-z]+",
            @"[^0-9A-Za-z\-]+",
            "[^0-9A-Za-z.]+",
            @"[^0-9A-Za-z\-.]+",

            @"[^+\-0-9]+",
            "[^a-zA-Z0-9@._-]+",
            @"[^A-Za-z0-9.,;:!@#$%^&*()_+=\[\]{}|'""<>/\\?~-]+",
            @"[^0-9A-Za-z.,\-/@#&()""'\\]+",

            @"[^0-9\-]+",
            "[^0-9]+",

            @"[^0-9\-.]+",
            "[^0-9.]+",

            @"[^0-9\-/]+"

        };

        private static readonly string[] patternDescription = new string[]
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

    #endregion
}