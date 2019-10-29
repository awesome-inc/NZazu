using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;
using NZazu.Fields.Controls;

namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuErrorListField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class NZazuErrorListField_Should
    {
        [ExcludeFromCodeCoverage]
        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            if (type == typeof(INZazuWpfView)) return Substitute.For<INZazuWpfView>();
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        [STAThread]
        public void Create_ValueControl_Matching_Description()
        {
            var sut = new NZazuErrorListField(new FieldDefinition {Key = "key", Description = "superhero is alive"},
                ServiceLocator);

            var label = (ErrorPanel) sut.ValueControl;
            label.Should().NotBeNull();
            label.Errors.Should().BeEmpty();

            sut.Dispose();
        }

        [Test]
        public void Create_ValueControl_On_Empty_Description()
        {
            var sut = new NZazuErrorListField(new FieldDefinition {Key = "key"}, ServiceLocator);
            sut.Definition.Description.Should().BeNullOrWhiteSpace();
            var label = (ErrorPanel) sut.ValueControl;
            label.Should().NotBeNull();

            sut.Dispose();
        }

        [Test]
        public void Return_null_StringValue_and_not_set_StringValue()
        {
            var sut = new NZazuErrorListField(new FieldDefinition {Key = "key"}, ServiceLocator);
            sut.GetValue().Should().BeNull();
            sut.SetValue("foobar");
            sut.GetValue().Should().BeNull();

            sut.Dispose();
        }

        [Test]
        public void Not_be_Editable()
        {
            var sut = new NZazuErrorListField(new FieldDefinition {Key = "key"}, ServiceLocator);
            sut.IsEditable.Should().BeFalse();

            sut.Dispose();
        }
    }
}