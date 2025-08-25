using SpaceInvaders.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceInvaders.Services
{
    public interface IHighScoreService
    {
        Task SaveHighScoreAsync(HighScore highScore);
        Task<List<HighScore>> GetHighScoresAsync();
        Task ClearHighScoresAsync();
    }
}
