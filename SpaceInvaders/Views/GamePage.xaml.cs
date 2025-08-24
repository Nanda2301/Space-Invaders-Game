using Windows.System;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SpaceInvaders.ViewModels;

namespace SpaceInvaders.Views
{
    public sealed partial class GamePage : Page
    {
        private GameViewModel VM => DataContext as GameViewModel;

        public GamePage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Garante foco para receber teclado
            await Microsoft.UI.Xaml.Input.FocusManager.TryFocusAsync(GameCanvas, FocusState.Programmatic);

            // (Opcional) fallback: escuta no nível da janela
            try
            {
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            }
            catch { /* em algumas plataformas não existe CoreWindow; ignore */ }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            }
            catch { }
        }

        // Captura tecla quando a Page tem foco
        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            HandleKey(e.Key);
            e.Handled = true;
        }

        // Captura tecla no nível da janela (fallback)
        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Mapeia virtual key do CoreWindow
            HandleKey(args.VirtualKey);
            args.Handled = true;
        }

        private void HandleKey(VirtualKey key)
        {
            if (VM == null) return;

            switch (key)
            {
                case VirtualKey.Left:
                    VM.MoveLeftCommand?.Execute(null);
                    break;

                case VirtualKey.Right:
                    VM.MoveRightCommand?.Execute(null);
                    break;

                case VirtualKey.Space:
                    VM.ShootCommand?.Execute(null);
                    break;

                case VirtualKey.P:
                    VM.PauseCommand?.Execute(null);
                    break;
            }
        }
    }
}
