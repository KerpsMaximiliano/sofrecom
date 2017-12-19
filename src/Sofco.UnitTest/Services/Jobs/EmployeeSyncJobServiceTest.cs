using System;
using System.Collections.Generic;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL;
using Sofco.Repository.Rh.Repositories.Interfaces;
using Sofco.Service.Implementations.Jobs;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.UnitTest.Services.Jobs
{
    [TestFixture]
    public class EmployeeSyncJobServiceTest
    {
        private EmployeeSyncJobService sut;

        private Mock<ITigerEmployeeRepository> tigerEmployeeRepositoryMock;

        private Mock<IRhproEmployeeRepository> rhproEmployeeRepositoryMock;

        private Mock<IEmployeeRepository> employeeRepositoryMock;

        private Mock<ILicenseTypeRepository> licenseTypeRepositoryMock;

        private Mock<IEmployeeLicenseRepository> employeeLicenseRepositoryMock;

        private Mock<IMapper> mapperMock;

        private Mock<IUnitOfWork> unitOfWork;

        [SetUp]
        public void Setup()
        {
            tigerEmployeeRepositoryMock = new Mock<ITigerEmployeeRepository>();
            rhproEmployeeRepositoryMock = new Mock<IRhproEmployeeRepository>();
            employeeRepositoryMock = new Mock<IEmployeeRepository>();
            licenseTypeRepositoryMock = new Mock<ILicenseTypeRepository>();
            employeeLicenseRepositoryMock = new Mock<IEmployeeLicenseRepository>();
            mapperMock = new Mock<IMapper>();

            unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.EmployeeRepository).Returns(employeeRepositoryMock.Object);
            unitOfWork.Setup(x => x.LicenseTypeRepository).Returns(licenseTypeRepositoryMock.Object);
            unitOfWork.Setup(x => x.EmployeeLicenseRepository).Returns(employeeLicenseRepositoryMock.Object);

            sut = new EmployeeSyncJobService(
                tigerEmployeeRepositoryMock.Object,
                rhproEmployeeRepositoryMock.Object,
                unitOfWork.Object,
                mapperMock.Object);
        }

        [Test]
        public void ShouldPassSyncTest()
        {
            rhproEmployeeRepositoryMock.Setup(s => s.GetLicenseTypes())
                .Returns(new List<RhproLicenseType>());
            rhproEmployeeRepositoryMock.Setup(s => s.GetEmployeeLicensesWithStartDate(It.IsAny<DateTime>()))
                .Returns(new List<RhproEmployeeLicense>());
            employeeLicenseRepositoryMock.Setup(s => s.Delete(It.IsAny<List<EmployeeLicense>>(), It.IsAny<DateTime>()));
            employeeLicenseRepositoryMock.Setup(s => s.Save(It.IsAny<List<EmployeeLicense>>()));
            tigerEmployeeRepositoryMock.Setup(s => s.GetWithStartDate(It.IsAny<DateTime>()))
                .Returns(new List<TigerEmployee>());
            licenseTypeRepositoryMock.Setup(s => s.Save(It.IsAny<List<LicenseType>>()));
            employeeRepositoryMock.Setup(s => s.Save(It.IsAny<List<Employee>>()));

            sut.Sync();

            rhproEmployeeRepositoryMock.Verify(s => s.GetLicenseTypes(), Times.Once);
            rhproEmployeeRepositoryMock.Verify(s => s.GetEmployeeLicensesWithStartDate(It.IsAny<DateTime>()), Times.Once);
            employeeLicenseRepositoryMock.Verify(s => s.Delete(It.IsAny<List<EmployeeLicense>>(), It.IsAny<DateTime>()), Times.Once);
            employeeLicenseRepositoryMock.Verify(s => s.Save(It.IsAny<List<EmployeeLicense>>()), Times.Once);
            tigerEmployeeRepositoryMock.Verify(s => s.GetWithStartDate(It.IsAny<DateTime>()), Times.Once);
            licenseTypeRepositoryMock.Verify(s => s.Save(It.IsAny<List<LicenseType>>()), Times.Once);
            employeeRepositoryMock.Verify(s => s.Save(It.IsAny<List<Employee>>()), Times.Once);
        }
    }
}
