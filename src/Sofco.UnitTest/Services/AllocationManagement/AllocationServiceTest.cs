using Moq;
using NUnit.Framework;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Service.Implementations.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.UnitTest.Services.AllocationManagement
{
    [TestFixture]
    public class AllocationServiceTest
    {
        private Mock<IAllocationRepository> allocationRepositoryMock;
        private Mock<IAnalyticRepository> analyticRepositoryMock;
        private Mock<IEmployeeRepository> employeeRepositoryMock;

        private AllocationService sut;

        [SetUp]
        public void Setup()
        {
            allocationRepositoryMock = new Mock<IAllocationRepository>();

            analyticRepositoryMock = new Mock<IAnalyticRepository>();

            employeeRepositoryMock = new Mock<IEmployeeRepository>();

            sut = new AllocationService(allocationRepositoryMock.Object, analyticRepositoryMock.Object, employeeRepositoryMock.Object);
        }

        [TestCase]
        public void ValidatePercentageCheckError()
        {
            var parameters = new AllocationDto { AnalyticId = 1, EmployeeId = 1, Months = new List<AllocationMonthDto>
            {
                new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 1, 1)},
                new AllocationMonthDto { AllocationId = 0, Percentage = 150, Date = new DateTime(2018, 1, 1)},
            } };

            analyticRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            var response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => $"{x.Folder}.{x.Code}" == Resources.AllocationManagement.Allocation.WrongPercentage));
        }

        [TestCase]
        public void ValidatePercentageCheckSuccess()
        {
            var parameters = new AllocationDto
            {
                AnalyticId = 1,
                EmployeeId = 1,
                ReleaseDate = new DateTime(2018, 1, 1),
                Months = new List<AllocationMonthDto>
                {
                    new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 1, 1), ReleaseDate = new DateTime(2018, 1, 1) },
                    new AllocationMonthDto { AllocationId = 0, Percentage = 0, Date = new DateTime(2018, 2, 1), ReleaseDate = new DateTime(2018, 1, 1)},
                    new AllocationMonthDto { AllocationId = 0, Percentage = 50, Date = new DateTime(2018, 3, 1), ReleaseDate = new DateTime(2018, 1, 1)}
                }
            };

            allocationRepositoryMock.Setup(x => x.GetAllocationsBetweenDays(parameters.EmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new Collection<Allocation>());

            analyticRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            var response = sut.Add(parameters);

            Assert.True(!response.HasErrors());
        }

        [TestCase]
        public void ReleaseDateRequired()
        {
            var parameters = new AllocationDto
            {
                AnalyticId = 1,
                EmployeeId = 1,
                ReleaseDate = null,
                Months = new List<AllocationMonthDto>
                {
                    new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 1, 1) },
                }
            };

            analyticRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            var response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => $"{x.Folder}.{x.Code}" == Resources.AllocationManagement.Allocation.ReleaseDateIsRequired));

            parameters.ReleaseDate = DateTime.MinValue;

            response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => $"{x.Folder}.{x.Code}" == Resources.AllocationManagement.Allocation.ReleaseDateIsRequired));
        }

        [TestCase]
        public void AddThreeAllocation()
        {
            var parameters = new AllocationDto
            {
                AnalyticId = 1,
                EmployeeId = 1,
                ReleaseDate = new DateTime(2018, 1, 1),
                Months = new List<AllocationMonthDto>
                {
                    new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 1, 1) },
                    new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 2, 1) },
                    new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 3, 1) }
                }
            };

            analyticRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            allocationRepositoryMock.Setup(x => x.GetAllocationsBetweenDays(parameters.EmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new Collection<Allocation>());

            var response = sut.Add(parameters);

            Assert.False(response.HasErrors());

            allocationRepositoryMock.Verify(x => x.Insert(It.IsAny<Allocation>()), Times.Exactly(3));
            allocationRepositoryMock.Verify(s => s.Save(), Times.Once);
        }

        [TestCase]
        public void AddAllocationWith3AllocationsOverlapped()
        {
            var parameters = new AllocationDto
            {
                AnalyticId = 1,
                EmployeeId = 1,
                ReleaseDate = new DateTime(2018, 1, 1),
                Months = new List<AllocationMonthDto>
                {
                    new AllocationMonthDto { AllocationId = 0, Percentage = 25, Date = new DateTime(2018, 1, 1) },
                    new AllocationMonthDto { AllocationId = 0, Percentage = 25, Date = new DateTime(2018, 2, 1) },
                    new AllocationMonthDto { AllocationId = 0, Percentage = 25, Date = new DateTime(2018, 3, 1) }
                }
            };

            analyticRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            allocationRepositoryMock.Setup(x => x.GetAllocationsBetweenDays(parameters.EmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new Collection<Allocation>
                {
                    new Allocation { StartDate = new DateTime(2018, 1, 1), Percentage = 25 },
                    new Allocation { StartDate = new DateTime(2018, 2, 1), Percentage = 25 },
                    new Allocation { StartDate = new DateTime(2018, 3, 1), Percentage = 25 }
                });

            var response = sut.Add(parameters);

            Assert.False(response.HasErrors());

            allocationRepositoryMock.Verify(x => x.Insert(It.IsAny<Allocation>()), Times.Exactly(3));
            allocationRepositoryMock.Verify(s => s.Save(), Times.Once);
        }

        [TestCase]
        public void CannotAddAllocationWith3AllocationsOverlapped()
        {
            var parameters = new AllocationDto
            {
                AnalyticId = 1,
                EmployeeId = 1,
                ReleaseDate = new DateTime(2018, 1, 1),
                Months = new List<AllocationMonthDto>
                {
                    new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 1, 1) },
                    new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 2, 1) },
                    new AllocationMonthDto { AllocationId = 0, Percentage = 100, Date = new DateTime(2018, 3, 1) }
                }
            };

            analyticRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            allocationRepositoryMock.Setup(x => x.GetAllocationsBetweenDays(parameters.EmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new Collection<Allocation>
                {
                    new Allocation { StartDate = new DateTime(2018, 1, 1), Percentage = 25 },
                    new Allocation { StartDate = new DateTime(2018, 2, 1), Percentage = 25 },
                    new Allocation { StartDate = new DateTime(2018, 3, 1), Percentage = 25 }
                });

            var response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => $"{x.Folder}.{x.Code}" == Resources.AllocationManagement.Allocation.CannotBeAssign));

            allocationRepositoryMock.Verify(x => x.Insert(It.IsAny<Allocation>()), Times.Never);
            allocationRepositoryMock.Verify(s => s.Save(), Times.Never);
        }

        [TestCase]
        public void UpdateThreeAllocation()
        {
            var parameters = new AllocationDto
            {
                AnalyticId = 1,
                EmployeeId = 1,
                ReleaseDate = new DateTime(2018, 1, 1),
                Months = new List<AllocationMonthDto>
                {
                    new AllocationMonthDto { AllocationId = 1, Percentage = 100, Date = new DateTime(2018, 1, 1), Updated = true },
                    new AllocationMonthDto { AllocationId = 1, Percentage = 100, Date = new DateTime(2018, 2, 1), Updated = true },
                    new AllocationMonthDto { AllocationId = 1, Percentage = 100, Date = new DateTime(2018, 3, 1), Updated = true }
                }
            };

            analyticRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            allocationRepositoryMock.Setup(x => x.GetAllocationsBetweenDays(parameters.EmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new Collection<Allocation>());

            var response = sut.Add(parameters);

            Assert.False(response.HasErrors());

            allocationRepositoryMock.Verify(x => x.UpdatePercentage(It.IsAny<Allocation>()), Times.Exactly(3));
            allocationRepositoryMock.Verify(s => s.Save(), Times.Once);
        }


        [TestCase]
        public void UpdateJustOneAllocation()
        {
            var parameters = new AllocationDto
            {
                AnalyticId = 1,
                EmployeeId = 1,
                ReleaseDate = new DateTime(2018, 1, 1),
                Months = new List<AllocationMonthDto>
                {
                    new AllocationMonthDto { AllocationId = 1, Percentage = 100, Date = new DateTime(2018, 1, 1), Updated = true },
                    new AllocationMonthDto { AllocationId = 1, Percentage = 100, Date = new DateTime(2018, 2, 1), Updated = false },
                    new AllocationMonthDto { AllocationId = 1, Percentage = 100, Date = new DateTime(2018, 3, 1), Updated = false }
                }
            };

            analyticRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);
            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            allocationRepositoryMock.Setup(x => x.GetAllocationsBetweenDays(parameters.EmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new Collection<Allocation>());

            var response = sut.Add(parameters);

            Assert.False(response.HasErrors());

            allocationRepositoryMock.Verify(x => x.UpdatePercentage(It.IsAny<Allocation>()), Times.Exactly(1));
            allocationRepositoryMock.Verify(s => s.Save(), Times.Once);
        }
    }
}
