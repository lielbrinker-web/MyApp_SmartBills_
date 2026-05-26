using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views
{
    public partial class AddEditTransactionPage : ContentPage
    {
        // הזרקה של ה-ViewModel ישירות לבנאי של הדף
        public AddEditTransactionPage(AddEditTransactionViewModel viewModel)
        {
            InitializeComponent();

            // השורה הזו היא ה"דבק" שמחבר את ה-XAML ל-ViewModel! בלעדיה שום דבר לא יעבוד
            BindingContext = viewModel;
        }
    }
}