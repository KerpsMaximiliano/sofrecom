using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Billing;
using Sofco.Core.DAL.Common;
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
using Sofco.Service.Implementations.Billing;

namespace Sofco.UnitTest.Services.Billing
{
    [TestFixture]
    public class PurchaseOrderServiceTest
    {
        private PurchaseOrderService sut;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<FileConfig> fileConfigMock;
        private Mock<ILogMailer<PurchaseOrderService>> loggerMock;
        private Mock<IUserData> userDataMock;
        private Mock<IMapper> mapperMock;
        private Mock<IPurchaseOrderStatusFactory> factoryMock;
        private Mock<IPurchaseOrderRepository> purchaseOrderRepositoryMock;
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IFileRepository> fileRepositoryMock;
        private Mock<IUserDelegateRepository> userDelegateRepositoryMock;
        private Mock<IMailSender> mailSenderMock;
        private Mock<IAnalyticRepository> analyticRepositoryMock;
        private Mock<ISectorRepository> sectorRepositoryMock;
        private Mock<IAreaRepository> areaRepositoryMock;

        private Mock<IMailBuilder> mailBuilderMock;
        private Mock<EmailConfig> emailConfig;
        private Mock<IPurchaseOrderStatusRecipientManager> recipientManagerMock;

        private const string ExistNumber = "A0001";
        private const string NotExistNumber = "A0002";

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            loggerMock = new Mock<ILogMailer<PurchaseOrderService>>();
            userDataMock = new Mock<IUserData>();
            mapperMock = new Mock<IMapper>();
            factoryMock = new Mock<IPurchaseOrderStatusFactory>();
            purchaseOrderRepositoryMock = new Mock<IPurchaseOrderRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            fileRepositoryMock = new Mock<IFileRepository>();
            userDelegateRepositoryMock = new Mock<IUserDelegateRepository>();
            fileConfigMock = new Mock<FileConfig>();
            mailSenderMock = new Mock<IMailSender>();
            analyticRepositoryMock = new Mock<IAnalyticRepository>();
            sectorRepositoryMock = new Mock<ISectorRepository>();
            areaRepositoryMock = new Mock<IAreaRepository>();
            emailConfig = new Mock<EmailConfig>();

            var fileOptions = new Mock<IOptions<FileConfig>>();
            fileOptions.SetupGet(x => x.Value).Returns(fileConfigMock.Object);

            var emailOptions = new Mock<IOptions<EmailConfig>>();
            emailOptions.SetupGet(x => x.Value).Returns(emailConfig.Object);

            recipientManagerMock = new Mock<IPurchaseOrderStatusRecipientManager>();
            mailBuilderMock = new Mock<IMailBuilder>();

            unitOfWorkMock.Setup(x => x.PurchaseOrderRepository).Returns(purchaseOrderRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.UserRepository).Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.FileRepository).Returns(fileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.UserDelegateRepository).Returns(userDelegateRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AreaRepository).Returns(areaRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AnalyticRepository).Returns(analyticRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.SectorRepository).Returns(sectorRepositoryMock.Object);

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

            purchaseOrderRepositoryMock.Setup(x => x.ExistNumber(ExistNumber, It.IsAny<int>())).Returns(true);
            purchaseOrderRepositoryMock.Setup(x => x.ExistNumber(NotExistNumber, It.IsAny<int>())).Returns(false);
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
            purchaseOrderRepositoryMock.Setup(x => x.Get(21)).Returns(GetPurchaseOrderWithFile(PurchaseOrderStatus.CompliancePending));
            purchaseOrderRepositoryMock.Setup(x => x.Get(22)).Returns(GetPurchaseOrderWithFile(PurchaseOrderStatus.ComercialPending));
            purchaseOrderRepositoryMock.Setup(x => x.Get(23)).Returns(GetPurchaseOrderWithFile(PurchaseOrderStatus.OperativePending));
            purchaseOrderRepositoryMock.Setup(x => x.Get(24)).Returns(GetPurchaseOrderWithFile(PurchaseOrderStatus.DafPending));

            areaRepositoryMock.Setup(x => x.Get(It.IsAny<int>())).Returns(new Area
            {
                Id = 1,
                ResponsableUserId = 2
            });

            analyticRepositoryMock.Setup(x => x.GetByPurchaseOrder(It.IsAny<int>())).Returns(new List<Analytic>
            {
                new Analytic { Id = 1, SectorId = 1 }
            });

            sectorRepositoryMock.Setup(x => x.Get(It.IsAny<List<int>>())).Returns(new List<Sector>
            {
                new Sector { Id = 1, ResponsableUserId = 2 }
            });

            sut = new PurchaseOrderService(unitOfWorkMock.Object,
                loggerMock.Object,
                fileOptions.Object,
                factoryMock.Object,
                userDataMock.Object,
                mapperMock.Object);
        }

        [Test]
        public void ShouldValidateEmptyModelPurchaseOrder()
        {
            var model = new PurchaseOrderModel()
            {
                AmmountDetails = new List<PurchaseOrderAmmountDetailModel>()
            };

            var response = sut.Add(model);

            Assert.True(response.HasErrors());

            var numberIsRequiredCode = Resources.Billing.PurchaseOrder.NumberIsRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(numberIsRequiredCode)));

            var analyticIsRequiredCode = Resources.Billing.PurchaseOrder.AnalyticIsRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(analyticIsRequiredCode)));

            var currencyIsRequiredCode = Resources.Billing.PurchaseOrder.CurrencyIsRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(currencyIsRequiredCode)));

            var clientIsRequiredCode = Resources.Billing.PurchaseOrder.ClientIsRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(clientIsRequiredCode)));

            var areaIsRequiredCode = Resources.Billing.PurchaseOrder.AreaIsRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(areaIsRequiredCode)));

            var datesRequiredCode = Resources.Billing.PurchaseOrder.DatesRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(datesRequiredCode)));

            var ammountRequiredCode = Resources.Billing.PurchaseOrder.AmmountRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(ammountRequiredCode)));
        }

        [Test]
        public void ShouldValidateSomeFieldsPurchaseOrder()
        {
            var model = new PurchaseOrderModel()
            {
                Number = ExistNumber,
                AnalyticIds = new[] { 1 },
                ClientExternalId = "Client",
                AreaId = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today,
                AmmountDetails = new List<PurchaseOrderAmmountDetailModel>
                {
                    new PurchaseOrderAmmountDetailModel
                    {
                        CurrencyId = 1,
                        Enable = false,
                        Ammount = -1,
                        Balance = 1
                    }
                }
            };

            var response = sut.Add(model);

            Assert.True(response.HasErrors());

            var numberAlreadyExistCode = Resources.Billing.PurchaseOrder.NumberAlreadyExist.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(numberAlreadyExistCode)));

            var currencyIsRequiredCode = Resources.Billing.PurchaseOrder.CurrencyIsRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(currencyIsRequiredCode)));

            var endDateLessThanStartDateCode = Resources.Billing.PurchaseOrder.EndDateLessThanStartDate.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(endDateLessThanStartDateCode)));
        }

        [Test]
        public void ShouldAddPurchaseOrder()
        {
            var model = new PurchaseOrderModel()
            {
                Number = NotExistNumber,
                AnalyticIds = new[] { 1 },
                ClientExternalId = "Client",
                AreaId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                AmmountDetails = new List<PurchaseOrderAmmountDetailModel>
                {
                    new PurchaseOrderAmmountDetailModel
                    {
                        CurrencyId = 1,
                        Enable = true,
                        Ammount = 1,
                        Balance = 1
                    }
                }
            };

            var response = sut.Add(model);

            Assert.False(response.HasErrors());
            purchaseOrderRepositoryMock.Verify(s => s.Insert(It.IsAny<PurchaseOrder>()), Times.Once);
            purchaseOrderRepositoryMock.Verify(s => s.AddPurchaseOrderAnalytic(It.IsAny<PurchaseOrderAnalytic>()), Times.Once);
            unitOfWorkMock.Verify(s => s.Save(), Times.Exactly(2));
        }

        [Test]
        public void ShouldValidateAdjustment()
        {
            var model = new List<PurchaseOrderAmmountDetailModel>
            {
                new PurchaseOrderAmmountDetailModel { Adjustment = 999999999 }
            };

            var response = sut.MakeAdjustment(0, model);

            Assert.True(response.HasErrors());

            var notFoundCode = Resources.Billing.PurchaseOrder.NotFound.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(notFoundCode)));

            var ammountRequiredCode = Resources.Billing.PurchaseOrder.AmmountRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(ammountRequiredCode)));
        }

        [Test]
        public void ShouldCreateAdjustment()
        {
            var model = new List<PurchaseOrderAmmountDetailModel>
            {
                new PurchaseOrderAmmountDetailModel { Adjustment = 99999999, CurrencyId = 1 }
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
            var commentsRequiredCode = Resources.Billing.PurchaseOrder.CommentsRequired.Split('.')[1];
            var userSectorWrongCode = Resources.Billing.PurchaseOrder.UserSectorWrong.Split('.')[1];
            var userAreaWrongCode = Resources.Billing.PurchaseOrder.UserAreaWrong.Split('.')[1];

            var response = sut.ChangeStatus(10, model);

            var fileRequiredCode = Resources.Billing.PurchaseOrder.FileRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(fileRequiredCode)));

            model.MustReject = true;

            response = sut.ChangeStatus(11, model);

            Assert.True(response.Messages.Any(x => x.Code.Equals(commentsRequiredCode)));

            model.MustReject = true;

            response = sut.ChangeStatus(12, model);

            Assert.True(response.Messages.Any(x => x.Code.Equals(commentsRequiredCode)));

            model.MustReject = false;

            response = sut.ChangeStatus(12, model);

            Assert.True(response.Messages.Any(x => x.Code.Equals(userAreaWrongCode)));

            response = sut.ChangeStatus(13, model);

            Assert.True(response.Messages.Any(x => x.Code.Equals(userSectorWrongCode)));

            model.MustReject = true;

            response = sut.ChangeStatus(14, model);

            Assert.True(response.Messages.Any(x => x.Code.Equals(commentsRequiredCode)));
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
                ClientExternalId = "Client",
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
