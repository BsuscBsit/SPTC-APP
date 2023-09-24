
using SPTC_APP.Objects;
using System.Windows;
using System.Windows.Controls;

namespace SPTC_APP.View.Pages.Leaflets
{
    /// <summary>
    /// Interaction logic for DriverInformationView.xaml
    /// </summary>
    public partial class DriverInformationView : Window
    {
        public DriverInformationView()
        {
            InitializeComponent();
            lblbodyNumber.Content = MainBody.selectedFranchise.BodyNumber;
            lblDriverName.Content = MainBody.selectedFranchise.Driver;
            lblDateOfBIrth.Content = MainBody.selectedFranchise.Driver?.birthday.ToLongDateString();
            lblPlateNo.Content = MainBody.selectedFranchise.LicenseNO;
            lblMTOPNo.Content = MainBody.selectedFranchise.MTOPNo;
        }

        public Grid Fetch()
        {
            if (DriverPanel.Parent != null)
            {
                Window currentParent = DriverPanel.Parent as Window;
                if (currentParent != null)
                {
                    currentParent.Content = null;
                }
            }
            this.Close();
            return DriverPanel;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (DriverPanel.Parent != null)
            {
                Grid currentParent = DriverPanel.Parent as Grid;
                if (currentParent != null)
                {
                    currentParent.Children.Remove(DriverPanel);
                }

            }
        }
    }
}
