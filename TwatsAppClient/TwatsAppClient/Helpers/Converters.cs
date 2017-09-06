using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TwatsAppClient.Helpers
{
   public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean && (bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }

    public class LastMessageTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset)
            {
                var time = (DateTimeOffset)value;
                if(time.Day.Equals(DateTimeOffset.Now.Day) && time.Month.Equals(DateTimeOffset.Now.Month) && time.Year.Equals(DateTimeOffset.Now.Year)) {
                    return time.ToString("hh:mm");
                }
                else
                {
                    return time.ToString("d");
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string) 
            {
                return DateTimeOffset.Parse((string)value);
            }
            return null;
        }
    }

}
