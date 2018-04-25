using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Sofco.Core.DAL.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.UnitTest.Framework.ValidationHelpers.Billing
{
    [TestFixture]
    public class SolfacValidationHelperTest
    {
        private Mock<ISolfacRepository> solfacRepositoryMock;

        [SetUp]
        public void Setup()
        {
            solfacRepositoryMock = new Mock<ISolfacRepository>();

            solfacRepositoryMock.Setup(s => s.GetById(It.IsAny<int>())).Returns(GetSolfacData());
            solfacRepositoryMock.Setup(s => s.GetTotalAmountById(It.IsAny<int>())).Returns(GetSolfacData().TotalAmount);
        }

        [Test]
        public void ShouldPassValidateCreditNote()
        {
            var solfac = GetSolfacData();

            var response = new Response();

            SolfacValidationHelper.ValidateCreditNote(solfac, solfacRepositoryMock.Object, response);

            Assert.False(response.HasErrors());
        }

        [Test]
        public void ShouldFailValidateCreditNote()
        {
            var solfac = GetSolfacData();

            solfac.Hitos.First().Details.First().Total = 1000;

            var response = new Response();

            SolfacValidationHelper.ValidateCreditNote(solfac, solfacRepositoryMock.Object, response);

            Assert.True(response.HasErrors());

            var actualMessage = response.Messages.First();

            Assert.True(Resources.Billing.Solfac.CreditNoteTotalExceededError.Contains(actualMessage.Code));
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
