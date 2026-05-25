using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microcharts;
using SkiaSharp;
using MyApp_SmartBills.Service;
using MyApp_SmartBills.Model;

namespace MyApp_SmartBills.ViewModels
{
    // החזרנו את ה-partial כדי לפתור את שגיאת הקומפילציה CS0260
    public partial class ReportsViewModel : INotifyPropertyChanged
    {
        private readonly IFinancialDataService _financialDataService;
        private Chart _expenseChart;

        public Chart ExpenseChart
        {
            get => _expenseChart;
            set
            {
                _expenseChart = value;
                OnPropertyChanged();
            }
        }

        public ReportsViewModel(IFinancialDataService financialDataService)
        {
            _financialDataService = financialDataService;
            LoadDynamicChartData();
        }

        public void LoadDynamicChartData()
        {
            var expenses = _financialDataService.GetTransactions()
                .Where(t => t.Type == TransactionType.Expense);

            if (!expenses.Any())
            {
                ExpenseChart = new DonutChart { Entries = Array.Empty<ChartEntry>() };
                return;
            }

            var entries = expenses
                .GroupBy(t => t.Category)
                .Select(group => new ChartEntry((float)group.Sum(t => t.Amount))
                {
                    Label = group.Key.ToString(),
                    ValueLabel = $"${group.Sum(t => t.Amount):N0}",
                    Color = SKColor.Parse(GetCategoryColor(group.Key))
                }).ToArray();

            ExpenseChart = new DonutChart
            {
                Entries = entries,
                LabelTextSize = 14f,
                Typeface = SKTypeface.FromFamilyName("Arial"),
                HoleRadius = 0.4f,
                LabelColor = SKColors.Black
            };
        }

        private string GetCategoryColor(TransactionCategory category)
        {
            return category switch
            {
                TransactionCategory.Food => "#FF5733",
                TransactionCategory.Electricity => "#33FF57",
                TransactionCategory.Entertainment => "#3357FF",
                TransactionCategory.Rent => "#F3FF33",
                TransactionCategory.Transport => "#FF33F3",
                _ => "#9B59B6"
            };
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