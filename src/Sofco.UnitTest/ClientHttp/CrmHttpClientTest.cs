using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Sofco.Common.Domains;
using Sofco.Service.Http;
using Sofco.Service.Http.Interfaces;

namespace Sofco.UnitTest.ClientHttp
{
    [TestFixture]
    public class CrmHttpClientTest
    {
        private Mock<IBaseHttpClient> baseHttpClientMock;

        private ICrmHttpClient sut;

        [SetUp]
        public void SetUp()
        {
            baseHttpClientMock = new Mock<IBaseHttpClient>();

            sut = new CrmHttpClient(baseHttpClientMock.Object);
        }

        [Test]
        public void ShouldPassGetMany()
        {
            baseHttpClientMock.Setup(s => s.GetMany<string>(string.Empty, null))
                .Returns(new Result<List<string>>());

            var actual = sut.GetMany<string>(string.Empty);

            Assert.False(actual.HasErrors);

            baseHttpClientMock.Verify(s => s.GetMany<string>(It.IsAny<string>(), null), Times.Once);
        }
    }
}
