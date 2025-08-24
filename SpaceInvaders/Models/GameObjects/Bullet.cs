using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Bullet : GameObject
    {
        public BulletType Type { get; set; }

        public Bullet(double x, double y, BulletType type)
        {
            Type = type;
            Bounds = new Rect(x, y, 3, 10);
            Speed = type == BulletType.Player ? 7 : 5;
        }

        public override void Update()
        {
            if (Type == BulletType.Player)
            {
                Bounds = new Rect(Bounds.Left, Bounds.Top - Speed, Bounds.Width, Bounds.Height);
            }
            else
            {
                Bounds = new Rect(Bounds.Left, Bounds.Top + Speed, Bounds.Width, Bounds.Height);
            }
        }
    }

    public enum BulletType
    {
        Player,
        Enemy
    }
}
