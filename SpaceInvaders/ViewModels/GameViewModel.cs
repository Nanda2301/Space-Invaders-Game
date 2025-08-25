using SpaceInvaders.Models;
using SpaceInvaders.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SoundEffects = SpaceInvaders.Models.SoundEffects;

namespace SpaceInvaders.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IGameService _gameService;
        private readonly ISoundService _soundService;
        private readonly INavigationService _navigationService;
        private readonly IHighScoreService _highScoreService;

        public GameState GameState => _gameService.GameState;

        // Comandos
        public RelayCommand MainMenuCommand { get; }
        public RelayCommand MoveLeftCommand { get; }
        public RelayCommand MoveRightCommand { get; }
        public RelayCommand ShootCommand { get; }
        public RelayCommand PauseCommand { get; }

        // Propriedades formatadas para binding
        public string FormattedScore => $"SCORE: {GameState.Player.Score:D4}";
        public string FormattedLives => $"LIVES: {GameState.Player.Lives}";
        public string FormattedLevel => $"LEVEL: {GameState.Level}";
        public string FormattedFinalScore => $"FINAL SCORE: {GameState.Player.Score:D4}";

        public event PropertyChangedEventHandler PropertyChanged;

        public GameViewModel(
            IGameService gameService,
            ISoundService soundService,
            INavigationService navigationService,
            IHighScoreService highScoreService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _soundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _highScoreService = highScoreService ?? throw new ArgumentNullException(nameof(highScoreService));

            // Inicializa comandos com CommunityToolkit.Mvvm.Input.RelayCommand
            MainMenuCommand = new RelayCommand(GoToMainMenu);
            MoveLeftCommand = new RelayCommand(MoveLeft, CanMove);
            MoveRightCommand = new RelayCommand(MoveRight, CanMove);
            ShootCommand = new RelayCommand(Shoot, CanShoot);
            PauseCommand = new RelayCommand(TogglePause);
        }

        public void StartGame()
        {
            _gameService.InitializeGame();
            UpdateUI();
        }

        private bool CanMove() => !GameState.IsPaused && !GameState.IsGameOver;
        private bool CanShoot() => !GameState.IsPaused && !GameState.IsGameOver && GameState.Player.CanShoot;

        private void MoveLeft()
        {
            if (CanMove())
            {
                GameState.Player.MoveLeft();
                UpdateUI();
            }
        }

        private void MoveRight()
        {
            if (CanMove())
            {
                GameState.Player.MoveRight(800); // Largura do canvas
                UpdateUI();
            }
        }

        private void Shoot()
        {
            if (CanShoot())
            {
                _soundService.PlaySound(SoundEffects.PlayerShoot);
                GameState.Player.CanShoot = false; // Bloqueia até próxima atualização
                UpdateUI();
            }
        }

        private void TogglePause()
        {
            GameState.IsPaused = !GameState.IsPaused;
            UpdateUI();
        }

        private void GoToMainMenu()
        {
            _navigationService.NavigateToMainMenu();
        }

        public async void SaveHighScore(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName)) return;

            var highScore = new HighScore
            {
                PlayerName = playerName.Trim(),
                Score = GameState.Player.Score,
                Date = DateTime.Now
            };

            try
            {
                await _highScoreService.SaveHighScoreAsync(highScore);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving high score: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateUI()
        {
            OnPropertyChanged(nameof(FormattedScore));
            OnPropertyChanged(nameof(FormattedLives));
            OnPropertyChanged(nameof(FormattedLevel));
            OnPropertyChanged(nameof(FormattedFinalScore));
            OnPropertyChanged(nameof(GameState));
        }

        public void Dispose()
        {
            _soundService?.Dispose();
        }
    }
}
