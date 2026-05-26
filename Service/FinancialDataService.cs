using System;
using System.Collections.ObjectModel;
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

        // התיקון נמצא כאן: קריאה ישירה לשדה המחרוזת שקיים ב-IAuthService
        private string GetCurrentUserId()
        {
            return _authService.CurrentUserId ?? throw new Exception("User must be logged in.");
        }

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