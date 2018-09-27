using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NZazu.Contracts;
using NZazu.Contracts.Adapter;
using NZazu.Contracts.Suggest;
using NZazu.Fields.Libs;

namespace NZazu.Fields
{
    internal class NZazuAutocompleteField : NZazuField<string>
    {
        private readonly IProvideSuggestions _suggester;

        public NZazuAutocompleteField(FieldDefinition definition, Func<Type, object> serviceLocatorFunc)
            : base(definition, serviceLocatorFunc)
        {
            _suggester = (IProvideSuggestions) serviceLocatorFunc(typeof(IProvideSuggestions));
            DataConnection = Definition.Settings.Get("dataconnection") ?? string.Empty;
        }

        public override void SetValue(string value) { Value = value; }
        public override string GetValue() { return Value; }
        public override DependencyProperty ContentProperty => TextBox.TextProperty;
        public string DataConnection { get; }

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
            result.Loaded += (sender, args) => { AttachSuggestor(sender); };

            return result;
        }

        private void AttachSuggestor(object sender)
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