using SpaceInvaders.Services;
using SpaceInvaders.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SpaceInvaders.Views;

namespace SpaceInvaders
{
    public sealed partial class App : Application
    {
        public static MainViewModel MainViewModel { get; private set; } = null!;
        public static GameViewModel GameViewModel { get; private set; } = null!;
        public static HighScoresViewModel HighScoresViewModel { get; private set; } = null!;
        public static INavigationService NavigationService { get; private set; } = null!;

        public App()
        {
            this.InitializeComponent(); 
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }

            NavigationService = new NavigationService(rootFrame);

            // Inicializar ViewModels
            var gameService = new GameService();
            var soundService = new SoundService();
            var highScoreService = new HighScoreService();

            MainViewModel = new MainViewModel(gameService, NavigationService);
            GameViewModel = new GameViewModel(gameService, soundService, NavigationService, highScoreService);
            HighScoresViewModel = new HighScoresViewModel(highScoreService, NavigationService);

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage));
            }

            Window.Current.Activate();
        }
    }
}
