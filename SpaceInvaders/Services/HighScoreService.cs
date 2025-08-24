using SpaceInvaders.Models;
using SpaceInvaders.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace SpaceInvaders.Services
{
    public class HighScoreService : IHighScoreService
    {
        private const string FileName = "highscores.txt";

        public async Task<List<HighScore>> GetHighScoresAsync()
        {
            var highscores = new List<HighScore>();

            try
            {
                var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(FileName) as StorageFile;
                if (file != null)
                {
                    var lines = await FileIO.ReadLinesAsync(file);
                    foreach (var line in lines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length == 3 && int.TryParse(parts[1], out int score) && DateTime.TryParse(parts[2], out DateTime date))
                        {
                            highscores.Add(new HighScore
                            {
                                PlayerName = parts[0],
                                Score = score,
                                Date = date
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or display error)
                System.Diagnostics.Debug.WriteLine($"Error loading high scores: {ex.Message}");
            }

            return highscores.OrderByDescending(h => h.Score).ToList();
        }

        public async Task SaveHighScoreAsync(HighScore highScore)
        {
            try
            {
                var highscores = await GetHighScoresAsync();
                highscores.Add(highScore);
                highscores = highscores.OrderByDescending(h => h.Score).Take(10).ToList();

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    FileName, CreationCollisionOption.ReplaceExisting);

                var lines = highscores.Select(h => $"{h.PlayerName}|{h.Score}|{h.Date:yyyy-MM-dd HH:mm:ss}");
                await FileIO.WriteLinesAsync(file, lines);
            }
            catch (Exception ex)
            {
                // Handle exception (log or display error)
                System.Diagnostics.Debug.WriteLine($"Error saving high score: {ex.Message}");
            }
        }
    }
}
