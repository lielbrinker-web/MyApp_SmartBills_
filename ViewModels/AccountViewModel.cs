using System;
using System.Collections.Generic;
using MyApp_SmartBills.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MyApp_SmartBills.ViewModels
{
    public partial class AccountViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private AppUser? _selectedUser;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private ImageSource? _userImageSource;

        private string _currentBase64Image = string.Empty;

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
            IsBusy = true;
            try
            {
                AppUser? targetUser = null;

                if (query.TryGetValue("selectedUser", out var userObj) && userObj is AppUser user)
                {
                    targetUser = user;
                }
                else if (Application.Current is App currentApp && currentApp.CurrentUser != null)
                {
                    targetUser = currentApp.CurrentUser;
                }

                if (targetUser != null)
                {
                    SelectedUser = targetUser;
                    FullName = $"{targetUser.FirstName} {targetUser.LastName}".Trim();
                    UserEmail = targetUser.UserEmail ?? string.Empty;
                    PhoneNumber = targetUser.UserMobile ?? string.Empty;

                    _currentBase64Image = Preferences.Default.Get($"profile_pic_{UserEmail}", string.Empty);
                    UserImageSource = ConvertBase64ToImageSource(_currentBase64Image);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "Error loading user data.";
                System.Diagnostics.Debug.WriteLine($"Error loading user: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
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
                await Task.Delay(500);

                if (SelectedUser != null)
                {
                    var names = FullName.Split(' ', 2);
                    SelectedUser.FirstName = names.Length > 0 ? names[0] : string.Empty;
                    SelectedUser.LastName = names.Length > 1 ? names[1] : string.Empty;
                    SelectedUser.UserEmail = UserEmail;
                    SelectedUser.UserMobile = PhoneNumber;

                    Preferences.Default.Set($"profile_pic_{UserEmail}", _currentBase64Image);
                }

                await Shell.Current.DisplayAlert("Success", "Profile updated successfully!", "OK");
                await GoBack();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "Failed to update profile.";
                System.Diagnostics.Debug.WriteLine($"Update Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ChangeImage()
        {
            if (IsBusy) return;

            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo == null) return;

                IsBusy = true;

                using (var stream = await photo.OpenReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    _currentBase64Image = Convert.ToBase64String(imageBytes);
                    UserImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to select image: " + ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task DeleteAccount()
        {
            bool answer = await Shell.Current.DisplayAlert("Warning", "Are you sure you want to delete this user?", "Yes", "No");
            if (answer)
            {
                await GoBack();
            }
        }

        private ImageSource ConvertBase64ToImageSource(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                return ImageSource.FromFile("profile_placeholder.png");
            }

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            catch
            {
                return ImageSource.FromFile("profile_placeholder.png");
            }
        }
    }
}