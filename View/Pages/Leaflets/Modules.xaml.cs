using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SPTC_APP.View.Pages.Leaflets
{
    public partial class Modules : Window
    {
        public const string HISTORY = "HistoryGrid";
        public const string CODING = "CodingGrid";
        public const string VIOLATION = "ViolationGrid";
        public const string SHARECAPITAL = "ShareCapitalGrid";
        public const string LOAN = "LoanGrid";
        public const string LTLOAN = "LTLoanGrid";
        public const string TRANSFER = "TransferGrid";

        public Grid module;

        public Modules(string moduleName)
        {
            InitializeComponent();
            switch (moduleName)
            {
                case HISTORY:
                    module = HistoryGrid;
                    break;
                case CODING:
                    module = CodingGrid;
                    break;
                case VIOLATION:
                    module = ViolationGrid;
                    break;
                case SHARECAPITAL:
                    module = ShareCapitalGrid;
                    break;
                case LOAN:
                    module = LoanGrid;
                    break;
                case LTLOAN:
                    module = LTLoanGrid;
                    break;
                case TRANSFER:
                    module = TransferGrid;
                    break;
                default:
                    module = null; // Handle invalid module names here
                    break;
            }

        }

        public Grid Fetch()
        {
            if (module.Parent != null)
            {
                Grid currentParent = module.Parent as Grid;
                if (currentParent != null)
                {
                    currentParent.Children.Remove(module);
                }
            }
            this.Close();
            module.Visibility = Visibility.Visible;
            return module;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            TestButton.Background = Brushes.Green;
        }
    }

    
}
