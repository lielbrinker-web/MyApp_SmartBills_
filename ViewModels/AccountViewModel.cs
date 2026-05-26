using System;
using System.Collections.Generic;
using MyApp_SmartBills.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace MyApp_SmartBills.ViewModels
{
    public partial class AccountViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private AppUser? _selectedUser;

        // שדות הקלט שיוצגו ב-XAML
        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private string _userImageSource = string.Empty;

        // ניהול מצבי מסך
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isDeleteButtonVisible = true;

        public AccountViewModel()
        {
            IsBusy = false;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("selectedUser", out var userObj) && userObj is AppUser user)
            {
                IsBusy = true;
                try
                {
                    SelectedUser = user;

                    // תיקון השגיאות: מייצרים את ה-FullName מחיבור של שם פרטי ומשפחה הקיימים ב-AppUser שלך
                    FullName = $"{user.FirstName} {user.LastName}".Trim();
                    UserEmail = user.UserEmail;

                    // אם אין PhoneNumber או ProfileImage ב-AppUser, נשים ערכי ברירת מחדל ריקים כדי שלא יקרוס
                    PhoneNumber = string.Empty;
                    UserImageSource = string.Empty;
                }
                catch (Exception)
                {
                    HasError = true;
                    ErrorMessage = "Error loading user data.";
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private async Task UpdateProfile()
        {
            if (IsBusy) return;

            IsBusy = true;
            HasError = false;

            try
            {
                await Task.Delay(1000); // סימולציה של שמירה

                if (SelectedUser != null)
                {
                    // כאן אנחנו מעדכנים חזרה את המודל לפי מה שהשתנה בתיבת הטקסט
                    // (מפרקים את השם המלא חזרה לשם פרטי ומשפחה)
                    var names = FullName.Split(' ', 2);
                    SelectedUser.FirstName = names.Length > 0 ? names[0] : string.Empty;
                    SelectedUser.LastName = names.Length > 1 ? names[1] : string.Empty;
                    SelectedUser.UserEmail = UserEmail;
                }

                await Shell.Current.DisplayAlert("Success", "Profile updated successfully!", "OK");
            }
            catch (Exception)
            {
                HasError = true;
                ErrorMessage = "Failed to update profile.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAccount()
        {
            bool answer = await Shell.Current.DisplayAlert("Warning", "Are you sure you want to delete this user?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task ChangeImage()
        {
            // לוגיקת שינוי תמונה עתידית
        }
    }
}