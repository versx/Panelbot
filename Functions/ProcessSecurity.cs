using System;
using System.ComponentModel;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Runtime.InteropServices;

namespace T.Functions
{
    //http://csharptest.net/1043/how-to-prevent-users-from-killing-your-service-process/
    class ProcessSecurity
    {
        public static void SetProcessSecurityDescriptor(IntPtr hWnd)
        {
            RawSecurityDescriptor sd = new RawSecurityDescriptor(ControlFlags.None, new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), null, null, new RawAcl(2, 0));
            sd.SetFlags(ControlFlags.DiscretionaryAclPresent | ControlFlags.DiscretionaryAclDefaulted);
            byte[] rawSd = new byte[sd.BinaryLength];
            sd.GetBinaryForm(rawSd, 0);
            if (!NativeMethods.Advapi32.SetKernelObjectSecurity(hWnd, (int)SecurityInfos.DiscretionaryAcl, rawSd))
                Console.WriteLine("Win32Exception(): {0}", new Win32Exception().Message);
        }
    }
}