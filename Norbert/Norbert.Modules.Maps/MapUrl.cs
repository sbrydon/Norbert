using System;
using Norbert.Modules.Common.Helpers;

namespace Norbert.Modules.Maps
{
    public class MapUrl
    {
        private const string MapUrlBase = "https://maps.googleapis.com/maps/api/staticmap";
        private const string IconUrlBase = "http://chart.apis.google.com/chart";

        private const int Zoom = 5;
        private const string Size = "640x640";

        private readonly double _lat;
        private readonly double _lon;
        private readonly string _label;
        private readonly string _apiKey;

        public string Formatted
        {
            get
            {
                var iconOpts = $"|bbT|{_label}|FFFFFF|000000";
                iconOpts = Uri.EscapeDataString(iconOpts);

                var iconQ = $"?chst=d_bubble_icon_text_small&chld=glyphish_planet{iconOpts}";
                iconQ = Uri.EscapeDataString(iconQ);

                var iconUrl = $"{IconUrlBase}{iconQ}";
                var mapQ = $"markers=icon:{iconUrl}|{_lat},{_lon}&zoom={Zoom}&size={Size}&key={_apiKey}";

                return $"{MapUrlBase}?{mapQ}";
            }
        }

        public MapUrl(double lat, double lon, string label, string apiKey)
        {
            _lat = lat;
            _lon = lon;
            _label = label.Truncate(11);
            _apiKey = apiKey;
        }
    }
}