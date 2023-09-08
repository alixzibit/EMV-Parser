using System;
using System.Globalization;
using System.Windows.Data;

namespace EMV_Parser
{
    public class TrimPrefixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            if (input == null) return null;

            int index = input.IndexOf('-');
            if (index >= 0 && index < input.Length - 1)
            {
                return input.Substring(index + 1);
            }

            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
