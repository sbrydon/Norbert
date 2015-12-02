using System;
using Norbert.Modules.Common.Helpers;

namespace Norbert.Modules.Maps
{
    public static class RandomCoords
    {
        private static readonly Random Random = new Random();

        private static readonly Area[] Areas =
        {
            new Area("UK", 50.3, 57, -5.7, 1.2),
            new Area("US", 26, 48, -123, -67)
        };

        public static Area NextArea()
        {
            return Areas[Random.Next(0, Areas.Length)];
        }

        public static double[] NextCoords(Area area)
        {
            return new[]
            {
                Random.NextDouble(area.MinLat, area.MaxLat),
                Random.NextDouble(area.MinLon, area.MaxLon)
            };
        }
    }
}