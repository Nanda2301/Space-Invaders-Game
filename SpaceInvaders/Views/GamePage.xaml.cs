using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using SpaceInvaders.Models;
using SpaceInvaders.Models.GameObjects;
using SpaceInvaders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.System;

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

        // Elementos visuais
        private readonly List<Rectangle> _enemyRects = new();
        private readonly List<Rectangle> _bulletRects = new();
        private readonly List<Rectangle> _shieldRects = new();

        public GamePage()
        {
            this.InitializeComponent();
            
            _gameService = new GameService();
            _soundService = new SoundService();

            // Configurar timers
            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; // 60 FPS
            _gameTimer.Tick += GameTimer_Tick;

            _enemyMoveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _enemyMoveTimer.Tick += EnemyMoveTimer_Tick;

            _enemyShootTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _enemyShootTimer.Tick += EnemyShootTimer_Tick;

            _redEnemyTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(120) }; // 2 minutos
            _redEnemyTimer.Tick += RedEnemyTimer_Tick;

            InitializeGame();
        }

        private void InitializeGame()
        {
            _gameService.InitializeGame();
            CreateVisualElements();
            UpdateUI();
            StartGame();
        }

        private void CreateVisualElements()
        {
            CreateEnemies();
            CreateShields();
        }

        private void CreateEnemies()
        {
            EnemiesCanvas.Children.Clear();
            _enemyRects.Clear();

            var enemies = _gameService.GameState.Enemies.ToList();
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                var rect = new Rectangle
                {
                    Width = enemy.Bounds.Width,
                    Height = enemy.Bounds.Height,
                    Fill = GetEnemyBrush(enemy.Type)
                };

                Canvas.SetLeft(rect, enemy.Bounds.Left);
                Canvas.SetTop(rect, enemy.Bounds.Top);
                
                EnemiesCanvas.Children.Add(rect);
                _enemyRects.Add(rect);
            }
        }

        private Brush GetEnemyBrush(EnemyType type)
        {
            return type switch
            {
                EnemyType.Small => new SolidColorBrush(Microsoft.UI.Colors.White),
                EnemyType.Medium => new SolidColorBrush(Microsoft.UI.Colors.Yellow),
                EnemyType.Large => new SolidColorBrush(Microsoft.UI.Colors.Orange),
                _ => new SolidColorBrush(Microsoft.UI.Colors.White)
            };
        }

        private void CreateShields()
        {
            ShieldsCanvas.Children.Clear();
            _shieldRects.Clear();

            var shields = _gameService.GameState.Shields.ToList();
            for (int i = 0; i < shields.Count; i++)
            {
                var shield = shields[i];
                var rect = new Rectangle
                {
                    Width = shield.Bounds.Width,
                    Height = shield.Bounds.Height,
                    Fill = GetShieldBrush(shield.Health)
                };

                Canvas.SetLeft(rect, shield.Bounds.Left);
                Canvas.SetTop(rect, shield.Bounds.Top);
                
                ShieldsCanvas.Children.Add(rect);
                _shieldRects.Add(rect);
            }
        }

        private Brush GetShieldBrush(int health)
        {
            return health switch
            {
                4 => new SolidColorBrush(Microsoft.UI.Colors.Green),
                3 => new SolidColorBrush(Microsoft.UI.Colors.YellowGreen),
                2 => new SolidColorBrush(Microsoft.UI.Colors.Orange),
                1 => new SolidColorBrush(Microsoft.UI.Colors.Red),
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

            if (_pressedKeys.ContainsKey(VirtualKey.Space) && _pressedKeys[VirtualKey.Space])
            {
                TryShoot();
            }
        }

        private void UpdatePlayerPosition()
        {
            var player = _gameService.GameState.Player;
            Canvas.SetLeft(PlayerRect, player.Bounds.Left);
            Canvas.SetTop(PlayerRect, player.Bounds.Top);
        }

        private void TryShoot()
        {
            if (DateTime.Now - _lastShotTime < _shotCooldown)
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
            var rect = new Rectangle
            {
                Width = bullet.Bounds.Width,
                Height = bullet.Bounds.Height,
                Fill = bullet.Type == BulletType.Player ? 
                    new SolidColorBrush(Microsoft.UI.Colors.White) : 
                    new SolidColorBrush(Microsoft.UI.Colors.Red)
            };

            Canvas.SetLeft(rect, bullet.Bounds.Left);
            Canvas.SetTop(rect, bullet.Bounds.Top);
            
            BulletsCanvas.Children.Add(rect);
            _bulletRects.Add(rect);
        }

        private void UpdateBullets()
        {
            var bullets = _gameService.GameState.Bullets.ToList();
            
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bullet.Update();

                // Remover balas que saíram da tela
                if (bullet.Bounds.Top < 0 || bullet.Bounds.Bottom > 600)
                {
                    _gameService.GameState.Bullets.Remove(bullet);
                    RemoveBulletVisual(i);
                    continue;
                }

                // Atualizar posição visual
                if (i < _bulletRects.Count)
                {
                    Canvas.SetLeft(_bulletRects[i], bullet.Bounds.Left);
                    Canvas.SetTop(_bulletRects[i], bullet.Bounds.Top);
                }
            }
        }

        private void RemoveBulletVisual(int index)
        {
            if (index >= 0 && index < _bulletRects.Count)
            {
                BulletsCanvas.Children.Remove(_bulletRects[index]);
                _bulletRects.RemoveAt(index);
            }
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

            // Verificar se precisa mudar direção
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
                // Mover para baixo e mudar direção
                for (int i = 0; i < enemies.Count; i++)
                {
                    var enemy = enemies[i];
                    enemy.Bounds = new Rect(
                        enemy.Bounds.Left,
                        enemy.Bounds.Top + 20,
                        enemy.Bounds.Width,
                        enemy.Bounds.Height
                    );

                    if (i < _enemyRects.Count)
                    {
                        Canvas.SetLeft(_enemyRects[i], enemy.Bounds.Left);
                        Canvas.SetTop(_enemyRects[i], enemy.Bounds.Top);
                    }
                }

                gameState.EnemiesMovingRight = !gameState.EnemiesMovingRight;
                gameState.EnemySpeedModifier += 0.1; // Aumentar velocidade
                _enemyMoveTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(200, 500 - (gameState.Level * 50)));
            }
            else
            {
                // Mover horizontalmente
                double direction = gameState.EnemiesMovingRight ? 1 : -1;
                
                for (int i = 0; i < enemies.Count; i++)
                {
                    var enemy = enemies[i];
                    enemy.Bounds = new Rect(
                        enemy.Bounds.Left + (moveDistance * direction),
                        enemy.Bounds.Top,
                        enemy.Bounds.Width,
                        enemy.Bounds.Height
                    );

                    if (i < _enemyRects.Count)
                    {
                        Canvas.SetLeft(_enemyRects[i], enemy.Bounds.Left);
                        Canvas.SetTop(_enemyRects[i], enemy.Bounds.Top);
                    }
                }
            }
        }

        private void EnemyShootTimer_Tick(object sender, object e)
        {
            if (_gameService.GameState.IsPaused || _gameService.GameState.IsGameOver)
                return;

            // Apenas inimigos de 40 pontos (Medium) atiram
            var enemies = _gameService.GameState.Enemies.Where(e => e.Type == EnemyType.Medium).ToList();
            if (!enemies.Any()) return;

            var random = new Random();
            var shooter = enemies[random.Next(enemies.Count)];

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

        private void ShowRedEnemy()
        {
            var redEnemy = _gameService.GameState.RedEnemy;
            if (redEnemy != null)
            {
                Canvas.SetLeft(RedEnemyRect, redEnemy.Bounds.Left);
                Canvas.SetTop(RedEnemyRect, redEnemy.Bounds.Top);
                RedEnemyRect.Visibility = Visibility.Visible;
            }
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
                RedEnemyRect.Visibility = Visibility.Collapsed;
            }
            else
            {
                Canvas.SetLeft(RedEnemyRect, redEnemy.Bounds.Left);
                Canvas.SetTop(RedEnemyRect, redEnemy.Bounds.Top);
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
                    // Colisão com inimigos
                    var enemies = gameState.Enemies.ToList();
                    for (int enemyIndex = enemies.Count - 1; enemyIndex >= 0; enemyIndex--)
                    {
                        var enemy = enemies[enemyIndex];
                        if (bullet.CheckCollision(enemy))
                        {
                            gameState.Player.Score += enemy.PointValue;
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

                    // Colisão com inimigo vermelho
                    if (!collision && gameState.RedEnemy != null && bullet.CheckCollision(gameState.RedEnemy))
                    {
                        gameState.Player.Score += gameState.RedEnemy.PointValue;
                        gameState.RedEnemy = null;
                        gameState.Bullets.Remove(bullet);
                        
                        RedEnemyRect.Visibility = Visibility.Collapsed;
                        RemoveBulletVisual(bulletIndex);
                        
                        _soundService.PlaySound(SoundEffects.RedEnemyKilled);
                        CheckExtraLife();
                        collision = true;
                    }
                }
                else // Enemy bullet
                {
                    // Colisão com jogador
                    if (bullet.CheckCollision(gameState.Player))
                    {
                        gameState.Player.Lives--;
                        gameState.Bullets.Remove(bullet);
                        RemoveBulletVisual(bulletIndex);
                        
                        _soundService.PlaySound(SoundEffects.Explosion);
                        collision = true;
                    }
                }

                // Colisão com escudos
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
            if (index >= 0 && index < _enemyRects.Count)
            {
                EnemiesCanvas.Children.Remove(_enemyRects[index]);
                _enemyRects.RemoveAt(index);
            }
        }

        private void UpdateShieldVisual(int index, Shield shield)
        {
            if (index >= 0 && index < _shieldRects.Count)
            {
                if (shield.IsActive)
                {
                    _shieldRects[index].Fill = GetShieldBrush(shield.Health);
                }
                else
                {
                    _shieldRects[index].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CheckExtraLife()
        {
            var player = _gameService.GameState.Player;
            if (player.Score >= 1000 && player.Score % 1000 < 50 && player.Lives < 6)
            {
                player.Lives++;
                _soundService.PlaySound(SoundEffects.ExtraLife);
            }
        }

        private void CheckGameEnd()
        {
            var gameState = _gameService.GameState;

            // Verificar se jogador perdeu todas as vidas
            if (gameState.Player.Lives <= 0)
            {
                GameOver();
                return;
            }

            // Verificar se inimigos chegaram até o jogador
            if (gameState.Enemies.Any(e => e.Bounds.Bottom >= gameState.Player.Bounds.Top - 10))
            {
                GameOver();
                return;
            }

            // Verificar se todos os inimigos foram eliminados
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
            
            // Recriar inimigos
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

        private void UpdateUI()
        {
            var gameState = _gameService.GameState;
            ScoreText.Text = $"SCORE: {gameState.Player.Score:D4}";
            LivesText.Text = $"LIVES: {gameState.Player.Lives}";
            LevelText.Text = $"LEVEL: {gameState.Level}";
            
            PauseOverlay.Visibility = gameState.IsPaused ? Visibility.Visible : Visibility.Collapsed;
        }

        // Event Handlers
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            StopGame();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _pressedKeys[e.Key] = true;
            
            if (e.Key == VirtualKey.P)
            {
                TogglePause();
            }
            else if (e.Key == VirtualKey.Escape)
            {
                ReturnToMenu();
            }
            
            e.Handled = true;
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            _pressedKeys[e.Key] = false;
            e.Handled = true;
        }

        private void TogglePause()
        {
            var gameState = _gameService.GameState;
            gameState.IsPaused = !gameState.IsPaused;
            
            if (gameState.IsPaused)
            {
                _gameTimer.Stop();
                _enemyMoveTimer.Stop();
                _enemyShootTimer.Stop();
            }
            else
            {
                _gameTimer.Start();
                _enemyMoveTimer.Start();
                _enemyShootTimer.Start();
            }
            
            UpdateUI();
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

        private void ReturnToMenu()
        {
            StopGame();
            App.NavigationService.NavigateToMainMenu();
        }
    }
}
