using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using MyApp_SmartBills.Model;
using MyApp_SmartBills.Service.DBService.FireBase;

namespace MyApp_SmartBills.Service
{
    public interface IFinancialDataService
    {
        Task<ObservableCollection<Transaction>> GetTransactionsAsync();
        Task AddTransactionAsync(Transaction transaction);
        Task<ObservableCollection<WarrantyItem>> GetWarrantiesAsync();
        Task AddWarrantyAsync(WarrantyItem warranty);
        Task<double> GetTotalIncomeAsync();
        Task<double> GetTotalExpensesAsync();
        Task<double> GetBalanceAsync();

        // פונקציות הפרופיל התואמות בדיוק למה שה-ViewModel שלך מחפש
        Task<Dictionary<string, string>> GetCurrentUserProfileAsync();
        Task<bool> UpdateUserProfileAsync(string fullName, string phoneNumber, string imageBase64);
        Task<Dictionary<string, string>> GetCurrentUserProfileAsync(string userId); // תמיכה במזהה ישיר
        Task<bool> UpdateUserProfileAsync(string userId, string fullName, string phoneNumber, string imageBase64);
    }

    public class FinancialDataService : IFinancialDataService
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly IAuthService _authService;

        public FinancialDataService(IAuthService authService)
        {
            _authService = authService;
            _firebaseClient = new FirebaseClient("https://lielsmartbills-default-rtdb.europe-west1.firebasedatabase.app/");
        }

        private string GetCurrentUserId()
        {
            return _authService.CurrentUserId ?? throw new Exception("User must be logged in.");
        }

        // --- ניהול פרופיל משתמש מול פיירבייס ---

        public async Task<Dictionary<string, string>> GetCurrentUserProfileAsync()
        {
            return await GetCurrentUserProfileAsync(GetCurrentUserId());
        }

        public async Task<Dictionary<string, string>> GetCurrentUserProfileAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return GetEmptyProfile();

                var nameData = await _firebaseClient.Child("Users").Child(userId).Child("FullName").OnceSingleAsync<string>();
                var phoneData = await _firebaseClient.Child("Users").Child(userId).Child("PhoneNumber").OnceSingleAsync<string>();
                var imageData = await _firebaseClient.Child("Users").Child(userId).Child("ImageBase64").OnceSingleAsync<string>();
                var emailData = await _firebaseClient.Child("Users").Child(userId).Child("Email").OnceSingleAsync<string>();

                return new Dictionary<string, string>
                {
                    { "FullName", nameData ?? "" },
                    { "PhoneNumber", phoneData ?? "" },
                    { "ImageBase64", imageData ?? "" },
                    { "Email", emailData ?? "" }
                };
            }
            catch (Exception)
            {
                return GetEmptyProfile();
            }
        }

        public async Task<bool> UpdateUserProfileAsync(string fullName, string phoneNumber, string imageBase64)
        {
            return await UpdateUserProfileAsync(GetCurrentUserId(), fullName, phoneNumber, imageBase64);
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, string fullName, string phoneNumber, string imageBase64)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return false;

                // תיקון קריטי: עטיפת מחרוזות פשוטות במירכאות כדי שפיירבייס יקבל אותן כ-JSON תקין
                await _firebaseClient.Child("Users").Child(userId).Child("FullName").PutAsync($"\"{fullName}\"");
                await _firebaseClient.Child("Users").Child(userId).Child("PhoneNumber").PutAsync($"\"{phoneNumber}\"");

                if (!string.IsNullOrEmpty(imageBase64))
                {
                    await _firebaseClient.Child("Users").Child(userId).Child("ImageBase64").PutAsync($"\"{imageBase64}\"");
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Update Error: {ex.Message}");
                return false;
            }
        }

        private Dictionary<string, string> GetEmptyProfile()
        {
            return new Dictionary<string, string>
            {
                { "FullName", "" },
                { "PhoneNumber", "" },
                { "ImageBase64", "" },
                { "Email", "" }
            };
        }

        // --- ניהול תנועות (Transactions) ---

        public async Task<ObservableCollection<Transaction>> GetTransactionsAsync()
        {
            try
            {
                string userId = GetCurrentUserId();

                var firebaseData = await _firebaseClient
                    .Child("Users")
                    .Child(userId)
                    .Child("Transactions")
                    .OnceAsync<Transaction>();

                var transactionsList = firebaseData
                    .Select(item => item.Object)
                    .OrderByDescending(t => t.Date)
                    .ToList();

                return new ObservableCollection<Transaction>(transactionsList);
            }
            catch (Exception)
            {
                return new ObservableCollection<Transaction>();
            }
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            string userId = GetCurrentUserId();

            if (transaction.Category == TransactionCategory.Salary)
            {
                transaction.Type = TransactionType.Income;
            }
            else
            {
                transaction.Type = TransactionType.Expense;
            }

            await _firebaseClient
                .Child("Users")
                .Child(userId)
                .Child("Transactions")
                .PostAsync(transaction);
        }

        // --- ניהול אחריות (Warranties) ---

        public async Task<ObservableCollection<WarrantyItem>> GetWarrantiesAsync()
        {
            try
            {
                string userId = GetCurrentUserId();
                var firebaseData = await _firebaseClient
                    .Child("Users")
                    .Child(userId)
                    .Child("Warranties")
                    .OnceAsync<WarrantyItem>();

                var warrantiesList = firebaseData.Select(item => item.Object).ToList();
                return new ObservableCollection<WarrantyItem>(warrantiesList);
            }
            catch (Exception)
            {
                return new ObservableCollection<WarrantyItem>();
            }
        }

        public async Task AddWarrantyAsync(WarrantyItem warranty)
        {
            string userId = GetCurrentUserId();

            await _firebaseClient
                .Child("Users")
                .Child(userId)
                .Child("Warranties")
                .PostAsync(warranty);
        }

        // --- חישובים וסיכומים ---

        public async Task<double> GetTotalIncomeAsync()
        {
            var transactions = await GetTransactionsAsync();
            return transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        }

        public async Task<double> GetTotalExpensesAsync()
        {
            var transactions = await GetTransactionsAsync();
            return transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        }

        public async Task<double> GetBalanceAsync()
        {
            var transactions = await GetTransactionsAsync();
            double income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            double expenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            return income - expenses;
        }
    }
}