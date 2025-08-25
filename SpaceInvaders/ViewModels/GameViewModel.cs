using Microsoft.UI.Xaml.Input;
using SpaceInvaders.Models;
using SpaceInvaders.Models.GameObjects;
using SpaceInvaders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using Windows.Foundation;
using Windows.System;
using Microsoft.UI.Xaml;
using SoundEffects = SpaceInvaders.Models.SoundEffects;
using Timer = System.Timers.Timer;

namespace SpaceInvaders.ViewModels
{
    public class GameViewModel : ViewModelBase, IDisposable
    {
        private readonly IGameService _gameService;
        private readonly ISoundService _soundService;
        private readonly INavigationService _navigationService;
        private readonly IHighScoreService _highScoreService;

        private Timer _gameTimer;
        private Timer _enemyShootTimer;
        private Timer _redEnemyTimer;
        private DispatcherTimer _keyboardTimer;

        private bool _isMovingLeft;
        private bool _isMovingRight;
        private bool _isShooting;

        private DateTime _lastExtraLifeScore;

        public GameState GameState => _gameService.GameState;

        private bool _isStartOverlayVisible = true;
        public bool IsStartOverlayVisible
        {
            get => _isStartOverlayVisible;
            set { _isStartOverlayVisible = value; OnPropertyChanged(); }
        }

        // Comandos MVVM
        public ICommand MainMenuCommand { get; }
        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }
        public ICommand ShootCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand StartGameCommand { get; }

        // Bindings para XAML
        public string FormattedScore => $"Score: {GameState.Player.Score}";
        public string FormattedLives => $"Lives: {GameState.Player.Lives}";
        public string FormattedLevel => $"Level: {GameState.Level}";
        public string FormattedFinalScore => $"Final Score: {GameState.Player.Score}";

        public GameViewModel(GameService gameService, ISoundService soundService,
            INavigationService navigationService, IHighScoreService highScoreService)
        {
            _gameService = gameService;
            _soundService = soundService;
            _navigationService = navigationService;
            _highScoreService = highScoreService;
            _lastExtraLifeScore = DateTime.MinValue;

            // Inicialização comandos
            MainMenuCommand = new RelayCommand(GoToMainMenu);
            MoveLeftCommand = new RelayCommand(MoveLeft);
            MoveRightCommand = new RelayCommand(MoveRight);
            ShootCommand = new RelayCommand(Shoot);
            PauseCommand = new RelayCommand(TogglePause);
            RestartCommand = new RelayCommand(StartGame);
            StartGameCommand = new RelayCommand(StartGame);

            InitializeTimers();
        }

        private void InitializeTimers()
        {
            _gameTimer = new Timer(16.67);
            _gameTimer.Elapsed += (s, e) => UpdateGame();
            _gameTimer.AutoReset = true;

            _enemyShootTimer = new Timer(1500);
            _enemyShootTimer.Elapsed += (s, e) => EnemyShoot();
            _enemyShootTimer.AutoReset = true;

            _redEnemyTimer = new Timer(60000);
            _redEnemyTimer.Elapsed += (s, e) => SpawnRedEnemy();
            _redEnemyTimer.AutoReset = true;

            _keyboardTimer = new DispatcherTimer();
            _keyboardTimer.Interval = TimeSpan.FromMilliseconds(16);
            _keyboardTimer.Tick += (s, e) => ProcessInput();
        }

        #region INPUT HANDLING
        public void ProcessKeyDown(VirtualKey key)
        {
            if (key == VirtualKey.Escape)
            {
                GoToMainMenu();
                return;
            }

            if (GameState.IsPaused || GameState.IsGameOver) return;

            switch (key)
            {
                case VirtualKey.Left: _isMovingLeft = true; break;
                case VirtualKey.Right: _isMovingRight = true; break;
                case VirtualKey.Space: _isShooting = true; break;
                case VirtualKey.P: TogglePause(); break;
            }
        }

        public void ProcessKeyUp(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Left: _isMovingLeft = false; break;
                case VirtualKey.Right: _isMovingRight = false; break;
                case VirtualKey.Space: _isShooting = false; break;
            }
        }

        private void ProcessInput()
        {
            if (GameState.IsPaused || GameState.IsGameOver) return;

            if (_isMovingLeft) MoveLeft();
            if (_isMovingRight) MoveRight();
            if (_isShooting && GameState.Player.CanShoot) Shoot();
        }
        #endregion

        #region PLAYER ACTIONS
        private void MoveLeft()
        {
            GameState.Player.MoveLeft();
            UpdateUI();
        }

        private void MoveRight()
        {
            GameState.Player.MoveRight(800);
            UpdateUI();
        }

        private void Shoot()
        {
            var bullet = new Bullet(
                GameState.Player.Bounds.Left + GameState.Player.Bounds.Width / 2 - 1.5,
                GameState.Player.Bounds.Top - 10,
                BulletType.Player
            );

            GameState.Bullets.Add(bullet);
            GameState.Player.CanShoot = false;
            _soundService.PlaySound(SoundEffects.PlayerShoot);
            UpdateUI();
        }
        #endregion

        #region ENEMY LOGIC
        private void EnemyShoot()
        {
            if (GameState.IsPaused || GameState.IsGameOver || GameState.Enemies.Count == 0) return;

            var random = new Random();
            var shooters = GameState.Enemies.Where(e => e.CanShoot).ToList();
            if (shooters.Any())
            {
                var shooter = shooters[random.Next(shooters.Count)];
                var bullet = new Bullet(
                    shooter.Bounds.Left + shooter.Bounds.Width / 2 - 1.5,
                    shooter.Bounds.Bottom,
                    BulletType.Enemy
                );
                GameState.Bullets.Add(bullet);
                _soundService.PlaySound(SoundEffects.EnemyShoot);
            }
        }

        private void SpawnRedEnemy()
        {
            if (GameState.IsPaused || GameState.IsGameOver || GameState.RedEnemy != null) return;

            GameState.RedEnemy = new RedEnemy();
            _soundService.PlaySound(SoundEffects.RedEnemyAppear);
        }

        private void UpdateEnemies()
        {
            if (GameState.Enemies.Count == 0) return;

            double moveAmount = GameState.EnemiesMovingRight ? GameState.EnemySpeedModifier : -GameState.EnemySpeedModifier;
            foreach (var enemy in GameState.Enemies)
                enemy.Move(moveAmount, GameState.EnemyDescentAmount);

            double leftmost = GameState.Enemies.Min(e => e.Bounds.Left);
            double rightmost = GameState.Enemies.Max(e => e.Bounds.Right);

            if ((GameState.EnemiesMovingRight && rightmost >= 780) || (!GameState.EnemiesMovingRight && leftmost <= 20))
            {
                GameState.EnemiesMovingRight = !GameState.EnemiesMovingRight;
                GameState.EnemyDescentAmount = 20;
            }
            else GameState.EnemyDescentAmount = 0;

            GameState.EnemySpeedModifier = 1.0 + (50 - GameState.Enemies.Count) * 0.03;
        }
        #endregion

        #region COLLISION DETECTION
        private void CheckCollisions(Bullet bullet)
        {
            if (bullet.Type == BulletType.Player) CheckPlayerBulletCollisions(bullet);
            else CheckEnemyBulletCollisions(bullet);
        }

        private void CheckPlayerBulletCollisions(Bullet bullet)
        {
            for (int i = GameState.Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = GameState.Enemies[i];
                if (bullet.CheckCollision(enemy))
                {
                    GameState.Player.AddScore(enemy.PointValue);
                    GameState.Enemies.RemoveAt(i);
                    GameState.Bullets.Remove(bullet);
                    GameState.Player.CanShoot = true;
                    _soundService.PlaySound(SoundEffects.Explosion);
                    CheckExtraLife();
                    UpdateUI();
                    return;
                }
            }

            if (GameState.RedEnemy != null && bullet.CheckCollision(GameState.RedEnemy))
            {
                GameState.Player.AddScore(GameState.RedEnemy.PointValue);
                GameState.RedEnemy = null;
                GameState.Bullets.Remove(bullet);
                GameState.Player.CanShoot = true;
                _soundService.PlaySound(SoundEffects.RedEnemyKilled);
                CheckExtraLife();
                UpdateUI();
                return;
            }

            foreach (var shield in GameState.Shields.Where(s => s.IsActive))
            {
                if (bullet.CheckCollision(shield))
                {
                    shield.TakeDamage();
                    GameState.Bullets.Remove(bullet);
                    GameState.Player.CanShoot = true;
                    UpdateUI();
                    return;
                }
            }
        }

        private void CheckEnemyBulletCollisions(Bullet bullet)
        {
            if (bullet.CheckCollision(GameState.Player))
            {
                GameState.Player.LoseLife();
                GameState.Bullets.Remove(bullet);
                _soundService.PlaySound(SoundEffects.Explosion);

                if (GameState.Player.Lives <= 0) GameOver();

                UpdateUI();
                return;
            }

            foreach (var shield in GameState.Shields.Where(s => s.IsActive))
            {
                if (bullet.CheckCollision(shield))
                {
                    shield.TakeDamage();
                    GameState.Bullets.Remove(bullet);
                    UpdateUI();
                    return;
                }
            }
        }

        private void CheckExtraLife()
        {
            int currentThousands = GameState.Player.Score / 1000;
            int lastThousands = (GameState.Player.Score - 100) / 1000;

            if (currentThousands > lastThousands && GameState.Player.Lives < 6)
            {
                GameState.Player.GainLife();
                _soundService.PlaySound(SoundEffects.ExtraLife);
                UpdateUI();
            }
        }
        #endregion

        #region GAME LOOP
        private void UpdateGame()
        {
            if (GameState.IsPaused || GameState.IsGameOver) return;

            for (int i = GameState.Bullets.Count - 1; i >= 0; i--)
            {
                var bullet = GameState.Bullets[i];
                bullet.Update();

                if (bullet.Bounds.Top < 0 || bullet.Bounds.Bottom > 600)
                {
                    GameState.Bullets.RemoveAt(i);
                    if (bullet.Type == BulletType.Player) GameState.Player.CanShoot = true;
                    continue;
                }

                CheckCollisions(bullet);
            }

            UpdateEnemies();

            if (GameState.RedEnemy != null)
            {
                GameState.RedEnemy.Update();
                if ((GameState.RedEnemy.MovingRight && GameState.RedEnemy.Bounds.Left > 800) ||
                    (!GameState.RedEnemy.MovingRight && GameState.RedEnemy.Bounds.Right < 0))
                {
                    GameState.RedEnemy = null;
                }
            }

            CheckGameEnd();
            UpdateUI();
        }

        private void CheckGameEnd()
        {
            if (GameState.Enemies.Count > 0)
            {
                double bottommost = GameState.Enemies.Max(e => e.Bounds.Bottom);
                if (bottommost >= GameState.Player.Bounds.Top - 20) GameOver();
            }

            if (GameState.Enemies.Count == 0) NextLevel();
        }

        private void GameOver()
        {
            GameState.IsGameOver = true;
            StopTimers();
            _soundService.PlaySound(SoundEffects.GameOver);
            UpdateUI();
        }

        private void NextLevel()
        {
            GameState.Level++;
            GameState.EnemySpeedModifier += 0.3;

            GameState.Enemies.Clear();
            for (int row = 0; row < 5; row++)
            {
                EnemyType type = row == 0 ? EnemyType.Small :
                    row < 3 ? EnemyType.Medium :
                    EnemyType.Large;

                for (int col = 0; col < 10; col++)
                    GameState.Enemies.Add(new Enemy(type, 100 + col * 50, 50 + row * 40));
            }

            GameState.EnemiesMovingRight = true;
            GameState.EnemyDescentAmount = 0;
            GameState.Bullets.Clear();
            GameState.Player.CanShoot = true;
            GameState.RedEnemy = null;

            UpdateUI();
        }
        #endregion

        #region GAME CONTROL
        private void TogglePause()
        {
            GameState.IsPaused = !GameState.IsPaused;

            if (GameState.IsPaused) PauseTimers();
            else ResumeTimers();

            UpdateUI();
        }

        private void GoToMainMenu()
        {
            StopTimers();
            _navigationService.NavigateToMainMenu();
        }

        public void StartGame()
        {
            GameState.InitializeGame();
            StartTimers();
            IsStartOverlayVisible = false;
            UpdateUI();
        }

        private void StartTimers()
        {
            _gameTimer.Start();
            _enemyShootTimer.Start();
            _redEnemyTimer.Start();
            _keyboardTimer.Start();
        }

        private void PauseTimers()
        {
            _gameTimer.Stop();
            _enemyShootTimer.Stop();
            _redEnemyTimer.Stop();
            _keyboardTimer.Stop();
        }

        private void ResumeTimers()
        {
            _gameTimer.Start();
            _enemyShootTimer.Start();
            _redEnemyTimer.Start();
            _keyboardTimer.Start();
        }

        private void StopTimers()
        {
            _gameTimer?.Stop();
            _enemyShootTimer?.Stop();
            _redEnemyTimer?.Stop();
            _keyboardTimer?.Stop();
        }

        public void Dispose()
        {
            StopTimers();
            _gameTimer?.Dispose();
            _enemyShootTimer?.Dispose();
            _redEnemyTimer?.Dispose();
            _keyboardTimer = null;
        }
        #endregion

        private void UpdateUI()
        {
            OnPropertyChanged(nameof(GameState));
            OnPropertyChanged(nameof(FormattedScore));
            OnPropertyChanged(nameof(FormattedLives));
            OnPropertyChanged(nameof(FormattedLevel));
            OnPropertyChanged(nameof(FormattedFinalScore));
        }

        public async void SaveHighScore(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName)) return;

            try
            {
                var highScore = new HighScore
                {
                    PlayerName = playerName.Trim(),
                    Score = GameState.Player.Score,
                    Date = DateTime.Now
                };

                await _highScoreService.SaveHighScoreAsync(highScore);
            }
            catch { }
        }
    }
}
