using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace EMV_Parser
{
    public class RemoveSpacesConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            return ((string)value).Replace(" ", "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
