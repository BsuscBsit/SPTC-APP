
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using static SPTC_APP.View.Controls.TextBoxHelper.AllowFormat;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for ChangeOperator.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        private Driver dholder;
        private Operator oholder;
        Franchise franchise;
        General type;
        public EditProfile(Franchise franchise, dynamic lholder, General type)
        {
            InitializeComponent();
            initTextBoxes();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); AppState.mainwindow?.Hide(); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            
            this.franchise = franchise;
            this.type = type;
            if(type == General.DRIVER)
            {
                Driver drv = lholder as Driver;
                tboxsFname.Text = drv.name?.firstname;
                tboxsMname.Text = drv.name?.middlename;
                tboxsLname.Text = drv.name?.lastname;
                dpBirthday.DisplayDate = drv.birthday;
                dpBirthday.DataContext = drv.birthday;
                dpBirthday.Text = drv.birthday.ToString();
                cbGender.SelectedIndex = drv.name?.sex ?? false ? 1: 0;
                tboxsContactNum.Text = drv.emergencyContact;
                tboxsCountry.Text = drv.address?.country;
                tboxsProvince.Text = drv.address?.province;
                tboxsCity.Text = drv.address?.city;
                tboxsBarangay.Text = drv.address?.barangay;
                tboxStreetName.Text = drv.address?.addressline1;
                this.dholder = drv;
            } else if (type == General.OPERATOR)
            {
                Operator optr = lholder as Operator;
                tboxsFname.Text = optr.name?.firstname;
                tboxsMname.Text = optr.name?.middlename;
                tboxsLname.Text = optr.name?.lastname;
                dpBirthday.DisplayDate = optr.birthday;
                dpBirthday.DataContext = optr.birthday;
                dpBirthday.Text = optr.birthday.ToString();
                cbGender.SelectedIndex = optr.name?.sex ?? false ? 1 : 0;
                tboxsContactNum.Text = optr.emergencyContact;
                tboxsCountry.Text = optr.address?.country;
                tboxsProvince.Text = optr.address?.province;
                tboxsCity.Text = optr.address?.city;
                tboxsBarangay.Text = optr.address?.barangay;
                tboxStreetName.Text = optr.address?.addressline1;
                this.oholder = optr;
            }
            DraggingHelper.DragWindow(topBar);
            tboxsFname.Focus();
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

            if (ControlWindow.ShowTwoway("Updating Fields.", "Confirm to save changes.", Icons.NOTIFY))
            {
                if (type == General.DRIVER)
                {
                    Driver drv = (franchise?.Driver == null)? dholder: franchise.Driver;
                    if (drv.name == null)
                        drv.name = new Name();
                    if(drv.address == null)
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
                        franchise.Save();
                    }
                    else
                    {
                        drv.Save();

                    }
                    (AppState.mainwindow as MainBody).ResetWindow(General.DRIVER);
                }
                else if (type == General.OPERATOR)
                {
                    Operator optr = (franchise?.Operator == null) ? oholder : franchise.Operator;
                    if (optr.name == null)
                        optr.name = new Name();
                    if (optr.address == null)
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
                        franchise.Save();
                    }
                    else
                    {
                        optr.Save();

                    }
                    (AppState.mainwindow as MainBody).ResetWindow(General.OPERATOR);
                }
                this.Close();
            } else
            {

            }


        }
        private void initTextBoxes()
        {
            tboxsFname.DefaultTextBoxBehavior(ALPHABETS, false, gridToast, "First name.", 0);
            tboxsMname.DefaultTextBoxBehavior(ALPHABETS, false, gridToast, "Middle name.", 2);
            tboxsLname.DefaultTextBoxBehavior(ALPHABETS, false, gridToast, "Last name.", 3);
            tboxsContactNum.DefaultTextBoxBehavior(PHONENUMBER, true, gridToast, "Contact number.", 4);
            tboxsCountry.DefaultTextBoxBehavior(ALPHANUMERIC, false, gridToast, "First Name", 0);
            tboxsProvince.DefaultTextBoxBehavior(ALPHANUMERIC, false, gridToast, "First Name", 0);
            tboxsCity.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "First Name", 0);
            tboxsBarangay.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "First Name", 0);
            tboxStreetName.DefaultTextBoxBehavior(ADDRESS, false, gridToast, "First Name", 0);

        }
    }
}
