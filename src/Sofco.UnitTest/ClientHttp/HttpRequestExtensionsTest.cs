using System;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using Sofco.Service.Http.Extensions;

namespace Sofco.UnitTest.ClientHttp
{
    [TestFixture]
    public class HttpRequestExtensionsTest
    {
        [Test]
        public void ShouldPassSetTimeout()
        {
            var request = new HttpRequestMessage();

            var timeOut = new TimeSpan(0, 0, 10);

            HttpRequestExtensions.SetTimeout(request, timeOut);

            var actual = request.Properties.Keys.FirstOrDefault(s => s == "RequestTimeout");

            Assert.IsNotNull(actual);
        }
    }
}
