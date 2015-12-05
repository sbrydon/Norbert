using System;

namespace Norbert.Modules.Tumblr
{
    public class Randomiser : IRandomiser
    {
        private static readonly Random Random = new Random();

        public int NextInt(int max)
        {
            return Random.Next(0, max);
        }

        public DateTime NextDateTime(DateTime min)
        {
            var range = (DateTime.Today - min).Days;
            return min.AddDays(Random.Next(range));
        }
    }
}