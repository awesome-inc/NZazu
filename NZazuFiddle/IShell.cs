namespace NZazuFiddle
{
    public interface IShell
    {
        IFormDefinitionViewModel Definition { get; }
        IFormDataViewModel Data { get; }
        IPreviewViewModel Preview { get; }

    }
}