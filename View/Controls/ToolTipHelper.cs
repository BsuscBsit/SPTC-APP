using SPTC_APP.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SPTC_APP.View.Controls
{
    public partial class ToolTipHelper
    {
        ToolTip tt;

        ToolTipHelper()
        {
            tt = new ToolTip();
        }

        public void SetButtonToolTip(Button button, string tooltip = "")
        {
            if(!string.IsNullOrEmpty(tooltip))
            {
                tt.Content = tooltip;
            }
            else
            {
                if (button.Content != null)
                {
                    tt.Content = button.Content;
                }
            }
            button.ToolTip = tt;
        }

        public void SetTextBoxToolTip(TextBox textBox, string tooltip, string tooltipIfEmpty = "")
        {
            if (!string.IsNullOrEmpty(tooltipIfEmpty))
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    tt.Content = tooltipIfEmpty;
                } 
                else
                {
                    tt.Content = tooltip;
                }
            }
            else
            {
                tt.Content = tooltip;
            }
            textBox.ToolTip = tt;
        }

        public void SetToolTip(FrameworkElement element, string tooltip)
        {
            tt.Content = tooltip;
            try
            {
                element.ToolTip = tt;
            } catch(Exception e) 
            {
                EventLogger.Post($"ERR: {e.Message}");
            }

        }
    }
}
