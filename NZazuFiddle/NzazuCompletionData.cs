using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace NZazuFiddle
{
    public class NzazuCompletionData : ICompletionData
    {
        public string Replacement { get; set; }
        public string Text { get; set; }

        public object Description { get; set; }
        public ImageSource Image => null;

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content => Text;
        public double Priority { get; set; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Replacement ?? Text);
        }
    }
}