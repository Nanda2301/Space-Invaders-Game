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
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.StartGame();
            this.Focus(FocusState.Programmatic);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Dispose();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _viewModel.ProcessKeyDown(e.Key);
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            _viewModel.ProcessKeyUp(e.Key);
        }
    }
}
