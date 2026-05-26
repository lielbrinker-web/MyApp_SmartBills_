using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Firebase.Database;
using Firebase.Database.Query;
using MyApp_SmartBills.Model;

namespace MyApp_SmartBills.ViewModels
{
    public class AddWarrantyViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseClient _firebaseClient = new FirebaseClient("https://lielsmartbills-default-rtdb.europe-west1.firebasedatabase.app/");

        private string _productName = string.Empty;
        private DateTime _purchaseDate = DateTime.Today;
        private string _warrantyMonthsText = string.Empty;
        private string _receiptImagePath = "receipt_placeholder.png";

        // משתנה חדש שיחזיק את התמונה המומרת לטקסט עבור פיירבייס
        private string _base64Image = string.Empty;

        #region Properties
        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        public DateTime PurchaseDate
        {
            get => _purchaseDate;
            set { _purchaseDate = value; OnPropertyChanged(); }
        }

        public string WarrantyMonthsText
        {
            get => _warrantyMonthsText;
            set { _warrantyMonthsText = value; OnPropertyChanged(); }
        }

        public string ReceiptImagePath
        {
            get => _receiptImagePath;
            set { _receiptImagePath = value; OnPropertyChanged(); }
        }
        #endregion

        public ICommand PickImageCommand { get; }
        public ICommand SaveWarrantyCommand { get; }
        public ICommand CancelCommand { get; }

        public AddWarrantyViewModel()
        {
            PickImageCommand = new Command(async () => await PickReceiptImage());
            SaveWarrantyCommand = new Command(async () => await SaveWarranty());
            CancelCommand = new Command(async () => await NavigateBack());
        }

        private async Task PickReceiptImage()
        {
            try
            {
                // שימוש ב-MediaPicker המובנה של MAUI לבחירת/צילום תמונה
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    // תצוגה מקומית זמנית במסך ההוספה
                    ReceiptImagePath = result.FullPath;

                    // קריאת הקובץ והמרתו למחרוזת Base64 (טקסט)
                    using (Stream stream = await result.OpenReadAsync())
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        byte[] imageBytes = ms.ToArray();

                        // שמירת התמונה כטקסט מוכן לשליחה לענן
                        _base64Image = Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error picking image: {ex.Message}");
            }
        }

        private async Task SaveWarranty()
        {
            if (string.IsNullOrWhiteSpace(ProductName) || string.IsNullOrWhiteSpace(WarrantyMonthsText))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Please fill all required fields.", "OK");
                return;
            }

            if (!int.TryParse(WarrantyMonthsText, out int months))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Warranty months must be a number.", "OK");
                return;
            }

            try
            {
                string currentUserId = Preferences.Get("UserId", string.Empty);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = "405KLU9dPCN5leIPRw8wdQT0lUT2";
                }

                var newWarranty = new WarrantyItem
                {
                    UserId = currentUserId,
                    ProductName = this.ProductName,
                    PurchaseDate = this.PurchaseDate,
                    WarrantyMonths = months,
                    // אם המשתמש בחר תמונה, נשמור את הטקסט שלה. אם לא, נשאיר ריק.
                    ReceiptImageSource = !string.IsNullOrEmpty(_base64Image) ? _base64Image : "receipt_placeholder.png"
                };

                await _firebaseClient
                    .Child("Users")
                    .Child(currentUserId)
                    .Child("Warranties")
                    .PostAsync(newWarranty);

                await Application.Current!.MainPage!.DisplayAlert("Success", "Warranty saved successfully!", "OK");
                await NavigateBack();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
            }
        }

        private async Task NavigateBack()
        {
            if (Application.Current?.Windows.Count > 0 && Application.Current.Windows[0].Page is Page currentPage)
            {
                await currentPage.Navigation.PopAsync();
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