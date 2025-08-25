using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class RedEnemy
    {
        public Rect Bounds { get; set; }
        public bool MovingRight { get; set; }

        public RedEnemy()
        {
            Bounds = new Rect(0, 20, 50, 30); // posição inicial exemplo
            MovingRight = true;
        }

        public void Update()
        {
            double speed = MovingRight ? 3 : -3;
            Bounds = new Rect(Bounds.Left + speed, Bounds.Top, Bounds.Width, Bounds.Height);
        }

        public int PointValue => 100;
    }
}
