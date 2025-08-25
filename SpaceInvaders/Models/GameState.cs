using SpaceInvaders.Models.GameObjects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace SpaceInvaders.Models
{
    public class GameState : INotifyPropertyChanged
    {
        public Player Player { get; set; }
        public ObservableCollection<Enemy> Enemies { get; set; } = new();
        public ObservableCollection<Bullet> Bullets { get; set; } = new();
        public ObservableCollection<Shield> Shields { get; set; } = new();
        public RedEnemy RedEnemy { get; set; }

        public int Score { get; set; }
        public int Lives { get; set; }
        public int Level { get; set; }
        public bool IsPaused { get; set; }
        public bool IsGameOver { get; set; }
        public bool EnemiesMovingRight { get; set; } = true;
        public double EnemySpeedModifier { get; set; } = 1.0;

        public double EnemyDescentAmount { get; set; } = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public GameState()
        {
            Player = new Player();
        }

        public void InitializeGame()
        {
            Score = 0;
            Lives = 3;
            Level = 1;
            IsPaused = false;
            IsGameOver = false;
            EnemiesMovingRight = true;
            EnemySpeedModifier = 1.0;
            RedEnemy = null;
            EnemyDescentAmount = 0;

            Player = new Player();
            Player.Bounds = new Rect(375, 550, 50, 30);

            Enemies.Clear();
            Bullets.Clear();
            Shields.Clear();

            // Cria inimigos 5x10
            for (int row = 0; row < 5; row++)
            {
                EnemyType type = row == 0 ? EnemyType.Small : row < 3 ? EnemyType.Medium : EnemyType.Large;
                for (int col = 0; col < 10; col++)
                {
                    var enemy = new Enemy(type, 100 + col * 50, 50 + row * 40);
                    Enemies.Add(enemy);
                }
            }

            // Cria 4 escudos
            for (int i = 0; i < 4; i++)
            {
                var shield = new Shield(150 + i * 150, 450, 60, 40);
                Shields.Add(shield);
            }

            OnPropertyChanged(nameof(Player));
            OnPropertyChanged(nameof(Enemies));
            OnPropertyChanged(nameof(Bullets));
            OnPropertyChanged(nameof(Shields));
            OnPropertyChanged(nameof(Score));
            OnPropertyChanged(nameof(Lives));
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(IsPaused));
            OnPropertyChanged(nameof(IsGameOver));
            OnPropertyChanged(nameof(RedEnemy));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
