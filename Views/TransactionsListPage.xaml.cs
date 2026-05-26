using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class TransactionsListPage : ContentPage
{
    public TransactionsListPage(TransactionsListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // טעינה ורענון אוטומטי של הרשימה בכל פעם שהמסך עולה
        if (BindingContext is TransactionsListViewModel vm)
        {
            await vm.LoadUserTransactionsAsync();
        }
    }
}