using Microsoft.Extensions.Logging;
using MyApp_SmartBills.Service;
using MyApp_SmartBills.Service.DBService;
using MyApp_SmartBills.Service.DBService.FireBase;

namespace MyApp_SmartBills
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    // רישום פונט האייקונים - ודאי שהשם ב-XAML תואם ל-MaterialIcons
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                })
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        // פונקציית הרחבה לרישום כל מסכי הממשק (Views / Pages)
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<Views.SignInView>();
            builder.Services.AddTransient<Views.SignUpView>();
            builder.Services.AddTransient<Views.MainPageView>();
            builder.Services.AddTransient<Views.AdminView>();
            builder.Services.AddTransient<Views.UsersListView>();
            builder.Services.AddTransient<Views.AccountView>();

            // הוספת הדפים החדשים של מערכת הניהול הפיננסי לצינור ה-Services
            builder.Services.AddTransient<Views.DashboardPage>();
            builder.Services.AddTransient<Views.TransactionsListPage>();
            builder.Services.AddTransient<Views.WarrantiesListPage>();
            builder.Services.AddTransient<Views.ReportsPage>();

            builder.Services.AddTransient<ViewModels.TransactionsListViewModel>();

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
            builder.Services.AddTransient<ViewModels.UsersListViewModel>();
            builder.Services.AddTransient<ViewModels.AccountViewModel>();

            // הוספת ה-ViewModel החדש של מסך הבית לצינור ה-Services
            builder.Services.AddTransient<ViewModels.DashboardViewModel>();

            return builder;
        }

        // פונקציית הרחבה לרישום שכבת השירותים והגישה לנתונים (Services / Repositories)
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IAppLogger, LogService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
            builder.Services.AddTransient<IAppUserRepository, FirebaseUsersRepository>();
            return builder;
        }
    }
}