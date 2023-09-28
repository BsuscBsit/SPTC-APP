
using SPTC_APP.Objects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SPTC_APP.View.Pages.Leaflets
{
    /// <summary>
    /// Interaction logic for FranchiseInformationView.xaml
    /// </summary>
    public partial class FranchiseInformationView : Window
    {
        private Button selectedButton = null;
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

        private void ClickColorControl(Button button)
        {
            if (selectedButton != null)
            {
                selectedButton.Background = Brushes.White;
            }

            button.Background = Brushes.Yellow;
            selectedButton = button;
        }

        private void HandleButtonClick(string moduleName)
        {
            ModuleGrid.Children.Clear();
            ModuleGrid.Children.Add((new Modules(moduleName)).Fetch());
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.HISTORY);
            ClickColorControl(sender as Button);
        }

        private void btnCoding_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.CODING);
            ClickColorControl(sender as Button);
        }

        private void btnViolation_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.VIOLATION);
            ClickColorControl(sender as Button);
        }

        private void btnShareCapital_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.SHARECAPITAL);
            ClickColorControl(sender as Button);
        }

        private void btnLoan_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.LOAN);
            ClickColorControl(sender as Button);
        }

        private void btnLTLoan_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.LTLOAN);
            ClickColorControl(sender as Button);
        }

        private void btnTransactionHistory_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.TRANSFER);
            ClickColorControl(sender as Button);
        }

    }
}
