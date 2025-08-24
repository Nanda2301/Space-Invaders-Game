using SpaceInvaders.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.Core;

namespace SpaceInvaders.Views
{
    public sealed partial class GamePage : Page
    {
        public GameViewModel ViewModel => (GameViewModel)DataContext;

        public GamePage()
        {
            this.InitializeComponent();
            DataContext = App.GameViewModel;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // ✅ Verificação de segurança
            if (App.NavigationService != null)
            {
                App.NavigationService.NavigateToMainMenu();
            }
            else
            {
                // Fallback: navegação manual
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame?.Navigate(typeof(MainPage));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.StartGame();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Tornar os métodos públicos no ViewModel ou usar uma abordagem diferente
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            ViewModel.ProcessKeyDown(args.VirtualKey);
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            ViewModel.ProcessKeyUp(args.VirtualKey);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;
            ViewModel.Dispose();
        }
    }
}
