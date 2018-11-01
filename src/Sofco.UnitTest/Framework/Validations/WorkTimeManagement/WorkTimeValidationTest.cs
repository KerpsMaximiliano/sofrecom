using System;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Validations.WorkTimeManagement;

namespace Sofco.UnitTest.Framework.Validations.WorkTimeManagement
{
    [TestFixture]
    public class WorkTimeValidationTest
    {
        private const int ValidAnalyticId = 1;

        private const int ValidEmployeeId = 1;

        private const int ValidYear = 2018;

        private const int ValidMonth1 = 10;

        private const int InValidMonth = 11;

        private const int ValidMonth2 = 12;

        private DateTime validDate;

        private WorkTimeValidation sut;

        private Mock<IUnitOfWork> unitOfWorkMock;

        private Mock<ISettingData> settingDataMock;

        private Mock<IAllocationRepository> allocationRepositoryMock;

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();

            settingDataMock = new Mock<ISettingData>();

            allocationRepositoryMock = new Mock<IAllocationRepository>();

            unitOfWorkMock.Setup(s => s.AllocationRepository).Returns(allocationRepositoryMock.Object);

            sut = new WorkTimeValidation(unitOfWorkMock.Object, settingDataMock.Object);
        }

        [Test]
        public void ShouldPassValidateAllocations()
        {
            var response = new Response();

            var releaseDate = new DateTime(ValidYear, ValidMonth2, 31);

            allocationRepositoryMock.Setup(s => s.GetByEmployee(ValidEmployeeId))
                .Returns(new Collection<Allocation> {
                    new Allocation {StartDate = new DateTime(ValidYear, ValidMonth1, 1), AnalyticId = ValidAnalyticId, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, InValidMonth, 1), AnalyticId = ValidAnalyticId, Percentage = 0, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 12, 1), AnalyticId = ValidAnalyticId, Percentage = 50, ReleaseDate = releaseDate}
                });

            validDate = new DateTime(ValidYear, ValidMonth1, 1);

            var model = new WorkTimeAddModel
            {
                Date = validDate, EmployeeId = ValidEmployeeId, AnalyticId = ValidAnalyticId
            };

            sut.ValidateAllocations(response, model);

            Assert.False(response.HasErrors());
        }
    }
}
