using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp_SmartBills.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        public MainPageViewModel()
        {
            // הגדרת ערך ברירת מחדל זמני כדי שלא יקרוס בעליית האפליקציה
            _name = "Hello";
        }

        // פונקציה חדשה שנקרא לה בכל פעם שהדף מוצג
        public void RefreshUserData()
        {
            var currentUser = (App.Current as App)?.CurrentUser;
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.FirstName))
            {
                // עדכון ה-Property הציבורי (באות גדולה) כדי שה-XAML יתעדכן אוטומטית
                Name = "Hello " + currentUser.FirstName;
            }
        }

        [RelayCommand]
        private async Task Settings()
        {
            await Shell.Current.GoToAsync("AccountView");
        }
    }
}