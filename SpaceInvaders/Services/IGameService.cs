using SpaceInvaders.Models;

namespace SpaceInvaders.Services
{
    public interface IGameService
    {
        GameState GameState { get; }
        void InitializeGame();
    }
}
