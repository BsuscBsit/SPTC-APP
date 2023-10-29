using SPTC_APP.Objects;
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

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for AddShareCaptital.xaml
    /// </summary>
    public partial class AddShareCaptital : Window
    {
        private Franchise franchise;
        public AddShareCaptital(Franchise franchise)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            AppState.mainwindow?.Hide();
            this.franchise = franchise;
            dpBdate.DisplayDate = DateTime.Now;
            tboxAmount.Text = AppState.TOTAL_SHARE_PER_MONTH.ToString();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ControlWindow.ShowDialogStatic("Adding new record", "Confirm?", Icons.NOTIFY))
            {
                PaymentDetails<Ledger.ShareCapital> capital = new PaymentDetails<Ledger.ShareCapital>();
                Ledger.ShareCapital share = new Ledger.ShareCapital();
                if(franchise.GetShareCapitals()?.FirstOrDefault() != null)
                {
                    share = franchise.GetShareCapitals().FirstOrDefault();
                } else
                {
                    share.WriteInto(franchise.id, DateTime.Now, 0, 0);
                }
                capital.WriteInto(share, false, false, dpBdate.DisplayDate, tboxRefNo.Text, Double.Parse(tboxAmount.Text), 0, "", share.lastBalance + Double.Parse(tboxAmount.Text));
                capital.Save();
                share.lastBalance = share.lastBalance + Double.Parse(tboxAmount.Text);
                share.Save();
                this.Close();
            }
        }
    }
}
