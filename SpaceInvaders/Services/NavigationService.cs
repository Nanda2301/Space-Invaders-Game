using Microsoft.UI.Xaml.Controls;
using SpaceInvaders.Views;

namespace SpaceInvaders.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }
        
        public void NavigateToMainMenu()
        {
            _frame.Navigate(typeof(MainPage));
        }

        public void NavigateToGame()
        {
            _frame.Navigate(typeof(GamePage));
        }

        public void NavigateToHighScores()
        {
            _frame.Navigate(typeof(HighScoresPage));
        }

        public void NavigateToControls()
        {
            _frame.Navigate(typeof(ControlsPage));
        }

        // CORREÇÃO: Adicionar método para navegar ao Game Over
        public void NavigateToGameOver(int score)
        {
            _frame.Navigate(typeof(GameOverPage), score);
        }
    }
}
