using Client.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        ServerListPage ServerListPage;

        Page _CurrentPage;
        public Page CurrentPage
        {
            get => _CurrentPage;
            set
            {
                _CurrentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

       public void SetServerListPage()
        {
            CurrentPage = ServerListPage;
        }

        public MainViewModel()
        {
            ServerListPage = new ServerListPage();
            CurrentPage = ServerListPage;
        }
    }
}
