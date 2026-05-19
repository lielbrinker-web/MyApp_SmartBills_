using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardPage(DashboardViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // רענון הברכה בכל כניסה מחדש למסך
            _viewModel?.UpdateDashboardWelcome();
        }
    }
}