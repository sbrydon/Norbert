using System;

namespace Norbert.Modules.Common
{
    public interface IRandomiser
    {
        int NextInt(int max);
        DateTime NextDateTime(DateTime min);
    }
}