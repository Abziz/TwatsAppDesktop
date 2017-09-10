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
    /// <summary>
    /// Converts a boolean type to Visibility type
    /// </summary>
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

    /// <summary>
    /// Converts a DateTimeOffset type to string type
    /// </summary>
    public class LastMessageTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset time)
            {
                if (time.Day.Equals(DateTimeOffset.Now.Day) && time.Month.Equals(DateTimeOffset.Now.Month) && time.Year.Equals(DateTimeOffset.Now.Year))
                {
                    return time.ToString("HH:mm");
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

    /// <summary>
    /// Converts a userDto to chat bubble color
    /// </summary>
    public class FromColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserDto user)
            {
                if (user.Id == Properties.Settings.Default.UserId)
                {
                    return "#DCF8C6";
                }
            }
            return "#FFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a userDto to Allignment direction
    /// </summary>
    public class FromAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserDto user)
            {
                if (user.Id == Properties.Settings.Default.UserId)
                {
                    return HorizontalAlignment.Right;
                }
            }
            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a userDto to integer specifing a column on a grid
    /// </summary>
    public class FromColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserDto user)
            {
                if (user.Id == Properties.Settings.Default.UserId)
                {
                    return 1;
                }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Converts a userDto to integer specifying the direction to flip
    /// </summary>
    public class FromFlipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserDto user)
            {
                if (user.Id == Properties.Settings.Default.UserId)
                {
                    return -1;
                }
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
