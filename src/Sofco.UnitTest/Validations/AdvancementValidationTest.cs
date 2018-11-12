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
            var model = new AdvancementModel { Details = new List<AdvancementDetailModel>() };

            var response = sut.ValidateAdd(model);

            Assert.True(response.HasErrors());

            var userIsRequiredCode = Resources.AdvancementAndRefund.Advancement.UserApplicantRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(userIsRequiredCode)));

            var paymentFormIsRequiredCode = Resources.AdvancementAndRefund.Advancement.PaymentFormRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(paymentFormIsRequiredCode)));

            var typeIsRequiredCode = Resources.AdvancementAndRefund.Advancement.TypeRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(typeIsRequiredCode)));

            var advancementReturnFormIsRequiredCode = Resources.AdvancementAndRefund.Advancement.AdvancementReturnFormRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(advancementReturnFormIsRequiredCode)));

            var startDateReturnIsRequiredCode = Resources.AdvancementAndRefund.Advancement.StartDateReturnRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(startDateReturnIsRequiredCode)));

            var analyticIsRequiredCode = Resources.AdvancementAndRefund.Advancement.AnalyticRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(analyticIsRequiredCode)));

            var currencyRequiredCode = Resources.AdvancementAndRefund.Advancement.CurrencyRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(currencyRequiredCode)));

            model = GetInvalidModel();
            model.Details.Add(GetInvalidDetail());

            response = sut.ValidateAdd(model);

            var currencyNotFoundCode = Resources.AdvancementAndRefund.Advancement.CurrencyNotFound.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(currencyNotFoundCode)));

            var analyticNotFoundCode = Resources.AllocationManagement.Analytic.NotFound.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(analyticNotFoundCode)));

            var advancementReturnFormNotFoundCode = Resources.AdvancementAndRefund.Advancement.AdvancementReturnFormNotFound.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(advancementReturnFormNotFoundCode)));

            var userNotFoundCode = Resources.Admin.User.NotFound.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(userNotFoundCode)));

            var dateItemRequiredCode = Resources.AdvancementAndRefund.Advancement.DateItemRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(dateItemRequiredCode)));

            var descriptionItemRequiredCode = Resources.AdvancementAndRefund.Advancement.DescriptionItemRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(descriptionItemRequiredCode)));

            var ammountItemRequiredCode = Resources.AdvancementAndRefund.Advancement.AmmountItemRequired.Split('.')[1];
            Assert.True(response.Messages.Any(x => x.Code.Equals(ammountItemRequiredCode)));
        }

        [Test]
        public void ShouldPass()
        {
            var response = sut.ValidateAdd(GetValidModel());

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
                AnalyticId = 2,
                Details = new List<AdvancementDetailModel>()
            };
        }

        private AdvancementDetailModel GetInvalidDetail()
        {
            return new AdvancementDetailModel
            {
                Date = null,
                Description = string.Empty,
                Ammount = 0
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
                AnalyticId = 1,
                CurrencyId = 1,
                Details = new List<AdvancementDetailModel>
                {
                    new AdvancementDetailModel
                    {
                        Date = DateTime.Today,
                        Description = "description",
                        Ammount = 1
                    }
                }
            };
        }
    }
}
