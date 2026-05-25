using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // הרצת רענון הערכים והברכה בכל פעם שהדף עולה
        if (BindingContext is DashboardViewModel vm)
        {
            vm.RefreshDashboardValues();
            vm.UpdateDashboardWelcome();
        }
    }
}