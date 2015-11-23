using System.Diagnostics.CodeAnalysis;

namespace Norbert.Modules.ChatLog
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class Config
    {
        public string Path { get; set; }
    }
}