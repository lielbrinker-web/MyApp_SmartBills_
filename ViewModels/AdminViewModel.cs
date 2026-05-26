using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MyApp_SmartBills.ViewModels
{
    // המחלקה חייבת להיות partial כדי שה-Generator יוכל להרחיב אותה
    public partial class AdminViewModel : ObservableObject
    {
        // ה-Generator מייצר מזה אוטומטית מאפיין ציבורי בשם IsBusy
        [ObservableProperty]
        private bool _isBusy;

        // ה-Generator מייצר מזה אוטומטית מאפיין ציבורי בשם IsDebugMode
        [ObservableProperty]
        private bool _isDebugMode;

        public AdminViewModel()
        {
            IsBusy = false;
            IsDebugMode = false;
            _ = LoadUsersAsync();
        }

        public async Task LoadUsersAsync()
        {
            IsBusy = true;
            try
            {
                await Task.Delay(500); // סימולציה קצרה של טעינה
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ה-Generator מייצר מזה אוטומטית פקודה בשם NavigateToUserListViewCommand
        [RelayCommand]
        private async Task NavigateToUserListView()
        {
            await Shell.Current.GoToAsync("UserListView");
        }
    }
}