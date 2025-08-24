using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SpaceInvaders.ViewModels;

namespace SpaceInvaders.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly GameManager _gameManager;
        
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = App.MainViewModel;

            _gameManager = new GameManager();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            App.GameViewModel.StartGame();
            App.NavigationService.NavigateToGame();
        }

        private void HighScoresButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateToHighScores();
        }

        private void ControlsButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateToControls();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        // Método para iniciar o jogo
        public void StartGame()
        {
            _gameManager.InitializeGame();
            App.NavigationService.NavigateToGame();
        }

        public void StopGame()
        {
            _gameManager.StopGame();
        }

        public void PauseGame()
        {
            _gameManager.PauseGame();
        }
    }
}
