using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts;

namespace Sample
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class NZazuSampleViewModel_Should
    {
        [Test]
        public void Skip_Setting_FormData_If_Equal()
        {
            var sut = new NZazuSampleViewModel();
            sut.MonitorEvents();
            
            sut.FormData = null;
            sut.FormData.Should().BeNull();

            sut.FormData = new FormData(new Dictionary<string, string> { { "key", "value" } });
            sut.ShouldRaisePropertyChangeFor(model => model.FormData);
            sut.MonitorEvents();

            sut.FormData = new FormData(new Dictionary<string, string> { { "key", "value" } });
            sut.ShouldNotRaisePropertyChangeFor(model => model.FormData);
        }
    }
}