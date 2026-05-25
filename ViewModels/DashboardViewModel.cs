using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MyApp_SmartBills.Model;
using MyApp_SmartBills.Service;

namespace MyApp_SmartBills.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IFinancialDataService _financialDataService;

        private ObservableCollection<Transaction> _recentTransactions;
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
            RecentTransactions = _financialDataService.GetTransactions();

            RefreshDashboardValues();
            UpdateDashboardWelcome();
        }

        public void RefreshDashboardValues()
        {
            TotalIncome = _financialDataService.GetTotalIncome();
            TotalExpenses = _financialDataService.GetTotalExpenses();
            Balance = _financialDataService.GetBalance();
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
                    WelcomeTitle = $"Hello {currentApp.CurrentUser.FirstName}";
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