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

namespace SPTC_APP.View.Pages.Leaflets
{
    /// <summary>
    /// Interaction logic for TupleView.xaml
    /// </summary>
    public partial class InputFranchiseView : Window
    {
        public InputFranchiseView()
        {
            InitializeComponent();
            AppState.mainwindow?.Hide();
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

        private void btnNextFranchiseInput_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
