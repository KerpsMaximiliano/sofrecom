using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Common.Domains;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.Config;
using Sofco.Domain.Crm;
using Sofco.Framework.CrmServices;
using Sofco.Service.Http.Interfaces;

namespace Sofco.UnitTest.Framework.CrmServices
{
    [TestFixture]
    public class CrmInvoiceServiceTest
    {
        private CrmInvoiceService sut;

        private Mock<ICrmHttpClient> clientMock;
        private Mock<IOptions<CrmConfig>> crmOptionsMock;

        [SetUp]
        public void Setup()
        {
            clientMock = new Mock<ICrmHttpClient>();

            crmOptionsMock = new Mock<IOptions<CrmConfig>>();

            crmOptionsMock.SetupGet(s => s.Value).Returns(new CrmConfig { Url = "sofcoarUrl" });

            var loggerMock = new Mock< ILoggerWrapper<CrmInvoiceService>>();

            sut = new CrmInvoiceService(clientMock.Object, crmOptionsMock.Object, loggerMock.Object);
        }

        [Test]
        public void ShouldPassGetHitosToExpire()
        {
            clientMock.Setup(s => s.GetMany<CrmHito>(It.IsAny<string>())).Returns(new Result<List<CrmHito>>());

            var actual = sut.GetHitosToExpire(5);

            Assert.False(actual.HasErrors);

            clientMock.Verify(s => s.GetMany<CrmHito>(It.IsAny<string>()), Times.Once);
        }
    }
}
