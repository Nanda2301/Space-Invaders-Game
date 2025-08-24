using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SpaceInvaders.Views
{
    public sealed partial class ControlsPage : Page
    {
        public ControlsPage()
        {
            this.InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService?.NavigateToMainMenu();
        }
    }
}
