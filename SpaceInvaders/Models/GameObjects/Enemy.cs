using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Enemy
    {
        public EnemyType Type { get; set; }
        public Rect Bounds { get; set; }
        public bool CanShoot { get; set; } = true;

        public Enemy(EnemyType type, double x, double y)
        {
            Type = type;
            Bounds = new Rect(x, y, 40, 30);
        }

        public void Move(double dx, double dy)
        {
            Bounds = new Rect(Bounds.Left + dx, Bounds.Top + dy, Bounds.Width, Bounds.Height);
        }

        public int PointValue => Type switch
        {
            EnemyType.Small => 10,
            EnemyType.Medium => 20,
            EnemyType.Large => 30,
            _ => 0
        };
    }

    public enum EnemyType
    {
        Small,
        Medium,
        Large
    }
}
