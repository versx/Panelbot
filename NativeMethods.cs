using System;
using System.Runtime.InteropServices;

namespace T
{
    class NativeMethods
    {
        // Remote Desktop
        // BitBlt dwRop parameter
        public const int SRCCOPY = 0xCC0020;

        // Credits to http://stackoverflow.com/questions/6750056/how-to-capture-the-screen-and-mouse-pointer-using-windows-apis
        public const Int32 CURSOR_SHOWING = 0x00000001;

        const string user32 = "user32.dll";
        const string kernel32 = "kernel32.dll";
        const string advapi32 = "advapi32.dll";
        const string gdi32 = "gdi32.dll";

        public class Kernel32
        {
            // VirtualMachine class native methods
            /*********************************************/
            [DllImport(kernel32)]
            public static extern uint EnumSystemFirmwareTables(
                uint FirmwareTableProviderSignature,
                IntPtr pFirmwareTableBuffer,
                uint BufferSize
            );
            [DllImport(kernel32)]
            public static extern uint GetSystemFirmwareTable(
                uint FirmwareTableProviderSignature,
                uint FirmwareTableID,
                IntPtr pFirmwareTableBuffer,
                uint BufferSize
            );
            /*********************************************/
        }
        public class Advapi32
        {
            // ProcessSecurity class native methods
            /*********************************************/
            [DllImport(advapi32, SetLastError = true)]
            public static extern bool SetKernelObjectSecurity(
                IntPtr Handle,
                int securityInformation,
                [In] byte[] pSecurityDescriptor
            );
            /*********************************************/
        }
        public class User32
        {
            // MessageBox
            /*********************************************/
            [DllImport(user32)]
            public static extern int MessageBoxA(
                int hWnd,
                string strMsg,
                string strCaption,
                int iType
            );
            /*********************************************/

            // Cursor Info
            /*******************************************/
            [StructLayout(LayoutKind.Sequential)]
            public struct CURSORINFO
            {
                public Int32 cbSize;
                public Int32 flags;
                public IntPtr hCursor;
                public POINTAPI ptScreenPos;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct POINTAPI
            {
                public int x;
                public int y;
            }

            [DllImport(user32)]
            public static extern bool GetCursorInfo(
                out CURSORINFO pci
            );

            [DllImport(user32)]
            public static extern bool DrawIcon(
                IntPtr hDC,
                int x,
                int y,
                IntPtr hIcon);
            /*******************************************/

            // Deskop Capture
            /*********************************************/
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport(user32)]
            public static extern IntPtr GetDesktopWindow();

            [DllImport(user32)]
            public static extern IntPtr GetWindowDC(
                IntPtr
                hWnd);

            [DllImport(user32)]
            public static extern IntPtr ReleaseDC(
                IntPtr hWnd,
                IntPtr hDC);

            [DllImport(user32)]
            public static extern IntPtr GetWindowRect(
                IntPtr hWnd,
                ref RECT rect);
            /*********************************************/
        }
        public class Gdi32
        {
            [DllImport(gdi32)]
            public static extern IntPtr CreateCompatibleBitmap(
                IntPtr hDC,
                int nWidth,
                int nHeight);

            [DllImport(gdi32)]
            public static extern bool BitBlt(
                IntPtr hObject,
                int nXDest,
                int nYDest,
                int nWidth,
                int nHeight,
                IntPtr hObjectSource,
                int nXSrc,
                int nYSrc,
                int dwRop);

            [DllImport(gdi32)]
            public static extern IntPtr CreateCompatibleDC(
                IntPtr hDC);

            [DllImport(gdi32)]
            public static extern bool DeleteDC(
                IntPtr hDC);

            [DllImport(gdi32)]
            public static extern bool DeleteObject(
                IntPtr hObject);

            [DllImport(gdi32)]
            public static extern IntPtr SelectObject(
                IntPtr hDC,
                IntPtr hObject);
        }
    }
}