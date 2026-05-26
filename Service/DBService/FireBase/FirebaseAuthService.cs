using Firebase.Auth;
using Firebase.Auth.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp_SmartBills.Service.DBService.FireBase
{
    internal class FirebaseAuthService : IAuthService
    {
        private FirebaseAuthClient? _authClient;
        private IAppLogger _logger;

        // תיקון: מימוש ה-Property מהאינטרפייס החדש
        public string CurrentUserId { get; private set; }

        public FirebaseAuthService(IAppLogger logger)
        {
            _logger = logger;

            // Initialize Firebase Authentication Client
            var config = new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyABahhR_hUAJ-SyDudb1vlcv-FI1NchKYs",
                AuthDomain = "lielsmartbills.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                    {
                        new EmailProvider()
                    },
            };
            _authClient = new FirebaseAuthClient(config);
            _logger = logger;
        }

        public async Task<string> SignIn(string userEmail, string userPassword)
        {
            string errorMessage = string.Empty;
            try
            {
                await _authClient!.SignInWithEmailAndPasswordAsync(userEmail, userPassword);

                // תיקון: שמירת ה-UID הנוכחי בשדה הציבורי של השירות
                CurrentUserId = _authClient.User.Info.Uid;

                return CurrentUserId;
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Message.Contains("INVALID_LOGIN_CREDENTIALS"))
                {
                    errorMessage = "Incorrect email or password!";
                    _logger.LogDebug($" SignInAuth failed: {userEmail} {userPassword}, {errorMessage}");
                }
                else
                {
                    errorMessage = "SignInAuth failed: Unknown exception!";
                    _logger.LogDebug($"SignInAuth failed: {userEmail} {userPassword}, Unknown exception!");
                }
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"SignInAuth failed: {userEmail} {userPassword}, {ex.Message}");
                throw new Exception("SignIn failed!");
            }
        }

        public async Task<string> CreateAuth(string userEmail, string userPassword)
        {
            try
            {
                await _authClient!.CreateUserWithEmailAndPasswordAsync(userEmail, userPassword);
                _logger.LogDebug($"AppUser Auth {userEmail} created successfully");

                // תיקון: שמירת ה-UID גם בזמן רישום משתמש חדש
                CurrentUserId = _authClient.User.Uid;

                return CurrentUserId;
            }
            catch (FirebaseAuthException ex)
            {
                string errorMessage = string.Empty;

                if (ex.Message.Contains("INVALID_EMAIL"))
                {
                    errorMessage = "Invalid email adress!";
                }
                if (ex.Message.Contains("EMAIL_EXISTS"))
                {
                    errorMessage = "This email already exists!";
                }
                if (ex.Message.Contains("WEAK_PASSWORD"))
                {
                    errorMessage = "Weak password!";
                }

                _logger.LogDebug($"CreateUserAuth failed: {ex.Message}");
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"CreateUserAuth failed: {ex.Message}");
                return "SignUp new user failed!";
            }
        }

        public async Task RemoveAuth(string userEmail, string userPassword)
        {
            try
            {
                await _authClient!.SignInWithEmailAndPasswordAsync(userEmail, userPassword);
                await _authClient.User.DeleteAsync();
                await _authClient!.SignInWithEmailAndPasswordAsync(
                    (App.Current as App)!.CurrentUser!.UserEmail,
                    (App.Current as App)!.CurrentUser!.UserPassword);

                _logger.LogDebug($"User {userEmail} removed from Auth successfully");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Remove user {userEmail} from Auth failed: {ex.Message}");
                throw new Exception("Remove user from Auth failed!");
            }
        }

        public async Task SignOut()
        {
            // תיקון: מימוש פונקציית הניתוק במקום זריקת שגיאה + איפוס ה-UID
            if (_authClient != null)
            {
                _authClient.SignOut();
            }
            CurrentUserId = null;
            await Task.CompletedTask;
        }
    }
}