using System;

namespace MyApp_SmartBills.Model
{
    public class WarrantyItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // מזהה המשתמש - זה יאפשר לנו בעתיד לסנן "רק את המוצרים של המשתמש המחובר"
        public string UserId { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        // משך האחריות בחודשים (למשל: 12, 24, 36)
        public int WarrantyMonths { get; set; }

        // נתיב לתמונת הקבלה (מקומי או URL בעתיד)
        public string ReceiptImageSource { get; set; } = string.Empty;

        // מאפיין מחושב: מתי פג תוקף האחריות
        public DateTime ExpiryDate => PurchaseDate.AddMonths(WarrantyMonths);

        // מאפיין מחושב: כמה ימים נשארו לאחריות
        public int DaysRemaining => (ExpiryDate - DateTime.Today).Days;

        // מאפיין מחושב: האם האחריות עומדת להסתיים (פחות מ-30 יום) או כבר פגה
        public bool IsExpiringSoonOrExpired => DaysRemaining <= 30;

        // מאפיין מחושב שמחזיר צבע מתאים ל-UI
        public string StatusColor => DaysRemaining < 0 ? "#FF3B30" : (DaysRemaining <= 30 ? "#FF9500" : "#2ECC71");
    }
}