using MyApp_SmartBills.Views;

namespace MyApp_SmartBills
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // רישום נתיב הניווט בצורה מפורשת עבור ה-Shell
            Routing.RegisterRoute(nameof(SignUpView), typeof(SignUpView));
            
        }
    }
}
