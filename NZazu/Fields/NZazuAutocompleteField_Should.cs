using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Adapter;
using NZazu.Contracts.Suggest;
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuAutocompleteField))]
    // ReSharper disable once InconsistentNaming
    internal class NZazuAutocompleteField_Should
    {
        private object ServiceLocator(Type type, IProvideSuggestions suggestor)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            if (type == typeof(IProvideSuggestions)) return suggestor;
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Be_Creatable()
        {
            var sug = Substitute.For<IProvideSuggestions>();
            var sut = new NZazuAutocompleteField(
                new FieldDefinition { Key = "test", Settings = { { "dataconnection", "datakey" } } },
                x => ServiceLocator(x, sug));

            sut.Should().NotBeNull();
            sut.ValueControl.Should().BeOfType<TextBox>();
            sut.Value.Should().BeNull();

            sut.DataConnection.Should().Be("datakey");
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Pass_Through_StoreKey()
        {
            var sug = Substitute.For<IProvideSuggestions>();
            var sut = new NZazuAutocompleteField(
                new FieldDefinition { Key = "test", Settings = { { "dataconnection", "datakey" } } },
                x => ServiceLocator(x, sug));

            var box = (TextBox)sut.ValueControl;


            RaiseLoadedEvent(box);
            box.Text = "ant";

            Assert.Inconclusive("wont work because no app therefore the manager is not initialized");
            sug.Received(1).For(Arg.Any<string>(), Arg.Any<string>());
            sug.Received(1).For("ant", "datakey");
        }

        private static void RaiseLoadedEvent(FrameworkElement element)
        {
            var eventMethod = typeof(FrameworkElement).GetMethod("OnLoaded",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var args = new RoutedEventArgs(FrameworkElement.LoadedEvent);

            if (eventMethod != null)
                eventMethod.Invoke(element, new object[] {args});
        }
    }
}