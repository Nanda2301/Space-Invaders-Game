using SpaceInvaders.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace SpaceInvaders.Services
{
    public class HighScoreService : IHighScoreService
    {
        private const string HIGHSCORES_FILENAME = "highscores.json";
        private List<HighScore> _cachedScores;
        private IHighScoreService _highScoreServiceImplementation;

        public async Task<List<HighScore>> GetHighScoresAsync()
        {
            if (_cachedScores != null)
                return _cachedScores.OrderByDescending(h => h.Score).Take(10).ToList();

            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.TryGetItemAsync(HIGHSCORES_FILENAME) as StorageFile;
                
                if (file == null)
                {
                    _cachedScores = new List<HighScore>();
                    return new List<HighScore>();
                }

                var json = await FileIO.ReadTextAsync(file);
                _cachedScores = JsonSerializer.Deserialize<List<HighScore>>(json) ?? new List<HighScore>();
                
                return _cachedScores.OrderByDescending(h => h.Score).Take(10).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading high scores: {ex.Message}");
                _cachedScores = new List<HighScore>();
                return new List<HighScore>();
            }
        }

        public Task ClearHighScoresAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SaveHighScoreAsync(HighScore highScore)
        {
            try
            {
                if (_cachedScores == null)
                {
                    _cachedScores = await GetHighScoresAsync();
                }

                _cachedScores.Add(highScore);
                
                // Ordenar por pontuação (maior primeiro) e manter apenas os top 10
                _cachedScores = _cachedScores.OrderByDescending(h => h.Score).Take(10).ToList();

                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.CreateFileAsync(HIGHSCORES_FILENAME, CreationCollisionOption.ReplaceExisting);
                
                var json = JsonSerializer.Serialize(_cachedScores, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving high score: {ex.Message}");
            }
        }

        public bool IsHighScore(int score)
        {
            if (_cachedScores == null || _cachedScores.Count < 10)
                return true;

            return score > _cachedScores.Min(h => h.Score);
        }

        public void ClearCache()
        {
            _cachedScores = null;
        }
    }
}
