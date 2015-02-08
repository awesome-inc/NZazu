using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace NZazuFiddle
{
    public partial class FormDefinitionView
    {
        private CompletionWindow _completionWindow;

        public FormDefinitionView()
        {
            InitializeComponent();

            TextEditor.TextArea.TextEntering += OnTextEntering;
            TextEditor.TextArea.TextEntered += OnTextEntered;
        }

        private void OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != ".") return;

            // Open code completion after the user has pressed dot:
            _completionWindow = new CompletionWindow(TextEditor.TextArea);
            var data = _completionWindow.CompletionList.CompletionData;
            data.Add(new MyCompletionData("Item1"));
            data.Add(new MyCompletionData("Item2"));
            data.Add(new MyCompletionData("Item3"));
            _completionWindow.Show();
            _completionWindow.Closed += delegate
            {
                _completionWindow = null;
            };
        }

        private void OnTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.        
        }
    }
}
