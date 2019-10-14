using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;
using NZazu.Contracts.Suggest;
using NZazu.Fields.Libs;

namespace NZazu.Fields
{
    internal class NZazuAutocompleteField : NZazuField<string>
    {
        private readonly IProvideSuggestions _suggester;
        private bool _suggesterAttached;

        public NZazuAutocompleteField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _suggester = (IProvideSuggestions) serviceLocatorFunc(typeof(IProvideSuggestions));
            DataConnection = Definition.Settings.Get("dataconnection") ?? string.Empty;
        }

        public override DependencyProperty ContentProperty => TextBox.TextProperty;
        public string DataConnection { get; }

        public override void SetValue(string value)
        {
            Value = value;
        }

        public override string GetValue()
        {
            return Value;
        }

        protected internal override Binding DecorateBinding(Binding binding)
        {
            binding.TargetNullValue = null;
            return base.DecorateBinding(binding);
        }

        protected override Control CreateValueControl()
        {
            var result = new TextBox();

            // we have to do this on load because some wpf stuff does not work if no parent is set
            // i.e. some popup magic on window
            result.Loaded += (sender, args) =>
            {
                if (_suggesterAttached) return;
                _suggesterAttached = true;

                AttachSuggester(sender);
            };

            return result;
        }

        private void AttachSuggester(object sender)
        {
            // no suggester, no suggestions ;)
            if (_suggester == null) return;

            var tb = (TextBox) sender;

            // ReSharper disable once ObjectCreationAsStatement
            var manager = new AutoCompleteManager
            {
                DataProvider = _suggester,
                Asynchronous = true,
                DataConnection = DataConnection
            };
            manager.AttachTextBox(tb);
        }
    }
}