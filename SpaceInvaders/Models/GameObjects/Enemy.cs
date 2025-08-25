using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public class Enemy : GameObject
    {
        public EnemyType Type { get; set; }
        public int PointValue { get; set; }
        public bool CanShoot => Type == EnemyType.Medium;

        public Enemy(EnemyType type, double x, double y)
        {
            Type = type;

            switch (type)
            {
                case EnemyType.Small:
                    Bounds = new Rect(x, y, 30, 30);
                    PointValue = 40;
                    break;
                case EnemyType.Medium:
                    Bounds = new Rect(x, y, 30, 30);
                    PointValue = 20;
                    break;
                case EnemyType.Large:
                    Bounds = new Rect(x, y, 30, 30);
                    PointValue = 10;
                    break;
            }

            Speed = 1; // Certifique-se de que GameObject tenha Speed
        }

        public void Move(double dx, double dy)
        {
            Bounds = new Rect(Bounds.Left + dx, Bounds.Top + dy, Bounds.Width, Bounds.Height);
        }
    }

    public enum EnemyType
    {
        Small,   // 30 pontos - linha superior
        Medium,  // 40 pontos - linhas do meio (ATIRAM)
        Large    // 10 pontos - linhas inferiores
    }
}
