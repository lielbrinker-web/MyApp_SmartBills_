using MyApp_SmartBills.Views;
using System;
using System.Linq;
using Microsoft.Maui.Controls;

namespace MyApp_SmartBills
{
    public partial class AppShell : Shell
    {
        public AppShell(DashboardPage dashboardPage)
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SignInView), typeof(SignInView));
            Routing.RegisterRoute(nameof(SignUpView), typeof(SignUpView));
            Routing.RegisterRoute(nameof(Views.AddEditTransactionPage), typeof(Views.AddEditTransactionPage));

            // רישום מפורש של הניווט - זה פותר את שגיאת ה-unable to figure out route!
            Routing.RegisterRoute("UserListView", typeof(UserListView));
            Routing.RegisterRoute("AccountView", typeof(AccountView));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            if (Application.Current is App currentApp)
            {
                currentApp.CurrentUser = null;
            }

            var adminTab = this.FindByName<Tab>("AdminTab");
            if (adminTab != null)
            {
                adminTab.IsVisible = false;
            }

            var appWindow = Application.Current?.Windows.FirstOrDefault();
            if (appWindow != null)
            {
                var signInView = IPlatformApplication.Current?.Services.GetService<SignInView>();
                if (signInView != null)
                {
                    appWindow.Page = new NavigationPage(signInView);
                }
            }
        }
    }
}