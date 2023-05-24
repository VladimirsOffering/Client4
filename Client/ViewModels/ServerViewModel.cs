using Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Policy;
using System.Threading;

namespace Client.ViewModels
{
    public class ServerViewModel : ViewModelBase
    {
        UDPServer _UDPServer = new UDPServer();

        public ObservableCollection<Server> _Servers { get; set; }
        public ObservableCollection<Server> Servers
        {
            get => _Servers;
            set
            {
                _Servers = value;
                OnPropertyChanged(nameof(Servers));
            }
        }

        public ServerViewModel()
        {
            Servers = new ObservableCollection<Server>();
        }

        private RelayCommand _UpdateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return _UpdateCommand ??
                  (_UpdateCommand = new RelayCommand(obj =>
                  {
                      Servers = new ObservableCollection<Server>(_UDPServer.UpdateServersList());
                  }));
            }
        }

       
    }
}
