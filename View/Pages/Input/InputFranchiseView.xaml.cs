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
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            bDay.DisplayDate = DateTime.Now;
            this.fran = (franchise ?? new Franchise());
            if(franchise != null)
            {
                tboxBodyNum.Text = franchise.BodyNumber;
                tboxMTOPplateNum.Text = franchise.MTOPNo;
                tboxLTOplateNum.Text = franchise.LicenseNO;

            }
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

        private void btnNextFranchiseInput_Click(object sender, RoutedEventArgs e)
        {
            Franchise franchise = fran;
            franchise.BodyNumber = tboxBodyNum.Text;
            franchise.MTOPNo = tboxMTOPplateNum.Text;
            franchise.LicenseNO = tboxLTOplateNum.Text;
            franchise.BuyingDate = bDay.DisplayDate;
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
    }
}
