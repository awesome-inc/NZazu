using NZazu.Contracts;

namespace NZazuFiddle
{
    public interface IShell
    {
        FormDefinition Definition { get; set; }
        FormData Data { get; set; }
    }
}