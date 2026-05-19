using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class TransactionsListPage : ContentPage
{
    // Injected automatically by MauiProgram service provider pipeline context
    public TransactionsListPage(TransactionsListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}