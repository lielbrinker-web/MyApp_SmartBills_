using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using MyApp_SmartBills.Model;
using MyApp_SmartBills.Service;
using MyApp_SmartBills.Service.DBService.FireBase;

namespace MyApp_SmartBills.ViewModels
{
    public class AddEditTransactionViewModel : INotifyPropertyChanged
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IAuthService _authService; // הוספת שירות האותנטיקציה עבור ה-UID

        private string _amountText = string.Empty;
        private DateTime _transactionDate = DateTime.Today;
        private string _selectedCategory = string.Empty;
        private bool _isBusiness = false;
        private string _receiptImagePath = "receipt_placeholder.png";
        private bool _isIncome = false; // שדה חדש לבחירה בין הכנסה להוצאה

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

        // פרופרטי חדש שיקושר ל-RadioButton או Switch במסך
        public bool IsIncome
        {
            get => _isIncome;
            set { _isIncome = value; OnPropertyChanged(); }
        }

        // טעינה דינמית של כל הקטגוריות הקיימות ב-Enum שלך
        public List<string> Categories { get; } = Enum.GetNames(typeof(TransactionCategory)).ToList();
        #endregion

        public ICommand PickImageCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand SaveTransactionCommand { get; }
        public ICommand CancelCommand { get; }

        // הזרקת ה-IAuthService בבנאי
        public AddEditTransactionViewModel(IFinancialDataService financialDataService, IAuthService authService)
        {
            _financialDataService = financialDataService;
            _authService = authService;

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
            catch (Exception) { }
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
            catch (Exception) { }
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

            if (!Enum.TryParse(SelectedCategory, out TransactionCategory categoryEnum))
            {
                categoryEnum = TransactionCategory.Other;
            }

            // יצירת אובייקט התנועה המלא
            var newTransaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(), // יצירת מפתח ייחודי לתנועה הנוכחית
                Amount = amount,
                Date = TransactionDate,
                Category = categoryEnum,
                IsBusiness = IsBusiness,
                ReceiptImageUrl = ReceiptImagePath,

                // 1. קביעה האם מדובר בהכנסה או הוצאה על בסיס הבחירה במסך
                Type = IsIncome ? TransactionType.Income : TransactionType.Expense,

                // 2. קישור ישיר ל-UID של המשתמש המחובר כעת
                UserId = _authService.CurrentUserId ?? "GuestUser"
            };

            try
            {
                // שמירה אסינכרונית בתוך ה-Realtime Database
                await _financialDataService.AddTransactionAsync(newTransaction);

                await Application.Current!.MainPage!.DisplayAlert("Success", "Transaction saved successfully!", "OK");
                await NavigateBack();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Failed to save to Firebase: {ex.Message}", "OK");
            }
        }

        private async Task NavigateBack()
        {
            await Shell.Current.GoToAsync("..");
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