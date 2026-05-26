using Microsoft.Extensions.Logging;
using MyApp_SmartBills.Service;
using MyApp_SmartBills.Service.DBService;
using MyApp_SmartBills.Service.DBService.FireBase;
using Microcharts.Maui;

namespace MyApp_SmartBills
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts() // מאתחל את מנוע הרינדור הגרפי
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                })
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // תיקון קריטי: רישום ה-Route בתוך ה-Shell כדי שכפתור ה-+ Add New Transaction יעביר אותך מסך בהצלחה!
            Routing.RegisterRoute(nameof(Views.AddEditTransactionPage), typeof(Views.AddEditTransactionPage));

            return app;
        }

        // פונקציית הרחבה לרישום כל מסכי הממשק (Views / Pages)
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<Views.SignInView>();
            builder.Services.AddTransient<Views.SignUpView>();
            builder.Services.AddTransient<Views.MainPageView>();
            builder.Services.AddTransient<Views.AdminView>();
            builder.Services.AddTransient<Views.UserListView>();
            builder.Services.AddTransient<Views.AccountView>();

            builder.Services.AddTransient<Views.DashboardPage>();
            builder.Services.AddTransient<Views.TransactionsListPage>();
            builder.Services.AddTransient<Views.WarrantiesListPage>();
            builder.Services.AddTransient<Views.ReportsPage>();

            // תיקון: רישום דף הוספת/עריכת תנועה במערכת הזרקת התלויות
            builder.Services.AddTransient<Views.AddEditTransactionPage>();

            return builder;
        }

        // פונקציית הרחבה לרישום כל מחלקות הלוגיקה (ViewModels)
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<ViewModels.AppShellViewModel>();
            builder.Services.AddTransient<ViewModels.SignInViewModel>();
            builder.Services.AddTransient<ViewModels.SignUpViewModel>();
            builder.Services.AddTransient<ViewModels.MainPageViewModel>();
            builder.Services.AddTransient<ViewModels.AdminViewModel>();
            builder.Services.AddTransient<ViewModels.UserListViewModel>();
            builder.Services.AddTransient<ViewModels.AccountViewModel>();

            builder.Services.AddTransient<ViewModels.DashboardViewModel>();
            builder.Services.AddTransient<ViewModels.ReportsViewModel>();
            builder.Services.AddTransient<ViewModels.TransactionsListViewModel>();

            // תיקון: רישום ה-ViewModel של מסך ההוספה כדי שה-Picker והכפתורים יתעוררו לחיים!
            builder.Services.AddTransient<ViewModels.AddEditTransactionViewModel>();

            return builder;
        }

        // פונקציית הרחבה לרישום שכבת השירותים והגישה לנתונים (Services / Repositories)
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<IFinancialDataService, FinancialDataService>();
            builder.Services.AddSingleton<IAppLogger, LogService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
            builder.Services.AddTransient<IAppUserRepository, FirebaseUsersRepository>();

            return builder;
        }
    }
}