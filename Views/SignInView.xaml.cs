using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views
{
    public partial class SignInView : ContentPage
    {
        // הבנאי שמקבל את ה-ViewModel באמצעות Dependency Injection
        public SignInView(SignInViewModel viewModel)
        {
            InitializeComponent();

            viewModel.Navigation = this.Navigation;

            // הגדרת ה-BindingContext בצורה נקייה
            BindingContext = viewModel;

            // תיקון: השורה הבעייתית שהייתה כאן (viewModel.Navigation = Navigation;) נמחקה!
        }
    }
}