using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SpaceInvaders.Views
{
    public sealed partial class GameOverPage : Page
    {
        private int _score;

        public GameOverPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int score)
            {
                _score = score;
                // Atualizar manualmente o texto
                if (FindName("ScoreText") is TextBlock scoreText)
                {
                    scoreText.Text = $"Pontuação Final: {_score}";
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (FindName("PlayerNameTextBox") is TextBox playerNameTextBox && 
                !string.IsNullOrWhiteSpace(playerNameTextBox.Text))
            {
                App.HighScoresViewModel.SaveHighScore(playerNameTextBox.Text, _score);
                playerNameTextBox.IsEnabled = false;
                ((Button)sender).IsEnabled = false;
            }
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateToGame();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateToMainMenu();
        }
    }
}
