using System;

namespace Norbert.Modules.Tumblr
{
    public interface IRandomiser
    {
        int NextInt(int max);
        DateTime NextDateTime(DateTime min);
    }
}