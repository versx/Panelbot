using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace T.Functions
{
    public class ScreenManager
    {
        static ScreenManager instance;
        public static ScreenManager Instance
        {
            get
            {
                lock (typeof(ScreenManager))
                {
                    if (instance == null)
                    {
                        instance = new ScreenManager();
                    }
                    return instance;
                }
            }
        }

        public Image CaptureScreen(bool captureMouse = true)
        {
            IntPtr desktop = NativeMethods.User32.GetDesktopWindow();
            // Get the hDC of the target window
            IntPtr hdcSrc = NativeMethods.User32.GetWindowDC(desktop);
            // Get the size
            NativeMethods.User32.RECT windowRect = new NativeMethods.User32.RECT();
            NativeMethods.User32.GetWindowRect(desktop, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // Create a device context we can copy to
            IntPtr hdcDest = NativeMethods.Gdi32.CreateCompatibleDC(hdcSrc);
            // Create a bitmap we can copy it to using GetDeviceCaps to get the width/height
            IntPtr hBitmap = NativeMethods.Gdi32.CreateCompatibleBitmap(hdcSrc, width, height);
            // Select the bitmap object
            IntPtr hOld = NativeMethods.Gdi32.SelectObject(hdcDest, hBitmap);

            // Bitblt over
            NativeMethods.Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, NativeMethods.SRCCOPY);
            // Restore selection
            NativeMethods.Gdi32.SelectObject(hdcDest, hOld);


            if (captureMouse)
            {
                NativeMethods.User32.CURSORINFO pci;
                pci.cbSize = Marshal.SizeOf(typeof(NativeMethods.User32.CURSORINFO));

                if (NativeMethods.User32.GetCursorInfo(out pci))
                {
                    if (pci.flags == NativeMethods.CURSOR_SHOWING)
                    {
                        NativeMethods.User32.DrawIcon(hdcDest, pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                    }
                }
            }

            // Clean up
            NativeMethods.Gdi32.DeleteDC(hdcDest);
            NativeMethods.User32.ReleaseDC(desktop, hdcSrc);

            // Get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // Free up the Bitmap object
            NativeMethods.Gdi32.DeleteObject(hBitmap);
            GC.Collect();

            return img;
        }
        public Image CompressImage(Image img, long compression = 90L)
        {
            if (img == null) return null;

            EncoderParameters encParameters = new EncoderParameters(1);
            encParameters.Param[0] = new EncoderParameter(Encoder.Quality, compression);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, GetEncoder(ImageFormat.Jpeg), encParameters);
            return Image.FromStream(ms);
        }

        ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}