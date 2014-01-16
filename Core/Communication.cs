using System;
using System.IO;
using System.Net;

using T.Utilities;

namespace T.Core
{
	public class Communication
	{
        string host;
        string encKey;

		public Communication(string _host, string _encKey)
		{
			if(string.IsNullOrEmpty(_host))   throw new Exception("Invalid Host.");
			if(!_host.EndsWith(".php"))       throw new Exception("Host must end with .php");
			if(string.IsNullOrEmpty(_encKey)) throw new Exception("Invalid EncKey.");
			
			host = _host;
			encKey = _encKey;
		}
        public string HttpRequest(string parameters)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(host);
                req.Method = "POST";
                req.UserAgent = Config.AuthCode;
                req.ContentType = "application/x-www-form-urlencoded";

                if (!string.IsNullOrEmpty(parameters))
                {
                    byte[] b = Utils.XorCrypt(Utils.GetBytes(parameters), encKey);
                    req.ContentLength = b.Length;
                    using (Stream s = req.GetRequestStream())
                    {
                        s.Write(b, 0, b.Length);
                        s.Close();
                    }
                }

                using (WebResponse res = req.GetResponse())
                {
                    if (res == null) return null;
                    using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                    {
                        string strResponse = sr.ReadToEnd();
                        Utils.DebugString(string.Format("Encrypted response from gate.php: {0}", strResponse));

                        strResponse = Utils.GetString(Utils.XorCrypt(Utils.GetBytes(strResponse), encKey));
                        Utils.DebugString(string.Format("Decrypted response from gate.php: {0}", strResponse));

                        return strResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HTTP Request Error: {0}", ex.Message);
            }

            return string.Empty;
        }
	}
}