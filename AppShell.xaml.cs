using MyApp_SmartBills.Views;

namespace MyApp_SmartBills
{
    public partial class AppShell : Shell
    {
        // הבנאי כעת מקבל את ה-DashboardPage מה-Container של האפליקציה
        public AppShell(DashboardPage dashboardPage)
        {
            InitializeComponent();

            // רישום נתיבי הניווט
            Routing.RegisterRoute(nameof(SignInView), typeof(SignInView));
            Routing.RegisterRoute(nameof(SignUpView), typeof(SignUpView));
            Routing.RegisterRoute(nameof(Views.AddEditTransactionPage), typeof(Views.AddEditTransactionPage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // חזרה למסך הלוגין
            if (Application.Current is App currentApp)
            {
                currentApp.CurrentUser = null;
            }

            var appWindow = Application.Current?.Windows.FirstOrDefault();
            if (appWindow != null)
            {
                // שליפת ה-SignInView המקורי הרשום במערכת כדי לשמור על הזרקת תלויות
                var signInView = IPlatformApplication.Current?.Services.GetService<SignInView>();
                if (signInView != null)
                {
                    appWindow.Page = new NavigationPage(signInView);
                }
            }
        }
    }
}