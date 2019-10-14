using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Contracts.Suggest;
using NZazu.Extensions;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuAutocompleteField))]
    // ReSharper disable once InconsistentNaming
    internal class NZazuAutocompleteField_Should
    {
        [ExcludeFromCodeCoverage]
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
                new FieldDefinition {Key = "test", Settings = {{"dataconnection", "datakey"}}},
                x => ServiceLocator(x, sug));

            sut.Should().NotBeNull();
            sut.ValueControl.Should().BeOfType<TextBox>();
            sut.Value.Should().BeNull();

            sut.DataConnection.Should().Be("datakey");
        }
    }
}