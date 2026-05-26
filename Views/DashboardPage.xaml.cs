using System;
using Microsoft.Maui.Controls;
using MyApp_SmartBills.ViewModels; // מוודא שה-ViewModels מזוהים כמו שצריך

namespace MyApp_SmartBills.Views
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage(DashboardViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // המתודה חייבת להיות מוכלת בתוך ה-class!
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // רענון הנתונים הפיננסיים מפיירבייס כשהמסך עולה
            if (BindingContext is DashboardViewModel vm)
            {
                await vm.InitializeDashboardAsync();

                // עדכון כותרת הברוך הבא
                vm.UpdateDashboardWelcome();
            }
        }
    }
}