using Microsoft.UI.Xaml.Data;
using System;

namespace SpaceInvaders.Views.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return string.Empty;
            
            if (parameter is string format)
            {
                return string.Format(format, value);
            }
            
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
