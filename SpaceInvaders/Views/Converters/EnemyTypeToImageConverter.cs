using Microsoft.UI.Xaml.Data;
using SpaceInvaders.Models.GameObjects;
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
                    EnemyType.Small => "ms-appx:///Assets/Images/invader1.gif",
                    EnemyType.Medium => "ms-appx:///Assets/Images/invader2.gif",
                    EnemyType.Large => "ms-appx:///Assets/Images/invader3.gif",
                    _ => "ms-appx:///Assets/Images/invader1.gif"
                };
            }
            return "ms-appx:///Assets/Images/invader1.gif";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
