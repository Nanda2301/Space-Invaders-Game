using SpaceInvaders.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceInvaders.Services
{
    public interface IHighScoreService
    {
        Task<List<HighScore>> GetHighScoresAsync();
        Task SaveHighScoreAsync(HighScore highScore);
    }
}
