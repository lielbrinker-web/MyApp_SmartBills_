using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MyApp_SmartBills.ViewModels
{
    public class AddWarrantyViewModel : INotifyPropertyChanged
    {
        private string _productName = string.Empty;
        private DateTime _purchaseDate = DateTime.Today;
        private string _warrantyMonthsText = string.Empty;
        private string _receiptImagePath = "receipt_placeholder.png";

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
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    ReceiptImagePath = result.FullPath;
                }
            }
            catch (Exception)
            {
                // טיפול במצב שהמשתמש ביטל או שאין הרשאות למצלמה/גלריה
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

            // כאן בעתיד נשמור ל-Firebase באמצעות ה-Builder
            await Application.Current!.MainPage!.DisplayAlert("Success", "Warranty saved successfully (Mock)!", "OK");

            await NavigateBack();
        }

        private async Task NavigateBack()
        {
            if (Application.Current?.Windows.Count > 0 && Application.Current.Windows[0].Page is Page currentPage)
            {
                // חזרה חלקה למסך הקודם (רשימת המוצרים)
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