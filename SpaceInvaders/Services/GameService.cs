using SpaceInvaders.Models;

namespace SpaceInvaders.Services
{
    public class GameService : IGameService
    {
        public GameState GameState { get; } = new GameState();

        public void InitializeGame()
        {
            GameState.InitializeGame();
        }
    }
}
