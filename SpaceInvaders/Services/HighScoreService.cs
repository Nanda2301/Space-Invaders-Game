using SpaceInvaders.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SpaceInvaders.Services
{
    public static class HighScoreService
    {
        private static readonly string HighScoresFilePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "highscores.json");
        
        public static void SaveHighScore(string playerName, int score)
        {
            var highScores = LoadHighScores();
            highScores.Add(new HighScore { PlayerName = playerName, Score = score });
            
            // Ordenar por pontuação (maior primeiro) e manter apenas os top 10
            highScores.Sort((a, b) => b.Score.CompareTo(a.Score));
            if (highScores.Count > 10)
            {
                highScores = highScores.GetRange(0, 10);
            }
            
            var json = JsonSerializer.Serialize(highScores);
            File.WriteAllText(HighScoresFilePath, json);
        }
        
        public static List<HighScore> LoadHighScores()
        {
            if (!File.Exists(HighScoresFilePath))
                return new List<HighScore>();
                
            var json = File.ReadAllText(HighScoresFilePath);
            return JsonSerializer.Deserialize<List<HighScore>>(json) ?? new List<HighScore>();
        }
    }
}
