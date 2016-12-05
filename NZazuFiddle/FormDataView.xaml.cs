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
