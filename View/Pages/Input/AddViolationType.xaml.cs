using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for AddViolationType.xaml
    /// </summary>
    public partial class AddViolationType : Window
    {
        private string closingMSG;
        private ViolationType selected;
        private ViolationTypeList window;
        public AddViolationType(ViolationTypeList list, ViolationType violationType = null)
        {
            InitializeComponent();
            window = list;
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); window?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };

            if(violationType != null)
            {
                selected = violationType;
                tbTitle.Text = selected.title;
                tbNumDays.Text = selected.numOfDays.ToString();
                tbDescription.Text = selected.details;
            }

            DraggingHelper.DragWindow(topBar);
            initTextBoxes();
            tbTitle.Focus();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            window?.Show();
            window?.displayToast(closingMSG);
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if(selected != null)
                this.closingMSG = "Editing violation type was cancelled.";
            else
                this.closingMSG = "Adding violation type was cancelled.";
            this.Close();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(tbTitle.Text.Length > 0 && tbNumDays.Text.Length > 0)
            {
                int numofdays = 0;
                if (Int32.TryParse(tbNumDays.Text, out numofdays))
                {
                    ViolationType violationType = (selected != null)? selected: new ViolationType();
                    violationType.WriteInto(tbTitle.Text, tbDescription.Text, numofdays, General.DRIVER);
                    violationType.Save();
                    if(selected != null)
                        this.closingMSG = "Editing violation type was successful.";
                    else
                        this.closingMSG = "Adding violation type was successful.";
                    this.Close();
                } else
                {
                    ControlWindow.ShowStatic("Found Empty Fields", "Some required fields are empty.\n Please fill them up before continuing.", Icons.DEFAULT);
                }
            } else
            {
                ControlWindow.ShowStatic("Found Empty Fields", "Some required fields are empty.\n Please fill them up before continuing.", Icons.DEFAULT);
            }
        }
        
        private void initTextBoxes()
        {
            tbTitle.DefaultTextBoxBehavior(ALPHANUMERICDASHPERIOD, false, gridToast, "Enter a descriptve title of violation type.", 0, false, 20);
            tbNumDays.DefaultTextBoxBehavior(NUMBER, true, gridToast, "Span of suspended days.", 1, false, 3);
            //tbDescription.DefaultTextBoxBehavior(COMMON, false, gridToast, "Enter description for this type of violation.", 2, false, 100);
        }
    }
}
