using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
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
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for NewOperator.xaml
    /// </summary>
    public partial class NewOptr_Drv : Window
    {
        Franchise franchise;
        General type;
        private string closingMSG;
        public NewOptr_Drv(Franchise franchise, General type)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.franchise = franchise;
            this.type = type;
            lblTitle.Content = "NEW " + AppState.GetEnumDescription(type);
            dpBirthday.Text = DateTime.Now.ToString();
            dpBirthday.DisplayDate = DateTime.Now;
            DraggingHelper.DragWindow(topBar);
            tboxsFname.Focus();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            AppState.mainwindow?.displayToast(closingMSG);
            base.OnClosing(e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ControlWindow.ShowTwoway("Creating new " + AppState.GetEnumDescription(type).ToLower(), "Confirm?", Icons.NOTIFY))
            {
                if (type == General.DRIVER)
                {
                    Driver drv = new Driver();
                    drv.name = new Name();
                    drv.address = new Address();
                    drv.name.firstname = tboxsFname.Text;
                    drv.name.middlename = tboxsMname.Text;
                    drv.name.lastname = tboxsLname.Text;
                    drv.birthday = (DateTime)dpBirthday.SelectedDate;
                    drv.emergencyContact = tboxsContactNum.Text;
                    drv.name.sex = cbGender.SelectedIndex == 1;
                    drv.address.country = tboxsCountry.Text;
                    drv.address.province = tboxsProvince.Text;
                    drv.address.city = tboxsCity.Text;
                    drv.address.barangay = tboxsBarangay.Text;
                    drv.address.streetname = tboxStreetName.Text;
                    drv.address.UpdateAddressLines();
                    if (franchise != null)
                    {
                        franchise.Driver = drv;
                    } else
                    {
                        drv.Save();
                    }
                    (AppState.mainwindow as MainBody).ResetWindow(General.DRIVER);
                }
                else if (type == General.OPERATOR)
                {
                    Operator optr = (franchise?.Operator != null)? franchise.Operator:new Operator();
                    optr.name = new Name();
                    optr.address = new Address();
                    optr.name.firstname = tboxsFname.Text;
                    optr.name.middlename = tboxsMname.Text;
                    optr.name.lastname = tboxsLname.Text;
                    optr.birthday = (DateTime)dpBirthday.SelectedDate;
                    optr.emergencyContact = tboxsContactNum.Text;
                    optr.name.sex = cbGender.SelectedIndex == 1;
                    optr.address.country = tboxsCountry.Text;
                    optr.address.province = tboxsProvince.Text;
                    optr.address.city = tboxsCity.Text;
                    optr.address.barangay = tboxsBarangay.Text;
                    optr.address.streetname = tboxStreetName.Text;
                    optr.address.UpdateAddressLines();

                    if (franchise != null)
                    {
                        franchise.Operator = optr;
                        Ledger.ShareCapital capital = new Ledger.ShareCapital();
                        capital.WriteInto(franchise.Save(), DateTime.Now, 0, 0);
                        capital.Save();
                    } else
                    {
                        optr.Save();
                    }
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE);
                }
                else if (type == General.TRANSFER_FRANCHISE_OWNERSHIP)
                {
                    Franchise newFranchise = new Franchise();
                    newFranchise.BodyNumber = franchise.BodyNumber;
                    newFranchise.MTOPNo = franchise.MTOPNo;
                    newFranchise.LicenseNO = franchise.LicenseNO;
                    newFranchise.Operator = new Operator();
                    newFranchise.Operator.name = new Name();
                    newFranchise.Operator.address = new Address();
                    newFranchise.Operator.name.firstname = tboxsFname.Text;
                    newFranchise.Operator.name.middlename = tboxsMname.Text;
                    newFranchise.Operator.name.lastname = tboxsLname.Text;
                    newFranchise.Operator.birthday = (DateTime)dpBirthday.SelectedDate;
                    newFranchise.Operator.emergencyContact = tboxsContactNum.Text;
                    newFranchise.Operator.name.sex = cbGender.SelectedIndex == 1;
                    newFranchise.Operator.address.country = tboxsCountry.Text;
                    newFranchise.Operator.address.province = tboxsProvince.Text;
                    newFranchise.Operator.address.city = tboxsCity.Text;
                    newFranchise.Operator.address.barangay = tboxsBarangay.Text;
                    newFranchise.Operator.address.addressline1 = tboxStreetName.Text;
                    newFranchise.Operator.address.UpdateAddressLines();
                        
                    newFranchise.lastFranchiseId = franchise.id;
                    newFranchise.Save();
                    Ledger.ShareCapital capital = new Ledger.ShareCapital();
                    capital.WriteInto(newFranchise.id, DateTime.Now, franchise.SaveShareCapital(), 0);
                    capital.Save();
                    (AppState.mainwindow as MainBody).ResetWindow(General.FRANCHISE);
                    
                }
                if (franchise != null)
                {
                    franchise.Save();
                }
                closingMSG = "Newly added record was successfully saved.\nPlease refresh the view to see changes.";
                this.Close();
            }
        }

        private void initTextBoxes()
        {
            tboxsFname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "First name.", 0);
            tboxsMname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "Middle name.", 2);
            tboxsLname.DefaultTextBoxBehavior(LETTERPERIOD, false, gridToast, "Last name.", 3);
            tboxsContactNum.DefaultTextBoxBehavior(PHONE, true, gridToast, "Contact number.", 4);
            tboxsCountry.DefaultTextBoxBehavior(ALPHANUMERICDASH, false, gridToast, "Country.", 5);
            tboxsProvince.DefaultTextBoxBehavior(ALPHANUMERICDASH, false, gridToast, "Province.", 6);
            tboxsCity.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "Enter city or municipality.", 7);
            tboxsBarangay.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "Enter barangay or village.", 8);
            tboxStreetName.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "Additional .", 9);

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            closingMSG = "Adding new record was canceled.\n No data was saved.";
            this.Close();
        }
    }
}
