using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using SpaceInvaders.Models.GameObjects;
using System;
using Windows.UI;
using Microsoft.UI;

namespace SpaceInvaders.Views.Converters
{
    public class BulletTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is BulletType bulletType)
            {
                return bulletType switch
                {
                    BulletType.Player => new SolidColorBrush(Colors.White),
                    BulletType.Enemy => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.White)
                };
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
