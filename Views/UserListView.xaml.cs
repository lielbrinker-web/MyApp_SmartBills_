using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class UserListView : ContentPage
{
    public UserListView(UserListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is UserListViewModel vm)
        {
            _ = vm.LoadUsersFromFirebase();
        }
    }
}