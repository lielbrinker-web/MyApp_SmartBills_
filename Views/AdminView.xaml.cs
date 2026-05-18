using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class AdminView : ContentPage
{
    public AdminView(AdminViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}