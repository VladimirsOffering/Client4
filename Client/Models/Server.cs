using Client.ViewModels;
using Client.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Models
{
    public class Server
    {
        private const int MAX_RESPONCE_TIME = 5000;
        public IPAddress IP { get; set; }
        public ushort PORT { get; set; }
        public ushort CurrentPlayers { get; set; }
        public ushort MaxPlayers { get; set; }

        private RelayCommand _ConnectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                return _ConnectCommand ??
                  (_ConnectCommand = new RelayCommand(obj =>
                  {
                      Connect();
                  }));
            }
        }


        public void Connect()
        {
            //try
            //{
                var i = IP.ToString();
                TcpClient client = new TcpClient(IP.ToString(), PORT);
                NetworkStream stream = client.GetStream();

                // Создаем объект класса StreamWriter для записи данных в поток
                StreamWriter writer = new StreamWriter(stream);

                // Записываем строку "add_player" в поток
                try
                {
                    writer.WriteLine("add_player");

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    writer.Close();
                    stream.Close();
                    client.Close();
                    ((MainViewModel)Application.Current.MainWindow.DataContext).CurrentPage = new ServerListPage();
                }
                writer.Flush();

                // Читаем ответ от сервера
                byte[] buffer = new byte[1024];

                stream.ReadTimeout = MAX_RESPONCE_TIME;
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Если получен ответ "OK", переходим на страницу игры

                if (bytesRead < 1) 
                {
                    MessageBox.Show("Сервер перестал работать");
                    writer.Close();
                    stream.Close();
                    client.Close();
                    ((MainViewModel)Application.Current.MainWindow.DataContext).CurrentPage = new ServerListPage();
                }

                if (response == "0")
                {
                    ((MainViewModel)Application.Current.MainWindow.DataContext).CurrentPage = new GamePage(this, client);
                }
                else if(response == "-1")
                {
                    MessageBox.Show("Игра уже началась, нельзя добавлять игроков.");
                }
                else 
                {      
                    MessageBox.Show("Достигнуто максимальное количество игроков.");
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при отправке на сервер: {ex.Message}");
            //}
        }

        public Server()
        {

        }
    }
}
