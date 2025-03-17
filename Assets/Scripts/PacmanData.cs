using System;

namespace Assets.Scripts
{
    [Serializable]
    public class PacmanData
    {
        public int Score;
        public int Lives = 3;
        public int Level = 1;

        public override string ToString()
        {
            return $"LIVES: {Lives}\n\nSCORE: {Score}\n\nLEVEL: {Level}";
        }
    }
}