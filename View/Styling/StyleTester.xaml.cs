using SPTC_APP.View.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Styling
{
    /// <summary>
    /// Interaction logic for StyleTester.xaml
    /// </summary>
    public partial class StyleTester : Window
    {
        public StyleTester()
        {
            InitializeComponent();
            initTextBoxes();
        }

        private void initTextBoxes()
        {
            alpabets.DefaultTextBoxBehavior(ALPHABETS, false, gridForToast, "A TextBox that only accepts letters.", 1);
            signeddigit.DefaultTextBoxBehavior(SIGNEDDIGIT, false, gridForToast);
            unsigneddigit.DefaultTextBoxBehavior(UNSIGNEDDIGIT, false, gridForToast);
            alphanumeric.DefaultTextBoxBehavior(ALPHANUMERIC, false, gridForToast);
            phonenumber.DefaultTextBoxBehavior(PHONENUMBER, true, gridForToast);
            email.DefaultTextBoxBehavior(EMAIL, true, gridForToast);
            common.DefaultTextBoxBehavior(COMMON, false, gridForToast);
            address.DefaultTextBoxBehavior(ADDRESS, false, gridForToast);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Toast toast = new Toast(gridForToast, "The Quick Brown Fox Jumps Over The Lazy Dog!");
        }

    }
}
