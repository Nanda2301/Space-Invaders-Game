using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SpaceInvaders.Services;
using SpaceInvaders.Models;
using System;

namespace SpaceInvaders.Views
{
    public sealed partial class GameOverPage : Page
    {
        private int _score;
        private bool _scoreSaved = false;

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
                UpdateScoreDisplay();
            }
        }

        private void UpdateScoreDisplay()
        {
            if (FindName("ScoreText") is TextBlock scoreText)
            {
                scoreText.Text = $"Final Score: {_score:N0}";
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FindName("PlayerNameTextBox") is TextBox playerNameTextBox)
                {
                    string playerName = playerNameTextBox.Text?.Trim();

                    if (string.IsNullOrWhiteSpace(playerName))
                    {
                        await ShowMessageDialog("Please enter your name!", "Invalid Name");
                        return;
                    }

                    if (_scoreSaved)
                    {
                        await ShowMessageDialog("Score already saved!", "Already Saved");
                        return;
                    }

                    // Criar o high score
                    var highScore = new HighScore
                    {
                        PlayerName = playerName,
                        Score = _score,
                        Date = DateTime.Now
                    };

                    // Salvar usando o serviço
                    var highScoreService = new HighScoreService();
                    await highScoreService.SaveHighScoreAsync(highScore);

                    // Atualizar UI
                    _scoreSaved = true;
                    playerNameTextBox.IsEnabled = false;
                    
                    if (sender is Button saveButton)
                    {
                        saveButton.IsEnabled = false;
                        saveButton.Content = "SCORE SAVED!";
                    }

                    await ShowMessageDialog($"Score saved successfully!\n{playerName}: {_score:N0} points", "Score Saved");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving score: {ex.Message}");
                await ShowMessageDialog("Error saving score. Please try again.", "Error");
            }
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Reiniciar o jogo
                App.GameViewModel?.StartGame();
                App.NavigationService?.NavigateToGame();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error restarting game: {ex.Message}");
                // Fallback: ir para o menu principal
                App.NavigationService?.NavigateToMainMenu();
            }
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService?.NavigateToMainMenu();
        }

        private async System.Threading.Tasks.Task ShowMessageDialog(string message, string title)
        {
            try
            {
                var dialog = new ContentDialog()
                {
                    Title = title,
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing dialog: {ex.Message}");
            }
        }

        // Método para verificar se é um high score
        private async System.Threading.Tasks.Task<bool> IsHighScore(int score)
        {
            try
            {
                var highScoreService = new HighScoreService();
                return highScoreService.IsHighScore(score);
            }
            catch
            {
                return true; // Se der erro, assume que é high score
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Focar no campo de texto se for um high score
            if (await IsHighScore(_score))
            {
                if (FindName("PlayerNameTextBox") is TextBox playerNameTextBox)
                {
                    playerNameTextBox.Focus(FocusState.Programmatic);
                }
            }
        }
    }
}
