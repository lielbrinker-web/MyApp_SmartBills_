using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp_SmartBills.ViewModels
{
    public partial class AdminViewModel : ObservableObject
    {

        public AdminViewModel()
        {
        }

        [RelayCommand]
        private async Task NavigateToUsersListView()
        {
            await Shell.Current.GoToAsync("UsersListView");
        }
    }
}
