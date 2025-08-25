using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Bullet
    {
        public Rect Bounds { get; set; }
        public BulletType Type { get; set; }

        public Bullet(double x, double y, BulletType type)
        {
            Bounds = new Rect(x, y, 3, 10);
            Type = type;
        }

        public void Update()
        {
            double speed = Type == BulletType.Player ? -5 : 5;
            Bounds = new Rect(Bounds.Left, Bounds.Top + speed, Bounds.Width, Bounds.Height);
        }

        public bool CheckCollision(dynamic obj)
        {
            Rect a = Bounds;
            Rect b = obj.Bounds;

            bool intersects = a.Left < b.Right &&
                              a.Right > b.Left &&
                              a.Top < b.Bottom &&
                              a.Bottom > b.Top;

            return intersects;
        }
    }

    public enum BulletType
    {
        Player,
        Enemy
    }
}
