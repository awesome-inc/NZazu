using System;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;
using NZazu.Contracts;

#pragma warning disable 618
namespace NZazu.Fields
{
    [TestFixtureFor(typeof(NZazuImageViewerField))]
    // ReSharper disable once InconsistentNaming
    internal class NZazuImageViewerField_Should
    {
        [Test]
        [STAThread]
        [RequiresSTA]
        public void Be_Creatable()
        {
            var sut = new NZazuImageViewerField(new FieldDefinition { Key = "test", Type = "imageViewer" });

            sut.Key.Should().Be("test");
            sut.ValueControl.Should().BeAssignableTo<ContentControl>();
        }
    }
}