using SpaceInvaders.Models.GameObjects;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace SpaceInvaders.Views.Converters
{
    public class EnemyTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is EnemyType enemyType)
            {
                return enemyType switch
                {
                    EnemyType.Small  => new BitmapImage(new Uri("ms-appx:///Assets/Images/invader1.gif")),
                    EnemyType.Medium => new BitmapImage(new Uri("ms-appx:///Assets/Images/invader2.gif")),
                    EnemyType.Large  => new BitmapImage(new Uri("ms-appx:///Assets/Images/invader3.gif")),
                    _ => new BitmapImage(new Uri("ms-appx:///Assets/Images/invader1.gif"))
                };
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
