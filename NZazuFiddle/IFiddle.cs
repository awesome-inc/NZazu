namespace NZazuFiddle
{
    public interface IFiddle
    {
        IFormDefinitionViewModel Definition { get; }
        IFormDataViewModel Data { get; }
        IPreviewViewModel Preview { get; }
    }
}