using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MyApp_SmartBills.Model;

namespace MyApp_SmartBills.ViewModels
{
    // מימוש ידני ומפורש של ממשק עדכון ה-UI כדי למנוע שגיאות קומפילציה
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Transaction> RecentTransactions { get; set; }
        public double TotalIncome { get; set; }
        public double TotalExpenses { get; set; }
        public double Balance { get; set; }

        private string _welcomeTitle = "Hello Guest";

        // המאפיין הדינמי שה-XAML מחפש
        public string WelcomeTitle
        {
            get => _welcomeTitle;
            set
            {
                if (_welcomeTitle != value)
                {
                    _welcomeTitle = value;
                    OnPropertyChanged(); // מעדכן את המסך מיד כשהערך משתנה
                }
            }
        }

        public DashboardViewModel()
        {
            RecentTransactions = new ObservableCollection<Transaction>();
            LoadMockData();
            UpdateDashboardWelcome();
        }

        // פונקציה ציבורית שנקראת גם מה-OnAppearing של הדף
        public void UpdateDashboardWelcome()
        {
            if (App.Current is App currentApp && currentApp.CurrentUser != null)
            {
                if (currentApp.CurrentUser.IsAdmin)
                {
                    WelcomeTitle = "Hello Admin";
                }
                else
                {
                    WelcomeTitle = $"Hello {currentApp.CurrentUser.FirstName}";
                }
            }
            else
            {
                WelcomeTitle = "Hello Guest";
            }
        }

        private void LoadMockData()
        {
            RecentTransactions.Add(new Transaction
            {
                Id = "1",
                Amount = 450,
                Category = TransactionCategory.Food,
                Type = TransactionType.Expense,
                Date = DateTime.Now.AddDays(-1),
                Notes = "Weekly grocery shopping"
            });

            RecentTransactions.Add(new Transaction
            {
                Id = "2",
                Amount = 6200,
                Category = TransactionCategory.Salary,
                Type = TransactionType.Income,
                Date = DateTime.Now.AddDays(-2),
                Notes = "Monthly Salary"
            });

            RecentTransactions.Add(new Transaction
            {
                Id = "3",
                Amount = 120,
                Category = TransactionCategory.Entertainment,
                Type = TransactionType.Expense,
                Date = DateTime.Now,
                Notes = "Movie night with friends"
            });

            RecentTransactions.Add(new Transaction
            {
                Id = "4",
                Amount = 350,
                Category = TransactionCategory.Electricity,
                Type = TransactionType.Expense,
                Date = DateTime.Now.AddDays(-5),
                Notes = "Electricity bill"
            });

            TotalIncome = RecentTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            TotalExpenses = RecentTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            Balance = TotalIncome - TotalExpenses;
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