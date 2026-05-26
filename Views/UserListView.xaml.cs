using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class UserListView : ContentPage
{
    public UserListView(UserListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // איפוס אוטומטי של המשתמש הנבחר בכל פעם שהדף עולה למסך
        if (BindingContext is UserListViewModel viewModel)
        {
            viewModel.ResetSelection();
            // אופציונלי: ריענון הרשימה מחדש מפיירבייס בעת חזרה לדף
            _ = viewModel.LoadUsersFromFirebase();
        }
    }
}