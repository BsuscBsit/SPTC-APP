using System;
using System.Drawing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SPTC_APP.Objects;
using Brushes = System.Windows.Media.Brushes;

namespace SPTC_APP.View
{
    /// <summary>
    /// Interaction logic for GeneratedIDPreview.xaml
    /// </summary>
    public partial class GeneratedIDPreview : Window
    {
        private GenerateID previous;
        private ID id;


        public GeneratedIDPreview()
        {
            InitializeComponent();
            AppState.mainwindow?.Hide();
            ContentRendered += (sender, e) => { AppState.WindowsCounter(true, sender); };
            Closed += (sender, e) => { AppState.WindowsCounter(false, sender); };
        }

        public void ReturnControl(GenerateID prev)
        {
            this.previous = prev;
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            (new PrintPreview()).Show();
            previous?.Close();
            this.Close();
        }

        private void btnBack(object sender, RoutedEventArgs e)
        {
            previous?.Show();
            this.Close();
        }

        private void btnContinue(object sender, RoutedEventArgs e)
        {
            PrintPreview print = new PrintPreview();
            print.NewID(id);
            print.Show();
            previous?.Close();
            this.Close();
        }

        public void Save(ID id)
        {
            this.id = id;
            imgFront.Source = id.RenderFrontID();
            imgBack.Source = id.RenderBackID();
        }
    }

    public class ID
    {
        public Franchise franchise;
        public General type;
        public int FrontPrint = 0;
        public int BackPrint = 0;
        public bool isSaved = false;
        private System.Windows.Controls.Image front;
        private System.Windows.Controls.Image back;


        public ID(Franchise franchise, General type)
        {
            this.franchise = franchise;
            this.type = type;
            EventLogger.Post($"OUT :: ID Generation :  Body#: {franchise.BodyNumber} type: {type.ToString()}");
        }

        public void incrementFrontPrint()
        {
            FrontPrint = FrontPrint + 1;
        }
        public void incrementBackPrint()
        {
            BackPrint = BackPrint + 1;
        }

        public void Render(bool isFront)
        {
            
            if (isFront)
            {
                FrontID page = new FrontID();
                page.Populate(franchise, type);
                page.Show();
                int renderScale = 4;

                var renderTargetBitmap = new RenderTargetBitmap((int)(page.DesiredSize.Width * renderScale), (int)(page.DesiredSize.Height * renderScale), 96 * renderScale, 96 * renderScale, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(page);

                var image = new System.Windows.Controls.Image();
                image.Source = renderTargetBitmap;

                page.Close();

                front = image;
            } else
            {
                BackID page = new BackID();

                page.Populate(franchise, type);
                page.Show();

                int renderScale = 4;

                var renderTargetBitmap = new RenderTargetBitmap((int)(page.DesiredSize.Width * renderScale), (int)(page.DesiredSize.Height * renderScale), 96 * renderScale, 96 * renderScale, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(page);

                var image = new System.Windows.Controls.Image();
                image.Source = renderTargetBitmap;

                page.Close();

                back = image;
            }
        }

        public ImageSource RenderFrontID()
        {
            if (front == null)
            {
                Render(true);
            }

            return front.Source;
        }
        public ImageSource RenderBackID()
        {
            if (back == null)
            {
                Render(false);
            }

            return back.Source;
        }

        public void SaveInfo()
        {
            franchise.Save();

            IDHistory history = new IDHistory();
            if(type == General.OPERATOR)
            {
                history.WriteInto(franchise.Operator.id, General.OPERATOR, franchise.Operator.name.id);
            } else
            {
                history.WriteInto(franchise.Driver.id, General.DRIVER, franchise.Driver.name.id);
            }
            history.Save();
            isSaved = true;
        }
    }
}
