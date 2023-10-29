
using SPTC_APP.Objects;
using SPTC_APP.View.Pages.Input;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for FranchiseInformationView.xaml
    /// </summary>
    public partial class FranchiseInformationView : Window
    {
        private Button selectedButton = null;
        TableView tableview;
        public FranchiseInformationView(TableView tableview = null)
        {
            InitializeComponent();
            UpdateContent();
            this.tableview = tableview;
            btnAddButton.Visibility = Visibility.Hidden;
            if((AppState.USER?.position?.accesses[0] ?? false))
            {
                btnChangeDriver.Visibility = Visibility.Visible;
            }
            if ((AppState.USER?.position?.accesses[8] ?? false))
            {
                btnDeleteFranchise.Visibility = Visibility.Visible;
            }
        }

        private void UpdateContent()
        {
            lblbodyNumber.Content = MainBody.selectedFranchise?.BodyNumber;
            lblOperatorName.Content = MainBody.selectedFranchise?.Operator;
            lblDateOfBIrth.Content = MainBody.selectedFranchise?.Operator?.birthday.ToLongDateString();
            lblPlateNo.Content = MainBody.selectedFranchise?.LicenseNO;
            lblMTOPNo.Content = MainBody.selectedFranchise?.MTOPNo;
            lblDriverName.Content = MainBody.selectedFranchise?.Driver?.name?.legalName ?? "N/A";

            imgProfilePic.ImageSource = MainBody.selectedFranchise?.Operator?.image?.GetSource();
            if (MainBody.selectedFranchise?.Driver == null)
            {
                btnChangeDriver.Content = "NEW DRIVER";
            }
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
                    if (tableview != null)
                    {
                        tableview.BackUpdate();
                    }
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
            UpdateContent();
        }

        private void HandleButtonClick(string moduleName, Button sender)
        {
            ClickColorControl(sender);
            ModuleGrid.Children.Clear();
            ModuleGrid.Children.Add((new Modules(moduleName, MainBody.selectedFranchise)).Fetch());
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            btnAddButton.Visibility = Visibility.Collapsed;
            HandleButtonClick(Modules.HISTORY, sender as Button);
            
        }

        private void btnCoding_Click(object sender, RoutedEventArgs e)
        {
            btnAddButton.Visibility = Visibility.Collapsed;
            HandleButtonClick(Modules.CODING, sender as Button);
        }

        private void btnViolation_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[12] ?? false))
            {

                if (MainBody.selectedFranchise?.Driver != null)
                {
                    btnAddButton.Content = "ADD RECORD";
                    btnAddButton.Visibility = Visibility.Visible;
                }
            }
            HandleButtonClick(Modules.VIOLATION, sender as Button);
        }

        private void btnShareCapital_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[18] ?? false))
            {
                btnAddButton.Content = "ADD RECORD";
                btnAddButton.Visibility = Visibility.Visible;
            }
            HandleButtonClick(Modules.SHARECAPITAL, sender as Button);
        }

        private void btnLoan_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[18] ?? false))
            {
                btnAddButton.Content = "ADD RECORD";
                if (MainBody.selectedFranchise.GetLoans().Count <= 0)
                {
                    btnAddButton.Content = "APPLY FOR LOAN";
                }

                btnAddButton.Visibility = Visibility.Visible;
            }
            HandleButtonClick(Modules.LOAN, sender as Button);
        }

        private void btnLTLoan_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[18] ?? false))
            {
                btnAddButton.Content = "ADD RECORD";
                if (MainBody.selectedFranchise.GetLTLoans().Count <= 0)
                {
                    btnAddButton.Content = "APPLY FOR LOAN";
                }

                btnAddButton.Visibility = Visibility.Visible;
            }
            HandleButtonClick(Modules.LTLOAN, sender as Button);
        }

        private void btnTransactionHistory_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[1] ?? false))
            {
                btnAddButton.Content = "TRANSFER";
                btnAddButton.Visibility = Visibility.Visible;
            }
            HandleButtonClick(Modules.TRANSFER, sender as Button);
        }

        private void btnChangeDriver_Click(object sender, RoutedEventArgs e)
        {
            int result = ControlWindow.ShowDialogStatic("Change Driver", "New or Existing?", "EXISTING", "NEW", Icons.DEFAULT);

            if (result == -1) 
            {
                 
            }
            else if(result == 1)
            {
                (new NewOptr_Drv(MainBody.selectedFranchise, General.DRIVER)).ShowDialog();
                UpdateContent();
            }
            else if (result == 0)
            {
                (new Selection(MainBody.selectedFranchise, General.DRIVER)).ShowDialog();
                UpdateContent();
            }
        }

        private void btnAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedButton == btnViolation)
            {
                (new ViolationInput(MainBody.selectedFranchise)).Show();
            }
            else if (selectedButton == btnShareCapital)
            {
                (new AddShareCaptital(MainBody.selectedFranchise)).Show();
            }
            else if (selectedButton == btnLoan)
            {
                (new AddLoan(MainBody.selectedFranchise)).Show();
            }
            else if (selectedButton == btnLTLoan)
            {
                (new AddLTLoan(MainBody.selectedFranchise)).Show();
            }
            else if (selectedButton == btnTransactionHistory)
            {
                (new NewOptr_Drv(MainBody.selectedFranchise, General.TRANSFER_FRANCHISE_OWNERSHIP)).Show();
            }
        }

        private void btnDeleteFranchise_Click(object sender, RoutedEventArgs e)
        {
            MainBody.selectedFranchise.delete();
        }
    }
}
