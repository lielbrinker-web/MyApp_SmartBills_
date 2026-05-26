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
using MyApp_SmartBills.Service; // ודא שקיים עבור השירות

namespace MyApp_SmartBills.ViewModels
{
    public class TransactionsListViewModel : INotifyPropertyChanged
    {
        private readonly IFinancialDataService _financialDataService;
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

        // הזרקת שירות הנתונים הפיננסיים בבנאי
        public TransactionsListViewModel(IFinancialDataService financialDataService)
        {
            _financialDataService = financialDataService;
            FilteredTransactions = new ObservableCollection<Transaction>();

            FilterChangedCommand = new Command<string>((filterType) =>
            {
                _selectedFilter = filterType;
                ApplyFilters();
            });

            NavigateToAddTransactionCommand = new Command(async () => await NavigateToAddTransaction());
        }

        // מתודה אסינכרונית חדשה שמחליפה את נתוני הדמה בנתונים חיים מפיירבייס
        public async Task LoadUserTransactionsAsync()
        {
            try
            {
                var liveList = await _financialDataService.GetTransactionsAsync();
                _allTransactions = liveList != null ? liveList.ToList() : new List<Transaction>();

                // הפעלת הסינון המובנה על המידע האמיתי שחזר
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching history: {ex.Message}");
            }
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
            // מיון מהחדש ביותר לישן ביותר בהיסטוריית התנועות
            foreach (var item in result.OrderByDescending(t => t.Date))
            {
                FilteredTransactions.Add(item);
            }
        }

        private async Task NavigateToAddTransaction()
        {
            await Shell.Current.GoToAsync(nameof(Views.AddEditTransactionPage));
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