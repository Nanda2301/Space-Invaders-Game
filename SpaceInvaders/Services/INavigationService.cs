namespace SpaceInvaders.Services
{
    public interface INavigationService
    {
        void NavigateToMainMenu();
        void NavigateToGame();
        void NavigateToHighScores();
        void NavigateToControls();
        void NavigateToGameOver(int score);
    }
}
