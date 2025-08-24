using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Shield : GameObject
    {
        public int Health { get; set; } = 4;

        public Shield(double x, double y)
        {
            Bounds = new Rect(x, y, 60, 40);
        }

        public void TakeDamage()
        {
            Health--;
            if (Health <= 0)
            {
                IsActive = false;
            }
        }
    }
}
