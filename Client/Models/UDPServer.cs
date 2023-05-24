using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Models
{
    public class UDPServer
    {
        const ushort UDP_PORT = 12346;
        const ushort TCP_PORT = 12345;
        const uint MaxSecondsWaiting = 1;
        public IEnumerable<Server> UpdateServersList()
        {
            List<Server> Servers = new List<Server>();
            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            byte[] data = Encoding.ASCII.GetBytes("Hello");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, UDP_PORT);
            client.Send(data, data.Length, endPoint);

            byte[] receiveBuffer = new byte[1024];
            IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);


            TimeSpan timeout = TimeSpan.FromSeconds(MaxSecondsWaiting);
            client.Client.ReceiveTimeout = (int)timeout.TotalMilliseconds;

            while (true)
            {
                try
                {
                    receiveBuffer = client.Receive(ref receiveEndPoint);
                    string message = Encoding.ASCII.GetString(receiveBuffer);
                    var players = message.Split('|');
                    Server serv = new Server();
                    serv.IP = receiveEndPoint.Address;
                    serv.PORT = TCP_PORT;
                    serv.CurrentPlayers = UInt16.Parse(players[0]);
                    serv.MaxPlayers = UInt16.Parse(players[1]);
                    Servers.Add(serv);
                }
                catch (Exception e)
                {
                    break;
                }
            }
            if (Servers.Count == 0)
            {
                MessageBox.Show("Сервера не найдены(((");
            }
            client.Close();
            return Servers;
        }
    }
}
