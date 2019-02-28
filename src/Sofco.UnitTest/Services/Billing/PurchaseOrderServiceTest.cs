using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.Billing.PurchaseOrder;

namespace Sofco.UnitTest.Services.Billing
{
    [TestFixture]
    public class PurchaseOrderServiceTest
    {
        private PurchaseOrderService sut;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<ILogMailer<PurchaseOrderService>> loggerMock;
        private Mock<IUserData> userDataMock;
        private Mock<IMapper> mapperMock;
        private Mock<IPurchaseOrderRepository> purchaseOrderRepositoryMock;
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IUserDelegateRepository> userDelegateRepositoryMock;

        private const string ExistNumber = "A0001";
        private const string NotExistNumber = "A0002";

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            loggerMock = new Mock<ILogMailer<PurchaseOrderService>>();
            userDataMock = new Mock<IUserData>();
            mapperMock = new Mock<IMapper>();
            purchaseOrderRepositoryMock = new Mock<IPurchaseOrderRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            userDelegateRepositoryMock = new Mock<IUserDelegateRepository>();

            unitOfWorkMock.Setup(x => x.PurchaseOrderRepository).Returns(purchaseOrderRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.UserRepository).Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.UserDelegateRepository).Returns(userDelegateRepositoryMock.Object);

            purchaseOrderRepositoryMock.Setup(x => x.ExistNumber(ExistNumber, It.IsAny<int>())).Returns(true);
            purchaseOrderRepositoryMock.Setup(x => x.ExistNumber(NotExistNumber, It.IsAny<int>())).Returns(false);
            purchaseOrderRepositoryMock.Setup(x => x.GetById(1)).Returns(GetPurchaseOrder(PurchaseOrderStatus.Draft));
            purchaseOrderRepositoryMock.Setup(x => x.Get(1)).Returns(GetPurchaseOrder(PurchaseOrderStatus.Draft));
            purchaseOrderRepositoryMock.Setup(x => x.GetById(0)).Returns(GetDomainNull);
            userDataMock.Setup(x => x.GetCurrentUser()).Returns(new UserLiteModel { UserName = "username", Id = 1 });

            sut = new PurchaseOrderService(unitOfWorkMock.Object,
                loggerMock.Object,
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

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.NumberIsRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.AnalyticIsRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.CurrencyIsRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.ClientIsRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.AreaIsRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.DatesRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.AmmountRequired)));
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

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.NumberAlreadyExist)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.CurrencyIsRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Billing.PurchaseOrder.EndDateLessThanStartDate)));
        }

        [Test]
        public void ShouldAddPurchaseOrder()
        {
            var model = new PurchaseOrderModel()
            {
                Number = NotExistNumber,
                AnalyticIds = new[] { 1 },
                ProposalIds = new []{ "1" },
                ClientExternalId = "Client",
                AreaId = 1,
                Margin = 10,
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

        private PurchaseOrder GetDomainNull()
        {
            return null;
        }
    }
}
