using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Firebase.Database;
using Firebase.Database.Query;
using MyApp_SmartBills.Model;

namespace MyApp_SmartBills.ViewModels
{
    public class WarrantiesListViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseClient _firebaseClient = new FirebaseClient("https://lielsmartbills-default-rtdb.europe-west1.firebasedatabase.app/");

        private ObservableCollection<WarrantyItem> _warranties;
        public ObservableCollection<WarrantyItem> Warranties
        {
            get => _warranties;
            set { _warranties = value; OnPropertyChanged(); }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        public ICommand NavigateToAddWarrantyCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewReceiptCommand { get; }

        public WarrantiesListViewModel()
        {
            Warranties = new ObservableCollection<WarrantyItem>();

            NavigateToAddWarrantyCommand = new Command(async () => await NavigateToAddWarranty());
            RefreshCommand = new Command(async () => await LoadUserWarrantiesAsync());
            ViewReceiptCommand = new Command<WarrantyItem>(async (item) => await ViewReceiptDetails(item));
        }

        public async Task LoadUserWarrantiesAsync()
        {
            IsRefreshing = true;
            try
            {
                string currentUserId = Preferences.Get("UserId", string.Empty);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = "405KLU9dPCN5leIPRw8wdQT0lUT2";
                }

                var firebaseWarranties = await _firebaseClient
                    .Child("Users")
                    .Child(currentUserId)
                    .Child("Warranties")
                    .OnceAsync<WarrantyItem>();

                Warranties.Clear();

                foreach (var item in firebaseWarranties)
                {
                    var warranty = item.Object;
                    warranty.Id = item.Key;
                    Warranties.Add(warranty);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching warranties: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// ניווט לדף הפירוט המלא במקום פתיחת חלונית טקסט קופצת
        /// </summary>
        private async Task ViewReceiptDetails(WarrantyItem item)
        {
            if (item == null) return;

            if (Application.Current?.Windows[0].Page is Shell shell)
            {
                var detailPage = new Views.WarrantyDetailPage(item);

                // מאזין לאירוע היעלמות הדף כדי לרענן את הרשימה אוטומטית אם המשתמש מחק או ערך
                detailPage.Disappearing += async (s, e) => await LoadUserWarrantiesAsync();

                await shell.Navigation.PushAsync(detailPage);
            }
        }

        private async Task NavigateToAddWarranty()
        {
            if (Application.Current?.Windows[0].Page is Shell shell)
            {
                var addPage = new Views.AddWarrantyPage();
                addPage.Disappearing += async (s, e) => await LoadUserWarrantiesAsync();
                await shell.Navigation.PushAsync(addPage);
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