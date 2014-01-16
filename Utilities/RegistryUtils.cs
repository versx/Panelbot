using System;
using Microsoft.Win32;

namespace T.Utilities
{
    class RegistryUtils
    {
        public static bool RegistryDataExists(RegistryKey rkKey, string subkey, string value)
        {
            using (RegistryKey rk = rkKey.OpenSubKey(subkey))
            {
                foreach (string name in rk.GetValueNames())
                {
                    if (name == value)
                    {
                        rk.Close();
                        return true;
                    }
                }
                rk.Close();
            }
            return false;
        }
        public static void AddRegKey(RegistryKey rkKey, string strPath, string strName, string strValue)
        {
            using (RegistryKey rk = rkKey.OpenSubKey(strPath, true))
            {
                rk.SetValue(strName, strValue);
                rk.Close();
            }
        }
        public static void RemoveRegKey(RegistryKey rkKey, string strPath, string strName)
        {
            using (RegistryKey rk = rkKey.OpenSubKey(strPath, true))
            {
                rk.DeleteValue(strName);
                rk.Close();
            }
        }
    }
}