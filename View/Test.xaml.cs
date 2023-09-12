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
        List<Franchise> fetchedData;
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
                    UpdateList();
                }
            }

        }

        private void UpdateLogin()
        {
            lblName.Content = $"My Name is {AppState.USER.name.wholename}";
            lblAddress.Content = $"I am from {AppState.USER.address}";
            lblPosition.Content = $"I am a {AppState.USER.position}";
        }


        //SAMPLE ONLY, use DataTable on next Iteration
        //Problematic because of Objects
        private async void UpdateList()
        {
            fetchedData = await Task.Run(() =>
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();

                    List<Franchise> franchises = new List<Franchise>();
                    franchises.AddRange(Retrieve.GetData<Franchise>(Table.FRANCHISE, Select.ALL, Where.ALL_NOTDELETED));
                    
                    return franchises;
                }
            });

            // Assuming you have a List<Franchise> fetchedData

            DataGrid dataGrid = new DataGrid();

            DataGridHelper<Franchise> dataGridHelper = new DataGridHelper<Franchise>(dataGrid);

            List<ColumnConfiguration> columnConfigurations = new List<ColumnConfiguration>
            {
                new ColumnConfiguration("bodynumber", "###", width: 30),
                new ColumnConfiguration("licenceNO", "Plate Number", width: 60, backgroundColor: Brushes.Yellow),
                new ColumnConfiguration("Operator", "Operator name"),
                new ColumnConfiguration("Driver_day", "Driver name", fontWeight: FontWeights.Black, width: 100, maxWidth: 150),

            };
            dataGridHelper.DesignGrid(fetchedData, columnConfigurations);

            DatagridList.Children.Add(dataGrid);
            
            dataGrid.SelectionChanged += dgList_SelectionChanged;
            //dataGrid.PreviewMouseLeftButtonDown += dgList_PreviewMouseLeftButtonDown;
        }

        /*foreach(Franchise f in fetchedData)
        {
            if(f.Driver_day != null)
            {
                (new GenerateID(f, true)).Show();
            }
        }*/

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            if (grid.SelectedCells.Count > 0)
            {
                (new GenerateID(fetchedData[grid.SelectedIndex], true)).Show();
            }
        }

        //PLANS ON EDITING FIELDS
        //first create a class that modifies
        //then attach a 
    }
}