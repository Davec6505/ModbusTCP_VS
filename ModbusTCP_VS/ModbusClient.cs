using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ModbusTCP_VS
{
    class ModbusClient
    {
        Socket socket;
        IPAddress ipAddress;
        int port;
        public ModbusClient()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
    }
}
