using SpaceInvaders.Models;
using SpaceInvaders.Services;
using System;
using SoundEffects = SpaceInvaders.Models.SoundEffects;

namespace SpaceInvaders
{
    public class GameManager
    {
        private readonly GameService _gameService;
        private readonly SoundService _soundService;
        private GameState _currentGameState;

        public GameManager()
        {
            _gameService = new GameService();
            _soundService = new SoundService();
            _currentGameState = _gameService.GameState;
        }

        public void InitializeGame()
        {
            try
            {
                // Inicializar o estado do jogo
                _gameService.InitializeGame();
                _currentGameState = _gameService.GameState;
                
                Console.WriteLine("Jogo inicializado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar jogo: {ex.Message}");
            }
        }

        public void StartGame()
        {
            if (_currentGameState.IsPaused)
            {
                _currentGameState.IsPaused = false;
            }
        }

        public void PauseGame()
        {
            if (!_currentGameState.IsPaused)
            {
                _currentGameState.IsPaused = true;
            }
        }

        public void StopGame()
        {
            _currentGameState.IsGameOver = true;
            _soundService.PlaySound(SoundEffects.GameOver);
        }

        public void UpdateGame()
        {
            if (!_currentGameState.IsPaused && !_currentGameState.IsGameOver)
            {
                // Lógica de atualização do jogo
                UpdatePlayer();
                UpdateEnemies();
                UpdateBullets();
                CheckCollisions();
            }
        }

        private void UpdatePlayer()
        {
            // Lógica de atualização do jogador
            // (será implementada pelo GameViewModel)
        }

        private void UpdateEnemies()
        {
            // Lógica de atualização dos inimigos
            // (será implementada pelo GameViewModel)
        }

        private void UpdateBullets()
        {
            // Lógica de atualização dos tiros
            // (será implementada pelo GameViewModel)
        }

        private void CheckCollisions()
        {
            // Lógica de detecção de colisões
            // (será implementada pelo GameViewModel)
        }

        // Propriedades de acesso ao estado do jogo
        public int Score => _currentGameState.Player.Score;
        public int Lives => _currentGameState.Player.Lives;
        public int Level => _currentGameState.Level;
        public bool IsGameRunning => !_currentGameState.IsPaused && !_currentGameState.IsGameOver;
    }
}
