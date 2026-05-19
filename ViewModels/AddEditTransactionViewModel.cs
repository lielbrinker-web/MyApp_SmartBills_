using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MyApp_SmartBills.ViewModels
{
    public class AddEditTransactionViewModel : INotifyPropertyChanged
    {
        private string _amountText = string.Empty;
        private DateTime _transactionDate = DateTime.Today;
        private string _selectedCategory = string.Empty;
        private bool _isBusiness = false;
        private string _receiptImagePath = "receipt_placeholder.png";

        #region Properties
        public string AmountText
        {
            get => _amountText;
            set { _amountText = value; OnPropertyChanged(); }
        }

        public DateTime TransactionDate
        {
            get => _transactionDate;
            set { _transactionDate = value; OnPropertyChanged(); }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        public bool IsBusiness
        {
            get => _isBusiness;
            set { _isBusiness = value; OnPropertyChanged(); }
        }

        public string ReceiptImagePath
        {
            get => _receiptImagePath;
            set { _receiptImagePath = value; OnPropertyChanged(); }
        }

        // רשימת קטגוריות לבחירה (Picker)
        public List<string> Categories { get; } = new List<string>
        {
            "Electricity", "Water", "Rent", "Salary", "Groceries", "Fuel", "Other"
        };
        #endregion

        public ICommand PickImageCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand SaveTransactionCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditTransactionViewModel()
        {
            PickImageCommand = new Command(async () => await PickImage());
            TakePhotoCommand = new Command(async () => await TakePhoto());
            SaveTransactionCommand = new Command(async () => await SaveTransaction());
            CancelCommand = new Command(async () => await NavigateBack());
        }

        private async Task PickImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null) ReceiptImagePath = result.FullPath;
            }
            catch (Exception) { /* טיפול בשגיאות או ביטול */ }
        }

        private async Task TakePhoto()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var result = await MediaPicker.Default.CapturePhotoAsync();
                    if (result != null) ReceiptImagePath = result.FullPath;
                }
            }
            catch (Exception) { /* טיפול בשגיאות או ביטול */ }
        }

        private async Task SaveTransaction()
        {
            if (string.IsNullOrWhiteSpace(AmountText) || string.IsNullOrWhiteSpace(SelectedCategory))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Please enter amount and select a category.", "OK");
                return;
            }

            if (!double.TryParse(AmountText, out double amount))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Amount must be a valid number.", "OK");
                return;
            }

            // כאן בעתיד נשמור ל-Firebase (הכנסה/הוצאה, פרטי/עסקי וכו')
            await Application.Current!.MainPage!.DisplayAlert("Success", "Transaction saved successfully (Mock)!", "OK");
            await NavigateBack();
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