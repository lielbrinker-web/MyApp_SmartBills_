using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace MyApp_SmartBills.Converters
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string base64String && !string.IsNullOrWhiteSpace(base64String))
            {
                // אם מדובר בטקסט הדיפולטיבי או ריק, נחזיר את תמונת הפלייסהולדר
                if (base64String == "receipt_placeholder.png")
                {
                    return ImageSource.FromFile("receipt_placeholder.png");
                }

                try
                {
                    // המרת מחרוזת הטקסט חזרה למערך בייטים
                    byte[] imageBytes = System.Convert.FromBase64String(base64String);

                    // יצירת מקור תמונה מתוך זרם הזיכרון
                    return ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error converting Base64 to Image: {ex.Message}");
                }
            }

            // ברירת מחדל אם אין תמונה תקנית
            return ImageSource.FromFile("receipt_placeholder.png");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}