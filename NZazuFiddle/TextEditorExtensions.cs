using System;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace NZazuFiddle
{
    internal static class TextEditorExtensions
    {
        public static void TabMovesFocus(this TextEditor textEditor)
        {
            textEditor.TextArea.PreviewKeyDown += (s, e) =>
            {
                if (e.Key != Key.Tab) return;
                e.Handled = true;
                textEditor.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => // input priority is always needed when changing focus
                    textEditor.TextArea.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next))));
            };
        }

        public static void SyntaxFrom(this TextEditor textEditor, string resourceName)
        {
            textEditor.SyntaxHighlighting = LoadSyntaxHighlighting(resourceName);
        }

        public static string GetTextBeforeCaret(this TextEditor editor, int words = 2)
        {
            if (editor == null)
                throw new ArgumentNullException(nameof(editor));
            int endOffset = editor.CaretOffset;
            int startOffset = FindPrevWordStart(editor.Document, endOffset, words);
            if (startOffset < 0)
                return string.Empty;
            else
                return editor.Document.GetText(startOffset, endOffset - startOffset);
        }

        private static int FindPrevWordStart(this ITextSource textSource, int offset, int words = 1)
        {
            var position = offset;
            for (int word = 0; word < words; word++)
                position = TextUtilities.GetNextCaretPosition(textSource, position, LogicalDirection.Backward, CaretPositioningMode.WordStart);
            return position;
        }

        private static IHighlightingDefinition LoadSyntaxHighlighting(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new ArgumentException("Resource not found", resourceName);
                using (var reader = new XmlTextReader(stream))
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

    }
}