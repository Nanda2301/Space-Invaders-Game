using SpaceInvaders.Models;
using SpaceInvaders.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SpaceInvaders.ViewModels
{
    public class HighScoresViewModel : ViewModelBase
    {
        private readonly IHighScoreService _highScoreService;
        private readonly INavigationService _navigationService;

        public ObservableCollection<HighScore> HighScores { get; set; } = new ObservableCollection<HighScore>();
        public ICommand MainMenuCommand { get; }

        public HighScoresViewModel(IHighScoreService highScoreService, INavigationService navigationService)
        {
            _highScoreService = highScoreService;
            _navigationService = navigationService;

            MainMenuCommand = new RelayCommand(GoToMainMenu);
            LoadHighScores();
        }

        private async void LoadHighScores()
        {
            var scores = await _highScoreService.GetHighScoresAsync();
            HighScores.Clear();
            
            foreach (var score in scores.Take(10))
            {
                HighScores.Add(score);
            }
            
            OnPropertyChanged(nameof(HighScores));
        }

        private void GoToMainMenu()
        {
            _navigationService.NavigateToMainMenu();
        }

        public async void SaveHighScore(string playerName, int score)
        {
            var highScore = new HighScore
            {
                PlayerName = playerName,
                Score = score,
                Date = DateTime.Now
            };

            await _highScoreService.SaveHighScoreAsync(highScore);
            LoadHighScores();
        }
    }
}
