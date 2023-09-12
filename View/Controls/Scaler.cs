using System.Windows;
using System.Windows.Media;

namespace SPTC_APP.View
{
    internal class Scaler
    {
        public static double InToDip(double inches)
        {
            return inches * 96;
        }
        public static double PtToPx(double pt)
        {
            return (pt / 72) * 96;
        }
    }
}
