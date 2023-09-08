using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace EMV_Parser
{
    public class TrimCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string category)
            {
                var match = Regex.Match(category, @"\((qVSDC|PayPass)\)");
                return match.Success ? match.Groups[1].Value : string.Empty;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
