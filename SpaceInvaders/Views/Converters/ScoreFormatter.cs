using Microsoft.UI.Xaml.Data;

namespace SpaceInvaders.Views.Converters
{
    public class ScoreFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int score)
            {
                return $"Pontuação Final: {score}";
            }
            return "Pontuação Final: 0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
