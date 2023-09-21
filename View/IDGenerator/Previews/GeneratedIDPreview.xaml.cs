using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SPTC_APP.Objects;

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
        }

        public void ReturnControl(GenerateID prev)
        {
            this.previous = prev;
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            (new PrintPreview()).Show();
            previous.Close();
            this.Close();
        }

        private void btnBack(object sender, RoutedEventArgs e)
        {
            previous.Show();
            this.Close();
        }

        private void btnContinue(object sender, RoutedEventArgs e)
        {
            PrintPreview print = new PrintPreview();
            print.NewID(id);
            print.Show();
            previous.Close();
            this.Close();
        }

        public void Save(ID id)
        {
            this.id = id;
            imgFront.Source = id.RenderFrontID().Source;
            imgBack.Source = id.RenderBackID().Source;
        }
    }

    public class ID
    {
        public Franchise franchise;
        private General type;
        public int FrontPrint = 0;
        public int BackPrint = 0;

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

        public System.Windows.Controls.Image RenderFrontID()
        {
            FrontID page = new FrontID();

            page.Populate(franchise, type);
            page.Show();


            int renderScale = 4; // Increase this value for higher resolution rendering

            var renderTargetBitmap = new RenderTargetBitmap((int)(page.ActualWidth * renderScale), (int)(page.ActualHeight * renderScale), 96 * renderScale, 96 * renderScale, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(page);

            var scaledBitmap = new TransformedBitmap(renderTargetBitmap, new ScaleTransform(1.0 / renderScale, 1.0 / renderScale));

            var image = new System.Windows.Controls.Image();
            image.Source = scaledBitmap;

            page.Close();

            return image;
        }


        public System.Windows.Controls.Image RenderBackID()
        {
            BackID page = new BackID();

            page.Populate(franchise, type);

            page.Show();

            int renderScale = 4; // Increase this value for higher resolution rendering

            var renderTargetBitmap = new RenderTargetBitmap((int)(page.ActualWidth * renderScale), (int)(page.ActualHeight * renderScale), 96 * renderScale, 96 * renderScale, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(page);

            var scaledBitmap = new TransformedBitmap(renderTargetBitmap, new ScaleTransform(1.0 / renderScale, 1.0 / renderScale));

            var image = new System.Windows.Controls.Image();
            image.Source = scaledBitmap;

            page.Close();

            return image;
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
        }
    }
}
