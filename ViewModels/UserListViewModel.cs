using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyApp_SmartBills.Model;
using MyApp_SmartBills.Service.DBService;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyApp_SmartBills.Model;
using MyApp_SmartBills.Service.DBService;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace MyApp_SmartBills.ViewModels
{
    public partial class UserListViewModel : ObservableObject
    {
        private readonly IAppUserRepository _userRepository;

        [ObservableProperty]
        private ObservableCollection<AppUser> _usersList;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _searchText;

        // מאפיין חדש: מחזיק את המשתמש שנלחץ כרגע בתוך הרשימה
        [ObservableProperty]
        private AppUser? _selectedUser;

        public UserListViewModel(IAppUserRepository userRepository)
        {
            _userRepository = userRepository;
            UsersList = new ObservableCollection<AppUser>();

            _ = LoadUsersFromFirebase();
        }

        public async Task LoadUsersFromFirebase()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var users = await _userRepository.GetAllAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UsersList.Clear();
                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            UsersList.Add(user);
                        }
                    }
                });
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

        // פקודת הניווט החדשה: מופעלת אוטומטית ברגע שלוחצים על כרטיס משתמש
        [RelayCommand]
        private async Task NavigateToAccountPage()
        {
            if (SelectedUser == null) return;

            // יצירת מילון פרמטרים והעברת המשתמש הנבחר למסך הבא
            var navigationParameter = new Dictionary<string, object>
            {
                { "selectedUser", SelectedUser }
            };

            // ניווט למסך ה-AccountView הקיים אצלך באפליקציה
            await Shell.Current.GoToAsync("AccountView", navigationParameter);
        }

        // מתודה לאיפוס הבחירה (תיקרא כאשר חוזרים חזרה למסך זה)
        public void ResetSelection()
        {
            SelectedUser = null;
        }
    }
}