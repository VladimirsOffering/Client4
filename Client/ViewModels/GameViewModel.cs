using Client.Models;
using Client.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Client.ViewModels
{

    public class GameViewModel : ViewModelBase
    {
        int Delay = 1000;
        public Game game;
        TcpClient client;


        ObservableCollection<Card> _PlayerCards;
        public ObservableCollection<Card> PlayerCards
        {
            get => _PlayerCards;
            set
            {
                _PlayerCards = value;
                OnPropertyChanged("PlayerCards");
            }
        }


        public GameViewModel(Server server, TcpClient client)
        {
            game = new Game(server, client);
            this.client = client;
            PlayerCards = new ObservableCollection<Card>();
            StartGame();
        }

        public async void StartGame()
        {
            await Task.Run(() =>
            {
                var result = game.Start();
                while (result != 0)
                {
                    result = game.Start();

                    if (result == -2)
                    {
                        MessageBox.Show("Ошибка сервера, отключение");
                        client.Close();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ((MainViewModel)Application.Current.MainWindow.DataContext).CurrentPage = new ServerListPage();
                        });
                        return;
                    }
                    if (result == -1)
                    {
                    }
                }
                TakeCard();
            });
        }

        public void TakeCard()
        {
            while (PlayerCards.Select(x => x.value).Sum() < 21)
            {
                if (MessageBox.Show("Взять еще 1 карту?", "Еще?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Card card = null;
                    var res = game.TakeCard(out card);
                    if (res == 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PlayerCards.Add(card);
                        });
                    }
                    if (res == -1)
                    {
                        break;
                    }
                    if (res == -2)
                    {
                        MessageBox.Show("Ошибка сервера, отключение");
                        client.Close();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ((MainViewModel)Application.Current.MainWindow.DataContext).CurrentPage = new ServerListPage();
                        });
                        return;
                    }
                }
                else
                {
                    break;
                }
            }
            EndGame();
        }

        public void EndGame()
        {
            var res = game.IEndGame();
            if (res == -2)
            {
                MessageBox.Show("Ошибка сервера, отключение");
                client.Close();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ((MainViewModel)Application.Current.MainWindow.DataContext).CurrentPage = new ServerListPage();
                });
                return;
            }
            CheckResult();
        }

        public void CheckResult()
        {
            var res = game.CheckResult();
            if (res == -2)
            {
                MessageBox.Show("Ошибка сервера, отключение");
                client.Close();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ((MainViewModel)Application.Current.MainWindow.DataContext).CurrentPage = new ServerListPage();
                });
                return;
            }
            while (res == -1)
            {
                res = game.CheckResult();
                //MessageBox.Show("Ожидание завершения игры другими игроками");
            }
            if (res == 1)
            {
                MessageBox.Show("Победа");
            }
            else
            {
                MessageBox.Show("Поражение(");
            }
            client.Close();
            Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainViewModel)Application.Current.MainWindow.DataContext).CurrentPage = new ServerListPage();
            });
            return;
        }


    }
}
