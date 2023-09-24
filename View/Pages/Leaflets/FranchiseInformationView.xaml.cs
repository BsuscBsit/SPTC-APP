
using SPTC_APP.Objects;
using System.Windows;
using System.Windows.Controls;

namespace SPTC_APP.View.Pages.Leaflets
{
    /// <summary>
    /// Interaction logic for FranchiseInformationView.xaml
    /// </summary>
    public partial class FranchiseInformationView : Window
    {
        public FranchiseInformationView()
        {
            InitializeComponent();
            lblbodyNumber.Content = MainBody.selectedFranchise.BodyNumber;
            lblOperatorName.Content = MainBody.selectedFranchise.Operator;
            lblDateOfBIrth.Content = MainBody.selectedFranchise.Operator?.birthday.ToLongDateString();
            lblPlateNo.Content = MainBody.selectedFranchise.LicenseNO;
            lblMTOPNo.Content = MainBody.selectedFranchise.MTOPNo;

            imgProfilePic.ImageSource = MainBody.selectedFranchise.Operator?.image?.GetSource();
        }

        public Grid Fetch()
        {
            if (FranchisePanel.Parent != null)
            {
                Window currentParent = FranchisePanel.Parent as Window;
                if (currentParent != null)
                {
                    currentParent.Content = null;
                }
            }
            this.Close();
            return FranchisePanel;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (FranchisePanel.Parent != null)
            {
                Grid currentParent = FranchisePanel.Parent as Grid;
                if (currentParent != null)
                {
                    currentParent.Children.Remove(FranchisePanel);
                }

            }
        }
    }
}
