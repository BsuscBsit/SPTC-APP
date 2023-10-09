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
    /// Interaction logic for ChangeOperator.xaml
    /// </summary>
    public partial class ChangeOperator : Window
    {
        public ChangeOperator()
        {
            InitializeComponent();
            AppState.mainwindow?.Hide();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }
    }
}
