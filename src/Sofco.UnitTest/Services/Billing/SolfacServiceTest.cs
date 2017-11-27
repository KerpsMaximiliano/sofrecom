using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Common.Domains;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using Sofco.Service.Implementations.Billing;

namespace Sofco.UnitTest.Services.Billing
{
    [TestFixture]
    public class SolfacServiceTest
    {
        private SolfacService sut;
        private Mock<ISolfacRepository> solfacRepositoryMock;
        private Mock<IInvoiceRepository> invoiceRepositoryMock;
        private Mock<ISolfacStatusFactory> solfacStatusFactoryMock;
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<CrmConfig> crmConfigMock;
        private Mock<IMailSender> mailSenderMock;
        private Mock<ICrmInvoiceService> crmInvoiceServiceMock;
        private Mock<ILogMailer<SolfacService>> loggerMock;

        [SetUp]
        public void Setup()
        {
            solfacRepositoryMock = new Mock<ISolfacRepository>();
            invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            solfacStatusFactoryMock = new Mock<ISolfacStatusFactory>();
            userRepositoryMock = new Mock<IUserRepository>();
            crmConfigMock = new Mock<CrmConfig>();
            mailSenderMock = new Mock<IMailSender>();
            crmInvoiceServiceMock = new Mock<ICrmInvoiceService>();
            loggerMock = new Mock<ILogMailer<SolfacService>>();

            var optionsMock = new Mock<IOptions<CrmConfig>>();
            optionsMock.SetupGet(s => s.Value).Returns(crmConfigMock.Object);

            solfacRepositoryMock.Setup(s => s.GetById(It.IsAny<int>())).Returns(GetSolfacData());

            crmInvoiceServiceMock.Setup(s => s.UpdateHitos(It.IsAny<ICollection<Hito>>())).Returns(new Response());

            crmInvoiceServiceMock.Setup(s => s.CreateHitoBySolfac(It.IsAny<Solfac>())).Returns(new Result<string>("1"));

            sut = new SolfacService(solfacRepositoryMock.Object,
                invoiceRepositoryMock.Object,
                solfacStatusFactoryMock.Object,
                userRepositoryMock.Object,
                optionsMock.Object,
                mailSenderMock.Object,
                crmInvoiceServiceMock.Object,
                loggerMock.Object);
        }

        [Test]
        public void ShouldPassCreateSolfac()
        {
            var solfac = GetSolfacData();

            var actual = sut.CreateSolfac(solfac, new[] { 1, 2 });

            Assert.False(actual.HasErrors());

            crmInvoiceServiceMock.Verify(s => s.CreateHitoBySolfac(It.IsAny<Solfac>()), Times.Never);
            solfacRepositoryMock.Verify(s => s.Insert(It.IsAny<Solfac>()), Times.Once);
            solfacRepositoryMock.Verify(s => s.Save(), Times.Exactly(2));
            invoiceRepositoryMock.Verify(s => s.UpdateSolfacId(It.IsAny<Invoice>()), Times.AtLeastOnce);
            invoiceRepositoryMock.Verify(s => s.UpdateStatus(It.IsAny<Invoice>()), Times.AtLeastOnce);
            crmInvoiceServiceMock.Verify(s => s.UpdateHitos(It.IsAny<ICollection<Hito>>()), Times.Once);
        }

        [TestCase(SolfacDocumentType.CreditNoteA)]
        [TestCase(SolfacDocumentType.CreditNoteB)]
        public void ShouldPassCreateSolfacCreditNoteType(int documentTypeId)
        {
            var solfac = GetSolfacData();

            solfac.DocumentTypeId = documentTypeId;

            var actual = sut.CreateSolfac(solfac, new List<int>());

            Assert.False(actual.HasErrors());

            crmInvoiceServiceMock.Verify(s => s.CreateHitoBySolfac(It.IsAny<Solfac>()), Times.Once);
            solfacRepositoryMock.Verify(s => s.Insert(It.IsAny<Solfac>()), Times.Once);
            solfacRepositoryMock.Verify(s => s.Save(), Times.Once);
            invoiceRepositoryMock.Verify(s => s.UpdateSolfacId(It.IsAny<Invoice>()), Times.Never);
            invoiceRepositoryMock.Verify(s => s.UpdateStatus(It.IsAny<Invoice>()), Times.Never);
            crmInvoiceServiceMock.Verify(s => s.UpdateHitos(It.IsAny<ICollection<Hito>>()), Times.Once);
        }

        [Test]
        public void ShouldPassCreateSolfacDebitNoteType()
        {
            var solfac = GetSolfacData();

            solfac.DocumentTypeId = SolfacDocumentType.DebitNote;

            var actual = sut.CreateSolfac(solfac, new List<int>());

            Assert.False(actual.HasErrors());

            crmInvoiceServiceMock.Verify(s => s.CreateHitoBySolfac(It.IsAny<Solfac>()), Times.Once);
            solfacRepositoryMock.Verify(s => s.Insert(It.IsAny<Solfac>()), Times.Once);
            solfacRepositoryMock.Verify(s => s.Save(), Times.Once);
            invoiceRepositoryMock.Verify(s => s.UpdateSolfacId(It.IsAny<Invoice>()), Times.Never);
            invoiceRepositoryMock.Verify(s => s.UpdateStatus(It.IsAny<Invoice>()), Times.Never);
            crmInvoiceServiceMock.Verify(s => s.UpdateHitos(It.IsAny<ICollection<Hito>>()), Times.Once);
        }

        private Solfac GetSolfacData()
        {
            var solfac = new Solfac
            {
                DocumentTypeId = 1,
                BuenosAiresPercentage = 100,
                PaymentTermId = 1,
                ContractNumber = "1",
                ImputationNumber1 = "1",
                UserApplicantId = 1,
                TotalAmount = 100,
                Hitos = new List<Hito>
                {
                    new Hito
                    {
                        SolfacId = 1,
                        Total = 100,
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
