using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Player : GameObject
    {
        public int Lives { get; set; } = 3;
        public int Score { get; set; }
        public bool CanShoot { get; set; } = true;

        public Player()
        {
            Bounds = new Rect(350, 500, 50, 30);
            Speed = 5;
        }

        public void MoveLeft()
        {
            if (Bounds.Left > 0)
            {
                Bounds = new Rect(Bounds.Left - Speed, Bounds.Top, Bounds.Width, Bounds.Height);
            }
        }

        public void MoveRight(double screenWidth)
        {
            if (Bounds.Right < screenWidth)
            {
                Bounds = new Rect(Bounds.Left + Speed, Bounds.Top, Bounds.Width, Bounds.Height);
            }
        }
    }
}
