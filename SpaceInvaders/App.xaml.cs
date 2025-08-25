using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SpaceInvaders.Services;
using SpaceInvaders.ViewModels;
using SpaceInvaders.Views;

namespace SpaceInvaders
{
    public partial class App : Application
    {
        private Window m_window;

        public static MainViewModel MainViewModel { get; private set; }
        public static GameViewModel GameViewModel { get; private set; }
        public static HighScoresViewModel HighScoresViewModel { get; private set; }
        public static NavigationService NavigationService { get; private set; }

        private static IGameService _gameService;
        private static ISoundService _soundService;
        private static IHighScoreService _highScoreService;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new Window();

            var rootFrame = new Frame();
            m_window.Content = rootFrame;

            NavigationService = new NavigationService(rootFrame);

            // Inicializa os serviços
            _soundService = new SoundService();
            _highScoreService = new HighScoreService();
            _gameService = new GameService(_soundService);

            // Cria os ViewModels
            CreateViewModels();

            rootFrame.Navigate(typeof(MainPage));

            m_window.Activate();
        }

        private static void CreateViewModels()
        {
            MainViewModel = new MainViewModel(_gameService, NavigationService);
            HighScoresViewModel = new HighScoresViewModel(_highScoreService, NavigationService);
            RecreateGameViewModel();
        }

        public static void RecreateGameViewModel()
        {
            GameViewModel?.Dispose();
            GameViewModel = new GameViewModel(
                (GameService)_gameService,
                _soundService,
                NavigationService,
                _highScoreService
            );
        }
    }
}
