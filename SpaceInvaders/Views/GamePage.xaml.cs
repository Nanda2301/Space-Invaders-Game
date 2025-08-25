using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using SpaceInvaders.Models;
using SpaceInvaders.Models.GameObjects;
using SpaceInvaders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.Foundation;

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
        
        public string PlayerName { get; set; }
        public string ScoreText => $"Score: {GameState.Score}";


        public GamePage()
        {
            this.InitializeComponent();
            
            // Inicializar serviços
            _gameService = new GameService();
            _soundService = new SoundService();
            
            // Configurar data context
            this.DataContext = _gameService;
            
            // Configurar timers
            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _gameTimer.Tick += GameTimer_Tick;

            _enemyMoveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _enemyMoveTimer.Tick += EnemyMoveTimer_Tick;

            _enemyShootTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _enemyShootTimer.Tick += EnemyShootTimer_Tick;

            _redEnemyTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(120) };
            _redEnemyTimer.Tick += RedEnemyTimer_Tick;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            StopGame();
        }

        private void InitializeGame()
        {
            _gameService.InitializeGame();
            StartGame();
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
            UpdateGameState();
            CheckGameEnd();
        }

        private void ProcessInput()
        {
            var player = _gameService.GameState.Player;
            if (_pressedKeys.GetValueOrDefault(VirtualKey.Left)) 
            { 
                player.MoveLeft(); 
            }
            if (_pressedKeys.GetValueOrDefault(VirtualKey.Right)) 
            { 
                player.MoveRight(800); 
            }
            if (_pressedKeys.GetValueOrDefault(VirtualKey.Space)) 
            {
                TryShoot();
            }
        }

        private void TryShoot()
        {
            var player = _gameService.GameState.Player;
            if (DateTime.Now - _lastShotTime < _shotCooldown || !player.CanShoot)
                return;

            // Criar nova bala
            var bullet = new Bullet(
                player.Bounds.Left + player.Bounds.Width / 2 - 1.5, 
                player.Bounds.Top - 10, 
                BulletType.Player);
            
            _gameService.GameState.Bullets.Add(bullet);
            _soundService.PlaySound(SoundEffects.PlayerShoot);
            _lastShotTime = DateTime.Now;
            player.CanShoot = false;
        }

        private void UpdateGameState()
        {
            // Atualizar balas
            for (int i = _gameService.GameState.Bullets.Count - 1; i >= 0; i--)
            {
                var bullet = _gameService.GameState.Bullets[i];
                bullet.Update();

                if (bullet.Bounds.Top < 0 || bullet.Bounds.Bottom > 600)
                {
                    _gameService.GameState.Bullets.RemoveAt(i);
                    if (bullet.Type == BulletType.Player) 
                        _gameService.GameState.Player.CanShoot = true;
                }
            }
            
            // Verificar colisões
            CheckCollisions();
        }

        private void CheckCollisions()
        {
            // Implementar lógica de colisão
            // Verificar colisões entre balas e inimigos
            // Verificar colisões entre balas e escudos
            // Verificar colisões entre balas inimigas e jogador
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
            if (gameState.Enemies.Count == 0) return;

            double moveDistance = 10 * gameState.EnemySpeedModifier;
            bool changeDirection = false;

            if (gameState.EnemiesMovingRight && gameState.Enemies.Max(e => e.Bounds.Right) + moveDistance >= 800)
                changeDirection = true;
            else if (!gameState.EnemiesMovingRight && gameState.Enemies.Min(e => e.Bounds.Left) - moveDistance <= 0)
                changeDirection = true;

            foreach (var enemy in gameState.Enemies)
            {
                if (changeDirection)
                    enemy.Bounds = new Rect(enemy.Bounds.Left, enemy.Bounds.Top + 20, enemy.Bounds.Width, enemy.Bounds.Height);
                else
                    enemy.Bounds = new Rect(enemy.Bounds.Left + (gameState.EnemiesMovingRight ? moveDistance : -moveDistance), enemy.Bounds.Top, enemy.Bounds.Width, enemy.Bounds.Height);
            }

            if (changeDirection)
                gameState.EnemiesMovingRight = !gameState.EnemiesMovingRight;
        }

        private void EnemyShootTimer_Tick(object sender, object e)
        {
            if (_gameService.GameState.IsPaused || _gameService.GameState.IsGameOver)
                return;
                
            var enemies = _gameService.GameState.Enemies.Where(en => en.Type == EnemyType.Medium).ToList();
            if (!enemies.Any()) return;

            var random = new Random();
            var shooter = enemies[random.Next(enemies.Count)];

            var bullet = new Bullet(
                shooter.Bounds.Left + shooter.Bounds.Width / 2 - 1.5, 
                shooter.Bounds.Bottom, 
                BulletType.Enemy);
                
            _gameService.GameState.Bullets.Add(bullet);
            _soundService.PlaySound(SoundEffects.EnemyShoot);
        }

        private void RedEnemyTimer_Tick(object sender, object e)
        {
            if (_gameService.GameState.IsPaused || _gameService.GameState.IsGameOver)
                return;
                
            if (_gameService.GameState.RedEnemy != null) return;
            
            _gameService.GameState.RedEnemy = new RedEnemy();
            _soundService.PlaySound(SoundEffects.RedEnemyAppear);
        }

        private void CheckGameEnd()
        {
            if (_gameService.GameState.IsGameOver)
            {
                StopGame();
            }
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.P)
            {
                TogglePause();
                e.Handled = true;
                return;
            }

            _pressedKeys[e.Key] = true;
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            _pressedKeys[e.Key] = false;
            
            // Permitir que o jogador atire novamente quando soltar a barra de espaço
            if (e.Key == VirtualKey.Space)
            {
                _gameService.GameState.Player.CanShoot = true;
            }
        }

        private void TogglePause()
        {
            _gameService.GameState.IsPaused = !_gameService.GameState.IsPaused;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Navegar para a página principal
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void SaveScoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PlayerName))
            {
                // Mostrar mensagem de erro
                return;
            }
            
            // Salvar pontuação
            HighScoreService.SaveHighScore(PlayerName, _gameService.GameState.Score);
            
            // Mostrar mensagem de sucesso
        }
    }
}
