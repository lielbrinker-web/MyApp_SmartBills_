using System;
using System.Collections.ObjectModel;
using System.Linq;
using MyApp_SmartBills.Model;

namespace MyApp_SmartBills.Service
{
    public interface IFinancialDataService
    {
        ObservableCollection<Transaction> GetTransactions();
        void AddTransaction(Transaction transaction);
        ObservableCollection<WarrantyItem> GetWarranties();
        void AddWarranty(WarrantyItem warranty);
        double GetTotalIncome();
        double GetTotalExpenses();
        double GetBalance();
    }

    public class FinancialDataService : IFinancialDataService
    {
        private readonly ObservableCollection<Transaction> _transactions;
        private readonly ObservableCollection<WarrantyItem> _warranties;

        public FinancialDataService()
        {
            _transactions = new ObservableCollection<Transaction>();
            _warranties = new ObservableCollection<WarrantyItem>();
            LoadInitialData();
        }

        public ObservableCollection<Transaction> GetTransactions() => _transactions;

        public void AddTransaction(Transaction transaction)
        {
            // סעיף 3: זיהוי אוטומטי של סוג התנועה לפי הקטגוריה
            if (transaction.Category == TransactionCategory.Salary)
            {
                transaction.Type = TransactionType.Income;
            }
            else
            {
                transaction.Type = TransactionType.Expense;
            }

            // הוספה לראש הרשימה
            _transactions.Insert(0, transaction);
        }

        public ObservableCollection<WarrantyItem> GetWarranties() => _warranties;

        public void AddWarranty(WarrantyItem warranty)
        {
            _warranties.Insert(0, warranty);
        }

        public double GetTotalIncome() => _transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        public double GetTotalExpenses() => _transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        public double GetBalance() => GetTotalIncome() - GetTotalExpenses();

        private void LoadInitialData()
        {
            // נתוני ברירת מחדל ראשוניים כדי שהמסכים לא יהיו ריקים לגמרי בהתחלה
            _transactions.Add(new Transaction { Amount = 6200, Category = TransactionCategory.Salary, Type = TransactionType.Income, Notes = "Monthly Salary", IsBusiness = false });
            _transactions.Add(new Transaction { Amount = 450, Category = TransactionCategory.Food, Type = TransactionType.Expense, Notes = "Weekly grocery shopping", IsBusiness = false });
            _transactions.Add(new Transaction { Amount = 120, Category = TransactionCategory.Entertainment, Type = TransactionType.Expense, Notes = "Movie night", IsBusiness = false });
        }
    }
}