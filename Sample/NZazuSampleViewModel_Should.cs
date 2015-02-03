using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Sample
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class NZazuSampleViewModel_Should
    {
        [Test]
        public void Skip_FormData_If_Sequence_Equal()
        {
            var sut = new NZazuSampleViewModel();
            sut.MonitorEvents();
            
            sut.FormData = null;
            sut.FormData.Should().BeEmpty();

            sut.FormData = new Dictionary<string, string> { { "key", "value" } };
            sut.ShouldRaisePropertyChangeFor(model => model.FormData);
            sut.MonitorEvents();

            sut.FormData = new Dictionary<string, string> { { "key", "value" } };
            sut.ShouldNotRaisePropertyChangeFor(model => model.FormData);
        }
    }
}