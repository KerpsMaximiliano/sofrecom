using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Framework.Validations.AdvancementAndRefund;

namespace Sofco.UnitTest.Validations
{
    [TestFixture]
    public class AdvancementValidationTest
    {
        private AdvancementValidation sut;

        private Mock<IUnitOfWork> unitOfWorkMock;

        private Mock<IUtilsRepository> utilsRepositoryMock;
        private Mock<IAnalyticRepository> analyticRepositoryMock;
        private Mock<IUserRepository> userRepositoryMock;

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            utilsRepositoryMock = new Mock<IUtilsRepository>();
            analyticRepositoryMock = new Mock<IAnalyticRepository>();
            userRepositoryMock = new Mock<IUserRepository>();

            unitOfWorkMock.Setup(x => x.UtilsRepository).Returns(utilsRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.AnalyticRepository).Returns(analyticRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.UserRepository).Returns(userRepositoryMock.Object);

            utilsRepositoryMock.Setup(x => x.ExistCurrency(1)).Returns(true);
            utilsRepositoryMock.Setup(x => x.ExistCurrency(2)).Returns(false);
            analyticRepositoryMock.Setup(x => x.Exist(1)).Returns(true);
            analyticRepositoryMock.Setup(x => x.Exist(2)).Returns(false);
            utilsRepositoryMock.Setup(x => x.ExistAdvancementReturnForm(1)).Returns(true);
            utilsRepositoryMock.Setup(x => x.ExistAdvancementReturnForm(2)).Returns(false);
            userRepositoryMock.Setup(x => x.ExistById(1)).Returns(true);
            userRepositoryMock.Setup(x => x.ExistById(2)).Returns(false);

            sut = new AdvancementValidation(unitOfWorkMock.Object);
        }

        [Test]
        public void ShouldValidate()
        {
            var model = new AdvancementModel();
            var response = new Response();

            sut.ValidateAdd(model, response);

            Assert.True(response.HasErrors());

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.UserApplicantRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.PaymentFormRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.TypeRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.AdvancementReturnFormRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.StartDateReturnRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.AnalyticRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.CurrencyRequired)));

            model = GetInvalidModel();

            response = new Response();

            sut.ValidateAdd(model, response);

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.CurrencyNotFound)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AllocationManagement.Analytic.NotFound)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.AdvancementReturnFormNotFound)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.Admin.User.NotFound)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.DateItemRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.DescriptionItemRequired)));

            Assert.True(response.Messages.Any(x => x.Text.Equals(Resources.AdvancementAndRefund.Advancement.AmmountItemRequired)));
        }

        [Test]
        public void ShouldPass()
        {
            var response = new Response();

            sut.ValidateAdd(GetValidModel(), response);

            Assert.False(response.HasErrors());
        }

        private AdvancementModel GetInvalidModel()
        {
            return new AdvancementModel
            {
                UserApplicantId = 2,
                PaymentForm = AdvancementPaymentForm.OwnBank,
                Type = AdvancementType.Salary,
                AdvancementReturnFormId = 2,
                StartDateReturn = DateTime.Now,
                CurrencyId = 2,
            };
        }

        private AdvancementModel GetValidModel()
        {
            return new AdvancementModel
            {
                UserApplicantId = 1,
                PaymentForm = AdvancementPaymentForm.OwnBank,
                Type = AdvancementType.Salary,
                AdvancementReturnFormId = 1,
                StartDateReturn = DateTime.Today,
                CurrencyId = 1,
                Description = "description",
                Ammount = 1
            };
        }
    }
}
