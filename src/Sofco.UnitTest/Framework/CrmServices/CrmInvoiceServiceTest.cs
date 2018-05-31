using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Common.Domains;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Logger;
using Sofco.Domain.Crm;
using Sofco.Framework.CrmServices;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
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

            clientMock.Setup(s => s.Post<string>(It.IsAny<string>(), It.IsAny<StringContent>())).Returns(new Result<string>("1"));

            crmOptionsMock = new Mock<IOptions<CrmConfig>>();

            crmOptionsMock.SetupGet(s => s.Value).Returns(new CrmConfig { Url = "sofcoarUrl" });

            var loggerMock = new Mock<ILogMailer<CrmInvoiceService>>();

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

        [Test]
        public void ShouldPassCreateHitoBySolfac()
        {
            var solfac = GetSolfacData();

            solfac.DocumentTypeId = SolfacDocumentType.CreditNoteA;

            var actual = sut.CreateHitoBySolfac(solfac);

            Assert.False(actual.HasErrors);

            clientMock.Verify(s => s.Post<string>(It.IsAny<string>(), It.IsAny<StringContent>()), Times.Once);
        }

        private Solfac GetSolfacData()
        {
            var solfac = new Solfac
            {
                DocumentTypeId = 1,
                BuenosAiresPercentage = 100,
                PaymentTermId = 1,
                PurchaseOrderId = 1,
                ImputationNumber1 = "1",
                UserApplicantId = 1,
                TotalAmount = 100,
                Hitos = new List<Hito>
                {
                    new Hito
                    {
                        SolfacId = 1,
                        Total = 100,
                        Description = "Hito 1",
                        Details = new List<HitoDetail>
                        {
                            new HitoDetail
                            {
                                Total = 100,
                                Quantity = 1,
                                UnitPrice = 1
                            }
                        }
                    }
                }
            };

            return solfac;
        }

    }
}
