using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks; // הוסף כדי לתמוך ב-Task
using Microcharts;
using SkiaSharp;
using MyApp_SmartBills.Service;
using MyApp_SmartBills.Model;

namespace MyApp_SmartBills.ViewModels
{
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

            // מחקנו מכאן את הזימון הישיר של הגרף, כי הבנאי לא יכול להמתין (await) לנתונים מהרשת.
            // הטעינה תתבצע ברגע שהמסך נפתח.
        }

        // שינוי ל-async Task כדי לאפשר עבודה מול פיירבייס בצורה תקינה
        public async Task LoadDynamicChartDataAsync()
        {
            try
            {
                // קריאה אסינכרונית עם await לשירות המעודכן
                var allTransactions = await _financialDataService.GetTransactionsAsync();

                if (allTransactions == null)
                {
                    ExpenseChart = new DonutChart { Entries = Array.Empty<ChartEntry>() };
                    return;
                }

                // סינון ההוצאות מתוך האוסף שהתקבל
                var expenses = allTransactions.Where(t => t.Type == TransactionType.Expense);

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error building charts: {ex.Message}");
            }
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