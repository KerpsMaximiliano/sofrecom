using System;
using Moq;
using NUnit.Framework;
using Sofco.Common.Domains;
using Sofco.Domain.Nolaborables;
using Sofco.Service.Http;
using Sofco.Service.Http.Interfaces;

namespace Sofco.UnitTest.ClientHttp
{
    [TestFixture]
    public class NolaborablesHttpClientTest
    {
        private Mock<IBaseHttpClient> baseHttpClientMock;

        private INolaborablesHttpClient sut;

        [SetUp]
        public void SetUp()
        {
            baseHttpClientMock = new Mock<IBaseHttpClient>();

            sut = new NolaborablesHttpClient(baseHttpClientMock.Object);
        }

        [Test]
        public void ShouldPassGet()
        {
            baseHttpClientMock
                .Setup(s => s.Get<Feriado>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>()
                )).Returns(new Result<Feriado>(new Feriado()));

            var actual = sut.Get<Feriado>(string.Empty);

            Assert.False(actual.HasErrors);

            baseHttpClientMock.Verify(s => s.Get<Feriado>(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TimeSpan>()), Times.Once);
        }

        [Test]
        public void ShouldFailGet()
        {
            var result = new Result<Feriado>(new Feriado());

            result.AddErrorFluent("Error1");

            baseHttpClientMock
                .Setup(s => s.Get<Feriado>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>()
                )).Returns(result);

            Assert.Throws<Exception>(() => sut.Get<Feriado>(string.Empty));

            baseHttpClientMock.Verify(s => s.Get<Feriado>(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}
