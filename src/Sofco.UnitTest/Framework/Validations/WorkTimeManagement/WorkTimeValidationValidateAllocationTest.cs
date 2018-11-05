using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Validations.WorkTimeManagement;

namespace Sofco.UnitTest.Framework.Validations.WorkTimeManagement
{
    [TestFixture]
    public class WorkTimeValidationValidateAllocationsTest
    {
        private const int ValidAnalyticId1 = 1;

        private const int ValidAnalyticId2 = 2;

        private const int ValidEmployeeId = 1;

        private const int ValidYear = 2018;

        private const int ValidMonth1 = 10;

        private const int InvalidMonth = 11;

        private const int ValidMonth2 = 12;

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

            unitOfWorkMock.SetupGet(s => s.AllocationRepository).Returns(allocationRepositoryMock.Object);

            sut = new WorkTimeValidation(unitOfWorkMock.Object, settingDataMock.Object);
        }

        [Test, TestCaseSource("PassValidateAllocationsSource")]
        public void ShouldPassValidateAllocations(WorkTimeAddModel model, Collection<Allocation> allocations)
        {
            var response = new Response();

            allocationRepositoryMock.Setup(s => s.GetByEmployee(ValidEmployeeId))
                .Returns(allocations);

            sut.ValidateAllocations(response, model);

            Assert.False(response.HasErrors());
        }

        [Test, TestCaseSource("FailValidateAllocationsSource")]
        public void ShouldFailValidateAllocations(WorkTimeAddModel model, Collection<Allocation> allocations)
        {
            var response = new Response();

            allocationRepositoryMock.Setup(s => s.GetByEmployee(ValidEmployeeId))
                .Returns(allocations);

            sut.ValidateAllocations(response, model);

            Assert.True(response.HasErrors());

            var actualError = response.Messages.First();

            Assert.AreEqual(MessageType.Error, actualError.Type);
        }

        private static IEnumerable<object[]> PassValidateAllocationsSource
        {
            get
            {
                var validDate = new DateTime(ValidYear, ValidMonth1, 1);

                var releaseDate = new DateTime(ValidYear, ValidMonth2, 31);

                var model = new WorkTimeAddModel
                {
                    Date = validDate,
                    EmployeeId = ValidEmployeeId,
                    AnalyticId = ValidAnalyticId1
                };

                var allocations1 = new Collection<Allocation> {
                    new Allocation {StartDate = new DateTime(ValidYear, ValidMonth1, 1), AnalyticId = ValidAnalyticId1, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, InvalidMonth, 1), AnalyticId = ValidAnalyticId1, Percentage = 0, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 12, 1), AnalyticId = ValidAnalyticId1, Percentage = 50, ReleaseDate = releaseDate}
                };

                var allocations2 = new Collection<Allocation> {
                    new Allocation {StartDate = new DateTime(ValidYear, ValidMonth1, 1), AnalyticId = ValidAnalyticId1, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, InvalidMonth, 1), AnalyticId = ValidAnalyticId1, Percentage = 0, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 12, 1), AnalyticId = ValidAnalyticId1, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 9, 1), AnalyticId = ValidAnalyticId2, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 10, 1), AnalyticId = ValidAnalyticId2, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 11, 1), AnalyticId = ValidAnalyticId2, Percentage = 50, ReleaseDate = releaseDate}
                };

                // TestCase#1 - Single Analytic Assigned | Valid date
                yield return new object[] { model, allocations1 };

                // TestCase#2 - Multiple Analytic Assigned | Valid date
                yield return new object[] { model, allocations2 };
            }
        }

        private static IEnumerable<object[]> FailValidateAllocationsSource
        {
            get
            {
                var invalidDate = new DateTime(ValidYear, InvalidMonth, 1);

                var releaseDate = new DateTime(ValidYear, ValidMonth2, 31);

                var model = new WorkTimeAddModel
                {
                    Date = invalidDate,
                    EmployeeId = ValidEmployeeId,
                    AnalyticId = ValidAnalyticId1
                };

                var allocations1 = new Collection<Allocation> {
                    new Allocation {StartDate = new DateTime(ValidYear, ValidMonth1, 1), AnalyticId = ValidAnalyticId1, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, InvalidMonth, 1), AnalyticId = ValidAnalyticId1, Percentage = 0, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 12, 1), AnalyticId = ValidAnalyticId1, Percentage = 50, ReleaseDate = releaseDate}
                };

                var allocations2 = new Collection<Allocation> {
                    new Allocation {StartDate = new DateTime(ValidYear, ValidMonth1, 1), AnalyticId = ValidAnalyticId1, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, InvalidMonth, 1), AnalyticId = ValidAnalyticId1, Percentage = 0, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 12, 1), AnalyticId = ValidAnalyticId1, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 9, 1), AnalyticId = ValidAnalyticId2, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 10, 1), AnalyticId = ValidAnalyticId2, Percentage = 50, ReleaseDate = releaseDate},
                    new Allocation {StartDate = new DateTime(ValidYear, 11, 1), AnalyticId = ValidAnalyticId2, Percentage = 50, ReleaseDate = releaseDate}
                };

                // TestCase#1 - Single Analytic Assigned | Invalid date
                yield return new object[] { model, allocations1 };

                // TestCase#2 - Multiple Analytic Assigned | Invalid date
                yield return new object[] { model, allocations2 };
            }
        }
    }
}
