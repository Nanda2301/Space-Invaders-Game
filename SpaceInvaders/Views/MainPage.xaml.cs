using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using Windows.Foundation;

namespace SpaceInvaders.Views
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _starsTimer;
        private Random _random = new Random();

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = App.MainViewModel;
            CreateStarsBackground();
            StartStarsAnimation();
        }

        private void CreateStarsBackground()
        {
            // Criar estrelas animadas no fundo
            for (int i = 0; i < 50; i++)
            {
                var star = new Ellipse
                {
                    Width = _random.Next(1, 4),
                    Height = _random.Next(1, 4),
                    Fill = new SolidColorBrush(Microsoft.UI.Colors.White),
                    Opacity = _random.NextDouble() * 0.8 + 0.2
                };

                Canvas.SetLeft(star, _random.NextDouble() * 800);
                Canvas.SetTop(star, _random.NextDouble() * 600);
                
                StarsCanvas.Children.Add(star);
            }
        }

        private void StartStarsAnimation()
        {
            _starsTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _starsTimer.Tick += StarsTimer_Tick;
            _starsTimer.Start();
        }

        private void StarsTimer_Tick(object sender, object e)
        {
            foreach (var child in StarsCanvas.Children)
            {
                if (child is Ellipse star)
                {
                    var currentTop = Canvas.GetTop(star);
                    currentTop += 0.5;

                    if (currentTop > 600)
                    {
                        Canvas.SetTop(star, -10);
                        Canvas.SetLeft(star, _random.NextDouble() * 800);
                    }
                    else
                    {
                        Canvas.SetTop(star, currentTop);
                    }
                }
            }
        }

        private void StartNewGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Inicializar o jogo no ViewModel
                App.GameViewModel?.StartGame();
                
                // Navegar para a página do jogo
                App.NavigationService?.NavigateToGame();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting game: {ex.Message}");
            }
        }

        private void HighScoresButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.NavigationService?.NavigateToHighScores();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating to high scores: {ex.Message}");
            }
        }

        private void ControlsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.NavigationService?.NavigateToControls();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navigating to controls: {ex.Message}");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Para aplicações UWP/WinUI, use Application.Current.Exit()
                Application.Current.Exit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exiting application: {ex.Message}");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _starsTimer?.Stop();
        }
    }
}
