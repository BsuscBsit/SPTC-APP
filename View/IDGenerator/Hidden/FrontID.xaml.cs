using System.Windows;
using System.Windows.Media;
using SPTC_APP.Objects;
using System.Windows.Controls;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for Pages.xaml
    /// </summary>
    public partial class FrontID : System.Windows.Window
    {
        private LayoutVersion layout;
        public FrontID(LayoutVersion layout = LayoutVersion.VER2023)
        {
            InitializeComponent();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
            this.layout = layout;
        }
        public void Populate(Franchise franchise, General type)
        {

            lblXPDate.Content = AppState.EXPIRATION_DATE;
            lblRegNum.Content = AppState.REGISTRATION_NO;
            lblChairman.Content = AppState.FetchChairman()?.name?.legalName;
            imgChairmanSignature.Source = AppState.FetchChairman()?.sign?.GetSource();

            if (type == General.OPERATOR)
            {
                lblName.Text = franchise.Operator.name.wholename.ToUpper();
                lblPosition.Text = type.ToString();
                if (franchise.Operator.image != null)
                {
                    imgID.Source = franchise.Operator.image.GetSource();
                }
            }
            else
            {
                lblName.Text = franchise.Driver.name.wholename.ToUpper();
                lblPosition.Text = "DRIVER";
                if (franchise.Driver.image != null)
                {
                    imgID.Source = franchise.Driver.image.GetSource();
                }
            }

        }

       

        private void window_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
            label1.FontSize = Scaler.PtToPx(9);
            label2.FontSize = Scaler.PtToPx(14);
            label3.FontSize = Scaler.PtToPx(14);
            label4.FontSize = Scaler.PtToPx(8);
            label5.FontSize = Scaler.PtToPx(8);

            label6.FontSize = Scaler.PtToPx(11);
            label7.FontSize = Scaler.PtToPx(9);

            lblRegNum.FontSize = Scaler.PtToPx(9);
            lblXPDate.FontSize = Scaler.PtToPx(11);
            lblChairman.FontSize = Scaler.PtToPx(11);

            viewbox2.Height = Scaler.InToDip(1);
            viewbox3.Height = Scaler.InToDip(0.32);
        }
        private void ver2023()
        {
            Color primary = (Color)ColorConverter.ConvertFromString("#1D314E");

            label1.FontSize = Scaler.PtToPx(9);
            label1.Foreground = Brushes.Black;
            label1.FontFamily = new FontFamily("Berlin Sans FB");
            label1.FontWeight = FontWeights.SemiBold;

            label2.FontSize = Scaler.PtToPx(14);
            label2.Foreground = new SolidColorBrush(primary);

            label3.FontSize = Scaler.PtToPx(14);
            label3.Foreground = new SolidColorBrush(primary);

            label4.Content = "Blk.1 Lot 8 Sitio Hulo, Brgy.Sapang Palay Proper";
            label4.FontSize = Scaler.PtToPx(9);
            label4.Foreground = Brushes.Black;
            label4.FontFamily = new FontFamily("Berlin Sans FB");
            label4.FontWeight = FontWeights.SemiBold;

            label5.FontSize = Scaler.PtToPx(9);
            label5.Foreground = Brushes.Black;
            label5.FontFamily = new FontFamily("Berlin Sans FB");
            label5.FontWeight = FontWeights.SemiBold;

            label6.FontSize = Scaler.PtToPx(11);
            label6.Foreground = Brushes.Black;
            label6.FontFamily = new FontFamily("Berlin Sans FB");
            label6.FontWeight = FontWeights.SemiBold;

            label7.FontSize = Scaler.PtToPx(9);
            label7.Foreground = Brushes.Black;

            lblRegNum.FontSize = Scaler.PtToPx(9);
            lblRegNum.Foreground = Brushes.Black;
            lblRegNum.FontFamily = new FontFamily("Berlin Sans FB");
            lblRegNum.FontWeight = FontWeights.SemiBold;

            lblXPDate.FontSize = Scaler.PtToPx(11);
            lblXPDate.Foreground = Brushes.Black;
            lblXPDate.FontFamily = new FontFamily("Berlin Sans FB");
            lblXPDate.FontWeight = FontWeights.SemiBold;

            lblChairman.FontSize = Scaler.PtToPx(11);
            lblChairman.Foreground = new SolidColorBrush(primary);

            lblPosition.FontFamily = new FontFamily("Berlin Sans FB");
            lblPosition.Fill = new SolidColorBrush(primary);

            lblName.Fill = Brushes.Black;

            viewbox2.Height = Scaler.InToDip(1);

            viewbox3.Height = Scaler.InToDip(0.32);
            viewbox3.Margin = new Thickness(10, 0, 10, 0);

            lblDesignerPetmalu.Content = "AllenLiao-Morado2023";
            hdiv1.Visibility = Visibility.Visible;
        }
        public enum LayoutVersion
        {
            VER2022, VER2023,
        }

    }
    
}
