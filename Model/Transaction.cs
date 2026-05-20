using System;

namespace MyApp_SmartBills.Model
{
    // הגדרת סוג התנועה: הכנסה או הוצאה
    public enum TransactionType
    {
        Expense, // הוצאה
        Income   // הכנסה
    }

    // הגדרת קטגוריות נפוצות לניהול פיננסי
    public enum TransactionCategory
    {
        Food,          // מזון וסופר
        Electricity,   // חשמל וחשבונות
        Entertainment, // בילויים ופנאי
        Transport,     // תחבורה ודלק
        Shopping,      // קניות
        Salary,         // משכורת
        Rent,           //שכר דירה 
        Other          // אחר
    }

    public class Transaction
    {
        // מזהה ייחודי של הפעולה (אותחול בטקסט ריק string.Empty כדי למנוע אזהרות Null)
        public string? Id { get; set; } = string.Empty;

        // מזהה המשתמש שביצע את הפעולה (כדי שכל משתמש יראה רק את המידע שלו)
        public string? UserId { get; set; } = string.Empty;

        // סכום הפעולה
        public double Amount { get; set; }

        // תאריך ביצוע הפעולה
        public DateTime Date { get; set; }

        // סוג הפעולה (הוצאה או הכנסה)
        public TransactionType Type { get; set; }

        // קטגוריית הפעולה
        public TransactionCategory Category { get; set; }

        // האם מדובר בהוצאה עסקית? (true = עסקי, false = פרטי)
        public bool IsBusiness { get; set; }

        // קישור לתמונת הקבלה ב-Firebase Storage (סימן השאלה אומר שהשדה יכול להיות ריק/Null אם אין תמונה)
        public string? ReceiptImageUrl { get; set; }

        // הערות נוספות שהמשתמש יכול להקליד (גם כאן, יכול להיות Null אם המשתמש לא כתב כלום)
        public string? Notes { get; set; }

        // קונסטרקטור (בנאי) ריק - חובה עבור העבודה מול Firebase
        public Transaction()
        {
            Date = DateTime.Now; // ברירת מחדל לתאריך והשעה של עכשיו
            Type = TransactionType.Expense;
            Category = TransactionCategory.Other;
        }
    }
}