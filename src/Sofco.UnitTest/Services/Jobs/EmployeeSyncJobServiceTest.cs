using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Repository.Rh.Repositories.Interfaces;
using Sofco.Service.Implementations.Jobs;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.UnitTest.Services.Jobs
{
    [TestFixture]
    public class EmployeeSyncJobServiceTest
    {
        private EmployeeSyncJobService sut;

        private Mock<ITigerEmployeeRepository> tigerEmployeeRepositoryMock;

        private Mock<IEmployeeRepository> employeeRepositoryMock;

        private Mock<IEmployeeSyncActionRepository> employeeSyncActionRepositoryMock;

        private Mock<IMapper> mapperMock;

        private Mock<IUnitOfWork> unitOfWork;

        private Mock<ILogMailer<EmployeeSyncJobService>> logger;

        private Mock<IMailSender> mailSender;

        private Mock<IMailBuilder> mailBuilder;

        private Mock<IOptions<EmailConfig>> emailOptionMock;

        [SetUp]
        public void Setup()
        {
            tigerEmployeeRepositoryMock = new Mock<ITigerEmployeeRepository>();

            employeeSyncActionRepositoryMock = new Mock<IEmployeeSyncActionRepository>();

            employeeRepositoryMock = new Mock<IEmployeeRepository>();

            mapperMock = new Mock<IMapper>();

            unitOfWork = new Mock<IUnitOfWork>();

            logger = new Mock<ILogMailer<EmployeeSyncJobService>>();

            mailSender = new Mock<IMailSender>();

            mailBuilder = new Mock<IMailBuilder>();

            emailOptionMock = new Mock<IOptions<EmailConfig>>();

            unitOfWork.SetupGet(x => x.EmployeeRepository).Returns(employeeRepositoryMock.Object);

            unitOfWork.SetupGet(x => x.EmployeeSyncActionRepository).Returns(employeeSyncActionRepositoryMock.Object);

            sut = new EmployeeSyncJobService(
                tigerEmployeeRepositoryMock.Object,
                unitOfWork.Object,
                mapperMock.Object,
                logger.Object,
                mailSender.Object,
                emailOptionMock.Object,
                mailBuilder.Object);
        }

        [Test]
        public void ShouldPassSyncNewEmployeesTest()
        {
            var tigerEmployees = new List<TigerEmployee> {new TigerEmployee
            {
                Legaj = 1, Feiem = DateTime.UtcNow.AddDays(-1)
            }};

            var storedEmployees = new List<Employee> {new Employee
            {
                EmployeeNumber = "2", StartDate = DateTime.UtcNow.AddYears(-1)
            }};

            tigerEmployeeRepositoryMock.Setup(s => s.GetWithStartDate(It.IsAny<DateTime>()))
                .Returns(tigerEmployees);

            mapperMock.Setup(s => s.Map<List<TigerEmployee>, List<Employee>>(It.IsAny<List<TigerEmployee>>()))
                .Returns(storedEmployees);

            mapperMock.Setup(s => s.Map<List<Employee>, List<EmployeeSyncAction>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeSyncAction>());

            employeeRepositoryMock.Setup(s => s.GetByEmployeeNumber(It.IsAny<string[]>()))
                .Returns(new List<Employee>());

            sut.SyncNewEmployees();

            tigerEmployeeRepositoryMock.Verify(s => s.GetWithStartDate(It.IsAny<DateTime>()), Times.Once);

            employeeSyncActionRepositoryMock.Verify(s => s.Save(It.IsAny<List<EmployeeSyncAction>>()), Times.Once);

            employeeRepositoryMock.Verify(s => s.GetByEmployeeNumber(It.IsAny<string[]>()), Times.Once);
        }

        [Test]
        public void ShouldPassSyncEndEmployeesTest()
        {
            tigerEmployeeRepositoryMock.Setup(s => s.GetWithEndDate(It.IsAny<DateTime>()))
                .Returns(new List<TigerEmployee>());

            mapperMock.Setup(s => s.Map<List<TigerEmployee>, List<Employee>>(It.IsAny<List<TigerEmployee>>()))
                .Returns(new List<Employee>());

            mapperMock.Setup(s => s.Map<List<Employee>, List<EmployeeSyncAction>>(It.IsAny<List<Employee>>()))
                .Returns(new List<EmployeeSyncAction>());

            employeeRepositoryMock.Setup(s => s.GetByEmployeeNumber(It.IsAny<string[]>()))
                .Returns(new List<Employee>());

            sut.SyncEndEmployees();

            tigerEmployeeRepositoryMock.Verify(s => s.GetWithEndDate(It.IsAny<DateTime>()), Times.Once);

            employeeSyncActionRepositoryMock.Verify(s => s.Save(It.IsAny<List<EmployeeSyncAction>>()), Times.Once);

            employeeRepositoryMock.Verify(s => s.GetByEmployeeNumber(It.IsAny<string[]>()), Times.Once);
        }
    }
}
