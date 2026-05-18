using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class MainPageView : ContentPage
{
    public MainPageView(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // מתודה שמתעוררת אוטומטית בכל פעם שהמסך מופיע למשתמש
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // גישה בטוחה ל-ViewModel ועדכון נתוני המשתמש
        if (BindingContext is MainPageViewModel vm)
        {
            vm.RefreshUserData();
        }
    }
}