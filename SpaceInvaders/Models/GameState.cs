using System.Collections.Generic;
using System.Collections.ObjectModel;
using SpaceInvaders.Models.GameObjects;

namespace SpaceInvaders.Models
{
    public class GameState
    {
        public Player Player { get; set; } = new Player();
        public ObservableCollection<Enemy> Enemies { get; set; } = new ObservableCollection<Enemy>();
        public ObservableCollection<Bullet> Bullets { get; set; } = new ObservableCollection<Bullet>();
        public ObservableCollection<Shield> Shields { get; set; } = new ObservableCollection<Shield>();
        public RedEnemy RedEnemy { get; set; }
        public bool IsGameOver { get; set; }
        public bool IsPaused { get; set; }
        public int Level { get; set; } = 1;
        public double EnemySpeedModifier { get; set; } = 1.0;
        public bool EnemiesMovingRight { get; set; } = true;
        public double EnemyDescentAmount { get; set; }
        
        public void InitializeGame()
        {
            Player = new Player();
            Enemies.Clear();
            Bullets.Clear();
            Shields.Clear();
            RedEnemy = null;
            IsGameOver = false;
            IsPaused = false;
            Level = 1;
            EnemySpeedModifier = 1.0;
            EnemiesMovingRight = true;
            EnemyDescentAmount = 0;

            // Create enemies
            for (int row = 0; row < 5; row++)
            {
                EnemyType type = row == 0 ? EnemyType.Small : 
                    row < 3 ? EnemyType.Medium : 
                    EnemyType.Large;
                
                for (int col = 0; col < 10; col++)
                {
                    Enemies.Add(new Enemy(type, 100 + col * 50, 50 + row * 40));
                }
            }

            // Create shields
            for (int i = 0; i < 4; i++)
            {
                Shields.Add(new Shield(100 + i * 200, 400));
            }
        }
    }
}
