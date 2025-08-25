using SpaceInvaders.Models;
using SpaceInvaders.Models.GameObjects;
using System.Collections.ObjectModel;

namespace SpaceInvaders.Services
{
    public class GameService : IGameService
    {
        public GameState GameState { get; private set; }

        public GameService()
        {
            GameState = new GameState();
        }

        public void InitializeGame()
        {
            GameState.InitializeGame();
        }
    }
}
