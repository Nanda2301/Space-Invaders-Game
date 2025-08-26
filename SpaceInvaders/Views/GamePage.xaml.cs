using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using SpaceInvaders.Models;
using SpaceInvaders.Models.GameObjects;
using SpaceInvaders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using SoundEffects = SpaceInvaders.Models.SoundEffects;

namespace SpaceInvaders.Views
{
    public sealed partial class GamePage : Page
    {
        private readonly GameService _gameService;
        private readonly SoundService _soundService;
        private readonly DispatcherTimer _gameTimer;
        private readonly DispatcherTimer _enemyMoveTimer;
        private readonly DispatcherTimer _enemyShootTimer;
        private readonly DispatcherTimer _redEnemyTimer;

        private readonly Dictionary<VirtualKey, bool> _pressedKeys = new();
        private DateTime _lastShotTime = DateTime.MinValue;
        private readonly TimeSpan _shotCooldown = TimeSpan.FromMilliseconds(300);
        private DateTime _gameStartTime;
        private bool _canShoot = true;

        // Elementos visuais
        private readonly List<FrameworkElement> _enemyElements = new();
        private readonly List<FrameworkElement> _bulletElements = new();
        private readonly List<Rectangle> _shieldElements = new();
        private Image PlayerImage;
        private Image _redEnemyImage;

        public GamePage()
        {
            this.InitializeComponent();

            _soundService = new SoundService();
            _gameService = new GameService(_soundService);

            // Configurar página para receber foco de teclado
            this.IsTabStop = true;
            this.Focus(FocusState.Programmatic);

            // Configurar timers
            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; // 60 FPS
            _gameTimer.Tick += GameTimer_Tick;

            _enemyMoveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _enemyMoveTimer.Tick += EnemyMoveTimer_Tick;

            _enemyShootTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(2000) };
            _enemyShootTimer.Tick += EnemyShootTimer_Tick;

            _redEnemyTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(60) }; // 60 segundos
            _redEnemyTimer.Tick += RedEnemyTimer_Tick;

            InitializeGame();
        }

        private void InitializeGame()
        {
            _gameService.InitializeGame();
            _gameStartTime = DateTime.Now;
            CreateVisualElements();
            UpdateUI();
            StartGame();
        }

        private void CreateVisualElements()
        {
            CreateEnemies();
            CreateShields();
            CreatePlayerVisual();
        }

        private void CreatePlayerVisual()
        {
            PlayerImage = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/player.png")),
                Width = 50,
                Height = 30
            };

            Canvas.SetLeft(PlayerImage, _gameService.GameState.Player.Bounds.Left);
            Canvas.SetTop(PlayerImage, _gameService.GameState.Player.Bounds.Top);
            GameCanvas.Children.Add(PlayerImage);
        }

        private void CreateEnemies()
        {
            EnemiesCanvas.Children.Clear();
            _enemyElements.Clear();

            var enemies = _gameService.GameState.Enemies.ToList();
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                var enemyImage = new Image
                {
                    Source = new BitmapImage(new Uri(GetEnemyImagePath(enemy.Type))),
                    Width = enemy.Bounds.Width,
                    Height = enemy.Bounds.Height
                };

                Canvas.SetLeft(enemyImage, enemy.Bounds.Left);
                Canvas.SetTop(enemyImage, enemy.Bounds.Top);

                EnemiesCanvas.Children.Add(enemyImage);
                _enemyElements.Add(enemyImage);
            }
        }

        private string GetEnemyImagePath(EnemyType type)
        {
            return type switch
            {
                EnemyType.Small => "ms-appx:///Assets/Images/invader1.gif",
                EnemyType.Medium => "ms-appx:///Assets/Images/invader2.gif",
                EnemyType.Large => "ms-appx:///Assets/Images/invader3.gif",
                _ => "ms-appx:///Assets/Images/invader1.gif"
            };
        }

        private void CreateShields()
        {
            ShieldsCanvas.Children.Clear();
            _shieldElements.Clear();

            var shields = _gameService.GameState.Shields.ToList();
            for (int i = 0; i < shields.Count; i++)
            {
                var shield = shields[i];
                var shieldRect = new Rectangle
                {
                    Width = shield.Bounds.Width,
                    Height = shield.Bounds.Height,
                    Fill = GetShieldBrush(shield.Health),
                    Stroke = new SolidColorBrush(Microsoft.UI.Colors.Green),
                    StrokeThickness = 1
                };

                Canvas.SetLeft(shieldRect, shield.Bounds.Left);
                Canvas.SetTop(shieldRect, shield.Bounds.Top);

                ShieldsCanvas.Children.Add(shieldRect);
                _shieldElements.Add(shieldRect);
            }
        }

        private Brush GetShieldBrush(int health)
        {
            return health switch
            {
                3 => new SolidColorBrush(Microsoft.UI.Colors.Green),
                2 => new SolidColorBrush(Microsoft.UI.Colors.YellowGreen),
                1 => new SolidColorBrush(Microsoft.UI.Colors.Orange),
                _ => new SolidColorBrush(Microsoft.UI.Colors.Transparent)
            };
        }

        private void StartGame()
        {
            _gameTimer.Start();
            _enemyMoveTimer.Start();
            _enemyShootTimer.Start();
            _redEnemyTimer.Start();

            this.Focus(FocusState.Programmatic);
        }

        private void StopGame()
        {
            _gameTimer.Stop();
            _enemyMoveTimer.Stop();
            _enemyShootTimer.Stop();
            _redEnemyTimer.Stop();
        }

        private void GameTimer_Tick(object sender, object e)
        {
            if (_gameService.GameState.IsPaused || _gameService.GameState.IsGameOver)
                return;

            ProcessInput();
            UpdateBullets();
            CheckCollisions();
            UpdateRedEnemy();
            UpdateUI();
            CheckGameEnd();
        }

        private void ProcessInput()
        {
            var player = _gameService.GameState.Player;

            if (_pressedKeys.ContainsKey(VirtualKey.Left) && _pressedKeys[VirtualKey.Left])
            {
                player.MoveLeft();
                UpdatePlayerPosition();
            }

            if (_pressedKeys.ContainsKey(VirtualKey.Right) && _pressedKeys[VirtualKey.Right])
            {
                player.MoveRight(800);
                UpdatePlayerPosition();
            }
        }

        private void UpdatePlayerPosition()
        {
            var player = _gameService.GameState.Player;

            if (PlayerImage != null)
            {
                Canvas.SetLeft(PlayerImage, player.Bounds.Left);
                Canvas.SetTop(PlayerImage, player.Bounds.Top);
            }
        }

        private void TryShoot()
        {
            if (!_canShoot || DateTime.Now - _lastShotTime < _shotCooldown)
                return;

            if (_gameService.GameState.Bullets.Any(b => b.Type == BulletType.Player))
                return;

            var player = _gameService.GameState.Player;
            var bullet = new Bullet(
                player.Bounds.Left + player.Bounds.Width / 2 - 1.5,
                player.Bounds.Top - 10,
                BulletType.Player
            );

            _gameService.GameState.Bullets.Add(bullet);
            _soundService.PlaySound(SoundEffects.PlayerShoot);
            _lastShotTime = DateTime.Now;

            CreateBulletVisual(bullet);
        }

        private void CreateBulletVisual(Bullet bullet)
        {
            var bulletRect = new Rectangle
            {
                Width = bullet.Bounds.Width,
                Height = bullet.Bounds.Height,
                Fill = bullet.Type == BulletType.Player ?
                    new SolidColorBrush(Microsoft.UI.Colors.White) :
                    new SolidColorBrush(Microsoft.UI.Colors.Red)
            };

            Canvas.SetLeft(bulletRect, bullet.Bounds.Left);
            Canvas.SetTop(bulletRect, bullet.Bounds.Top);

            BulletsCanvas.Children.Add(bulletRect);
            _bulletElements.Add(bulletRect);
        }

        private void UpdateBullets()
        {
            var bullets = _gameService.GameState.Bullets.ToList();

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bullet.Update();

                if (bullet.Bounds.Top < 0 || bullet.Bounds.Bottom > 600)
                {
                    _gameService.GameState.Bullets.Remove(bullet);
                    RemoveBulletVisual(i);
                    continue;
                }

                if (i < _bulletElements.Count)
                {
                    var bulletElement = _bulletElements[i];
                    Canvas.SetLeft(bulletElement, bullet.Bounds.Left);
                    Canvas.SetTop(bulletElement, bullet.Bounds.Top);
                }
            }
        }

        private void RemoveBulletVisual(int index)
        {
            if (index >= 0 && index < _bulletElements.Count)
            {
                BulletsCanvas.Children.Remove(_bulletElements[index]);
                _bulletElements.RemoveAt(index);
            }
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGame();
            this.Focus(FocusState.Programmatic);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            StopGame();
        }

        private void EnemyMoveTimer_Tick(object sender, object e)
        {
            if (_gameService.GameState.IsPaused || _gameService.GameState.IsGameOver)
                return;

            MoveEnemies();
        }

        private void MoveEnemies()
        {
            var gameState = _gameService.GameState;
            var enemies = gameState.Enemies.ToList();

            if (!enemies.Any()) return;

            bool changeDirection = false;
            double moveDistance = 10 * gameState.EnemySpeedModifier;

            if (gameState.EnemiesMovingRight)
            {
                double rightmost = enemies.Max(e => e.Bounds.Right);
                if (rightmost + moveDistance >= 800)
                    changeDirection = true;
            }
            else
            {
                double leftmost = enemies.Min(e => e.Bounds.Left);
                if (leftmost - moveDistance <= 0)
                    changeDirection = true;
            }

            if (changeDirection)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    var enemy = enemies[i];
                    enemy.Bounds = new Rect(enemy.Bounds.Left, enemy.Bounds.Top + 20, enemy.Bounds.Width, enemy.Bounds.Height);

                    if (i < _enemyElements.Count)
                    {
                        var element = _enemyElements[i];
                        Canvas.SetLeft(element, enemy.Bounds.Left);
                        Canvas.SetTop(element, enemy.Bounds.Top);
                    }
                }

                gameState.EnemiesMovingRight = !gameState.EnemiesMovingRight;
                gameState.EnemySpeedModifier += 0.1;

                int remainingEnemies = enemies.Count;
                double speedFactor = Math.Max(0.3, 1.0 - (50 - remainingEnemies) * 0.02);
                _enemyMoveTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(200, 500 * speedFactor));
            }
            else
            {
                double direction = gameState.EnemiesMovingRight ? 1 : -1;

                for (int i = 0; i < enemies.Count; i++)
                {
                    var enemy = enemies[i];
                    enemy.Bounds = new Rect(enemy.Bounds.Left + (moveDistance * direction), enemy.Bounds.Top, enemy.Bounds.Width, enemy.Bounds.Height);

                    if (i < _enemyElements.Count)
                    {
                        var element = _enemyElements[i];
                        Canvas.SetLeft(element, enemy.Bounds.Left);
                        Canvas.SetTop(element, enemy.Bounds.Top);
                    }
                }
            }
        }

        private void EnemyShootTimer_Tick(object sender, object e)
        {
            if (_gameService.GameState.IsPaused || _gameService.GameState.IsGameOver)
                return;

            var shootingEnemies = _gameService.GameState.Enemies
                .Where(en => en.Type == EnemyType.Small)
                .ToList();

            if (!shootingEnemies.Any()) return;

            var random = new Random();
            var shooter = shootingEnemies[random.Next(shootingEnemies.Count)];

            var bullet = new Bullet(
                shooter.Bounds.Left + shooter.Bounds.Width / 2 - 1.5,
                shooter.Bounds.Bottom,
                BulletType.Enemy
            );

            _gameService.GameState.Bullets.Add(bullet);
            _soundService.PlaySound(SoundEffects.EnemyShoot);
            CreateBulletVisual(bullet);
        }

        private void RedEnemyTimer_Tick(object sender, object e)
        {
            if (_gameService.GameState.IsPaused || _gameService.GameState.IsGameOver)
                return;

            if (_gameService.GameState.RedEnemy == null)
            {
                _gameService.GameState.RedEnemy = new RedEnemy();
                _soundService.PlaySound(SoundEffects.RedEnemyAppear);
                ShowRedEnemy();
            }
        }
        
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            GameOverOverlay.Visibility = Visibility.Collapsed;
            InitializeGame();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToMenu();
        }


        private void ShowRedEnemy()
        {
            var redEnemy = _gameService.GameState.RedEnemy;
            if (redEnemy == null) return;

            if (_redEnemyImage == null)
            {
                _redEnemyImage = new Image
                {
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/invaderRED.gif")),
                    Width = redEnemy.Bounds.Width,
                    Height = redEnemy.Bounds.Height
                };
                GameCanvas.Children.Add(_redEnemyImage);
            }

            Canvas.SetLeft(_redEnemyImage, redEnemy.Bounds.Left);
            Canvas.SetTop(_redEnemyImage, redEnemy.Bounds.Top);
            _redEnemyImage.Visibility = Visibility.Visible;
        }

        private void UpdateRedEnemy()
        {
            var redEnemy = _gameService.GameState.RedEnemy;
            if (redEnemy == null) return;

            redEnemy.Update();

            if ((redEnemy.MovingRight && redEnemy.Bounds.Left > 800) ||
                (!redEnemy.MovingRight && redEnemy.Bounds.Right < 0))
            {
                _gameService.GameState.RedEnemy = null;
                if (_redEnemyImage != null)
                    _redEnemyImage.Visibility = Visibility.Collapsed;
            }
            else if (_redEnemyImage != null)
            {
                Canvas.SetLeft(_redEnemyImage, redEnemy.Bounds.Left);
                Canvas.SetTop(_redEnemyImage, redEnemy.Bounds.Top);
            }
        }

        private void CheckCollisions()
        {
            var gameState = _gameService.GameState;
            var bullets = gameState.Bullets.ToList();

            for (int bulletIndex = bullets.Count - 1; bulletIndex >= 0; bulletIndex--)
            {
                var bullet = bullets[bulletIndex];
                bool collision = false;

                if (bullet.Type == BulletType.Player)
                {
                    var enemies = gameState.Enemies.ToList();
                    for (int enemyIndex = enemies.Count - 1; enemyIndex >= 0; enemyIndex--)
                    {
                        var enemy = enemies[enemyIndex];
                        if (bullet.CheckCollision(enemy))
                        {
                            gameState.Player.AddScore(enemy.PointValue);
                            gameState.Enemies.Remove(enemy);
                            gameState.Bullets.Remove(bullet);

                            RemoveEnemyVisual(enemyIndex);
                            RemoveBulletVisual(bulletIndex);

                            _soundService.PlaySound(SoundEffects.Explosion);
                            CheckExtraLife();
                            collision = true;
                            break;
                        }
                    }

                    if (!collision && gameState.RedEnemy != null && bullet.CheckCollision(gameState.RedEnemy))
                    {
                        gameState.Player.AddScore(gameState.RedEnemy.PointValue);
                        gameState.RedEnemy = null;
                        gameState.Bullets.Remove(bullet);

                        if (_redEnemyImage != null)
                            _redEnemyImage.Visibility = Visibility.Collapsed;
                        RemoveBulletVisual(bulletIndex);

                        _soundService.PlaySound(SoundEffects.RedEnemyKilled);
                        CheckExtraLife();
                        collision = true;
                    }
                }
                else
                {
                    if (bullet.CheckCollision(gameState.Player))
                    {
                        gameState.Player.LoseLife();
                        gameState.Bullets.Remove(bullet);
                        RemoveBulletVisual(bulletIndex);

                        _soundService.PlaySound(SoundEffects.Explosion);
                        collision = true;
                    }
                }

                if (!collision)
                {
                    var shields = gameState.Shields.ToList();
                    for (int shieldIndex = 0; shieldIndex < shields.Count; shieldIndex++)
                    {
                        var shield = shields[shieldIndex];
                        if (shield.IsActive && bullet.CheckCollision(shield))
                        {
                            shield.TakeDamage();
                            gameState.Bullets.Remove(bullet);
                            RemoveBulletVisual(bulletIndex);

                            UpdateShieldVisual(shieldIndex, shield);
                            collision = true;
                            break;
                        }
                    }
                }
            }
        }

        private void RemoveEnemyVisual(int index)
        {
            if (index >= 0 && index < _enemyElements.Count)
            {
                EnemiesCanvas.Children.Remove(_enemyElements[index]);
                _enemyElements.RemoveAt(index);
            }
        }

        private void UpdateShieldVisual(int index, Shield shield)
        {
            if (index >= 0 && index < _shieldElements.Count)
            {
                var shieldRect = _shieldElements[index];
                if (shield.IsActive)
                {
                    shieldRect.Fill = GetShieldBrush(shield.Health);
                }
                else
                {
                    shieldRect.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CheckExtraLife()
        {
            var player = _gameService.GameState.Player;
            if (player.Score >= 1000 && player.Score % 1000 < 50 && player.Lives < 6)
            {
                player.GainLife();
                _soundService.PlaySound(SoundEffects.ExtraLife);
            }
        }

        private void CheckGameEnd()
        {
            var gameState = _gameService.GameState;

            if (gameState.Player.Lives <= 0 || gameState.Enemies.Any(e => e.Bounds.Bottom >= gameState.Player.Bounds.Top - 10))
            {
                GameOver();
                return;
            }

            if (!gameState.Enemies.Any())
            {
                NextLevel();
            }
        }

        private void GameOver()
        {
            var gameState = _gameService.GameState;
            gameState.IsGameOver = true;
            StopGame();

            _soundService.PlaySound(SoundEffects.GameOver);

            FinalScoreText.Text = $"FINAL SCORE: {gameState.Player.Score:D4}";
            GameOverOverlay.Visibility = Visibility.Visible;
        }

        private void NextLevel()
        {
            var gameState = _gameService.GameState;
            gameState.Level++;

            gameState.Enemies.Clear();
            for (int row = 0; row < 5; row++)
            {
                EnemyType type = row == 0 ? EnemyType.Small :
                                row < 3 ? EnemyType.Medium :
                                EnemyType.Large;

                for (int col = 0; col < 10; col++)
                {
                    gameState.Enemies.Add(new Enemy(type, 100 + col * 50, 50 + row * 40));
                }
            }

            gameState.EnemiesMovingRight = true;
            gameState.EnemySpeedModifier += 0.2;

            CreateEnemies();
        }
        
        private void ReturnToMenu()
        {
            StopGame();
            App.NavigationService?.NavigateToMainMenu();
        }

        private void UpdateUI()
        {
            ScoreText.Text = $"SCORE: {_gameService.GameState.Player.Score:D4}";
            LivesText.Text = $"LIVES: {_gameService.GameState.Player.Lives}";
            LevelText.Text = $"LEVEL: {_gameService.GameState.Level}";
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _pressedKeys[e.Key] = true;

            if (e.Key == VirtualKey.Space)
            {
                TryShoot();
            }
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            _pressedKeys[e.Key] = false;
        }
    }
}
