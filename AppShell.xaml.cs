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

            // רישום מפורש של הניווט
            Routing.RegisterRoute("UserListView", typeof(UserListView));
            Routing.RegisterRoute("AccountView", typeof(AccountView));
            Routing.RegisterRoute(nameof(AccountView), typeof(AccountView));

            // פתרון הניווט האוטומטי: מאזינים לכל לחיצה על הטאבים והתפריטים של ה-Shell
            this.Navigating += OnShellNavigating;
        }

        private async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            // בדיקה: אם המשתמש לוחץ על הטאב שמוביל לאזור האדמין, והוא כרגע נמצא בתוך עריכת משתמש
            if (e.Target.Location.OriginalString.Contains("AdminView") &&
                Shell.Current.CurrentPage is AccountView)
            {
                // 1. מבטלים את הניווט הרגיל כדי שלא יטען את אותו הדף שוב
                e.Cancel();

                // 2. מקפיצים אותו ישירות לדף רשימת המשתמשים
                await Shell.Current.GoToAsync("UserListView");
            }
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