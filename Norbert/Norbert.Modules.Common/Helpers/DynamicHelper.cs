using System;
using Microsoft.CSharp.RuntimeBinder;

namespace Norbert.Modules.Common.Helpers
{
    public static class DynamicHelper
    {
        public static bool HasProperty(Func<string> getValue)
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