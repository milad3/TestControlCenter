using System;
using System.Globalization;
using System.Windows.Data;
using TestControlCenter.Tools;

namespace TestControlCenter.Converter
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime date)
            {
                return GlobalTools.GetPersianDate(date);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string persianDate)
            {
                return GlobalTools.GetDate(persianDate);
            }

            return default(DateTime);
        }
    }
}
