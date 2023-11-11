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

namespace SPTC_APP.View.Configuration
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
            List<TextBox> controls = new List<TextBox>
            {
                letter, letterperiod, letterdash, letterdashperiod,
                number, numberperiod, numberdash, numberdashperiod,
                numberexpressions, alphanumeric, alphanumericperiod,
                alphanumericdash, alphanumericdashperiod, phone,
                email, common, address
            };

            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].DefaultTextBoxBehavior((TextBoxHelper.AllowFormat)i, true, gridtoast, null, i,true,3);
            }
            wholesigned.NumericTextBoxBehavior(DECIMALSIGNED, gridtoast, null, null, -500, 100);
        }

        private void wholesigned_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
