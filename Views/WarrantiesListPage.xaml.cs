using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views
{
    public partial class WarrantiesListPage : ContentPage
    {
        public WarrantiesListPage()
        {
            InitializeComponent();
            // אם ה-BindingContext לא מוזרק מבחוץ, הוא נוצר מה-XAML וזה מצוין
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // קריאה לפונקציה שיוצרת את הטעינה האמיתית מהפיירבייס
            if (BindingContext is WarrantiesListViewModel vm)
            {
                await vm.LoadUserWarrantiesAsync();
            }
        }
    }
}