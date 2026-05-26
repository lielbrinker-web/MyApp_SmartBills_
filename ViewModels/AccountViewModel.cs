using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp_SmartBills.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyApp_SmartBills.ViewModels
{
    public partial class AccountViewModel : ObservableObject
    {
        private readonly IFinancialDataService _dataService;

        [ObservableProperty]
        private string fullName;

        [ObservableProperty]
        private string userEmail;

        [ObservableProperty]
        private string phoneNumber;

        [ObservableProperty]
        private string userImageSource;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isDeleteButtonVisible = true;

        public AccountViewModel(IFinancialDataService dataService)
        {
            _dataService = dataService;
            _ = LoadUserProfileAsync();
        }

        public async Task LoadUserProfileAsync()
        {
            try
            {
                // איפוס שגיאות ומצב טעינה
                IsBusy = true;
                HasError = false;
                ErrorMessage = string.Empty;

                var profileData = await _dataService.GetCurrentUserProfileAsync();

                if (profileData != null)
                {
                    FullName = profileData.ContainsKey("FullName") ? profileData["FullName"] : "";
                    UserEmail = profileData.ContainsKey("Email") ? profileData["Email"] : "";
                    PhoneNumber = profileData.ContainsKey("PhoneNumber") ? profileData["PhoneNumber"] : "";

                    // בדיקה בטוחה של תמונת ה-Base64 מול פיירבייס
                    if (profileData.ContainsKey("ImageBase64") && !string.IsNullOrEmpty(profileData["ImageBase64"]))
                    {
                        UserImageSource = profileData["ImageBase64"];
                    }
                    else
                    {
                        UserImageSource = "user_icon.png"; // תמונת ברירת מחדל מקומית
                    }
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "Failed to load profile data.";
                System.Diagnostics.Debug.WriteLine($"Error loading profile: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateProfile()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                HasError = true;
                ErrorMessage = "Full Name cannot be empty.";
                return;
            }

            try
            {
                IsBusy = true;
                HasError = false;
                ErrorMessage = string.Empty;

                // בדיקה: אם התמונה היא עדיין קובץ מקומי ולא קוד Base64 אמיתי, לא נשלח אותה לשרת
                string imageToSend = null;
                if (!string.IsNullOrEmpty(UserImageSource) && !UserImageSource.EndsWith(".png") && !UserImageSource.EndsWith(".jpg"))
                {
                    imageToSend = UserImageSource;
                }

                // שליחת הנתונים המעובדים והבטוחים לשרת
                bool isSuccess = await _dataService.UpdateUserProfileAsync(FullName, PhoneNumber, imageToSend);

                if (isSuccess)
                {
                    await Shell.Current.DisplayAlert("Success", "Profile updated successfully!", "OK");
                    await LoadUserProfileAsync(); // רענון הנתונים מהשרת לוודא סנכרון
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Could not save updates to server.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error saving profile: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ChangeImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Please pick a profile photo"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);

                    byte[] imageBytes = memoryStream.ToArray();

                    // המרה ל-Base64 נקי
                    UserImageSource = Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error picking photo: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to select image.", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteAccount()
        {
            bool confirm = await Shell.Current.DisplayAlert("Warning", "Are you sure?", "Yes", "No");
            if (confirm)
            {
                // לוגיקת מחיקה...
            }
        }
    }
}