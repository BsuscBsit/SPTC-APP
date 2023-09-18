using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Controls;

namespace SPTC_APP.View
{
    public partial class Test : System.Windows.Window
    {

        public Test()
        {
            InitializeComponent();
            EventLogger.Post("VIEW :: TEST Window");
            tabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        //Test for VideoCamera
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            using (var capture = new VideoCapture(0))
            {
                Mat image = new Mat();
                while (true)
                {
                    capture.Read(image);
                    var bitmap = BitmapConverter.ToBitmap(image);
                    var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    imageRender.Source = bitmapSource;
                    Cv2.WaitKey(1);
                }
            }*/
        }

        //Test for Signature pad

        //Test for List
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                if (selectedTab.Header.ToString() == "Test Login")
                {
                    UpdateLogin();
                }
                if (selectedTab.Header.ToString() == "Test List")
                {
                    btnFranchise_Click(null, null);
                }
            }

        }

        private void UpdateLogin()
        {
            lblName.Content = $"My Name is {AppState.USER.name.wholename}";
            lblAddress.Content = $"I am from {AppState.USER.address}";
            lblPosition.Content = $"I am a {AppState.USER.position}";
        }


        //SAMPLE ONLY, use DataTable on next Iterati

        /*foreach(Franchise f in fetchedData)
        {
            if(f.Driver_day != null)
            {
                (new GenerateID(f, true)).Show();
            }
        }*/

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void btnFranchise_Click(object sender, RoutedEventArgs e)
        {
            List<Franchise> fetchedData = (new TableObject<Franchise>(Table.FRANCHISE)).data;
            DataGrid dataGrid = new DataGrid();

            DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(dataGrid);

            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("BodyNumber", "###", width: 30),
                new ColumnConfiguration("LicenseNO", "Plate Number", width: 60, backgroundColor: Brushes.Yellow),
                new ColumnConfiguration("Operator", "Operator name"),
                new ColumnConfiguration("Driver", "Driver name", fontWeight: FontWeights.Black, width: 100, maxWidth: 150),
            };
            dataGridHelper.DesignGrid(fetchedData, columnConfigurations);
            DatagridList.Children.Add(dataGrid);

            dataGrid.SelectionChanged += dgList_SelectionChanged;
        }

        private void btnDriver_Click(object sender, RoutedEventArgs e)
        {
            List<Driver> fetchedData = (new TableObject<Driver>(Table.DRIVER)).data;
            DataGrid dataGrid = new DataGrid();

            DataGridHelper<Driver> dataGridHelper = new DataGridHelper<Driver>(dataGrid);

            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("name", "FullName", width: 30),
                new ColumnConfiguration("birthday", "Date of Birth", width: 60, backgroundColor: Brushes.Yellow),
                new ColumnConfiguration("emergencyPerson", "Em Person"),
                new ColumnConfiguration("emergencyContact", "Em Contact", fontWeight: FontWeights.Black, width: 100, maxWidth: 150),
                new ColumnConfiguration("isDayShift", "Is Day Shift")
            };
            dataGridHelper.DesignGrid(fetchedData, columnConfigurations);
            
            DatagridList.Children.Add(dataGrid);

            dataGrid.SelectionChanged += dgList_SelectionChanged;
        }

        private void btnOperator_Click(object sender, RoutedEventArgs e)
        {
            List<Operator> fetchedData = (new TableObject<Operator>(Table.OPERATOR)).data;
            DataGrid dataGrid = new DataGrid();

            DataGridHelper<Operator> dataGridHelper = new DataGridHelper<Operator>(dataGrid);

            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("name", "FullName", width: 30),
                new ColumnConfiguration("birthday", "Date of Birth", width: 60, backgroundColor: Brushes.Yellow),
                new ColumnConfiguration("emergencyPerson", "Em Person"),
                new ColumnConfiguration("emergencyContact", "Em Contact", fontWeight: FontWeights.Black, width: 100, maxWidth: 150),
            };
            dataGridHelper.DesignGrid(fetchedData, columnConfigurations);
            
            DatagridList.Children.Add(dataGrid);

            dataGrid.SelectionChanged += dgList_SelectionChanged;
        }

        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            List<Employee> fetchedData = (new TableObject<Employee>(Table.EMPLOYEE)).data;
            DataGrid dataGrid = new DataGrid();

            DataGridHelper<Employee> dataGridHelper = new DataGridHelper<Employee>(dataGrid);

            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("name", "FullName", width: 30),
                new ColumnConfiguration("position", "Position", width: 60, backgroundColor: Brushes.Yellow)
            };
            dataGridHelper.DesignGrid(fetchedData, columnConfigurations);
            
            DatagridList.Children.Add(dataGrid);

            dataGrid.SelectionChanged += dgList_SelectionChanged;
        }

        //PLANS ON EDITING FIELDS
        //first create a class that modifies
        //then attach a 
    }
}