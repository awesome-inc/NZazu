using System;
using Caliburn.Micro;

namespace NZazuFiddle
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FiddleViewModel : Screen, IFiddle
    {
        public IFormDefinitionViewModel Definition { get; }
        public IFormDataViewModel Data { get; }
        public IPreviewViewModel Preview { get; }

        public FiddleViewModel(IFormDefinitionViewModel definition,
            IFormDataViewModel data,
            IPreviewViewModel preview)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Preview = preview ?? throw new ArgumentNullException(nameof(preview));
        }
    }
}