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
            // Cria primeiro o SoundService
            _soundService = new SoundService();

            // Passa o soundService para o GameService
            _gameService = new GameService(_soundService);

            // Inicializa o estado do jogo
            _currentGameState = _gameService.GameState;
        }

        public void InitializeGame()
        {
            try
            {
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
                UpdatePlayer();
                UpdateEnemies();
                UpdateBullets();
                CheckCollisions();
            }
        }

        private void UpdatePlayer()
        {
            // Lógica de atualização do jogador
        }

        private void UpdateEnemies()
        {
            // Lógica de atualização dos inimigos
        }

        private void UpdateBullets()
        {
            // Lógica de atualização dos tiros
        }

        private void CheckCollisions()
        {
            // Lógica de detecção de colisões
        }

        // Propriedades de acesso ao estado do jogo
        public int Score => _currentGameState.Player.Score;
        public int Lives => _currentGameState.Player.Lives;
        public int Level => _currentGameState.Level;
        public bool IsGameRunning => !_currentGameState.IsPaused && !_currentGameState.IsGameOver;
    }
}
