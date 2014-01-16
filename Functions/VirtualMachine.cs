using System;
using System.Text;
using System.Runtime.InteropServices;

namespace T.Functions
{
    class VirtualMachine
    {
        // Checks BIOS FIRM(ware) to contain the keyword 'virtual'
        public static bool IsPresent()
        {
            byte[] buffer = null;
            uint firmwareTableProviderSignature = 0x4649524D; //FIRM in hexidecimal
            uint bufferSize = NativeMethods.Kernel32.EnumSystemFirmwareTables(firmwareTableProviderSignature, IntPtr.Zero, 0);
            IntPtr pFirmwareTableBuffer = Marshal.AllocHGlobal((int)bufferSize);
            uint firmwareTableID = 0xC0000;
            bufferSize = NativeMethods.Kernel32.GetSystemFirmwareTable(firmwareTableProviderSignature, firmwareTableID, IntPtr.Zero, 0);
            buffer = new byte[bufferSize];
            pFirmwareTableBuffer = Marshal.AllocHGlobal((int)bufferSize);
            NativeMethods.Kernel32.GetSystemFirmwareTable(firmwareTableProviderSignature, firmwareTableID, pFirmwareTableBuffer, bufferSize);
            Marshal.Copy(pFirmwareTableBuffer, buffer, 0, buffer.Length);
            Marshal.FreeHGlobal(pFirmwareTableBuffer);
            //new System.IO.StreamWriter("buffer.txt").Write(Encoding.ASCII.GetString(buffer));
            if (Encoding.ASCII.GetString(buffer).ToLower().Contains("virtual"))
                return true;
            return false;
        }
    }
}