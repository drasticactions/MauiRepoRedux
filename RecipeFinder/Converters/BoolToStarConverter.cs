using System.Globalization;

namespace RecipeFinder.Converters
{
    public class BoolToStarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isFavorite)
            {
                return isFavorite ? "dotnet_bot.png" : "dotnet_bot.png";
            }
            return "dotnet_bot.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}