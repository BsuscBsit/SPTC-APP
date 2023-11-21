using System.Windows;
using System.Windows.Media;
using SPTC_APP.Objects;
using System.Windows.Controls;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for BackID.xaml
    /// </summary>
    public partial class BackID : Window
    {
        LayoutVersion layout;
        
        public BackID(LayoutVersion layout = LayoutVersion.VER2023)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.layout = layout;
        }
        public void Populate(Franchise franchise, General type)
        {
            if (type == General.OPERATOR)
            {
                lblName.Content = franchise.Operator.name?.legalName?.ToUpper();

                lblLicense.Content = franchise.PlateNo;
                lblBodyNum.Content = franchise.BodyNumber;
                lblEmePer.Content = franchise.Operator.emergencyPerson;
                lblAddressBuilding.Content = franchise.Operator.address?.addressline1;
                lblAddressStreet.Content = franchise.Operator.address?.addressline2;
                lblContact.Content = franchise.Operator.emergencyContact;
                if (franchise.Operator.signature != null)
                {
                    imgSign.Source = franchise.Operator.signature?.GetSource();
                }
            }
            else
            {
                lblName.Content = franchise.Driver.name?.legalName?.ToUpper();

                lblLicense.Content = franchise.PlateNo;
                lblBodyNum.Content = franchise.BodyNumber;
                lblEmePer.Content = franchise.Driver.emergencyPerson;
                lblAddressBuilding.Content = franchise.Driver.address?.addressline1;
                lblAddressStreet.Content = franchise.Driver.address?.addressline2;
                lblContact.Content = franchise.Driver.emergencyContact;
                if (franchise.Driver.signature != null)
                {
                    imgSign.Source = franchise.Driver.signature?.GetSource();
                }
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (layout)
            {
                case LayoutVersion.VER2022:
                    ver2022();
                break;
                case LayoutVersion.VER2023:
                    ver2023();
                break;
            }
        }
        
        private void ver2022()
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
            lbodyView.Height = Scaler.PtToPx(85);
            lblEmePer.FontSize = Scaler.PtToPx(11);
            lblAddressBuilding.FontSize = Scaler.PtToPx(10);
            lblAddressStreet.FontSize = Scaler.PtToPx(10);
            lblContact.FontSize = Scaler.PtToPx(11);
            lblName.FontSize = Scaler.PtToPx(12);

            imgSign.Height = Scaler.InToDip(0.6);
        }
        private void ver2023()
        {
            Color primary = (Color)ColorConverter.ConvertFromString("#1D314E");

            grpEmePerHead.Visibility = Visibility.Collapsed;
            grpEmePer.Visibility = Visibility.Collapsed;
            grpAddress.Visibility = Visibility.Collapsed;
            grpContact.Visibility = Visibility.Collapsed;
            hdiv3.Visibility = Visibility.Visible;

            Grid.SetRow(grpLicense, 1);
            Grid.SetRow(grpExpDate, 2);
            Grid.SetRow(grpBodyNo, 0);

            label1.FontSize = Scaler.PtToPx(10);
            label1.FontWeight = FontWeights.Bold;
            label1.Foreground = new SolidColorBrush(primary);
            hdiv1.Height = 2;

            label2.FontSize = Scaler.PtToPx(10);
            label2.FontWeight = FontWeights.Bold;
            label2.Foreground = new SolidColorBrush(primary);
            hdiv2.Height = 2;

            label3.Content = "SPTC No.";
            label3.FontSize = Scaler.PtToPx(12);
            label3.FontWeight = FontWeights.Bold;
            label3.Foreground = new SolidColorBrush(primary);
            label3.Margin = new Thickness(Scaler.InToDip(0.1), 0, 0, Scaler.InToDip(0.2));

            label9.FontSize = Scaler.PtToPx(10);
            label9.FontWeight = FontWeights.Bold;
            label9.Foreground = new SolidColorBrush(primary);
            hdiv7.Height = 2;

            lbodyView.Height = Scaler.PtToPx(100);
            lblLicense.FontSize = Scaler.PtToPx(15);
            lblXPDate.FontSize = Scaler.PtToPx(10);
            lblName.FontSize = Scaler.PtToPx(10);


        }
        public enum LayoutVersion
        {
            VER2022, VER2023,
        }

    }

}
