namespace Norbert.Modules.Maps
{
    public class Area
    {
        public string Name { get; private set; }
        public double MinLat { get; private set; }
        public double MaxLat { get; private set; }
        public double MinLon { get; private set; }
        public double MaxLon { get; private set; }

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