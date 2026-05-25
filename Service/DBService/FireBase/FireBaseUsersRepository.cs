using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using MyApp_SmartBills.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp_SmartBills.Service.DBService.FireBase
{
    public class FirebaseUsersRepository : FirebaseRealtimeService, IAppUserRepository
    {
        private readonly IAuthService _authService;
        private readonly IAppLogger _appLogger;

        // תיקון קריטי 1: המשתנה הפנימי private FirebaseClient _firebaseClient נמחק!
        // כעת הקוד ישתמש בצורה נכונה ב-Client שמוגדר ומאותחל בתוך מחלקת האב FirebaseRealtimeService.

        // תיקון קריטי 2: הוספת : base() בבנאי כדי לוודא שמחלקת האב מאתחלת את ה-Client שלה בזמן עליית השירות
        public FirebaseUsersRepository(IAuthService authService, IAppLogger appLogger) : base()
        {
            _authService = authService;
            _appLogger = appLogger;
        }

        public async Task<AppUser> SignInAsync(string userEmail, string userPassword)
        {
            try
            {
                string userId = await _authService.SignIn(userEmail, userPassword);
                AppUser appUser = await GetUserByIdAsync(userId);
                _appLogger.LogDebug($"FirebaseUsersRepository {userEmail} SignIn successfully");
                return appUser;
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseUsersRepository SignIn failed: {ex.Message}");
                if (!ex.Message.Contains("Incorrect email or password"))
                    throw new Exception("SignIn failed!");

                throw new Exception(ex.Message);
            }
        }

        public async Task<string> CreateAsync(AppUser appUser)
        {
            try
            {
                string userId = await _authService.CreateAuth(appUser.UserEmail!, appUser.UserPassword!);
                appUser.Id = userId;
                await RegisterAppUser(appUser);
                _appLogger.LogDebug($"FirebaseUsersRepository {appUser.UserEmail} SignUp successfully");
                return userId;
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseUsersRepository SignIn failed: {ex.Message}");
                if (!ex.Message.Contains("RealTimeDB"))
                    throw new Exception(ex.Message);

                throw new Exception("SignUp new user failed!");
            }
        }

        public async Task DeleteAsync(AppUser appUser)
        {
            try
            {
                // 1 Delete user data from Firebase Auth module
                await _authService.RemoveAuth(appUser.UserEmail!, appUser.UserPassword!);

                // 2 Delete user data from Realtime Database
                // (משתמש כעת בגלובלי התקין של מחלקת האב)
                await _firebaseClient!
                    .Child("users")
                    .Child(appUser.Id)
                    .DeleteAsync();
                _appLogger.LogDebug($"FirebaseUsersRepository Delete User {appUser.UserEmail} successfully.");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"FirebaseUsersRepository Delete User {appUser.UserEmail} failed: {ex.Message}");
                throw new Exception("Delete user failed!");
            }
        }

        // --- התיקון המרכזי כאן: הפונקציה מומשה בצורה אסינכרונית מלאה שמתאימה לאינטרפייס ול-ViewModel ---
        public async Task<List<AppUser>> GetAllAsync()
        {
            try
            {
                var users = await _firebaseClient!
                    .Child("users")
                    .OnceAsync<AppUser>();

                return users.Select(u => new AppUser()
                {
                    Id = u.Object.Id,
                    FirstName = u.Object.FirstName,
                    LastName = u.Object.LastName,
                    UserEmail = u.Object.UserEmail,
                    UserPassword = u.Object.UserPassword,
                    RegDate = u.Object.RegDate,
                    UBDate = u.Object.UBDate,
                    IsAdmin = u.Object.IsAdmin
                }).ToList();
            }
            catch (FirebaseException ex)
            {
                _appLogger.LogDebug($"GetAllUsers failed: {ex.Message}");
                return new List<AppUser>();
            }
        }

        public async Task<AppUser> GetUserByIdAsync(string userId)
        {
            string errorMessage = string.Empty;
            try
            {
                // כאן הייתה הקריסה - כעת היא נפתרה מכיוון שהאובייקט מאותחל מהאב
                var user = await _firebaseClient!
                    .Child("users")
                    .Child(userId)
                    .OnceSingleAsync<AppUser>();

                return user;
            }
            catch (FirebaseException ex)
            {
                if (ex.Message.Contains("401") || ex.Message.Contains("Permission denied"))
                {
                    errorMessage = "GetUserByIdAsync failed: Permissions denied!";
                }
                else if (ex.Message.Contains("404"))
                {
                    errorMessage = "GetUserByIdAsync failed: Wrong db path!";
                }
                else
                {
                    errorMessage = "GetUserByIdAsync failed: Unknown exception!";
                }

                _appLogger.LogDebug($"FirebaseUsersRepository {errorMessage}");
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"FirebaseUsersRepository GetUserByIdAsync failed! {ex.Message}");
            }
        }

        public async Task UpdateAsync(AppUser appUser)
        {
            try
            {
                await _firebaseClient!
                    .Child("users")
                    .Child(appUser.Id)
                    .PatchAsync(new
                    {
                        FirstName = appUser.FirstName,
                        LastName = appUser.LastName,
                        UserMobile = appUser.UserMobile
                    });

                _appLogger.LogDebug($"Update user {appUser.UserEmail} detailes successfully.");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"Error updating user details: {ex.Message}");
                throw new Exception("Update failed!");
            }
        }

        public async Task RegisterAppUser(AppUser appUser)
        {
            try
            {
                await _firebaseClient!
               .Child("users")
               .Child(appUser.Id)
               .PutAsync(new AppUser()
               {
                   Id = appUser.Id,
                   FirstName = appUser.FirstName,
                   LastName = appUser.LastName,
                   UserEmail = appUser.UserEmail,
                   UserPassword = appUser.UserPassword,
                   UserMobile = appUser.UserMobile,
                   RegDate = appUser.RegDate,
                   UBDate = appUser.UBDate,
                   IsAdmin = appUser.IsAdmin
               });
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"RealTimeDB SignUp failed: {ex.Message}");
                throw new Exception("RealTimeDB add new user failed");
            }
        }

        public async Task SetToAdmin(string userId)
        {
            try
            {
                await _firebaseClient!
                    .Child("users")
                    .Child(userId)
                    .PatchAsync(new { IsAdmin = true });

                _appLogger.LogDebug("User admin status updated successfully.");
            }
            catch (Exception ex)
            {
                _appLogger.LogDebug($"Error updating field: {ex.Message}");
                throw new Exception("SetToAdmin failed!");
            }
        }

        public IObservable<FirebaseEvent<AppUser>> SubscribeToUserChanges()
        {
            try
            {
                return _firebaseClient!
                .Child("users")
                .AsObservable<AppUser>();
            }
            catch (Exception ex)
            {
                _appLogger.LogError("SubscribeToUserChanges failed: " + ex.Message);
                throw new Exception("SubscribeToUserChanges failed!");
            }
        }
    }
}