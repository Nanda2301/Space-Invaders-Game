using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Player
    {
        public Rect Bounds { get; set; }
        public bool CanShoot { get; set; } = true;
        public int Score { get; private set; }
        public int Lives { get; private set; } = 3;

        public Player()
        {
            // Inicializa no centro inferior da área de 800x600
            Bounds = new Rect(375, 550, 50, 30);
            Score = 0;
            Lives = 3;
            CanShoot = true;
        }

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

        // Novo método para permitir que o Score seja atualizado externamente
        public void SetScore(int newScore)
        {
            Score = newScore;
        }

        public void GainLife()
        {
            if (Lives < 6) Lives++;
        }

        public void LoseLife()
        {
            if (Lives > 0) Lives--;
        }

        public void ResetPosition(double canvasWidth, double canvasHeight)
        {
            // Reset para o centro inferior
            double centerX = (canvasWidth - Bounds.Width) / 2;
            double bottomY = canvasHeight - Bounds.Height - 20; // 20 pixels da borda inferior
            Bounds = new Rect(centerX, bottomY, Bounds.Width, Bounds.Height);
        }
    }
}
