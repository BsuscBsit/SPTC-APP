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
            alpabets.DefaultTextBoxBehavior(ALPHABETS);
            signeddigit.DefaultTextBoxBehavior(SIGNEDDIGIT);
            unsigneddigit.DefaultTextBoxBehavior(UNSIGNEDDIGIT);
            alphanumeric.DefaultTextBoxBehavior(ALPHANUMERIC);
            phonenumber.DefaultTextBoxBehavior(PHONENUMBER, true);
            address.DefaultTextBoxBehavior(ADDRESS);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Toast toast = new Toast(gridForToast, "The Quick Brown Fox Jumps Over The Lazy Dog!");
        }
    }
}
