using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Framework.Validations.AdvancementAndRefund;

namespace Sofco.UnitTest.Validations
{
    public class RefundValidationTest
    {
        private RefundValidation sut;

        private Mock<IUnitOfWork> unitOfWorkMock;

        private Mock<IUtilsRepository> utilsRepositoryMock;
        private Mock<IAnalyticRepository> analyticRepositoryMock;
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IAdvancementRepository> advancementRepositoryMock;

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            utilsRepositoryMock = new Mock<IUtilsRepository>();
            analyticRepositoryMock = new Mock<IAnalyticRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            advancementRepositoryMock = new Mock<IAdvancementRepository>();

            unitOfWorkMock.Setup(x => x.UtilsRepository).Returns(utilsRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AnalyticRepository).Returns(analyticRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.UserRepository).Returns(userRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AdvancementRepository).Returns(advancementRepositoryMock.Object);

            utilsRepositoryMock.Setup(x => x.ExistCurrency(1)).Returns(true);
            utilsRepositoryMock.Setup(x => x.ExistCurrency(2)).Returns(false);
            analyticRepositoryMock.Setup(x => x.Exist(1)).Returns(true);
            analyticRepositoryMock.Setup(x => x.Exist(2)).Returns(false);
            utilsRepositoryMock.Setup(x => x.ExistCurrency(1)).Returns(true);
            utilsRepositoryMock.Setup(x => x.ExistCurrency(2)).Returns(false);
            userRepositoryMock.Setup(x => x.ExistById(1)).Returns(true);
            userRepositoryMock.Setup(x => x.ExistById(2)).Returns(false);
            advancementRepositoryMock.Setup(x => x.Exist(1)).Returns(true);
            advancementRepositoryMock.Setup(x => x.Exist(2)).Returns(false);

            sut = new RefundValidation(unitOfWorkMock.Object);
        }

        [Test]
        public void ShouldValidate()
        {
            var model = GetInvalidModel();

            var response = new Response();

            sut.ValidateAdd(model, response);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.CurrencyRequired)));
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.UserApplicantRequired)));
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Refund.DetailsRequired)));

            model.Details = GetInvalidItems();
            model.CurrencyId = 2;
            model.UserApplicantId = 2;
            model.Advancements = new List<int> { 2 };

            sut.ValidateAdd(model, response);

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AllocationManagement.Analytic.NotFound)));
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.CurrencyNotFound)));
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Admin.User.NotFound)));
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.NotFound)));
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Refund.DetailDescriptionRequired)));
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Refund.DetailAmmountRequired)));
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Refund.DetailDateRequired)));
        }

        [Test]
        public void ShouldPass()
        {
            var response = new Response();

            sut.ValidateAdd(GetValidModel(), response);

            Assert.False(response.HasErrors());
        }

        [Test]
        public void ShouldFailValidateDetailDate()
        {
            var model = GetInvalidModel();
            model.Details = GetInvalidItems();
            model.Details.First().CreationDate = DateTime.UtcNow.AddDays(1);

            var response = new Response();

            sut.ValidateAdd(model, response);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Refund.DetailDateInvalidRange)));
        }

        private RefundModel GetValidModel()
        {
            return new RefundModel
            {
                UserApplicantId = 1,
                CurrencyId = 1,
                Advancements = new List<int> { 1 },
                Details = new List<RefundDetailModel>
                {
                    new RefundDetailModel
                    {
                        CreationDate = DateTime.Now,
                        Ammount = 1000,
                        AnalyticId = 1,
                        Description = "description"
                    }
                }
            };
        }

        private RefundModel GetInvalidModel()
        {
            return new RefundModel
            {
                UserApplicantId = null,
                CurrencyId = null,
                Advancements = new List<int>(),
                Details = new List<RefundDetailModel>()
            };
        }

        private List<RefundDetailModel> GetInvalidItems()
        {
            return new List<RefundDetailModel>
            {
                new RefundDetailModel
                {
                    CreationDate = null,
                    Ammount = -1,
                    AnalyticId = 2,
                    Description = string.Empty
                }
            };
        }
    }
}
