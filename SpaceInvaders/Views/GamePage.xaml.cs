using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SpaceInvaders.ViewModels;
using SpaceInvaders.Services;
using Windows.System;

namespace SpaceInvaders.Views
{
    public sealed partial class GamePage : Page
    {
        private GameViewModel _viewModel => App.GameViewModel;

        public GamePage()
        {
            this.InitializeComponent();
            this.DataContext = _viewModel; // garante que os bindings funcionem
            
            System.Diagnostics.Debug.WriteLine("GamePage created and DataContext set");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("GamePage loaded");
            this.Focus(FocusState.Programmatic);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("GamePage unloaded");
            _viewModel?.Dispose();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _viewModel?.ProcessKeyDown(e.Key);
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            _viewModel?.ProcessKeyUp(e.Key);
        }

        private void SaveScoreButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                var playerName = PlayerNameTextBox?.Text?.Trim();
                if (!string.IsNullOrWhiteSpace(playerName))
                {
                    _viewModel?.SaveHighScore(playerName);
                    
                    // Disable the button after saving
                    if (sender is Button button)
                    {
                        button.IsEnabled = false;
                        button.Content = "SCORE SAVED!";
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Score saved for player: {playerName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Player name is empty, cannot save score");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving score: {ex.Message}");
            }
        }
    }
}
