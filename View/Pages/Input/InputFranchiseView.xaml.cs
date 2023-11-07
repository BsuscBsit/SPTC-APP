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
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for TupleView.xaml
    /// </summary>
    public partial class InputFranchiseView : Window
    {
        Franchise fran;
        public InputFranchiseView(Franchise franchise = null)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            DateIssued.DisplayDate = DateTime.Now;
            this.fran = (franchise ?? new Franchise());
            if(franchise != null)
            {
                tboxBodyNum.Text = franchise.BodyNumber;
                tboxMTOPplateNum.Text = franchise.MTOPNo;
                tboxLTOplateNum.Text = franchise.LicenseNO;

            }
            DraggingHelper.DragWindow(topBar);
            tboxBodyNum.Focus();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        private void btnCanceFranchiseInfo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //Make Requirements into dropdown, out database will only save tin_number and voters_id_number
        //NOTE: gather all posible requirements for Franchise only

        // (08-11-23) Design Temporary: TIN & Voter's as labels til' other requirements incomplete.

        private void btnNextFranchiseInput_Click(object sender, RoutedEventArgs e)
        {
            Franchise franchise = fran;
            franchise.BodyNumber = tboxBodyNum.Text;
            franchise.MTOPNo = tboxMTOPplateNum.Text;
            franchise.LicenseNO = tboxLTOplateNum.Text;
            franchise.BuyingDate = DateIssued.DisplayDate;
            if (franchise.Operator == null)
            {
                franchise.Operator = new Operator();
                franchise.Operator.tinNumber = tboxIDNum1.Text;
                franchise.Operator.votersNumbewr = tboxIDNum2.Text;
                (new NewOptr_Drv(franchise, General.OPERATOR)).Show();
            } else
            {
                franchise.Operator.tinNumber = tboxIDNum1.Text;
                franchise.Operator.votersNumbewr = tboxIDNum2.Text;
                franchise.Save();
            }
            this.Close();

        }

        private void initTextBoxes()
        {
            tboxBodyNum.DefaultTextBoxBehavior(NUMBERS, true, gridToast, "Body number.", 0, true);
            tboxMTOPplateNum.DefaultTextBoxBehavior(STRICTALPHANUMERICDASHES, true, gridToast, "Motorcycle/Tricycle Plate number.", 1, true);
            tboxLTOplateNum.DefaultTextBoxBehavior(STRICTALPHANUMERICDASHES, true, gridToast, "Land Transportation Office Plate number.", 2, true);
            tboxIDNum1.DefaultTextBoxBehavior(TIN, true, gridToast, "Taxpayer's Identification Number", 3, true);
            tboxIDNum2.DefaultTextBoxBehavior(VIN, true, gridToast, "Voter's Identification number.", 4, true);
        }
    }
}
