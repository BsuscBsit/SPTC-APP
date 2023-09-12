using SPTC_APP.Objects;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for Pages.xaml
    /// </summary>
    public partial class FrontID : System.Windows.Window
    {
        public FrontID()
        {
            InitializeComponent();
        }
        public void Populate(Franchise franchise, General type)
        {

            lblXPDate.Content = AppState.EXPIRATION_DATE;
            lblChairman.Content = AppState.CHAIRMAN;
            lblRegNum.Content = AppState.REGISTRATION_NO;

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
                lblName.Text = franchise.Driver_day.name.wholename.ToUpper();
                lblPosition.Text = "DRIVER";
                if (franchise.Driver_day.image != null)
                {
                    imgID.Source = franchise.Driver_day.image.GetSource();
                }
            }

        }



        private void window_Loaded(object sender, System.Windows.RoutedEventArgs e)
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

            viewbox2.Height = Scaler.InToDip(0.90);
            viewbox3.Height = Scaler.InToDip(0.32);

        }
    }
}
