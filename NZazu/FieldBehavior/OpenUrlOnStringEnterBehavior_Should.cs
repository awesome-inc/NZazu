using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

// ReSharper disable StringLiteralTypo

namespace NZazu.FieldBehavior
{
    [TestFixtureFor(typeof(OpenUrlOnStringEnterBehavior))]
    // ReSharper disable once InconsistentNaming
    internal class OpenUrlOnStringEnterBehavior_Should
    {
        [Test]
        [TestCase("asdfklj asöfkdljsa fdöakfjl saöljad fösadfa", 6, null)]
        [TestCase("asdfklj asöfkdljsa http://google.de fdöakfjl saöljad fösadfa", 3, null)]
        [TestCase("asdfklj asöfkdljsa ftp://google.de fdöakfjl saöljad fösadfa", 25, null)]
        [TestCase("asdfklj asöfkdljsa http://google.de fdöakfjl saöljad fösadfa", 25, "http://google.de")]
        [TestCase("http://google.de asdfklj asöfkdljsa fdöakfjl saöljad fösadfa", 4, "http://google.de")]
        [TestCase("http://google.de", 0, "http://google.de")]
        [TestCase("asdfklj asöfkdljsa fdöakfjl saöljad fösadfa http://google.de", 56, "http://google.de")]
        public void Extract_Url(string text, int position, string expected)
        {
            var sut = new OpenUrlOnStringEnterBehavior();
            var url = sut.GetLinkAtPosition(text, position);
            url.Should().Be(expected);
        }
    }
}