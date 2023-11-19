
using SPTC_APP.Objects;
using SPTC_APP.View.Pages.Input;
using SPTC_APP.View.Styling;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for FranchiseInformationView.xaml
    /// </summary>
    public partial class FranchiseInformationView : Window
    {
        private Button selectedButton = null;
        Grid tableview;
        public FranchiseInformationView(Grid tableview = null)
        {
            InitializeComponent();
            this.tableview = tableview;
            UpdateContent();
            spRecordButtons.Visibility = Visibility.Hidden;
            if((AppState.USER?.position?.accesses[0] ?? false))
            {
                btnChangeDriver.Visibility = Visibility.Visible;
            }
            if ((AppState.USER?.position?.accesses[8] ?? false))
            {
                btnDeleteFranchise.Visibility = Visibility.Visible;
            }
            if ((AppState.USER?.position?.accesses[5] ?? false))
            {
                btnEditFranchise.Visibility = Visibility.Visible;
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
            connumber.Content = MainBody.selectedFranchise?.Operator?.emergencyContact ?? "N/A";

            if (MainBody.selectedFranchise?.Operator?.image?.GetSource() != null)
            {
                imgProfilePic.ImageSource = MainBody.selectedFranchise?.Operator?.image?.GetSource();
            }
            else
            {
                imgProfilePic.ImageSource = new BitmapImage(new Uri("pack://application:,,,/SPTC APP;component/View/Images/icons/person.png"));
            }
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
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE);
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
            Grid f = (new Modules(moduleName, MainBody.selectedFranchise)).Fetch();
            if (moduleName == Modules.VIOLATION)
            {
                if (spRecordButtons.Visibility != Visibility.Visible)
                {
                    f.RowDefinitions[2] = new RowDefinition() { Height = new GridLength(0) };
                }
                ModuleGrid.Children.Add(f);
            }
            else
            {
                ModuleGrid.Children.Add(f);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            spRecordButtons.FadeOut(0.2, () => { spRecordButtons.Visibility = Visibility.Collapsed; });
            HandleButtonClick(Modules.HISTORY, sender as Button);
            
        }

        private void btnCoding_Click(object sender, RoutedEventArgs e)
        {
            spRecordButtons.FadeOut(0.2, () => { spRecordButtons.Visibility = Visibility.Collapsed; });
            HandleButtonClick(Modules.CODING, sender as Button);
        }

        private void btnViolation_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[12] ?? false))
            {

                if (MainBody.selectedFranchise?.Driver != null)
                {
                    addBtnInit("Add Record");
                } else
                {
                    addBtnInit();

                }
            } else
            {
                spRecordButtons.FadeOut(0.2, () => { spRecordButtons.Visibility = Visibility.Collapsed; });
            }
            HandleButtonClick(Modules.VIOLATION, sender as Button);
        }

        private void btnShareCapital_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[18] ?? false))
            {
                addBtnInit("Add Record");
            } else
            {
                addBtnInit();
            }
            HandleButtonClick(Modules.SHARECAPITAL, sender as Button);
        }

        private void btnLoanApply_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[18] ?? false))
            {
                addBtnInit("Add Loan");
            }
            else
            {
                addBtnInit();
            }
            HandleButtonClick(Modules.LOAN_APPLY, sender as Button);
        }

        private void btnLoan_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[18] ?? false))
            {
                addBtnInit("Add Record");
                if (MainBody.selectedFranchise.GetLoans().Count <= 0)
                {
                    //btnAddButton.Content = "APPLY FOR LOAN";
                    spRecordButtons.FadeOut(0.2, () => { spRecordButtons.Visibility = Visibility.Collapsed; });
                }

            }
            else
            {
                addBtnInit();
            }
            HandleButtonClick(Modules.LOAN, sender as Button);
        }

        private void btnLTLoan_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[18] ?? false))
            {
                addBtnInit("Add Record");
                if (MainBody.selectedFranchise.GetLTLoans().Count <= 0)
                {
                    spRecordButtons.FadeOut(0.2, () => { spRecordButtons.Visibility = Visibility.Collapsed; });
                } 
            }
            else
            {
                addBtnInit();
            }
            HandleButtonClick(Modules.LTLOAN, sender as Button);
        }

        private void btnTransactionHistory_Click(object sender, RoutedEventArgs e)
        {
            if ((AppState.USER?.position?.accesses[1] ?? false))
            {
                addBtnInit("Transfer", false);
            }
            else
            {
                addBtnInit();
            }
            HandleButtonClick(Modules.TRANSFER, sender as Button);
        }

        private void btnChangeDriver_Click(object sender, RoutedEventArgs e)
        {
            int result = ControlWindow.ShowThreway("Change Driver", "New or Existing?", "EXISTING", "NEW", Icons.DEFAULT);

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
            else if(selectedButton == btnLoanApply)
            {
                (new ApplyLoan(MainBody.selectedFranchise)).Show();
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
                if (MainBody.selectedFranchise.LoanBalance + MainBody.selectedFranchise.LongTermLoanBalance == 0)
                {
                    (new NewOptr_Drv(MainBody.selectedFranchise, General.TRANSFER_FRANCHISE_OWNERSHIP)).Show();
                } else
                {
                    ControlWindow.ShowStatic("Franchise transfer failed", "Cannot transfer franchise while having loan balance", Icons.DEFAULT);
                }
            }
        }

        private void btnDeleteFranchise_Click(object sender, RoutedEventArgs e)
        {
            if (ControlWindow.ShowTwoway("Deleting Franchise", "Are you sure you want to continue?", Icons.ERROR))
            {
                MainBody.selectedFranchise.delete();
                (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE);
                AppState.mainwindow?.displayToast("Franchise deleted", 5);
            }
        }

        private void btnEditFranchise_Click(object sender, RoutedEventArgs e)
        {
            (new InputFranchiseView(MainBody.selectedFranchise)).Show();
        }

        private void addBtnInit(string content = null, bool isAdd = true)
        {
            if (content != null)
            {
                spRecordButtons.FadeIn(0.2, () =>
                {
                    lblBtnAddContent.Content = content;
                    if (isAdd)
                    {
                        iconAdd.Visibility = Visibility.Visible;
                        iconTransfer.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        iconTransfer.Visibility = Visibility.Visible;
                        iconAdd.Visibility = Visibility.Hidden;
                    }
                    btnAddButton.Visibility = Visibility.Visible;
                });
            }
            else
            {
                spRecordButtons.FadeOut(0.2);
                spRecordButtons.Visibility = Visibility.Collapsed;
            }
        }
        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            if (selectedButton != null)
            {
                selectedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
