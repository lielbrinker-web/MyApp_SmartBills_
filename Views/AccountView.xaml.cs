using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class AccountView : ContentPage
{
	public AccountView(AccountViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}