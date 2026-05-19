using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApp_SmartBills.Helper;
using MyApp_SmartBills.Model;
using MyApp_SmartBills.Service.DBService;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MyApp_SmartBills.ViewModels
{
    public partial class SignUpViewModel : ObservableObject
    {
        private AppUser? newUser;
        private readonly IAppUserRepository _dbService;

        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _userEmail = string.Empty;
        private string _userPassword = string.Empty;
        private string _phoneNumber = string.Empty;

        #region Properties
        public INavigation Navigation { get; set; }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                    ((Command)SignUpCommand).ChangeCanExecute();
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                    ((Command)SignUpCommand).ChangeCanExecute();
                }
            }
        }

        public string UserEmail
        {
            get => _userEmail;
            set
            {
                if (_userEmail != value)
                {
                    _userEmail = value;
                    OnPropertyChanged();
                    ((Command)SignUpCommand).ChangeCanExecute();
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
                    ((Command)SignUpCommand).ChangeCanExecute();
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                    ((Command)SignUpCommand).ChangeCanExecute();
                }
            }
        }

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _passwordIconCode;

        [ObservableProperty]
        private bool _entryAsPassword;

        [ObservableProperty]
        private bool _signUpMessageVisible;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public ICommand SignUpCommand { get; }
        #endregion

        public SignUpViewModel(IAppUserRepository dbService)
        {
            _isBusy = false;
            _dbService = dbService;
            _entryAsPassword = true;
            _passwordIconCode = FontHelper.OPEN_EYE_ICON;
            SignUpCommand = new Command(SignUp, Validate);
        }

        private async void SignUp()
        {
            IsBusy = true;

            newUser = new AppUser()
            {
                FirstName = FirstName,
                LastName = LastName,
                UserEmail = UserEmail,
                UserPassword = UserPassword,
                UserMobile = PhoneNumber,
                RegDate = DateTime.Now.ToShortDateString(),
                UBDate = DateTime.Now.ToShortDateString()
            };

            try
            {
                newUser.Id = await _dbService!.CreateAsync(newUser);

                IsBusy = false;

                if (App.Current is App currentApp)
                {
                    currentApp.CurrentUser = newUser;
                }

                var mainPage = IPlatformApplication.Current!.Services.GetService<AppShell>();
                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = mainPage;
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
            if (EntryAsPassword)
                PasswordIconCode = FontHelper.OPEN_EYE_ICON;
            else
                PasswordIconCode = FontHelper.CLOSED_EYE_ICON;
        }

        [RelayCommand]
        private async Task NavigateToSignIn()
        {
            try
            {
                if (Navigation != null)
                {
                    await Navigation.PopAsync();
                }
            }
            catch (Exception)
            {
                // Error handling
            }
        }

        private bool Validate()
        {
            var fnameOK = !string.IsNullOrEmpty(FirstName);
            var lnameOK = !string.IsNullOrEmpty(LastName);
            var emailOK = !string.IsNullOrEmpty(UserEmail);
            var passOK = !string.IsNullOrEmpty(UserPassword) && UserPassword.Length > 5;
            var mobileOK = !string.IsNullOrEmpty(PhoneNumber) && PhoneNumber.Length == 10;

            return fnameOK && lnameOK && emailOK && passOK && mobileOK;
        }

        private void ShowErrorMessage(string message)
        {
            SignUpMessageVisible = true;
            ErrorMessage = message;
        }
    }
}