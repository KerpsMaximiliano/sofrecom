using Moq;
using NUnit.Framework;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Models.TimeManagement;
using Sofco.Service.Implementations.AllocationManagement;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

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
        public void DatesOutOfRange()
        {
            var parameters = new AllocationAsignmentParams { AnalyticId = 1, Percentage = 100, DateSince = new DateTime(2018, 06, 1), DateTo = new DateTime(2018, 06, 30), EmployeeId = 1 };

            analyticRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Analytic, bool>>>())).Returns(new Analytic { Id = 1, StartDateContract = new DateTime(2017, 1, 1), EndDateContract = new DateTime(2017, 12, 31) });

            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            var response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => x.Description == Resources.es.AllocationManagement.Allocation.DateSinceOutOfRange));
            Assert.True(response.Messages.Any(x => x.Description == Resources.es.AllocationManagement.Allocation.DateToOutOfRange));
        }

        [TestCase]
        public void ValidatePercentage()
        {
            var parameters = new AllocationAsignmentParams { AnalyticId = 1, Percentage = 0, DateSince = new DateTime(2018, 06, 1), DateTo = new DateTime(2018, 06, 30), EmployeeId = 1 };

            analyticRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Analytic, bool>>>())).Returns(new Analytic { Id = 1, StartDateContract = new DateTime(2017, 1, 1), EndDateContract = new DateTime(2017, 12, 31) });

            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            var response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => x.Description == Resources.es.AllocationManagement.Allocation.WrongPercentage));

            parameters.Percentage = 101;

            response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => x.Description == Resources.es.AllocationManagement.Allocation.WrongPercentage));
        }

        [TestCase]
        public void DatesRequired()
        {
            var parameters = new AllocationAsignmentParams { AnalyticId = 1, Percentage = 100, EmployeeId = 1 };

            analyticRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Analytic, bool>>>())).Returns(new Analytic { Id = 1, StartDateContract = new DateTime(2017, 1, 1), EndDateContract = new DateTime(2017, 12, 31) });

            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            var response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => x.Description == Resources.es.AllocationManagement.Allocation.DateSinceRequired));
            Assert.True(response.Messages.Any(x => x.Description == Resources.es.AllocationManagement.Allocation.DateToRequired));
        }

        [TestCase]
        public void DateToLessThanDateSince()
        {
            var parameters = new AllocationAsignmentParams { AnalyticId = 1, Percentage = 100, EmployeeId = 1, DateSince = new DateTime(2018, 06, 1), DateTo = new DateTime(2017, 06, 30) };

            analyticRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Analytic, bool>>>())).Returns(new Analytic { Id = 1, StartDateContract = new DateTime(2017, 1, 1), EndDateContract = new DateTime(2017, 12, 31) });

            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            var response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => x.Description == Resources.es.AllocationManagement.Allocation.DateToLessThanDateSince));
        }

        [TestCase]
        public void AddAllocation()
        {
            var parameters = new AllocationAsignmentParams { AnalyticId = 1, Percentage = 100, EmployeeId = 1, DateSince = new DateTime(2017, 06, 1), DateTo = new DateTime(2017, 06, 30) };

            analyticRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Analytic, bool>>>())).Returns(new Analytic { Id = 1, StartDateContract = new DateTime(2017, 1, 1), EndDateContract = new DateTime(2017, 12, 31) });

            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            allocationRepositoryMock.Setup(x => x.GetBetweenDaysByEmployeeId(parameters.EmployeeId, parameters.DateSince.Value, parameters.DateTo.Value))
                .Returns(new Collection<Allocation>());

            var response = sut.Add(parameters);

            Assert.False(response.HasErrors());

            allocationRepositoryMock.Verify(x => x.Insert(It.IsAny<Allocation>()), Times.Once);
            allocationRepositoryMock.Verify(s => s.Save(), Times.Once);
        }

        [TestCase]
        public void AddAllocationWith3AllocationsOverlapped()
        {
            var parameters = new AllocationAsignmentParams { AnalyticId = 1, Percentage = 50, EmployeeId = 1, DateSince = new DateTime(2017, 06, 1), DateTo = new DateTime(2017, 06, 30) };

            analyticRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Analytic, bool>>>())).Returns(new Analytic { Id = 1, StartDateContract = new DateTime(2017, 1, 1), EndDateContract = new DateTime(2017, 12, 31) });

            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            allocationRepositoryMock.Setup(x => x.GetBetweenDaysByEmployeeId(parameters.EmployeeId, parameters.DateSince.Value, parameters.DateTo.Value))
                .Returns(new Collection<Allocation>
                {
                    new Allocation { StartDate = new DateTime(2017, 6, 1), EndDate = new DateTime(2017, 6, 10), Percentage = 25 },
                    new Allocation { StartDate = new DateTime(2017, 6, 11), EndDate = new DateTime(2017, 6, 20), Percentage = 25 },
                    new Allocation { StartDate = new DateTime(2017, 6, 21), EndDate = new DateTime(2017, 6, 30), Percentage = 25 }
                });

            var response = sut.Add(parameters);

            Assert.False(response.HasErrors());

            allocationRepositoryMock.Verify(x => x.Insert(It.IsAny<Allocation>()), Times.Once);
            allocationRepositoryMock.Verify(s => s.Save(), Times.Once);
        }

        [TestCase]
        public void CannotAddAllocationWith3AllocationsOverlapped()
        {
            var parameters = new AllocationAsignmentParams { AnalyticId = 1, Percentage = 100, EmployeeId = 1, DateSince = new DateTime(2017, 06, 1), DateTo = new DateTime(2017, 06, 30) };

            analyticRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Expression<Func<Analytic, bool>>>())).Returns(new Analytic { Id = 1, StartDateContract = new DateTime(2017, 1, 1), EndDateContract = new DateTime(2017, 12, 31) });

            employeeRepositoryMock.Setup(x => x.Exist(It.IsAny<int>())).Returns(true);

            allocationRepositoryMock.Setup(x => x.GetBetweenDaysByEmployeeId(parameters.EmployeeId, parameters.DateSince.Value, parameters.DateTo.Value))
                .Returns(new Collection<Allocation>
                {
                    new Allocation { StartDate = new DateTime(2017, 6, 1), EndDate = new DateTime(2017, 6, 10), Percentage = 25 },
                    new Allocation { StartDate = new DateTime(2017, 6, 11), EndDate = new DateTime(2017, 6, 20), Percentage = 25 },
                    new Allocation { StartDate = new DateTime(2017, 6, 21), EndDate = new DateTime(2017, 6, 30), Percentage = 25 }
                });

            var response = sut.Add(parameters);

            Assert.True(response.HasErrors());
            Assert.True(response.Messages.Any(x => x.Description == Resources.es.AllocationManagement.Allocation.CannotBeAssign));

            allocationRepositoryMock.Verify(x => x.Insert(It.IsAny<Allocation>()), Times.Never);
            allocationRepositoryMock.Verify(s => s.Save(), Times.Never);
        }
    }
}
