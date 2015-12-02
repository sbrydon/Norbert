namespace Norbert.Modules.Maps
{
    public class Area
    {
        public string Name { get; }
        public double MinLat { get; }
        public double MaxLat { get; }
        public double MinLon { get; }
        public double MaxLon { get; }

        public Area(string name, double minLat, double maxLat, double minLon, double maxLon)
        {
            Name = name;
            MinLat = minLat;
            MaxLat = maxLat;
            MinLon = minLon;
            MaxLon = maxLon;
        }
    }
}