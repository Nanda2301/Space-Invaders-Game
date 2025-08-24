using System;

namespace SpaceInvaders.Models
{
    public class HighScore : IComparable<HighScore>
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }

        public int CompareTo(HighScore other)
        {
            if (other == null) return 1;
            return other.Score.CompareTo(Score);
        }
    }
}
