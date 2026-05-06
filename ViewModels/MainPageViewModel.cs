using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp_SmartBills.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        public MainPageViewModel()
        {
            _name = "Hello " + (App.Current as App)!.CurrentUser!.FirstName!;
        }

        [RelayCommand]
        private async Task Settings()
        {
            await Shell.Current.GoToAsync("AccountView");
        }
    }
}
