using System.Windows;
using SPTC_APP.Objects;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for BackID.xaml
    /// </summary>
    public partial class BackID : Window
    {
        public BackID()
        {
            InitializeComponent();
        }
        public void Populate(Franchise franchise, General type)
        {
            if (type == General.OPERATOR)
            {
                lblName.Content = franchise.Operator.name.wholename;

                lblLicense.Content = franchise.licenceNO;
                lblBodyNum.Content = franchise.bodynumber;
                lblEmePer.Content = franchise.Operator.emergencyPerson;
                lblAddressBuilding.Content = franchise.Operator.address.addressline1;
                lblAddressStreet.Content = franchise.Operator.address.addressline2;
                lblContact.Content = franchise.Operator.emergencyContact;
                if (franchise.Operator.signature != null)
                {
                    imgSign.Source = franchise.Operator.signature.GetSource();
                }
            }
            else
            {
                lblName.Content = franchise.Driver_day.name.wholename;

                lblLicense.Content = franchise.licenceNO;
                lblBodyNum.Content = franchise.bodynumber;
                lblEmePer.Content = franchise.Driver_day.emergencyPerson;
                lblAddressBuilding.Content = franchise.Driver_day.address.addressline1;
                lblAddressStreet.Content = franchise.Driver_day.address.addressline2;
                lblContact.Content = franchise.Driver_day.emergencyContact;
                if (franchise.Driver_day.signature != null)
                {
                    imgSign.Source = franchise.Driver_day.signature.GetSource();
                }
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            label1.FontSize = Scaler.PtToPx(11);
            label2.FontSize = Scaler.PtToPx(11);

            label3.FontSize = Scaler.PtToPx(14);
            label4.FontSize = Scaler.PtToPx(12);
            label5.FontSize = Scaler.PtToPx(12);

            label6.FontSize = Scaler.PtToPx(10);
            label7.FontSize = Scaler.PtToPx(11);
            label8.FontSize = Scaler.PtToPx(11);
            label9.FontSize = Scaler.PtToPx(10);

            lblLicense.FontSize = Scaler.PtToPx(12);
            lblXPDate.FontSize = Scaler.PtToPx(12);
            lblBodyNum.FontSize = Scaler.PtToPx(72);
            lblEmePer.FontSize = Scaler.PtToPx(11);
            lblAddressBuilding.FontSize = Scaler.PtToPx(10);
            lblAddressStreet.FontSize = Scaler.PtToPx(10);
            lblContact.FontSize = Scaler.PtToPx(11);
            lblName.FontSize = Scaler.PtToPx(12);

            imgSign.Height = Scaler.InToDip(0.6);
            //viewbox1.Height = Scaler.InToDip();
        }



    }

}
