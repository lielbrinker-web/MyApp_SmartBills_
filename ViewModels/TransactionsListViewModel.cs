using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MyApp_SmartBills.Model;

namespace MyApp_SmartBills.ViewModels
{
    public class TransactionsListViewModel : INotifyPropertyChanged
    {
        private List<Transaction> _allTransactions = new List<Transaction>();
        private string _selectedFilter = "All";
        private string _selectedMonth = "All Months";

        public ObservableCollection<Transaction> FilteredTransactions { get; set; }

        public List<string> MonthsFilterList { get; } = new List<string>
        {
            "All Months", "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        public string SelectedMonth
        {
            get => _selectedMonth;
            set { _selectedMonth = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public ICommand FilterChangedCommand { get; }
        public ICommand NavigateToAddTransactionCommand { get; }

        public TransactionsListViewModel()
        {
            FilteredTransactions = new ObservableCollection<Transaction>();

            FilterChangedCommand = new Command<string>((filterType) =>
            {
                _selectedFilter = filterType;
                ApplyFilters();
            });

            NavigateToAddTransactionCommand = new Command(async () => await NavigateToAddTransaction());

            LoadMockTransactions();
        }

        private void LoadMockTransactions()
        {
            // תיקון מוחלט: שימוש ב-Category הקיים במודל שלך במקום Description או Title
            _allTransactions = new List<Transaction>
            {
               // new Transaction { Id = "1", Category = TransactionCategory.Rent, Amount = 1500, IsBusiness = true, Date = DateTime.Today },
               // new Transaction { Id = "2", Category = TransactionCategory.Groceries, Amount = 320, IsBusiness = false, Date = DateTime.Today.AddDays(-1) },
                new Transaction { Id = "3", Category = TransactionCategory.Salary, Amount = 4500, IsBusiness = true, Date = DateTime.Today.AddDays(-2) },
                new Transaction { Id = "4", Category = TransactionCategory.Electricity, Amount = 85, IsBusiness = false, Date = DateTime.Today.AddDays(-3) },
                new Transaction { Id = "5", Category = TransactionCategory.Other, Amount = 120.50, IsBusiness = false, Date = DateTime.Today.AddDays(-4) }
            };

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var result = _allTransactions.AsEnumerable();

            if (_selectedFilter == "Business") result = result.Where(t => t.IsBusiness);
            else if (_selectedFilter == "Personal") result = result.Where(t => !t.IsBusiness);

            if (SelectedMonth != "All Months")
            {
                int monthIndex = MonthsFilterList.IndexOf(SelectedMonth);
                result = result.Where(t => t.Date.Month == monthIndex);
            }

            FilteredTransactions.Clear();
            foreach (var item in result)
            {
                FilteredTransactions.Add(item);
            }
        }

        private async Task NavigateToAddTransaction()
        {
            if (Application.Current?.Windows.Count > 0 && Application.Current.Windows[0].Page is Page currentPage)
            {
                await currentPage.Navigation.PushAsync(new Views.AddEditTransactionPage());
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