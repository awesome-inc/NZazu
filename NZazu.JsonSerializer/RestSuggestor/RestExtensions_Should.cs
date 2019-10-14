using System;
using System.Net.Http;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;

namespace NZazu.JsonSerializer.RestSuggestor
{
    [TestFixtureFor(typeof(RestExtensions))]
    // ReSharper disable once InconsistentNaming
    internal class RestExtensions_Should
    {
        [Test]
        public void Wrap_Head()
        {
            var client = Substitute.For<IRestClient>();

            var uri = Guid.NewGuid().ToString();
            var result = client.Head(uri).Result;

            client.Received(1).Request(HttpMethod.Head, uri);
        }

        [Test]
        public void Wrap_Get()
        {
            var client = Substitute.For<IRestClient>();

            var uri = Guid.NewGuid().ToString();
            var result = client.Get(uri).Result;

            client.Received(1).Request(HttpMethod.Get, uri);
        }

        [Test]
        public void Wrap_Post()
        {
            var client = Substitute.For<IRestClient>();

            var uri = Guid.NewGuid().ToString();
            var result = client.Post(uri).Result;

            client.Received(1).Request(HttpMethod.Post, uri);
        }

        [Test]
        public void Wrap_Put()
        {
            var client = Substitute.For<IRestClient>();

            var uri = Guid.NewGuid().ToString();
            var result = client.Put(uri).Result;

            client.Received(1).Request(HttpMethod.Put, uri);
        }

        [Test]
        public void Wrap_Delete()
        {
            var client = Substitute.For<IRestClient>();

            var uri = Guid.NewGuid().ToString();
            var result = client.Delete(uri).Result;

            client.Received(1).Request(HttpMethod.Delete, uri);
        }
    }
}