using System.Linq;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace NZazuFiddle
{
    // ReSharper disable once UnusedMember.Global
    public partial class FormDefinitionView
    {
        private CompletionWindow _completionWindow;

        public FormDefinitionView()
        {
            InitializeComponent();

            TextEditor.TextArea.TextEntering += OnTextEntering;
            TextEditor.TextArea.TextEntered += OnTextEntered;
            TextEditor.SyntaxFrom("NZazuFiddle.NZazu.xshd");
            TextEditor.TabMovesFocus();
        }

        private void OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != ":") return;

            // Open code completion after the user has pressed ':':
            _completionWindow = new CompletionWindow(TextEditor.TextArea);
            var data = _completionWindow.CompletionList.CompletionData;

            var word = TextEditor.GetTextBeforeCaret();
            data.AutoCompleteFor(word);

            if (!data.Any()) return;

            _completionWindow.Show();
            _completionWindow.Closed += delegate { _completionWindow = null; };
        }

        private void OnTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length <= 0 || _completionWindow == null) return;

            if (!char.IsLetterOrDigit(e.Text[0]))
                // Whenever a non-letter is typed while the completion window is open,
                // insert the currently selected element.
                _completionWindow.CompletionList.RequestInsertion(e);

            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.        
        }
    }
}