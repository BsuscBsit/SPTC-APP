
using SPTC_APP.Objects;
using System;
using System.ComponentModel;
using System.Windows;

namespace SPTC_APP.View.Pages.Input
{
    /// <summary>
    /// Interaction logic for ChangeOperator.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        Franchise franchise;
        General type;
        public EditProfile(Franchise franchise, General type)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.franchise = franchise;
            this.type = type;
            AppState.mainwindow?.Hide();
            if(type == General.DRIVER)
            {
                Driver drv = franchise.Driver;
                tboxsFname.Text = drv.name.firstname;
                tboxsMname.Text = drv.name.middlename;
                tboxsLname.Text = drv.name.lastname;
                dpBirthday.DisplayDate = drv.birthday;
                dpBirthday.DataContext = drv.birthday;
                dpBirthday.Text = drv.birthday.ToString();
                cbGender.SelectedIndex = drv.name.sex ? 1: 0;
                tboxsContactNum.Text = drv.emergencyContact;
                tboxsCountry.Text = drv.address.country;
                tboxsProvince.Text = drv.address.province;
                tboxsCity.Text = drv.address.city;
                tboxsBarangay.Text = drv.address.barangay;
                tboxStreetName.Text = drv.address.streetname;
            } else if (type == General.OPERATOR)
            {
                Operator optr = franchise.Operator;
                tboxsFname.Text = optr.name.firstname;
                tboxsMname.Text = optr.name.middlename;
                tboxsLname.Text = optr.name.lastname;
                dpBirthday.DisplayDate = optr.birthday;
                dpBirthday.DataContext = optr.birthday;
                dpBirthday.Text = optr.birthday.ToString();
                cbGender.SelectedIndex = optr.name.sex ? 1 : 0;
                tboxsContactNum.Text = optr.emergencyContact;
                tboxsCountry.Text = optr.address.country;
                tboxsProvince.Text = optr.address.province;
                tboxsCity.Text = optr.address.city;
                tboxsBarangay.Text = optr.address.barangay;
                tboxStreetName.Text = optr.address.streetname;
            }
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

            if (ControlWindow.ShowDialog("Updating Fields.", "Confirm to save changes.", Icons.NOTIFY))
            {
                if (type == General.DRIVER)
                {
                    Driver drv = franchise.Driver;
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
                }
                else if (type == General.OPERATOR)
                {
                    Operator optr = franchise.Operator;
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
                }
                franchise.Save();
                this.Close();
            } else
            {

            }


        }
    }
}
