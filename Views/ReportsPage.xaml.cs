using System;
using Microsoft.Maui.Controls;
using MyApp_SmartBills.ViewModels;

namespace MyApp_SmartBills.Views
{
    public partial class ReportsPage : ContentPage
    {
        private readonly ReportsViewModel _viewModel;

        public ReportsPage(ReportsViewModel viewModel)
        {
            InitializeComponent();

            // השמת ה-ViewModel שהוזרק מהמערכת
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        /// <summary>
        /// אירוע המתחרש בכל פעם שהמסך הופך לגלוי למשתמש
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // קריאה לפונקציה המצוינת שכתבת כדי למשוך נתונים ולרענן את הגרף
            if (_viewModel != null)
            {
                await _viewModel.LoadDynamicChartDataAsync();
            }
        }
    }
}