using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class RedEnemy : GameObject
    {
        public int PointValue { get; set; } = 100;
        public bool MovingRight { get; set; } = true;

        public RedEnemy()
        {
            Bounds = new Rect(0, 50, 40, 30);
            Speed = 3;
        }

        public override void Update()
        {
            if (MovingRight)
            {
                Bounds = new Rect(Bounds.Left + Speed, Bounds.Top, Bounds.Width, Bounds.Height);
            }
            else
            {
                Bounds = new Rect(Bounds.Left - Speed, Bounds.Top, Bounds.Width, Bounds.Height);
            }
        }
    }
}
