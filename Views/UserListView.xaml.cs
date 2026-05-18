using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class UserListView : ContentPage
{
    public UserListView(UsersListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void InitializeComponent()
    {
        throw new NotImplementedException();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as UsersListViewModel)!.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        (BindingContext as UsersListViewModel)!.OnDisappearing();
    }
}