using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp_SmartBills.Helper;
using MyApp_SmartBills.Service.DBService;
using MyApp_SmartBills.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage; // נדרש עבור SecureStorage ו-Preferences

namespace MyApp_SmartBills.ViewModels
{
    public partial class SignInViewModel : ObservableObject
    {
        private Page _signupView;
        private readonly IAppUserRepository _dbService;
        private string _userEmail;
        private string _userPassword;

        #region Properties
        public INavigation Navigation { get; set; }
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

        public SignInViewModel(IAppUserRepository dbService, SignUpView signupView)
        {
            _signupView = signupView;
            _dbService = dbService;

            // 1. ביטול המילוי האוטומטי הקבוע שהיה כאן! (שדות ריקים כברירת מחדל)
            _userEmail = string.Empty;
            _userPassword = string.Empty;

            _isBusy = false;
            _isDebugMode = false; // ביטול מצב הפיתוח במסך
            _entryAsPassword = true;
            _passwordIconCode = FontHelper.OPEN_EYE_ICON;

            SignInCommand = new Command(SignIn, () =>
                !(string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPassword)));

            // 2. הפעלת בדיקת "Remember Me" בעת עליית המסך
            _ = InitRememberMeAsync();
        }

        private async void SignIn()
        {
            IsBusy = true;
            try
            {
                var user = await _dbService.SignInAsync(UserEmail!, UserPassword!);

                // 3. שמירה או ניקוי של פרטי המשתמש לפי בחירתו ב-Remember Me
                if (IsRememberMeChecked)
                {
                    await SecureStorage.Default.SetAsync("saved_email", UserEmail);
                    await SecureStorage.Default.SetAsync("saved_password", UserPassword);
                    Preferences.Default.Set("remember_me_checked", true);
                }
                else
                {
                    SecureStorage.Default.Remove("saved_email");
                    SecureStorage.Default.Remove("saved_password");
                    Preferences.Default.Set("remember_me_checked", false);
                }

                IsBusy = false;

                // השמה בטוחה של המשתמש הנוכחי במחלקת ה-App
                if (Application.Current is App currentApp)
                {
                    currentApp.CurrentUser = user;
                }

                // שליפת ה-AppShell הרשום במערכת
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var appShell = IPlatformApplication.Current?.Services.GetService<AppShell>();
                    var appWindow = Application.Current?.Windows.FirstOrDefault();

                    if (appWindow != null && appShell != null)
                    {
                        // הפעלת בדיקת הרשאות אדמין ישירות בתוך ה-Shell (נטפל בזה בשלב הבא)
                        if (UserEmail.ToLower() == "admin@gmail.com")
                        {
                            var adminTab = appShell.FindByName<Tab>("AdminTab");
                            if (adminTab != null) adminTab.IsVisible = true;
                        }

                        appWindow.Page = appShell;
                    }
                });
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ShowErrorMessage(ex.Message);
            }
        }

        // טעינת הנתונים השמורים אם המשתמש סימן בעבר "זכור אותי"
        private async Task InitRememberMeAsync()
        {
            try
            {
                IsRememberMeChecked = Preferences.Default.Get("remember_me_checked", false);

                if (IsRememberMeChecked)
                {
                    var email = await SecureStorage.Default.GetAsync("saved_email");
                    var password = await SecureStorage.Default.GetAsync("saved_password");

                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                    {
                        UserEmail = email;
                        UserPassword = password;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Remember Me load failed: {ex.Message}");
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
                if (Navigation != null)
                {
                    await Navigation.PushAsync(_signupView);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ForgetPassword()
        {
            if (string.IsNullOrWhiteSpace(UserEmail))
            {
                await Shell.Current.DisplayAlert("Forgot Password", "Please enter your email address first in the input field.", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                // קריאה זמנית לשחזור - נחבר את זה סופית ברגע שנעדכן את ה-AuthService שלך בשלב הבא
                await Shell.Current.DisplayAlert("Reset Link Sent", $"If an account exists for {UserEmail}, a password reset link has been sent.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ShowErrorMessage(string message)
        {
            SignInMessageVisible = true;
            ErrorMessage = message;
        }
    }
}