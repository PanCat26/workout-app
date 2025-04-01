using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutApp.Converters
{
    internal class ColorToIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return false;
            return value.ToString() == parameter.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return null;
            return (bool)value ? parameter : null;
        }
    }

    public class SizeToIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null && value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isChecked && isChecked)
                return parameter?.ToString(); // Ensure parameter is not null before converting

            return null;
        }
    }
}
