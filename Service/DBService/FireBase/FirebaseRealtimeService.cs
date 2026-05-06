using Firebase.Database;

namespace MyApp_SmartBills.Service.DBService.FireBase
{
    public class FirebaseRealtimeService : IDbInstance
    {
        protected FirebaseClient? _firebaseClient;

        public FirebaseRealtimeService()
        {
            _firebaseClient = new FirebaseClient("https://lielendproject-default-rtdb.europe-west1.firebasedatabase.app/");
        }
        public string Info()
        {
            return "Type: Google Firebase RealTime Database client";
        }
    }
}