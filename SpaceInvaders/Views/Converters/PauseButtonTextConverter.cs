using Microsoft.UI.Xaml.Data;

namespace SpaceInvaders.Views
{
    public class PauseButtonTextConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            if (value is bool isPaused)
            {
                return isPaused ? "Continuar" : "Pausar";
            }
            return "Pausar";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
