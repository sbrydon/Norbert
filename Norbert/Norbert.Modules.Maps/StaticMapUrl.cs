using System;

namespace Norbert.Modules.Maps
{
    public class StaticMapUrl
    {
        private const string BaseUrl = "https://maps.googleapis.com/maps/api/staticmap";

        private const int Zoom = 6;
        private const string Size = "640x640";
        private const int Scale = 2;

        public string Formatted { get; }

        public StaticMapUrl(string place, string apiKey)
        {
            place = Uri.EscapeDataString(place);
            var mapQ = $"markers={place}&zoom={Zoom}&size={Size}&scale={Scale}&key={apiKey}";

            Formatted = $"{BaseUrl}?{mapQ}";
        }
    }
}