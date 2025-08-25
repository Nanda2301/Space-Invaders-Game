using SpaceInvaders.Services;
using SpaceInvaders.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SpaceInvaders.Views;
using System;

namespace SpaceInvaders
{
    public sealed partial class App : Application
    {
        public static MainViewModel MainViewModel { get; private set; }
        public static GameViewModel GameViewModel { get; private set; }
        public static HighScoresViewModel HighScoresViewModel { get; private set; }
        public static INavigationService NavigationService { get; private set; }

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            try
            {
                Frame rootFrame = Window.Current?.Content as Frame;

                if (rootFrame == null)
                {
                    rootFrame = new Frame();
                    Window.Current.Content = rootFrame;
                }

                // Configurar serviços
                NavigationService = new NavigationService(rootFrame);

                // Inicializar serviços
                var soundService = new SoundService();               // Criar primeiro
                var gameService = new GameService(soundService);     // Passar o soundService para o GameService
                var highScoreService = new HighScoreService();

                // Inicializar ViewModels
                MainViewModel = new MainViewModel(gameService, NavigationService);
                GameViewModel = new GameViewModel(gameService, soundService, NavigationService, highScoreService);
                HighScoresViewModel = new HighScoresViewModel(highScoreService, NavigationService);

                // Navegar para a página inicial
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage));
                }

                // Ativar janela
                Window.Current.Activate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during app launch: {ex.Message}");
                
                // Fallback: criar uma janela simples
                try
                {
                    var frame = new Frame();
                    frame.Navigate(typeof(MainPage));
                    Window.Current.Content = frame;
                    Window.Current.Activate();
                }
                catch
                {
                    // Se tudo falhar, pelo menos tente ativar a janela
                    Window.Current?.Activate();
                }
            }
        }
    }
}
