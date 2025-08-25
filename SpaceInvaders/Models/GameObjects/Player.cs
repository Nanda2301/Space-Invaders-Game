using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Player
    {
        public Rect Bounds { get; set; }
        public bool CanShoot { get; set; } = true;
        public int Score { get; private set; }
        public int Lives { get; private set; } = 3;

        public void MoveLeft()
        {
            if (Bounds.Left > 0)
                Bounds = new Rect(Bounds.Left - 5, Bounds.Top, Bounds.Width, Bounds.Height);
        }

        public void MoveRight(double canvasWidth)
        {
            if (Bounds.Right < canvasWidth)
                Bounds = new Rect(Bounds.Left + 5, Bounds.Top, Bounds.Width, Bounds.Height);
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        public void GainLife()
        {
            if (Lives < 6) Lives++;
        }

        public void LoseLife()
        {
            if (Lives > 0) Lives--;
        }
    }
}
