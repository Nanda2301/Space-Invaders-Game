using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace SpaceInvaders.Views
{
    public sealed partial class ControlsPage : Page
    {
        public ControlsPage()
        {
            this.InitializeComponent();
            this.DataContext = this; // Para binding se necessário
            
            // Permitir que a página receba foco para capturar teclas
            this.IsTabStop = true;
            this.Focus(FocusState.Programmatic);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                GoBackToMainMenu();
                e.Handled = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBackToMainMenu();
        }

        private void GoBackToMainMenu()
        {
            try
            {
                // Primeira opção: usar o NavigationService da aplicação
                if (App.NavigationService != null)
                {
                    App.NavigationService.NavigateToMainMenu();
                    return;
                }

                // Segunda opção: usar o Frame diretamente
                if (this.Frame != null)
                {
                    if (this.Frame.CanGoBack)
                    {
                        this.Frame.GoBack();
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(MainPage));
                    }
                    return;
                }

                // Terceira opção: navegar diretamente
                this.Frame?.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating back to main menu: {ex.Message}");
                
                // Fallback: tentar criar nova instância
                try
                {
                    this.Frame?.Navigate(typeof(MainPage));
                }
                catch
                {
                    // Se tudo falhar, pelo menos imprimir no debug
                    System.Diagnostics.Debug.WriteLine("Failed to navigate back to main menu");
                }
            }
        }
    }
}
