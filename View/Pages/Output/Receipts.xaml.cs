using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
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
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for Receipts.xaml
    /// </summary>
    public partial class Receipts : Window
    {
        private List<object> data;
        private string filename;
        public Receipts(string filename)
        {
            InitializeComponent();
            this.filename = filename;
        }

        public void Populate(List<object> reportList)
        {
            this.data = reportList;
            lblTitle.Content = "SPTC RECEIPT : " + filename;




        }

        public void StartPrint()
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
                xpsWriter.Write(receiptpage, printTicket);
            }

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                XpsDocument xpsDocToPrint = new XpsDocument(outputPath, FileAccess.Read);
                FixedDocumentSequence fixedDocumentSequence = xpsDocToPrint.GetFixedDocumentSequence();

                // Get a paginator for the document sequence
                DocumentPaginator paginator = fixedDocumentSequence.DocumentPaginator;

                // Print the paginator
                printDialog.PrintDocument(paginator, "Printing XPS Document");
                xpsDocToPrint.Close();
            }
        }
    }
}
