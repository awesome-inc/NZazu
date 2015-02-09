using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace NZazuFiddle
{
    public class NzazuCompletionData : ICompletionData
    {
        public string Text { get; set; }
        public string Replacement { get; set; }

        public object Description { get; set; }
        public System.Windows.Media.ImageSource Image { get { return null; } }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content { get { return Text; } }
        public double Priority { get; set; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Replacement ?? Text);
        }
    }
}