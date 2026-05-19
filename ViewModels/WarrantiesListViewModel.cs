using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MyApp_SmartBills.Model;

namespace MyApp_SmartBills.ViewModels
{
    public class WarrantiesListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WarrantyItem> Warranties { get; set; }

        public ICommand NavigateToAddWarrantyCommand { get; }

        public WarrantiesListViewModel()
        {
            Warranties = new ObservableCollection<WarrantyItem>();

            // פקודה למעבר למסך הוספת אחריות
            NavigateToAddWarrantyCommand = new Command(async () => await NavigateToAddWarranty());

            LoadMockData();
        }

        private void LoadMockData()
        {
            // נתוני דמה לבדיקת ה-UI והצבעים
            Warranties.Add(new WarrantyItem
            {
                ProductName = "Samsung Refrigerator",
                PurchaseDate = DateTime.Today.AddMonths(-11), // נקנה לפני 11 חודשים
                WarrantyMonths = 12, // אחריות לשנה (נשאר חודש אחד -> יצבע בכתום/אדום)
                ReceiptImageSource = "receipt_placeholder.png"
            });

            Warranties.Add(new WarrantyItem
            {
                ProductName = "Apple iPhone 15",
                PurchaseDate = DateTime.Today.AddMonths(-3),
                WarrantyMonths = 24, // אחריות לשנתיים (מצב מצוין -> ירוק)
                ReceiptImageSource = "receipt_placeholder.png"
            });

            Warranties.Add(new WarrantyItem
            {
                ProductName = "Dyson Vacuum Cleaner",
                PurchaseDate = DateTime.Today.AddMonths(-25),
                WarrantyMonths = 24, // האחריות כבר פגה (פחות מ-0 ימים -> ירוץ באדום)
                ReceiptImageSource = "receipt_placeholder.png"
            });
        }

        private async Task NavigateToAddWarranty()
        {
            // מעבר למסך ההוספה (נשתמש בנתיב הרגיל של Shell או Navigation לפי מה שמוגדר אצלך)
            // אם את משתמשת ב-Shell, זה יהיה: Shell.Current.GoToAsync("AddWarrantyPage");
            // כרגע נעשה ניווט ישיר דרך ה-Page הנוכחי לטובת הפשטות:
            if (Application.Current?.Windows[0].Page is Shell shell)
            {
                await shell.Navigation.PushAsync(new Views.AddWarrantyPage());
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}