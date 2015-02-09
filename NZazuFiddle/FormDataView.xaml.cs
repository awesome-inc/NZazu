using System;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace NZazuFiddle
{
    public partial class FormDataView
    {
        public FormDataView()
        {
            InitializeComponent();

            TextEditor.SyntaxFrom("NZazuFiddle.NZazu.xshd");
            TextEditor.TabMovesFocus();
        }
    }
}
