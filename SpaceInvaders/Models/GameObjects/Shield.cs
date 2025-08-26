using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Shield
    {
        public Rect Bounds { get; set; }
        public int Health { get; private set; } = 10; // Agora suporta 10 tiros

        public Shield()
        {
            Bounds = new Rect(0, 0, 60, 40); 
            Health = 10;
        }

        public Shield(double x, double y, double width = 60, double height = 40)
        {
            Bounds = new Rect(x, y, width, height);
            Health = 10;
        }

        public bool IsActive => Health > 0;

        public void TakeDamage()
        {
            if (Health > 0)
            {
                Health--;
                System.Diagnostics.Debug.WriteLine($"Shield took damage! Health now: {Health}");
            }
        }

        public bool CheckCollision(dynamic obj)
        {
            if (obj?.Bounds == null) return false;

            Rect a = Bounds;
            Rect b = obj.Bounds;

            return a.Left < b.Right &&
                   a.Right > b.Left &&
                   a.Top < b.Bottom &&
                   a.Bottom > b.Top;
        }
    }
}
