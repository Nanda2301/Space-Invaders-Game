using SpaceInvaders.Models.GameObjects;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace SpaceInvaders.Views
{
    public class EnemyTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            if (value is EnemyType enemyType)
            {
                switch (enemyType)
                {
                    case EnemyType.Small:
                        return new BitmapImage(new System.Uri("ms-appx:///Assets/Images/invader1.gif"));
                    case EnemyType.Medium:
                        return new BitmapImage(new System.Uri("ms-appx:///Assets/Images/invader2.gif"));
                    case EnemyType.Large:
                        return new BitmapImage(new System.Uri("ms-appx:///Assets/Images/invader3.gif"));
                }
            }
            return null;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
