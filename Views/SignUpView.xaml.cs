using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views;

public partial class SignUpView : ContentPage
{
    public SignUpView(SignUpViewModel vm)
    {
        InitializeComponent();
        vm.Navigation = this.Navigation;
        BindingContext = vm;
    }
}