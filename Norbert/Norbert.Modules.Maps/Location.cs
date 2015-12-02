namespace Norbert.Modules.Maps
{
    public class Location
    {
        public double Lat { get; private set; }
        public double Lon { get; private set; }
        public string Address { get; private set; }

        public Location(double lat, double lon, string address)
        {
            Lat = lat;
            Lon = lon;
            Address = address;
        }
    }
}