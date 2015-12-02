namespace Norbert.Modules.Maps
{
    public class Location
    {
        public double Lat { get; }
        public double Lon { get; }
        public string Address { get; }

        public Location(double lat, double lon, string address)
        {
            Lat = lat;
            Lon = lon;
            Address = address;
        }
    }
}