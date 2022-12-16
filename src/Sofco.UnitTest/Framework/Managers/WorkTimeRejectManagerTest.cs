using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.Managers;

namespace Sofco.UnitTest.Framework.Managers
{
    [TestFixture]
    public class WorkTimeRejectManagerTest
    {
        private Mock<IUnitOfWork> unitOfWork;

        private Mock<IWorkTimeRepository> workTimeRepositoryMock;

        private Mock<IUserData> userDataMock;

        private Mock<ILogMailer<WorkTimeRejectManager>> loggerMock;

        private Mock<IWorkTimeRejectMailManager> workTimeRejectMailManagerMock;

        private IWorkTimeRejectManager sut;

        private UserLiteModel currentUserModel;

        private WorkTime validWorkTime;

        private WorkTime inValidWorkTime;

        [SetUp]
        public void Setup()
        {
            unitOfWork = new Mock<IUnitOfWork>();

            workTimeRepositoryMock = new Mock<IWorkTimeRepository>();

            unitOfWork.SetupGet(s => s.WorkTimeRepository).Returns(workTimeRepositoryMock.Object);

            userDataMock = new Mock<IUserData>();

            loggerMock = new Mock<ILogMailer<WorkTimeRejectManager>>();

            currentUserModel = new UserLiteModel
            {
                Id = 1, Email = "spawn@sofredigital.com.ar", ManagerId = "1",
                Name = "Spawn", UserName = "spawn"
            };

            userDataMock.Setup(s => s.GetCurrentUser()).Returns(currentUserModel);

            workTimeRejectMailManagerMock = new Mock<IWorkTimeRejectMailManager>();

            validWorkTime = new WorkTime
            {
                Id = 1, Status = WorkTimeStatus.Sent,
                EmployeeId = 1, AnalyticId = 1
            };

            inValidWorkTime = new WorkTime
            {
                Id = 2, Status = WorkTimeStatus.Rejected,
                EmployeeId = 0, AnalyticId = 0
            };

            sut = new WorkTimeRejectManager(unitOfWork.Object, userDataMock.Object,
                loggerMock.Object, workTimeRejectMailManagerMock.Object);
        }

        [Test]
        public void ShouldPassReject()
        {
            const string rejectComment = "One";

            workTimeRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<WorkTime, bool>>>()))
                .Returns(validWorkTime);

            var actualResponse = sut.Reject(validWorkTime.Id, rejectComment, false);

            Assert.False(actualResponse.HasErrors());

            workTimeRepositoryMock.Verify(s => s.UpdateStatus(validWorkTime), Times.Once);
            workTimeRepositoryMock.Verify(s => s.UpdateApprovalComment(validWorkTime), Times.Once);
            unitOfWork.Verify(s => s.Save(), Times.Once);
            workTimeRejectMailManagerMock.Verify(s => s.SendEmail(validWorkTime), Times.Once);
        }

        [Test]
        public void ShouldFailReject()
        {
            const string rejectComment = "One";

            workTimeRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<WorkTime, bool>>>()))
                .Returns(inValidWorkTime);

            var actualResponse = sut.Reject(validWorkTime.Id, rejectComment, false);

            Assert.True(actualResponse.HasErrors());

            workTimeRepositoryMock.Verify(s => s.UpdateStatus(validWorkTime), Times.Never);
            workTimeRepositoryMock.Verify(s => s.UpdateApprovalComment(validWorkTime), Times.Never);
            unitOfWork.Verify(s => s.Save(), Times.Never);
            workTimeRejectMailManagerMock.Verify(s => s.SendEmail(validWorkTime), Times.Never);
        }
    }
}
