using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Validations;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Service.Implementations.WorkTimeManagement;

namespace Sofco.UnitTest.Services.WorkTimeManagement
{
    [TestFixture]
    public class WorkTimeServiceTest
    {
        private WorkTimeService sut;

        private Mock<ILogMailer<WorkTimeService>> loggerMock;

        private Mock<IUnitOfWork> unitOfWorkMock;

        private Mock<IUserData> userDataMock;

        private Mock<IHostingEnvironment> hostingEnvironmentMock;

        private Mock<IEmployeeData> employeeDataMock;

        private Mock<IWorkTimeValidation> workTimeValidationMock;

        private Mock<IWorkTimeFileManager> workTimeFileManagerMock;

        private Mock<IWorkTimeResumeManager> workTimeResumeMangerMock;

        private Mock<IMailSender> mailSenderMock;

        private Mock<IMailBuilder> mailBuilderMock;

        private Mock<IWorkTimeRepository> workTimeRepositoryMock;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogMailer<WorkTimeService>>();

            unitOfWorkMock = new Mock<IUnitOfWork>();

            userDataMock = new Mock<IUserData>();

            hostingEnvironmentMock = new Mock<IHostingEnvironment>();

            employeeDataMock = new Mock<IEmployeeData>();

            workTimeValidationMock = new Mock<IWorkTimeValidation>();

            workTimeFileManagerMock = new Mock<IWorkTimeFileManager>();

            workTimeResumeMangerMock = new Mock<IWorkTimeResumeManager>();

            mailSenderMock = new Mock<IMailSender>();

            mailBuilderMock = new Mock<IMailBuilder>();

            workTimeRepositoryMock = new Mock<IWorkTimeRepository>();

            workTimeRepositoryMock.Setup(s => s.Get(It.IsAny<int>())).Returns(new WorkTime());

            unitOfWorkMock.Setup(s => s.WorkTimeRepository).Returns(workTimeRepositoryMock.Object);

            sut = new WorkTimeService(loggerMock.Object, unitOfWorkMock.Object, userDataMock.Object, hostingEnvironmentMock.Object, employeeDataMock.Object, workTimeValidationMock.Object, workTimeFileManagerMock.Object, workTimeResumeMangerMock.Object, mailSenderMock.Object, mailBuilderMock.Object);
        }

        [Test]
        public void ShouldPassApprove()
        {
            const int workTimeId = 1;

            var workTime = new WorkTime {Id = workTimeId, Status = WorkTimeStatus.Sent};

            workTimeRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<WorkTime, bool>>>())).Returns(workTime);

            userDataMock.Setup(s => s.GetCurrentUser()).Returns(new UserLiteModel{ Id = 1 });

            var actual = sut.Approve(workTimeId);

            Assert.False(actual.HasErrors());
        }

        [Test]
        public void ShouldPassApproveAll()
        {
            const int workTimeId1 = 1;

            const int workTimeId2 = 2;

            var workTimes = new List<int> { workTimeId1, workTimeId2 };

            workTimeRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<WorkTime, bool>>>()))
                .Returns(
                    (Expression<Func<WorkTime, bool>> predicate) 
                    => new WorkTime { Id = 1, Status = WorkTimeStatus.Sent });

            userDataMock.Setup(s => s.GetCurrentUser()).Returns(new UserLiteModel { Id = 1 });

            var actual = sut.ApproveAll(workTimes);

            Assert.False(actual.HasErrors());

            var firstMessage = actual.Messages.First();
            var actualMessage = firstMessage.Folder + "." + firstMessage.Code;
            var expectedMessage = Resources.WorkTimeManagement.WorkTime.ApprovedSuccess;

            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [Test]
        public void ShouldFailApproveAll()
        {
            const int workTimeId1 = 1;

            const int workTimeId2 = 2;

            var workTimes = new List<int> { workTimeId1, workTimeId2 };

            workTimeRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<WorkTime, bool>>>()))
                .Returns(
                    (Expression<Func<WorkTime, bool>> predicate) 
                    => new WorkTime { Id = 1, Status = WorkTimeStatus.Approved });

            userDataMock.Setup(s => s.GetCurrentUser()).Returns(new UserLiteModel { Id = 1 });

            var actual = sut.ApproveAll(workTimes);

            Assert.True(actual.HasErrors());

            var firstMessage = actual.Messages.First(x => x.Type == MessageType.Error);
            var actualMessage = firstMessage.Folder + "." + firstMessage.Code;
            var expectedMessage = Resources.Common.ErrorSave;

            Assert.AreEqual(expectedMessage, actualMessage);
        }
    }
}
