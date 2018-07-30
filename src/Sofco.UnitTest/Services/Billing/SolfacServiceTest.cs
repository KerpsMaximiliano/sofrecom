using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Common.Domains;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Logger;
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
        private Mock<ICrmInvoiceService> crmInvoiceServiceMock;
        private Mock<ILogMailer<SolfacService>> loggerMock;
        private Mock<IUserData> userDataMock;
        private Mock<IPurchaseOrderRepository> purchaseOrderRepositoryMock;

        private Mock<IUnitOfWork> unitOfWork;

        [SetUp]
        public void Setup()
        {
            solfacRepositoryMock = new Mock<ISolfacRepository>();
            invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            solfacStatusFactoryMock = new Mock<ISolfacStatusFactory>();
            userRepositoryMock = new Mock<IUserRepository>();
            crmConfigMock = new Mock<CrmConfig>();
            crmInvoiceServiceMock = new Mock<ICrmInvoiceService>();
            loggerMock = new Mock<ILogMailer<SolfacService>>();
            userDataMock = new Mock<IUserData>();
            purchaseOrderRepositoryMock = new Mock<IPurchaseOrderRepository>();

            var optionsMock = new Mock<IOptions<CrmConfig>>();
            optionsMock.SetupGet(s => s.Value).Returns(crmConfigMock.Object);

            unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.SolfacRepository).Returns(solfacRepositoryMock.Object);
            unitOfWork.Setup(x => x.InvoiceRepository).Returns(invoiceRepositoryMock.Object);
            unitOfWork.Setup(x => x.UserRepository).Returns(userRepositoryMock.Object);
            unitOfWork.Setup(x => x.PurchaseOrderRepository).Returns(purchaseOrderRepositoryMock.Object);

            solfacRepositoryMock.Setup(s => s.GetById(It.IsAny<int>())).Returns(GetSolfacData());
            solfacRepositoryMock.Setup(s => s.GetTotalAmountById(It.IsAny<int>())).Returns(GetSolfacData().TotalAmount);

            crmInvoiceServiceMock.Setup(s => s.UpdateHitos(It.IsAny<ICollection<Hito>>())).Returns(new Response());
            crmInvoiceServiceMock.Setup(s => s.CreateHitoBySolfac(It.IsAny<Solfac>())).Returns(new Result<string>("1"));

            purchaseOrderRepositoryMock.Setup(s => s.HasAmmountDetails(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            sut = new SolfacService(solfacStatusFactoryMock.Object,
                unitOfWork.Object,
                userDataMock.Object,
                optionsMock.Object,
                crmInvoiceServiceMock.Object,
                loggerMock.Object);
        }

        [Test]
        public void ShouldPassCreateSolfac()
        {
            var solfac = GetSolfacData();

            var actual = sut.CreateSolfac(solfac, new[] { 1, 2 }, new List<int>());

            Assert.False(actual.HasErrors());

            crmInvoiceServiceMock.Verify(s => s.CreateHitoBySolfac(It.IsAny<Solfac>()), Times.Never);
            solfacRepositoryMock.Verify(s => s.Insert(It.IsAny<Solfac>()), Times.Once);
            unitOfWork.Verify(s => s.Save(), Times.Exactly(2));
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

            var actual = sut.CreateSolfac(solfac, new List<int>(), new List<int>());

            Assert.False(actual.HasErrors());

            crmInvoiceServiceMock.Verify(s => s.CreateHitoBySolfac(It.IsAny<Solfac>()), Times.Once);
            solfacRepositoryMock.Verify(s => s.Insert(It.IsAny<Solfac>()), Times.Once);
            unitOfWork.Verify(s => s.Save(), Times.Once);
            invoiceRepositoryMock.Verify(s => s.UpdateSolfacId(It.IsAny<Invoice>()), Times.Never);
            invoiceRepositoryMock.Verify(s => s.UpdateStatus(It.IsAny<Invoice>()), Times.Never);
        }

        [Test]
        public void ShouldPassCreateSolfacDebitNoteType()
        {
            var solfac = GetSolfacData();

            solfac.DocumentTypeId = SolfacDocumentType.DebitNote;

            var actual = sut.CreateSolfac(solfac, new List<int>(), new List<int>());

            Assert.False(actual.HasErrors());

            crmInvoiceServiceMock.Verify(s => s.CreateHitoBySolfac(It.IsAny<Solfac>()), Times.Once);
            solfacRepositoryMock.Verify(s => s.Insert(It.IsAny<Solfac>()), Times.Once);
            unitOfWork.Verify(s => s.Save(), Times.Once);
            invoiceRepositoryMock.Verify(s => s.UpdateSolfacId(It.IsAny<Invoice>()), Times.Never);
            invoiceRepositoryMock.Verify(s => s.UpdateStatus(It.IsAny<Invoice>()), Times.Never);
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
                BusinessName = "Test",
                Hitos = new List<Hito>
                {
                    new Hito
                    {
                        SolfacId = 1,
                        Total = 100,
                        OpportunityId = "16546510-3460-0346-0346-034634634600",
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
