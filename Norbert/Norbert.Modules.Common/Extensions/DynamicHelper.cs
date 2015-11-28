using Microsoft.CSharp.RuntimeBinder;

namespace Norbert.Modules.Common.Extensions
{
    public static class DynamicHelper
    {
        public delegate string GetValueDelegate();

        public static bool HasProperty(GetValueDelegate getValue)
        {
            try
            {
                getValue();
                return true;
            }
            catch (RuntimeBinderException)
            {
                return false;
            }
        }
    }
}