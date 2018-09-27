using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.JsonSerializer.RestSuggestor
{
    [TestFixtureFor(typeof(RestClient))]
    // ReSharper disable once InconsistentNaming
    internal class RestClient_Should
    {
        [Test]
        public void EnsureSuccessStatusCode()
        {
            var client = Substitute.For<IHttpClient>();
            var sut = new RestClient(client);
            var expected = new HttpResponseMessage(HttpStatusCode.OK);
            client.SendAsync(Arg.Any<HttpRequestMessage>()).Returns(Task.FromResult(expected));
            expected.Content.Should().BeNull();
            var actual = sut.Head().Result;

            expected.Content = new StringContent(string.Empty);
            actual = sut.Head().Result;

            expected.Content = new StringContent("not json");
            Func<Task> act = async () => await sut.Head();
            act.Should().Throw<JsonReaderException>();
            expected.StatusCode = HttpStatusCode.BadRequest;
            //actual = sut.Head().Result;
            act.Should().Throw<HttpRequestException>();
        }
    }
}