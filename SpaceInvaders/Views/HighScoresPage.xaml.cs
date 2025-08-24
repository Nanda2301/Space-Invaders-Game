using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;

namespace SpaceInvaders.Views
{
    public sealed partial class HighScoresPage : Page
    {
        public HighScoresPage()
        {
            this.InitializeComponent();
            LoadHighScores();
        }

        private async void LoadHighScores()
        {
            try
            {
                var highScoreService = new SpaceInvaders.Services.HighScoreService();
                var scores = await highScoreService.GetHighScoresAsync();
                
                if (scores.Any())
                {
                    // Limpar container existente (manter header)
                    var header = ScoresContainer.Children.FirstOrDefault();
                    ScoresContainer.Children.Clear();
                    
                    if (header != null)
                        ScoresContainer.Children.Add(header);
                    
                    // Adicionar scores
                    for (int i = 0; i < Math.Min(10, scores.Count); i++)
                    {
                        var score = scores[i];
                        var scoreGrid = CreateScoreRow(i + 1, score.PlayerName, score.Score, score.Date);
                        ScoresContainer.Children.Add(scoreGrid);
                    }
                    
                    NoScoresText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    NoScoresText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading high scores: {ex.Message}");
                NoScoresText.Text = "Error loading high scores.";
                NoScoresText.Visibility = Visibility.Visible;
            }
        }

        private Grid CreateScoreRow(int rank, string playerName, int score, DateTime date)
        {
            var grid = new Grid { Margin = new Thickness(0, 2) };
            
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

            // Rank
            var rankText = new TextBlock
            {
                Text = rank.ToString(),
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.White),
                FontFamily = new FontFamily("Consolas")
            };
            Grid.SetColumn(rankText, 0);
            grid.Children.Add(rankText);

            // Player Name
            var nameText = new TextBlock
            {
                Text = playerName,
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.White),
                FontFamily = new FontFamily("Consolas")
            };
            Grid.SetColumn(nameText, 1);
            grid.Children.Add(nameText);

            // Score
            var scoreText = new TextBlock
            {
                Text = score.ToString("D4"),
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.White),
                FontFamily = new FontFamily("Consolas"),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetColumn(scoreText, 2);
            grid.Children.Add(scoreText);

            // Date
            var dateText = new TextBlock
            {
                Text = date.ToString("dd/MM/yy"),
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.LightGray),
                FontFamily = new FontFamily("Consolas"),
                HorizontalAlignment = HorizontalAlignment.Right,
                FontSize = 10
            };
            Grid.SetColumn(dateText, 3);
            grid.Children.Add(dateText);

            return grid;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService?.NavigateToMainMenu();
        }
    }
}
