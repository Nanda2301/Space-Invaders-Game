using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Shield : GameObject
    {
        private const int MaxHits = 6;
        private int _hits;

        public bool IsActive => _hits < MaxHits;

        public Shield(double x, double y)
        {
            Bounds = new Rect(x, y, 60, 30);
            _hits = 0;
        }

        public void TakeDamage()
        {
            _hits++;
        }
    }
}
