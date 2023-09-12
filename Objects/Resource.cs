using System;

using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media.Imaging;



namespace SPTC_APP.Objects
{
    public class Resource
    {
        public static class BitmapConversion
        {
            public static BitmapSource ToBitmapSource(Bitmap bitmap)
            {
                IntPtr hBitmap = bitmap.GetHbitmap();
                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        System.Windows.Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    NativeMethods.DeleteObject(hBitmap);
                }
            }

            public static Bitmap ToBitmap(BitmapSource bitmapSource)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(stream);

                    using (Bitmap bitmap = new Bitmap(stream))
                    {
                        return new Bitmap(bitmap);
                    }
                }
            }
        }

        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool DeleteObject(IntPtr hObject);
        }

    }
}
