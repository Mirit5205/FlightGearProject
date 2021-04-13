using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorProject.Interfaces
{
    public interface IClient
    {

        void Connect(string ip, int port);

        void DisConnect();

        void SendMessage(string s);
    }
}

