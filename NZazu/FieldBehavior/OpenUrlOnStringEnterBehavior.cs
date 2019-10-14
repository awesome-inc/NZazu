using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace NZazu.FieldBehavior
{
    public class OpenUrlOnStringEnterBehavior : INZazuWpfFieldBehavior
    {
        private Control _control;
        private KeyEventHandler _handler;
        private Regex _regex = new Regex("^(http|https)://");

        // ReSharper disable once UnusedMember.Local
        public string UrlPattern
        {
            get => _regex.ToString();
            set => _regex = new Regex(value);
        }

        public void AttachTo(INZazuWpfField field, INZazuWpfView view)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (view == null)
                throw new ArgumentNullException(nameof(view),
                    "this value should not be null and for testing we check it here");
            var valueControl = field.ValueControl;

            _control = valueControl;

            _handler = HandleKeyUp;
            _control.KeyUp += _handler;
        }

        public void Detach()
        {
            if (_control == null) return;

            _control.KeyUp -= _handler;
            _handler = null;
            _control = null;
        }

        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (!(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Return)) return;

            var link = GetLinkAtPosition(((TextBox) _control).Text, ((TextBox) _control).SelectionStart);
            if (link == null) return;

            Process.Start(link);
        }

        internal string GetLinkAtPosition(string text, int position)
        {
            var spaceBefore = text.Substring(0, position).LastIndexOf(" ", StringComparison.Ordinal);
            if (spaceBefore < 0) spaceBefore = 0;
            else spaceBefore += 1;

            var spaceAfter = text.Substring(position).IndexOf(" ", StringComparison.Ordinal);
            if (spaceAfter < 0) spaceAfter = text.Length;
            else spaceAfter += position;

            var length = spaceAfter - spaceBefore;
            if (length <= 10) return null;
            var textUnderCursor = text.Substring(spaceBefore, length);
            return _regex.IsMatch(textUnderCursor) ? textUnderCursor : null;
        }
    }
}