using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using SpaceInvaders.Models.GameObjects;
using System;
using Windows.UI;
using Microsoft.UI;

namespace SpaceInvaders.Views.Converters
{
    // Este converter já existe mas vou garantir que está correto
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

    // NOVO: Converter para escudos baseado na vida
    public class ShieldHealthToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int health)
            {
                return health switch
                {
                    3 => new SolidColorBrush(Colors.Green),      // Full health
                    2 => new SolidColorBrush(Colors.Yellow),     // Medium health
                    1 => new SolidColorBrush(Colors.Orange),     // Low health
                    0 => new SolidColorBrush(Colors.Transparent), // Destroyed
                    _ => new SolidColorBrush(Colors.Green)
                };
            }
            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    // NOVO: Converter para visibilidade baseado na vida do escudo
    public class ShieldHealthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int health)
            {
                return health > 0 ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
            }
            return Microsoft.UI.Xaml.Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
