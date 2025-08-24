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
            
            // Inicializar o gerenciador do jogo
            _gameManager = new GameManager();
        }

        // Método para iniciar o jogo
        public void StartGame()
        {
            _gameManager.InitializeGame();
            App.NavigationService.NavigateToGame();
        }

        // Método para parar o jogo
        public void StopGame()
        {
            _gameManager.StopGame();
        }

        // Método para pausar o jogo
        public void PauseGame()
        {
            _gameManager.PauseGame();
        }
    }
}
