using FlightSimulatorProject.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorProject.Helpers
{

    class Client : IClient
    {
        const string LOCAL_HOST = "127.0.0.1";
        const int REMOTE_PORT = 5400;

        IPAddress ipaddress;
        IPEndPoint endPoint;
        Socket fg;
        byte[] data;
        bool isConnect = false;

        public bool IsConnect { get; }
        public void Connect(string ip, int port)
        {
            ipaddress = IPAddress.Parse(LOCAL_HOST);
            endPoint = new IPEndPoint(ipaddress, REMOTE_PORT);

            fg = new Socket(endPoint.Address.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            try
            {
                fg.Connect(endPoint);
                isConnect = true;
            }
            catch (Exception ex)
            {
                isConnect = false;
                Console.WriteLine("Could not connect to server: " + ex.Message);
            }
        }

            public void DisConnect()
        {
            if (!isConnect)
            {
                isConnect = false;
                fg.Shutdown(SocketShutdown.Both);

                fg.Disconnect(true);
            }
        }

        public void SendMessage(string s)
        {
            if (isConnect)
            {
                data = Encoding.ASCII.GetBytes(s);
                fg.Send(data);
                data = Encoding.ASCII.GetBytes("\r\n");
                fg.Send(data);
            }
        }
    }
}

