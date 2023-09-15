using System.Drawing;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Image = System.Windows.Controls.Image;

namespace SPTC_APP.View.IDGenerator.Hidden
{
    /// <summary>
    /// Interaction logic for PrintPaper.xaml
    /// </summary>
    public partial class PrintPaper : Window
    {
        //double dpiScale = DpiHelper.GetDpiScale();
        Border[] borders;
        public PrintPaper()
        {
            InitializeComponent();
            ChangeHW();
            borders = new Border[4];
            borders[0] = brd1;
            borders[1] = brd2;
            borders[2] = brd3;
            borders[3] = brd4;

        }

        private void ChangeHW()
        {

            PresentationSource source = PresentationSource.FromVisual(this);

            if (source?.CompositionTarget != null)
            {
                //double dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                //double dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;

                this.Width = 8.5 * 96.0;  // 8.5 inches * DPI in X direction
                this.Height = 11 * 96.0;  // 11 inches * DPI in Y direction
            }

        }

        public bool StartPrint(ID[] arr, bool isFront)
        {
            if (isFront)
            {
                for (int i = 0; i < borders.Length; i++)
                {
                    borders[i].Child = new Image();
                    if (arr[i] != null)
                    {
                        borders[i].Child = arr[i].RenderFrontID();
                    }
                }

                /* Pwede gamitin sa paggawa ng panibagong feature "print alignment program" ganun.
                 * Pwede iadjust yung paper size, mag test print kahit yung mismong border lang
                 * ng ID. Tapos ito, pwede i-adjust.
                 * Itong margin na to ang iaadjust para maitulak yung front page paloob.*/
                Thickness newMargin = new Thickness(AppState.PRINT_AJUSTMENTS, 0, 0, 0);
                frontPage.Margin = newMargin;


                frontPage.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {

                for (int i = 0; i < borders.Length; i++)
                {
                    borders[i].Child = new Image();
                    if (arr[i] != null)
                    {

                        borders[i].Child = arr[i].RenderBackID();
                    }

                }

                frontPage.HorizontalAlignment = HorizontalAlignment.Right;
            }

            string page = (isFront) ? "Front" : "Back";
            string outputPath = AppState.OUTPUT_PATH + page + "_page.xps";

            // Check if the output directory exists, and create it if it doesn't
            if (!Directory.Exists(AppState.OUTPUT_PATH))
            {
                Directory.CreateDirectory(AppState.OUTPUT_PATH);
            }

            using (XpsDocument xpsDoc = new XpsDocument(outputPath, FileAccess.Write))
            {
                XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

                PrintTicket printTicket = new PrintTicket(); // Optional, you can configure print settings here
                xpsWriter.Write(frontPage, printTicket);
            }

            // Print the XPS file
            PrintDialog printDialog = new PrintDialog();// Set the desired printer name

            if (printDialog.ShowDialog() == true)
            {
                XpsDocument xpsDocToPrint = new XpsDocument(outputPath, FileAccess.Read);
                FixedDocumentSequence fixedDocumentSequence = xpsDocToPrint.GetFixedDocumentSequence();

                // Get a paginator for the document sequence
                DocumentPaginator paginator = fixedDocumentSequence.DocumentPaginator;

                // Print the paginator
                printDialog.PrintDocument(paginator, "Printing XPS Document");
                xpsDocToPrint.Close();

                ControlWindow.Show("Success", page + " page was printed successfully!" + ((isFront) ? "\nPress OK to print the next page." : ""), Icons.NOTIFY);
                foreach (ID id in arr)
                {
                    if (id != null)
                    {
                        if (isFront)
                        {
                            id.incrementFrontPrint();
                        }
                        else
                        {
                            id.incrementBackPrint();
                        }
                    }
                }

                return true;
            }
            else
            {
                ControlWindow.Show("Failed", page + " page was not printed.", Icons.ERROR);
                return false;
            }
        }
    }
}
