using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp_SmartBills.Helper;
using MyApp_SmartBills.Service.DBService;
using MyApp_SmartBills.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyApp_SmartBills.ViewModels
{
    public partial class SignInViewModel : ObservableObject
    {
        private readonly IAppUserRepository _dbService;
        private string _userEmail;
        private string _userPassword;

        #region Properties
        public string UserEmail
        {
            get => _userEmail;
            set
            {
                if (_userEmail != value)
                {
                    _userEmail = value;
                    OnPropertyChanged();
                    (SignInCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        public string UserPassword
        {
            get => _userPassword;
            set
            {
                if (_userPassword != value)
                {
                    _userPassword = value;
                    OnPropertyChanged();
                    (SignInCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        [ObservableProperty]
        private string _passwordIconCode;

        [ObservableProperty]
        private bool _entryAsPassword;

        [ObservableProperty]
        private bool _signInMessageVisible;

        [ObservableProperty]
        private bool _isRememberMeChecked;

        [ObservableProperty]
        private bool _isDebugMode;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isBusy;
        #endregion

        public ICommand SignInCommand { get; }

        // תיקון: הסרת SignUpView מהבנאי למניעת תלות מעגלית
        public SignInViewModel(IAppUserRepository dbService)
        {
            // Development Mode Active Configuration
            _userEmail = "admin@gmail.com";
            _userPassword = "123456";

            _isBusy = false;
            _dbService = dbService;
            _isDebugMode = true;
            _entryAsPassword = true;
            _passwordIconCode = FontHelper.OPEN_EYE_ICON;

            SignInCommand = new Command(SignIn, () =>
                !(string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPassword)));
        }

        private async void SignIn()
        {
            IsBusy = true;
            try
            {
                var user = await _dbService.SignInAsync(UserEmail!, UserPassword!);
                IsBusy = false;

                // Set CurrentUser safely
                if (App.Current is App currentApp)
                {
                    currentApp.CurrentUser = user;
                }

                // תיקון גישה בטוחה להחלפת דף הבית הראשי של האפליקציה ל-Shell
                var appShell = IPlatformApplication.Current?.Services.GetService<AppShell>();
                if (appShell != null && Application.Current != null)
                {
                    Application.Current.MainPage = appShell;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ShowErrorMessage(ex.Message);
            }
        }

        [RelayCommand]
        private void TogglePassword()
        {
            EntryAsPassword = !EntryAsPassword;
            PasswordIconCode = EntryAsPassword ? FontHelper.OPEN_EYE_ICON : FontHelper.CLOSED_EYE_ICON;
        }

        [RelayCommand]
        private async Task NavigateToSignUp()
        {
            try
            {
                // ניווט מבוסס נתיב מנותק דרך מנגנון ה-Shell המובנה
                await Shell.Current.GoToAsync(nameof(SignUpView));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        // תיקון: הוספת הפקודה שהייתה חסרה ונקראה מתוך קובץ ה-XAML
        [RelayCommand]
        private async Task ForgetPassword()
        {
            // מימוש עתידי לשחזור סיסמה
            await Task.CompletedTask;
        }

        private void ShowErrorMessage(string message)
        {
            SignInMessageVisible = true;
            ErrorMessage = message;
        }
    }
}