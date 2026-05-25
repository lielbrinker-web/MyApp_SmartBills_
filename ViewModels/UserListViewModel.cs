using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MyApp_SmartBills.Model;
using MyApp_SmartBills.Service.DBService;
using Microsoft.Maui.ApplicationModel;

namespace MyApp_SmartBills.ViewModels
{
    public partial class UserListViewModel : INotifyPropertyChanged
    {
        private readonly IAppUserRepository _userRepository;
        private ObservableCollection<AppUser> _usersList;

        public ObservableCollection<AppUser> UsersList
        {
            get => _usersList;
            set
            {
                _usersList = value;
                OnPropertyChanged();
            }
        }

        public UserListViewModel(IAppUserRepository userRepository)
        {
            _userRepository = userRepository;
            UsersList = new ObservableCollection<AppUser>();

            _ = LoadUsersFromFirebase();
        }

        public async Task LoadUsersFromFirebase()
        {
            try
            {
                // קריאה אסינכרונית תקינה לטעינת המשתמשים
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
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}