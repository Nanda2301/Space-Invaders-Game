using SpaceInvaders.Models.GameObjects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpaceInvaders.Models
{
    public class GameState : INotifyPropertyChanged
    {
        private Player _player;
        private ObservableCollection<Enemy> _enemies;
        private ObservableCollection<Bullet> _bullets;
        private ObservableCollection<Shield> _shields;
        private RedEnemy _redEnemy;
        private int _score;
        private int _lives;
        private int _level;
        private bool _isPaused;
        private bool _isGameOver;
        private bool _enemiesMovingRight;
        private double _enemySpeedModifier;

        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        public ObservableCollection<Enemy> Enemies
        {
            get => _enemies;
            set => SetProperty(ref _enemies, value);
        }

        public ObservableCollection<Bullet> Bullets
        {
            get => _bullets;
            set => SetProperty(ref _bullets, value);
        }

        public ObservableCollection<Shield> Shields
        {
            get => _shields;
            set => SetProperty(ref _shields, value);
        }

        public RedEnemy RedEnemy
        {
            get => _redEnemy;
            set => SetProperty(ref _redEnemy, value);
        }

        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        public int Lives
        {
            get => _lives;
            set => SetProperty(ref _lives, value);
        }

        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        public bool IsPaused
        {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }

        public bool IsGameOver
        {
            get => _isGameOver;
            set => SetProperty(ref _isGameOver, value);
        }

        public bool EnemiesMovingRight
        {
            get => _enemiesMovingRight;
            set => SetProperty(ref _enemiesMovingRight, value);
        }

        public double EnemySpeedModifier
        {
            get => _enemySpeedModifier;
            set => SetProperty(ref _enemySpeedModifier, value);
        }
        
        public double EnemyDescentAmount { get; set; } = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void InitializeGame()
        {
            throw new NotImplementedException();
        }
    }
}
