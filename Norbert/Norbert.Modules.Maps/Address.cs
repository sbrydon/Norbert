using System.Collections.Generic;
using System.Linq;

namespace Norbert.Modules.Maps
{
    public class Address
    {
        private readonly string _level1;
        private readonly string _level2;
        private readonly string _level3;

        public string Formatted => $"{_level1}, {_level2}, {_level3}";

        private Address(dynamic level1, dynamic level2, dynamic level3)
        {
            _level1 = level1;
            _level2 = level2;
            _level3 = level3;
        }

        public static Address FromComponents(dynamic components)
        {
            var political = ((IEnumerable<dynamic>) components).Where(
                c => ((IEnumerable<dynamic>) c.types).Any(t => t.ToString() == "political")).ToArray();

            if (political.Length < 3)
                return null;

            if (political.Length > 3)
                political = political.Skip(political.Length - 3).ToArray();

            return new Address(political[0].long_name, political[1].long_name, political[2].long_name);
        }
    }
}