using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;

namespace NZazuFiddle
{
    public sealed class AvalonEditBehaviour : Behavior<TextEditor>
    {
        public static readonly DependencyProperty GiveMeTheTextProperty =
            DependencyProperty.Register("GiveMeTheText", typeof(string), typeof(AvalonEditBehaviour),
                new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    PropertyChangedCallback));

        public string GiveMeTheText
        {
            get => (string) GetValue(GiveMeTheTextProperty);
            set => SetValue(GiveMeTheTextProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                //AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
                AssociatedObject.LostFocus += AssociatedObjectOnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                //AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
                AssociatedObject.LostFocus -= AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor == null) return;
            if (textEditor.Document != null)
                GiveMeTheText = textEditor.Document.Text;
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = (AvalonEditBehaviour) dependencyObject;
            if (behavior.AssociatedObject == null) return;
            var editor = behavior.AssociatedObject;
            if (editor.Document == null) return;

            var caretOffset = editor.CaretOffset;

            editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();

            try
            {
                editor.CaretOffset = caretOffset;
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }
    }
}