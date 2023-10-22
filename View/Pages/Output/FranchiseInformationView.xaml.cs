
using SPTC_APP.Objects;
using SPTC_APP.View.Pages.Input;
using System;
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
        }

        private void UpdateContent()
        {
            lblbodyNumber.Content = MainBody.selectedFranchise?.BodyNumber;
            lblOperatorName.Content = MainBody.selectedFranchise?.Operator;
            lblDateOfBIrth.Content = MainBody.selectedFranchise?.Operator?.birthday.ToLongDateString();
            lblPlateNo.Content = MainBody.selectedFranchise?.LicenseNO;
            lblMTOPNo.Content = MainBody.selectedFranchise?.MTOPNo;

            imgProfilePic.ImageSource = MainBody.selectedFranchise?.Operator?.image?.GetSource();
            if (MainBody.selectedFranchise.Driver == null)
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
            HandleButtonClick(Modules.HISTORY, sender as Button);
            
        }

        private void btnCoding_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.CODING, sender as Button);
        }

        private void btnViolation_Click(object sender, RoutedEventArgs e)
        {
            
            HandleButtonClick(Modules.VIOLATION, sender as Button);
        }

        private void btnShareCapital_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.SHARECAPITAL, sender as Button);
        }

        private void btnLoan_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.LOAN, sender as Button);
        }

        private void btnLTLoan_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonClick(Modules.LTLOAN, sender as Button);
        }

        private void btnTransactionHistory_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
