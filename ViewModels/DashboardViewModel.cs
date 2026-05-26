using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MyApp_SmartBills.Model;
using MyApp_SmartBills.Service;
using Microsoft.Maui.Controls;

namespace MyApp_SmartBills.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IFinancialDataService _financialDataService;

        private ObservableCollection<Transaction> _recentTransactions = new ObservableCollection<Transaction>();
        private double _totalIncome;
        private double _totalExpenses;
        private double _balance;
        private string _welcomeTitle = "Hello Guest";

        public ObservableCollection<Transaction> RecentTransactions
        {
            get => _recentTransactions;
            set { _recentTransactions = value; OnPropertyChanged(); }
        }

        public double TotalIncome
        {
            get => _totalIncome;
            set { _totalIncome = value; OnPropertyChanged(); }
        }

        public double TotalExpenses
        {
            get => _totalExpenses;
            set { _totalExpenses = value; OnPropertyChanged(); }
        }

        public double Balance
        {
            get => _balance;
            set { _balance = value; OnPropertyChanged(); }
        }

        public string WelcomeTitle
        {
            get => _welcomeTitle;
            set { _welcomeTitle = value; OnPropertyChanged(); }
        }

        public DashboardViewModel(IFinancialDataService financialDataService)
        {
            _financialDataService = financialDataService;
            UpdateDashboardWelcome();
        }

        public async Task InitializeDashboardAsync()
        {
            try
            {
                // קריאה לרענון שם המשתמש בכל פעם שהדאשבורד מאותחל או עולה מחדש
                UpdateDashboardWelcome();

                var transactionsList = await _financialDataService.GetTransactionsAsync();

                RecentTransactions.Clear();
                if (transactionsList != null)
                {
                    var recentItems = transactionsList.OrderByDescending(t => t.Date).Take(5);
                    foreach (var transaction in recentItems)
                    {
                        RecentTransactions.Add(transaction);
                    }
                }

                TotalIncome = await _financialDataService.GetTotalIncomeAsync();
                TotalExpenses = await _financialDataService.GetTotalExpensesAsync();
                Balance = await _financialDataService.GetBalanceAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
            }
        }

        public void UpdateDashboardWelcome()
        {
            if (Application.Current is App currentApp && currentApp.CurrentUser != null)
            {
                if (currentApp.CurrentUser.IsAdmin)
                {
                    WelcomeTitle = "Hello Admin";
                }
                else
                {
                    // תיקון: שליפת השם הפרטי ושם המשפחה האמיתיים של המשתמש המחובר מהאפליקציה
                    string firstName = currentApp.CurrentUser.FirstName;
                    string lastName = currentApp.CurrentUser.LastName;

                    if (!string.IsNullOrWhiteSpace(firstName))
                    {
                        WelcomeTitle = $"Hello {firstName} {lastName}".Trim();
                    }
                    else
                    {
                        WelcomeTitle = "Hello User";
                    }
                }
            }
            else
            {
                WelcomeTitle = "Hello Guest";
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}