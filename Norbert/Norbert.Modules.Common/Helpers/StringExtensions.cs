namespace Norbert.Modules.Common.Helpers
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int length, string pad = "..")
        {
            if (value.Length <= length)
                return value;

            if (length <= pad.Length)
                return pad.Substring(0, length);

            return value.Substring(0, length - pad.Length).TrimEnd() + pad;
        }
    }
}