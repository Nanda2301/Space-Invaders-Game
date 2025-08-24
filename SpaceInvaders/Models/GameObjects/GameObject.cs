using System;
using Windows.Foundation;

namespace SpaceInvaders.Models.GameObjects
{
    public abstract class GameObject
    {
        public Rect Bounds { get; set; }
        public double Speed { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual void Update()
        {
            // Base implementation can be overridden
        }

        public virtual bool CheckCollision(GameObject other)
        {
            // Implementação manual de colisão
            return Bounds.Left < other.Bounds.Right &&
                   Bounds.Right > other.Bounds.Left &&
                   Bounds.Top < other.Bounds.Bottom &&
                   Bounds.Bottom > other.Bounds.Top;
        }
    }
}
