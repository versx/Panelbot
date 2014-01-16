using T.Core;
using T.Utilities;

using System;
using System.Diagnostics;

namespace T
{
    class Program
    {
        static void Main()
        {
            if (!SysUtils.IsSingleInstance(Config.Mutex))
            {
                foreach (Process p in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
                {
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(p.MainModule.FileName);
                    Version v1 = new Version(myFileVersionInfo.FileVersion.ToString());
                    Version v2 = new Version(Config.Version);

                    int result = v1.CompareTo(v2);

                    //if already running version is greater than this, exit this app
                    if (result > 0) Environment.Exit(-1);

                    if (result < 0)
                    {
                        p.Kill();
                        //new Installation(Config.FileName, Config.RegistryName).Install();
                        CmdHandler.Run();
                    }
                }
            }
            else
            {
                //this run for the first instance of the program
                CmdHandler.Run();
            }
        }
    }
}