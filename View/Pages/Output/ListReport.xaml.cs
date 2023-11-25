using System;
using System.Data;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using MySql.Data.MySqlClient;
using SPTC_APP.Database;
using SPTC_APP.View;
using Window = System.Windows.Window;

namespace SPTC_APP.View.Pages.Output
{
    public partial class ListReport: Window
    {
        private readonly string _query;

        int pagenumber;
        int itemcount = 25;

        public ListReport(string query)
        {
            _query = query;
        }

        public void DisplayReport(string filename)
        {
            pagenumber = 0;

            while (true)
            {
                DataTable dataSource = GetYourData(_query, pagenumber, itemcount);

                if (dataSource.Rows.Count == 0)
                    break;

                this.Title = $"Report Window: {filename} - Page {pagenumber + 1}";

                DataGrid dataGrid = new DataGrid();
                dataGrid.ItemsSource = dataSource.DefaultView;


                this.Show();
                if (pagenumber > 0)
                {
                    if (!ControlWindow.ShowTwoway("Printing", $"Do you want to continue printing page {pagenumber + 1}", Icons.NOTIFY))
                    {
                        break;
                    }
                }
                Print(filename);

                pagenumber++;
            }
        }


        private DataTable GetYourData(string query, int page, int count)
        {
            DataTable dataTable = new DataTable();

            int offset = page * count;

            query = $"{query} LIMIT {offset}, {count};";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }


        private void Print(string filename)
        {
            string outputPath = AppState.OUTPUT_PATH + filename + "_page.xps";

            if (!Directory.Exists(AppState.OUTPUT_PATH))
            {
                Directory.CreateDirectory(AppState.OUTPUT_PATH);
            }

            using (XpsDocument xpsDoc = new XpsDocument(outputPath, FileAccess.Write))
            {
                XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

                PrintTicket printTicket = new PrintTicket();
                xpsWriter.Write(reportpage, printTicket);
            }
            this.Close();

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                XpsDocument xpsDocToPrint = new XpsDocument(outputPath, FileAccess.Read);
                FixedDocumentSequence fixedDocumentSequence = xpsDocToPrint.GetFixedDocumentSequence();

                // Get a paginator for the document sequence
                DocumentPaginator paginator = fixedDocumentSequence.DocumentPaginator;

                // Print the paginator
                printDialog.PrintDocument(paginator, "Printing SPTC Report");
                xpsDocToPrint.Close();
                ControlWindow.ShowStatic("Print Success", $"{filename} is printed", Icons.NOTIFY);
            }
        }

    }
}
