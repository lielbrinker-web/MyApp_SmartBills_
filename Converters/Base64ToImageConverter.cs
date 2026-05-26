using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace MyApp_SmartBills.Converters // ודאי שה-Namespace מתאים לתיקייה שלו
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64String && !string.IsNullOrWhiteSpace(base64String))
            {
                try
                {
                    // אם המחרוזת מכילה כבר את ה-Header של ה-Data URL, ננקה אותו
                    if (base64String.Contains(","))
                    {
                        base64String = base64String.Split(',')[1];
                    }

                    byte[] imageBytes = System.Convert.FromBase64String(base64String);
                    return ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                catch
                {
                    return "user_icon.png"; // תמונת ברירת מחדל במקרה של שגיאה בהמרה
                }
            }
            return "user_icon.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}