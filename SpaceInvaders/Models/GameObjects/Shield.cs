using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Shield
    {
        public Rect Bounds { get; set; }
        public int Health { get; private set; } = 3;

        public bool IsActive => Health > 0;

        public void TakeDamage()
        {
            if (Health > 0) Health--;
        }
    }
}
