using SpaceInvaders.Models;
using SpaceInvaders.Models.GameObjects;
using SpaceInvaders.Services;
using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Input;
using Windows.Foundation;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Timer = System.Timers.Timer;
using System.Linq;

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

        public GameState GameState => _gameService.GameState;
        public ICommand PauseCommand { get; }
        public ICommand MainMenuCommand { get; }

        // 🔹 Propriedades formatadas para binding no XAML
        public string FormattedScore => $"Pontuação: {GameState.Player.Score}";
        public string FormattedLives => $"Vidas: {GameState.Player.Lives}";
        public string FormattedLevel => $"Nível: {GameState.Level}";
        public string FormattedFinalScore => $"Pontuação Final: {GameState.Player.Score}";

        public GameViewModel(IGameService gameService, ISoundService soundService, 
                           INavigationService navigationService, IHighScoreService highScoreService)
        {
            _gameService = gameService;
            _soundService = soundService;
            _navigationService = navigationService;
            _highScoreService = highScoreService;

            PauseCommand = new RelayCommand(TogglePause);
            MainMenuCommand = new RelayCommand(GoToMainMenu);

            InitializeTimers();
        }
        

        private void InitializeTimers()
        {
            _gameTimer = new Timer(16.67); // ~60 FPS
            _gameTimer.Elapsed += (s, e) => UpdateGame();
            _gameTimer.AutoReset = true;

            _enemyShootTimer = new Timer(1000);
            _enemyShootTimer.Elapsed += (s, e) => EnemyShoot();
            _enemyShootTimer.AutoReset = true;

            _redEnemyTimer = new Timer(20000);
            _redEnemyTimer.Elapsed += (s, e) => SpawnRedEnemy();
            _redEnemyTimer.AutoReset = true;

            _keyboardTimer = new DispatcherTimer();
            _keyboardTimer.Interval = TimeSpan.FromMilliseconds(16);
            _keyboardTimer.Tick += (s, e) => ProcessInput();
        }
        
        public void ProcessKeyDown(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Left:  _isMovingLeft = true; break;
                case VirtualKey.Right: _isMovingRight = true; break;
                case VirtualKey.Space: _isShooting = true; break;
                case VirtualKey.P:     TogglePause(); break;
            }
        }

        public void ProcessKeyUp(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Left:  _isMovingLeft = false; break;
                case VirtualKey.Right: _isMovingRight = false; break;
                case VirtualKey.Space: _isShooting = false; break;
            }
        }

        private void ProcessInput()
        {
            if (GameState.IsPaused || GameState.IsGameOver) return;

            if (_isMovingLeft)
            {
                GameState.Player.MoveLeft();
                OnPropertyChanged(nameof(GameState));
            }

            if (_isMovingRight)
            {
                GameState.Player.MoveRight(Window.Current.Bounds.Width);
                OnPropertyChanged(nameof(GameState));
            }

            if (_isShooting && GameState.Player.CanShoot)
            {
                Shoot();
            }
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

        private void EnemyShoot()
        {
            if (GameState.IsPaused || GameState.IsGameOver || GameState.Enemies.Count == 0) return;

            var random = new Random();
            var shooters = new List<Enemy>();

            foreach (var column in GameState.Enemies.GroupBy(e => e.Bounds.Left))
            {
                var lowestEnemy = column.OrderByDescending(e => e.Bounds.Top).First();
                shooters.Add(lowestEnemy);
            }

            if (shooters.Count > 0)
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

        private void UpdateGame()
        {
            if (GameState.IsPaused || GameState.IsGameOver) return;

            for (int i = GameState.Bullets.Count - 1; i >= 0; i--)
            {
                var bullet = GameState.Bullets[i];
                bullet.Update();

                if (bullet.Bounds.Top < 0 || bullet.Bounds.Bottom > Window.Current.Bounds.Height)
                {
                    GameState.Bullets.RemoveAt(i);
                    if (bullet.Type == BulletType.Player)
                        GameState.Player.CanShoot = true;
                    continue;
                }

                CheckCollisions(bullet);
            }

            UpdateEnemies();

            if (GameState.RedEnemy != null)
            {
                GameState.RedEnemy.Update();
                
                if ((GameState.RedEnemy.MovingRight && GameState.RedEnemy.Bounds.Left > Window.Current.Bounds.Width) ||
                    (!GameState.RedEnemy.MovingRight && GameState.RedEnemy.Bounds.Right < 0))
                {
                    GameState.RedEnemy = null;
                }
            }

            CheckGameOver();
            OnPropertyChanged(nameof(GameState));
            UpdateUI();
        }

        private void UpdateEnemies()
        {
            if (GameState.Enemies.Count == 0) return;

            double moveAmount = GameState.EnemiesMovingRight ? 
                GameState.EnemySpeedModifier : -GameState.EnemySpeedModifier;

            foreach (var enemy in GameState.Enemies)
            {
                enemy.Bounds = new Rect(
                    enemy.Bounds.Left + moveAmount,
                    enemy.Bounds.Top + GameState.EnemyDescentAmount,
                    enemy.Bounds.Width,
                    enemy.Bounds.Height
                );
            }

            double leftmost = GameState.Enemies.Min(e => e.Bounds.Left);
            double rightmost = GameState.Enemies.Max(e => e.Bounds.Right);
            double bottommost = GameState.Enemies.Max(e => e.Bounds.Bottom);

            if ((GameState.EnemiesMovingRight && rightmost >= Window.Current.Bounds.Width - 20) ||
                (!GameState.EnemiesMovingRight && leftmost <= 20))
            {
                GameState.EnemiesMovingRight = !GameState.EnemiesMovingRight;
                GameState.EnemyDescentAmount = 20;
            }
            else
            {
                GameState.EnemyDescentAmount = 0;
            }

            GameState.EnemySpeedModifier = 1.0 + (55 - GameState.Enemies.Count) * 0.02;
        }

        private void CheckCollisions(Bullet bullet)
        {
            if (bullet.Type == BulletType.Player)
            {
                for (int i = GameState.Enemies.Count - 1; i >= 0; i--)
                {
                    var enemy = GameState.Enemies[i];
                    if (bullet.CheckCollision(enemy))
                    {
                        GameState.Player.Score += enemy.PointValue;
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
                    GameState.Player.Score += GameState.RedEnemy.PointValue;
                    GameState.RedEnemy = null;
                    GameState.Bullets.Remove(bullet);
                    GameState.Player.CanShoot = true;
                    _soundService.PlaySound(SoundEffects.RedEnemyKilled);
                    CheckExtraLife();
                    UpdateUI();
                    return;
                }

                foreach (var shield in GameState.Shields)
                {
                    if (shield.IsActive && bullet.CheckCollision(shield))
                    {
                        shield.TakeDamage();
                        GameState.Bullets.Remove(bullet);
                        GameState.Player.CanShoot = true;
                        return;
                    }
                }
            }
            else
            {
                if (bullet.CheckCollision(GameState.Player))
                {
                    GameState.Player.Lives--;
                    GameState.Bullets.Remove(bullet);
                    _soundService.PlaySound(SoundEffects.Explosion);
                    
                    if (GameState.Player.Lives <= 0)
                    {
                        GameOver();
                    }
                    UpdateUI();
                    return;
                }

                foreach (var shield in GameState.Shields)
                {
                    if (shield.IsActive && bullet.CheckCollision(shield))
                    {
                        shield.TakeDamage();
                        GameState.Bullets.Remove(bullet);
                        return;
                    }
                }
            }
        }

        private void CheckExtraLife()
        {
            if (GameState.Player.Score >= 1000 && GameState.Player.Score % 1000 < 50 && 
                GameState.Player.Lives < 6)
            {
                GameState.Player.Lives++;
                _soundService.PlaySound(SoundEffects.ExtraLife);
                UpdateUI();
            }
        }

        private void CheckGameOver()
        {
            double bottommost = GameState.Enemies.Max(e => e.Bounds.Bottom);
            if (bottommost >= GameState.Player.Bounds.Top - 20)
            {
                GameOver();
                return;
            }

            if (GameState.Enemies.Count == 0)
            {
                NextLevel();
            }
        }

        private void GameOver()
        {
            GameState.IsGameOver = true;
            StopTimers();
            _soundService.PlaySound(SoundEffects.GameOver);
            _navigationService.NavigateToGameOver(GameState.Player.Score);
            UpdateUI();
        }

        private void NextLevel()
        {
            GameState.Level++;
            GameState.EnemySpeedModifier += 0.2;
            GameState.Enemies.Clear();

            for (int row = 0; row < 5; row++)
            {
                EnemyType type = row == 0 ? EnemyType.Small : 
                                row < 3 ? EnemyType.Medium : 
                                EnemyType.Large;
                
                for (int col = 0; col < 10; col++)
                {
                    GameState.Enemies.Add(new Enemy(type, 100 + col * 50, 50 + row * 40));
                }
            }

            GameState.EnemiesMovingRight = true;
            GameState.EnemyDescentAmount = 0;
            GameState.Bullets.Clear();
            GameState.Player.CanShoot = true;
            GameState.RedEnemy = null;

            UpdateUI();
        }

        private void TogglePause()
        {
            GameState.IsPaused = !GameState.IsPaused;
            
            if (GameState.IsPaused)
                StopTimers();
            else
                StartTimers();
            
            OnPropertyChanged(nameof(GameState));
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
            OnPropertyChanged(nameof(GameState));
            UpdateUI();
        }

        private void StartTimers()
        {
            _gameTimer.Start();
            _enemyShootTimer.Start();
            _redEnemyTimer.Start();
            _keyboardTimer.Start();
        }

        private void StopTimers()
        {
            _gameTimer.Stop();
            _enemyShootTimer.Stop();
            _redEnemyTimer.Stop();
            _keyboardTimer.Stop();
        }

        public void Dispose()
        {
            StopTimers();
            _gameTimer.Dispose();
            _enemyShootTimer.Dispose();
            _redEnemyTimer.Dispose();
        }

        // 🔹 Método central para atualizar UI Bindings
        private void UpdateUI()
        {
            OnPropertyChanged(nameof(FormattedScore));
            OnPropertyChanged(nameof(FormattedLives));
            OnPropertyChanged(nameof(FormattedLevel));
            OnPropertyChanged(nameof(FormattedFinalScore));
        }
    }
}
