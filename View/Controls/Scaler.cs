using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SPTC_APP.View
{
    static class Scaler
    {
        public static double InToDip(double inches)
        {
            return inches * 96;
        }

        public static double PtToPx(double pt)
        {
            return (pt / 72) * 96;
        }

        public static int RoundUp(double number)
        {
            return (int)Math.Ceiling(number / 100) * 100;
        }

        public static string NumberShorthand(double value, string excludeabvr = null)
        {
            string res = ShortHand(value, excludeabvr);
            int indexOfE = res.IndexOf("E");
            if (indexOfE != -1)
            {
                res = res.Substring(0, indexOfE) + res.Substring(res.Length-1);
            }
            return res;
        }

        private static string ShortHand(double value, string abvr)
        {
            string exclude = abvr != null ? abvr.ToUpper() : string.Empty;
            bool notExcluded(string ABVR)
            {
                return exclude.Contains(ABVR) ? false : true;
            };
            if (Math.Abs(value) >= 1e12)
            {
                return (value / 1e12).ToString("0.###E+00") + "T";
            }
            else if (Math.Abs(value) >= 1e9)
            {
                return (value / 1e9).ToString("0.###E+00") + "B";
            }
            else if (Math.Abs(value) >= 1e6 && notExcluded("M"))
            {
                return (value / 1e6).ToString("0.###E+00") + "M";
            }
            else if (Math.Abs(value) >= 1e3 && notExcluded("K"))
            {
                return (value / 1e3).ToString("0.###E+00") + "K";
            }
            else
            {
                return value.ToString("0.###");
            }
        }
    }
}
