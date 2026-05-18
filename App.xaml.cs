using MyApp_SmartBills.Model;
using MyApp_SmartBills.Views;

namespace MyApp_SmartBills
{
    public partial class App : Application
    {
        private Page _page;
        public AppUser? CurrentUser { get; set; } = null;

        public App(SignInView view)
        {
            InitializeComponent();
            _page = view;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // פתיחת ה-SignInView בתוך NavigationPage כדי לאפשר תנועה קדימה ואחורה (ל-SignUp ובחזרה)
            return new Window(new NavigationPage(_page));
        }
    }
}