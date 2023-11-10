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
        #region Fields

        private static Grid grid;

        #endregion

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
            if (errorGrid != null)
                grid = errorGrid;

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
            tb.PreviewKeyDown += (sender, e) => PreviewKeyDown_LengthDelimiter(sender, e, げんかい);

            if (format != null)
                tb.PreviewTextInput += (sender, e) => PreviewTextInput_Formatter(sender, e, format, MSG);

            if (blockSpaces)
                tb.TextChanged += TextChanged_SpaceBlocker;

            tb.PreviewMouseDoubleClick += PreviewMouseDoubleClick_TextSelection;
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

        private static void PreviewKeyDown_LengthDelimiter(object sender, KeyEventArgs e, int? げんかい)
        {
            if (げんかい != null && sender is TextBox tb && IsCharacterKey(e.Key) && tb.Text.Length > げんかい - 1 && tb.SelectionLength != tb.Text.Length)
            {
                e.Handled = true;
                ShowError(null, null, "Maximum length for this field has reached.");
                tb.Focus();
            }

            var source = e.OriginalSource as FrameworkElement;
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                source.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private static void PreviewTextInput_Formatter(object sender, TextCompositionEventArgs e, AllowFormat? format, string MSG)
        {
            if (sender is TextBox tb)
            {
                string pattern = patterns[(int)format];

                if (Regex.IsMatch(e.Text, pattern))
                {
                    e.Handled = true;
                    ShowError(format, e.Text, MSG);
                    tb.Focus();
                }
            }
        }

        private static void TextChanged_SpaceBlocker(object sender, TextChangedEventArgs e)
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
        private static void ShowError(AllowFormat? format, string input, string MSG)
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
                    msg = $"' {post} ' is not a valid input for an email address.";
                    break;

                case ADDRESS:
                    msg = $"' {post} ' is not a valid input for an address.";
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

        #endregion

        #region Patterns

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

    #endregion
}