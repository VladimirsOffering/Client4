using Client.ViewModels;
using Client.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Models
{
    public class Game
    {
        int Delay = 500;
        Server server;
        TcpClient client;

        public ushort columns { get; set; }
        public ushort rows { get; set; }


        public Game(Server server, TcpClient client)
        {
            this.server = server;
            this.client = client;
            columns = 0;
            rows = 0;
        }

        public int Start()
        {
            var response = "";
            var result = 0;
            result = SendAndRecv("start_game", out response);

            if (result != 0)
            {
                return -2;
            }
            if (response == "0")
            {
                return 0;
            }
            if (response == "-1")
            {
                return -1;
            }
            return -2;
        }

        public int TakeCard(out Card card)
        {
            var response = "";
            var result = 0;
            result = SendAndRecv("give_card", out response);

            if (result != 0)
            {
                card = null;
                return -2;
            }
            if (response == "-1")
            {
                card = null;
                return -1;
            }
            var res = response.Split('|');

            //res[0] - номер карты
            //res[1][0] - масть карты
            card = new Card(Int32.Parse(res[0]), res[1][0]);
            return 0;
        }

        public int IEndGame()
        {
            var response = "";
            var result = 0;
            result = SendAndRecv("i_end_game", out response);

            if (result != 0)
            {
                return -2;
            }
            return 0;
        }

        public int CheckResult()
        {
            var response = "";
            var result = 0;
            result = SendAndRecv("result", out response);

            if (result != 0)
            {
                return -2;
            }
            if (response == "-1")
            {
                return -1;
            }
            if (response == "win")
            {
                return 1;
            }
            if (response == "lose")
            {
                return 0;
            }
            return -2;
        }


        public int SendAndRecv(string request, out string response)
        {
            byte[] requestData = Encoding.ASCII.GetBytes(request);
            try
            {
                client.GetStream().Write(requestData, 0, requestData.Length);
            }
            catch (Exception e)
            {
                response = e.Message;
                return -2;
            }

            // Получаем ответ от сервера
            byte[] responseData = new byte[8192];
            try
            {
                int bytesRead = client.GetStream().Read(responseData, 0, responseData.Length);
                response = Encoding.ASCII.GetString(responseData, 0, bytesRead);

                if (bytesRead < 1)
                {
                    response = "Сервер перестал работать";
                    return -1;
                }
                return 0;
            }
            catch (Exception e)
            {
                response = e.Message;
                return -2;
            }
            return -2;
        }

        public int OnlyRead(out string response) 
        {
            // Получаем ответ от сервера
            byte[] responseData = new byte[8192];
            try
            {
                int bytesRead = client.GetStream().Read(responseData, 0, responseData.Length);
                response = Encoding.ASCII.GetString(responseData, 0, bytesRead);

                if (bytesRead < 1)
                {
                    response = "Сервер перестал работать";
                    return -1;
                }
                return 0;
            }
            catch (Exception e)
            {
                response = e.Message;
                return -2;
            }
            return -2;
        }    

    }
}
