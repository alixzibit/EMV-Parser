using System;
using System.Globalization;
using System.Windows.Data;

public class CategoryConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string category = value.ToString();

        if (category == "C")
        {
            return "Contact";
        }
        else if (category == "CL")
        {
            return "Contactless";
        }
        else
        {
            return "";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
