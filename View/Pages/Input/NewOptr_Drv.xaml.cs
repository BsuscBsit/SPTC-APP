using SPTC_APP.Objects;
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
    /// Interaction logic for NewOperator.xaml
    /// </summary>
    public partial class NewOptr_Drv : Window
    {
        Franchise franchise;
        General type;
        public NewOptr_Drv(Franchise franchise, General type)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            AppState.mainwindow?.Hide();
            this.franchise = franchise;
            this.type = type;
            lblTitle.Content = "NEW " + AppState.GetEnumDescription(type);
            dpBirthday.Text = DateTime.Now.ToString();
            dpBirthday.DisplayDate = DateTime.Now;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            AppState.mainwindow?.Show();
            base.OnClosing(e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ControlWindow.ShowDialogStatic("Creating new " + AppState.GetEnumDescription(type).ToLower(), "Confirm?", Icons.NOTIFY))
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
                    franchise.Driver = drv;
                }
                else if (type == General.OPERATOR)
                {
                    Operator optr = new Operator();
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
                    franchise.Operator = optr;
                }
                franchise.Save();
                this.Close();
            }
            else
            {

            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
