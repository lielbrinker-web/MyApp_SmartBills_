using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp_SmartBills.Helper;
using MyApp_SmartBills.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp_SmartBills.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private Page _page;

        [ObservableProperty]
        public bool? _isAdmin = false;

        [ObservableProperty]
        private string _logoutIcon;

        [ObservableProperty]
        private string _adminIcon;

        [ObservableProperty]
        private string _homeIcon;

        [ObservableProperty]
        private string _accountIcon;

        public AppShellViewModel(SignInView signInView)
        {
            _page = signInView;
            _isAdmin = (App.Current as App)!.CurrentUser!.IsAdmin; ///
			_logoutIcon = FontHelper.LOGOUT_ICON;
            _adminIcon = FontHelper.ADMIN_ICON;
            _homeIcon = FontHelper.HOME_ICON;
            _accountIcon = FontHelper.PERSON_ICON;
        }

        [RelayCommand]
        private void Logout()
        {
            (App.Current as App)!.CurrentUser = null;
            Application.Current.Windows[0].Page = new NavigationPage(_page);
        }

        [RelayCommand]
        private async Task NavigateToAdminPage()
        {
            await Shell.Current.GoToAsync("AdminView");
        }

        [RelayCommand]
        private async Task NavigateToHomePage()
        {
            await Shell.Current.GoToAsync("MainPageView");
        }

        [RelayCommand]
        private async Task NavigateToAccountPage()
        {
            await Shell.Current.GoToAsync("AccountView");
        }


    }
}
