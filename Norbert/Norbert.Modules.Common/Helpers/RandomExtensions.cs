using System;

namespace Norbert.Modules.Common.Helpers
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random random, double minValue, double maxValue,
            int digits = 4)
        {
            var value = random.NextDouble() * (maxValue - minValue) + minValue;
            return Math.Round(value, digits);
        }
    }
}