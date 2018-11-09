using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Validations;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Managers;

namespace Sofco.UnitTest.Framework.Managers
{
    [TestFixture]
    public class WorkTimeSendManagerTest
    {
        private const int ValidEmployeeId = 1;

        private Mock<IUnitOfWork> unitOfWorkMock;

        private Mock<IUserData> userDataMock;

        private Mock<IEmployeeData> employeeDataMock;

        private Mock<ILogMailer<WorkTimeSendManager>> loggerMock;

        private Mock<IWorkTimeSendMailManager> workTimeSendMailManagerMock;

        private Mock<IWorkTimeRepository> workTimeRepositoryMock;

        private Mock<IUserRepository> userRepositoryMock;

        private Mock<IWorkTimeValidation> workTimeValidationMock;

        private UserLiteModel currentUserModel;

        private IWorkTimeSendManager sut;

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();

            userDataMock = new Mock<IUserData>();

            employeeDataMock = new Mock<IEmployeeData>();

            loggerMock = new Mock<ILogMailer<WorkTimeSendManager>>();

            workTimeSendMailManagerMock = new Mock<IWorkTimeSendMailManager>();

            workTimeValidationMock = new Mock<IWorkTimeValidation>();

            sut = new WorkTimeSendManager(workTimeSendMailManagerMock.Object,
                userDataMock.Object,
                unitOfWorkMock.Object,  
                employeeDataMock.Object, 
                loggerMock.Object,
                workTimeValidationMock.Object);

            currentUserModel = new UserLiteModel
            {
                Id = 1,
                Email = "spawn@sofrecom.com.ar",
                ManagerId = "1",
                Name = "Spawn",
                UserName = "spawn"
            };

            userDataMock.Setup(s => s.GetCurrentUser()).Returns(currentUserModel);

            userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(s => s.HasManagerGroup(currentUserModel.UserName)).Returns(false);

            unitOfWorkMock.SetupGet(s => s.UserRepository).Returns(userRepositoryMock.Object);

            var currentEmployee = new Employee
            {
                Id = ValidEmployeeId
            };

            employeeDataMock.Setup(s => s.GetCurrentEmployee()).Returns(currentEmployee);

            workTimeRepositoryMock = new Mock<IWorkTimeRepository>();

            workTimeRepositoryMock.Setup(s => s.GetWorkTimeDraftByEmployeeId(It.IsAny<int>())).Returns(new List<WorkTime>
            {
                new WorkTime{Id = 1}
            });

            unitOfWorkMock.SetupGet(s => s.WorkTimeRepository).Returns(workTimeRepositoryMock.Object);

            workTimeValidationMock.Setup(s =>
                s.ValidateAllocations(It.IsAny<Response>(), It.IsAny<WorkTimeAddModel>()));
        }

        [Test]
        public void ShouldPassSend()
        {
            var actual = sut.Send();

            Assert.False(actual.HasErrors());

            workTimeRepositoryMock.Verify(s => s.SendHours(ValidEmployeeId), Times.Once);
            workTimeRepositoryMock.Verify(s => s.SendManagerHours(ValidEmployeeId), Times.Never);

            workTimeSendMailManagerMock.Verify(s => s.SendEmail(It.IsAny<List<WorkTime>>()), Times.Once);
        }

        [Test]
        public void ShouldPassSendByManager()
        {
            userRepositoryMock.Setup(s => s.HasManagerGroup(currentUserModel.UserName)).Returns(true);

            var actual = sut.Send();

            Assert.False(actual.HasErrors());

            workTimeRepositoryMock.Verify(s => s.SendHours(ValidEmployeeId), Times.Never);
            workTimeRepositoryMock.Verify(s => s.SendManagerHours(ValidEmployeeId), Times.Once);

            workTimeSendMailManagerMock.Verify(s => s.SendEmail(It.IsAny<List<WorkTime>>()), Times.Never);
        }

        [Test]
        public void ShouldFailSend()
        {
            workTimeRepositoryMock.Setup(s => s.SendHours(ValidEmployeeId)).Throws(new Exception());

            var actual = sut.Send();

            Assert.True(actual.HasErrors());

            workTimeRepositoryMock.Verify(s => s.SendHours(ValidEmployeeId), Times.Once);
            workTimeRepositoryMock.Verify(s => s.SendManagerHours(ValidEmployeeId), Times.Never);

            workTimeSendMailManagerMock.Verify(s => s.SendEmail(It.IsAny<List<WorkTime>>()), Times.Never);
        }
    }
}
