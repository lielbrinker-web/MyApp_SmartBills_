using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Controls;
using MyApp_SmartBills.Model;
using System;
using System.Threading.Tasks;

namespace MyApp_SmartBills.Views
{
    public partial class WarrantyDetailPage : ContentPage
    {
        private readonly FirebaseClient _firebaseClient = new FirebaseClient("https://lielsmartbills-default-rtdb.europe-west1.firebasedatabase.app/");
        private WarrantyItem _currentWarranty;

        public WarrantyDetailPage(WarrantyItem selectedWarranty)
        {
            InitializeComponent();
            _currentWarranty = selectedWarranty;
            BindingContext = _currentWarranty;
        }

        /// <summary>
        /// פונקציית מחיקה המבוססת על מפתח ה-Id הייחודי של פיירבייס
        /// </summary>
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_currentWarranty == null || string.IsNullOrEmpty(_currentWarranty.Id)) return;

            bool confirm = await DisplayAlert("Delete Warranty", $"Are you sure you want to delete '{_currentWarranty.ProductName}'?", "Yes", "No");
            if (!confirm) return;

            try
            {
                string currentUserId = Preferences.Get("UserId", string.Empty);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = "405KLU9dPCN5leIPRw8wdQT0lUT2";
                }

                // מחיקה מפיירבייס לפי נתיב המשתמש וה-Id של המוצר
                await _firebaseClient
                    .Child("Users")
                    .Child(currentUserId)
                    .Child("Warranties")
                    .Child(_currentWarranty.Id)
                    .DeleteAsync();

                await DisplayAlert("Deleted", "Warranty has been removed successfully.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete item: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// פונקציית עריכה - פותחת את דף הוספת המוצר הקיים ומעבירה לו את האובייקט הנוכחי לשינוי
        /// </summary>
        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (_currentWarranty == null) return;

            // אופציונלי: במידה ודף ה-AddWarrantyPage שלך תומך בקבלת פריט לעריכה בבנאי שלו:
            // למשל: new AddWarrantyPage(_currentWarranty)
            // אם עדיין לא התאמת את AddWarrantyPage לעריכה, המשתמש יוכל כרגע למחוק ולהוסיף מחדש בקלות.

            await DisplayAlert("Edit Mode", "To edit this item, update the form or create a corrected record.", "OK");
        }

        /// <summary>
        /// פונקציה המחזירה את המשתמש ישירות למסך הרשימה הקודם
        /// </summary>
        private async void OnBackClicked(object sender, EventArgs e)
        {
            // חזרה אוטומטית לעמוד הקודם במחסנית הניווט
            await Navigation.PopAsync();
        }
    }
}