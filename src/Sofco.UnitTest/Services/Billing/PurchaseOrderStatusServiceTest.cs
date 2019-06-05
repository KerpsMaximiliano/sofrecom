using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Data.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.StatusHandlers;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;
using Sofco.Framework.StatusHandlers.PurchaseOrder;
using Sofco.Service.Implementations.Billing.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.UnitTest.Services.Billing
{
    [TestFixture]
    public class PurchaseOrderStatusServiceTest
    {
        private PurchaseOrderStatusService sut;

        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<ILogMailer<PurchaseOrderStatusService>> loggerMock;
        private Mock<IUserData> userDataMock;
        private Mock<IPurchaseOrderStatusFactory> factoryMock;
        private Mock<IMailSender> mailSenderMock;
        private Mock<IMailBuilder> mailBuilderMock;
        private Mock<EmailConfig> emailConfig;
        private Mock<IPurchaseOrderStatusRecipientManager> recipientManagerMock;
        private Mock<IPurchaseOrderRepository> purchaseOrderRepositoryMock;
        private Mock<ISectorRepository> sectorRepositoryMock;
        private Mock<IAreaRepository> areaRepositoryMock;
        private Mock<IAnalyticRepository> analyticRepositoryMock;

        private const string ExistNumber = "A0001";

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            loggerMock = new Mock<ILogMailer<PurchaseOrderStatusService>>();
            userDataMock = new Mock<IUserData>();
            factoryMock = new Mock<IPurchaseOrderStatusFactory>();
            purchaseOrderRepositoryMock = new Mock<IPurchaseOrderRepository>();
            mailSenderMock = new Mock<IMailSender>();
            emailConfig = new Mock<EmailConfig>();
            sectorRepositoryMock = new Mock<ISectorRepository>();
            areaRepositoryMock = new Mock<IAreaRepository>();
            analyticRepositoryMock = new Mock<IAnalyticRepository>();

            var emailOptions = new Mock<IOptions<EmailConfig>>();
            emailOptions.SetupGet(x => x.Value).Returns(emailConfig.Object);

            recipientManagerMock = new Mock<IPurchaseOrderStatusRecipientManager>();
            mailBuilderMock = new Mock<IMailBuilder>();

            unitOfWorkMock.Setup(x => x.PurchaseOrderRepository).Returns(purchaseOrderRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AreaRepository).Returns(areaRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.SectorRepository).Returns(sectorRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AnalyticRepository).Returns(analyticRepositoryMock.Object);

            factoryMock.Setup(x => x.GetInstance(PurchaseOrderStatus.Draft)).Returns(
                new PurchaseOrderStatusDraft(unitOfWorkMock.Object, mailBuilderMock.Object, mailSenderMock.Object,
                    emailConfig.Object, recipientManagerMock.Object));

            factoryMock.Setup(x => x.GetInstance(PurchaseOrderStatus.CompliancePending)).Returns(
                new PurchaseOrderStatusCompliancePending(unitOfWorkMock.Object, mailBuilderMock.Object, mailSenderMock.Object,
                    emailConfig.Object, recipientManagerMock.Object));

            factoryMock.Setup(x => x.GetInstance(PurchaseOrderStatus.ComercialPending)).Returns(
                new PurchaseOrderStatusComercialPending(unitOfWorkMock.Object, mailBuilderMock.Object, mailSenderMock.Object,
                    emailConfig.Object, recipientManagerMock.Object, userDataMock.Object));

            factoryMock.Setup(x => x.GetInstance(PurchaseOrderStatus.OperativePending)).Returns(
                new PurchaseOrderStatusOperativePending(unitOfWorkMock.Object, mailBuilderMock.Object, mailSenderMock.Object,
                    emailConfig.Object, recipientManagerMock.Object, userDataMock.Object));

            factoryMock.Setup(x => x.GetInstance(PurchaseOrderStatus.DafPending)).Returns(
                new PurchaseOrderStatusDafPending(unitOfWorkMock.Object, mailBuilderMock.Object, mailSenderMock.Object,
                    emailConfig.Object, recipientManagerMock.Object));

            purchaseOrderRepositoryMock.Setup(x => x.GetById(1)).Returns(GetPurchaseOrder(PurchaseOrderStatus.Draft));
            purchaseOrderRepositoryMock.Setup(x => x.Get(1)).Returns(GetPurchaseOrder(PurchaseOrderStatus.Draft));
            purchaseOrderRepositoryMock.Setup(x => x.GetById(0)).Returns(GetDomainNull);
            userDataMock.Setup(x => x.GetCurrentUser()).Returns(new UserLiteModel { UserName = "username", Id = 1 });
            mailSenderMock.Setup(x => x.Send(It.IsAny<Email>()));

            purchaseOrderRepositoryMock.Setup(x => x.Get(10)).Returns(GetPurchaseOrder(PurchaseOrderStatus.Draft));
            purchaseOrderRepositoryMock.Setup(x => x.Get(11)).Returns(GetPurchaseOrder(PurchaseOrderStatus.CompliancePending));
            purchaseOrderRepositoryMock.Setup(x => x.Get(12)).Returns(GetPurchaseOrder(PurchaseOrderStatus.ComercialPending));
            purchaseOrderRepositoryMock.Setup(x => x.Get(13)).Returns(GetPurchaseOrder(PurchaseOrderStatus.OperativePending));
            purchaseOrderRepositoryMock.Setup(x => x.Get(14)).Returns(GetPurchaseOrder(PurchaseOrderStatus.DafPending));

            purchaseOrderRepositoryMock.Setup(x => x.Get(20)).Returns(GetPurchaseOrderWithFile(PurchaseOrderStatus.Draft));

            analyticRepositoryMock.Setup(x => x.GetByPurchaseOrder(It.IsAny<int>())).Returns(new List<Analytic>
            {
                new Analytic { Id = 1, SectorId = 1 }
            });

            areaRepositoryMock.Setup(x => x.Get(It.IsAny<int>())).Returns(new Area
            {
                Id = 1,
                ResponsableUserId = 2
            });

            sectorRepositoryMock.Setup(x => x.Get(It.IsAny<List<int>>())).Returns(new List<Sector>
            {
                new Sector { Id = 1, ResponsableUserId = 2 }
            });

            sut = new PurchaseOrderStatusService(unitOfWorkMock.Object,
                loggerMock.Object,
                factoryMock.Object,
                userDataMock.Object);
        }

        [Test]
        public void ShouldValidateAdjustment()
        {
            var model = new PurchaseOrderAdjustmentModel
            {
                Items = new List<PurchaseOrderAmmountDetailModel> {
                   new PurchaseOrderAmmountDetailModel { Adjustment = 999999999 }
                }
            };

            var response = sut.MakeAdjustment(0, model);

            Assert.True(response.HasErrors());

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.NotFound)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.AmmountRequired)));
        }

        [Test]
        public void ShouldCreateAdjustment()
        {
            var model = new PurchaseOrderAdjustmentModel
            {
                Items = new List<PurchaseOrderAmmountDetailModel> {
                    new PurchaseOrderAmmountDetailModel { Adjustment = 999999999, CurrencyId = 1}
                }
            };

            var response = sut.MakeAdjustment(1, model);

            Assert.False(response.HasErrors());
            purchaseOrderRepositoryMock.Verify(x => x.UpdateStatus(It.IsAny<PurchaseOrder>()), Times.Once);
            purchaseOrderRepositoryMock.Verify(x => x.UpdateAdjustment(It.IsAny<PurchaseOrder>()), Times.Once);
            purchaseOrderRepositoryMock.Verify(x => x.UpdateDetail(It.IsAny<PurchaseOrderAmmountDetail>()), Times.Once);
            unitOfWorkMock.Verify(s => s.Save(), Times.Exactly(1));
        }

        [Test]
        public void ShouldValidateChangeStatus()
        {
            var model = new PurchaseOrderStatusParams();

            var response = sut.ChangeStatus(10, model);

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.FileRequired)));

            model.MustReject = true;

            response = sut.ChangeStatus(11, model);

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.CommentsRequired)));

            model.MustReject = true;

            response = sut.ChangeStatus(12, model);

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.CommentsRequired)));

            model.MustReject = false;

            response = sut.ChangeStatus(12, model);

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.UserAreaWrong)));

            response = sut.ChangeStatus(13, model);

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.UserSectorWrong)));

            model.MustReject = true;

            response = sut.ChangeStatus(14, model);

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.CommentsRequired)));
        }

        [Test]
        public void ShouldChangeStatusDraft()
        {
            var model = new PurchaseOrderStatusParams();

            var response = sut.ChangeStatus(20, model);

            Assert.False(response.HasErrors());
        }

        private PurchaseOrder GetPurchaseOrder(PurchaseOrderStatus status)
        {
            var model = new PurchaseOrder()
            {
                Number = ExistNumber,
                AccountId = "Client",
                AreaId = 1,
                Id = 1,
                Status = status,
                PurchaseOrderAnalytics = new List<PurchaseOrderAnalytic>()
                {
                    new PurchaseOrderAnalytic { PurchaseOrderId = 1, AnalyticId = 1 }
                },
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                AmmountDetails = new List<PurchaseOrderAmmountDetail>
                {
                    new PurchaseOrderAmmountDetail
                    {
                        CurrencyId = 1,
                        Ammount = 1,
                        Balance = 1
                    }
                }
            };

            return model;
        }

        private PurchaseOrder GetPurchaseOrderWithFile(PurchaseOrderStatus status)
        {
            var model = GetPurchaseOrder(status);
            model.FileId = 1;
            return model;
        }

        private PurchaseOrder GetDomainNull()
        {
            return null;
        }
    }
}
