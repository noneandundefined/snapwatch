using System;
using System.Globalization;
using System.Windows.Media;

namespace snapwatch.UI.Styles.Controllers
{
    public class MoodToColorConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string mood = value as string;
            string part = parameter as string;

            if (mood == null || part == null) return Colors.Gray;

            switch (mood)
            {
                case "Бодрое":
                    return (part == "Start") ? Color.FromRgb(255, 140, 0) : Color.FromRgb(255, 70, 0);
                case "Весёлое":
                    return (part == "Start") ? Color.FromRgb(173, 255, 47) : Color.FromRgb(0, 200, 0);
                case "Спокойное":
                    return (part == "Start") ? Color.FromRgb(0, 191, 255) : Color.FromRgb(0, 128, 255);
                case "Грустное":
                    return (part == "Start") ? Color.FromRgb(123, 104, 238) : Color.FromRgb(72, 61, 139);
                default:
                    return Colors.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
