using SpaceInvaders.Services;
using System.Windows.Input;

namespace SpaceInvaders.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IGameService _gameService;
        private readonly INavigationService _navigationService;
        private readonly GameManager _gameManager;

        public ICommand StartNewGameCommand { get; }
        public ICommand ShowHighScoresCommand { get; }
        public ICommand ShowControlsCommand { get; }
        public ICommand ExitCommand { get; }
        

        public MainViewModel(IGameService gameService, INavigationService navigationService)
        {
            _gameService = gameService;
            _navigationService = navigationService;
            _gameManager = new GameManager();

            StartNewGameCommand = new RelayCommand(StartNewGame);
            ShowHighScoresCommand = new RelayCommand(ShowHighScores);
            ShowControlsCommand = new RelayCommand(ShowControls);
            ExitCommand = new RelayCommand(Exit);
        }

        private void StartNewGame()
        {
            _gameManager.InitializeGame();
            _navigationService.NavigateToGame();
        }

        private void ShowHighScores()
        {
            _navigationService.NavigateToHighScores();
        }

        private void ShowControls()
        {
            _navigationService.NavigateToControls();
        }

        private void Exit()
        {
            _gameManager.StopGame();
            // Fechar aplicação
        }

        // Propriedades para binding
        public bool CanStartGame => true;
    }
}
