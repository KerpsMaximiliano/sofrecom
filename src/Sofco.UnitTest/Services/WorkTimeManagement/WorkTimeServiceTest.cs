using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.Rrhh;
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

        private Mock<IEmployeeData> employeeDataMock;

        private Mock<IWorkTimeValidation> workTimeValidationMock;

        private Mock<IWorkTimeImportFileManager> workTimeFileManagerMock;

        private Mock<IWorkTimeExportFileManager> workTimeExportFileManagerMock;

        private Mock<IWorkTimeResumeManager> workTimeResumeMangerMock;

        private Mock<IWorkTimeRepository> workTimeRepositoryMock;

        private Mock<IWorkTimeRejectManager> workTimeRejectManagerMock;

        private Mock<IWorkTimeSendManager> workTimeSendManagerMock;

        private Mock<ILicenseGenerateWorkTimeService> licenseGenerateWorkTimeServiceMock;

        private Mock<IOptions<AppSetting>> appSettingMock;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogMailer<WorkTimeService>>();

            unitOfWorkMock = new Mock<IUnitOfWork>();

            userDataMock = new Mock<IUserData>();

            employeeDataMock = new Mock<IEmployeeData>();

            workTimeValidationMock = new Mock<IWorkTimeValidation>();

            workTimeFileManagerMock = new Mock<IWorkTimeImportFileManager>();

            workTimeExportFileManagerMock = new Mock<IWorkTimeExportFileManager>();

            workTimeResumeMangerMock = new Mock<IWorkTimeResumeManager>();

            workTimeRepositoryMock = new Mock<IWorkTimeRepository>();

            workTimeRepositoryMock.Setup(s => s.Get(It.IsAny<int>())).Returns(new WorkTime());

            unitOfWorkMock.Setup(s => s.WorkTimeRepository).Returns(workTimeRepositoryMock.Object);

            workTimeRejectManagerMock = new Mock<IWorkTimeRejectManager>();

            workTimeSendManagerMock = new Mock<IWorkTimeSendManager>();

            licenseGenerateWorkTimeServiceMock = new Mock<ILicenseGenerateWorkTimeService>();

            appSettingMock = new Mock<IOptions<AppSetting>>();

            sut = new WorkTimeService(loggerMock.Object, unitOfWorkMock.Object, userDataMock.Object, employeeDataMock.Object,
                workTimeValidationMock.Object, appSettingMock.Object, workTimeFileManagerMock.Object, workTimeExportFileManagerMock.Object, 
                workTimeResumeMangerMock.Object, licenseGenerateWorkTimeServiceMock.Object, workTimeRejectManagerMock.Object, workTimeSendManagerMock.Object);
        }

        [Test]
        public void ShouldPassApprove()
        {
            const int workTimeId = 1;

            var workTime = new WorkTime {Id = workTimeId, Status = WorkTimeStatus.Sent};

            workTimeRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<WorkTime, bool>>>())).Returns(workTime);

            userDataMock.Setup(s => s.GetCurrentUser()).Returns(new UserLiteModel{ Id = 1 });

            var actual = sut.Approve(workTimeId, new List<BankHoursSplitted>());

            Assert.False(actual.HasErrors());
        }

        [Test]
        public void ShouldPassApproveAll()
        {
            const int workTimeId1 = 1;

            const int workTimeId2 = 2;

            var workTimes = new List<HoursToApproveModel>
            {
                new HoursToApproveModel{ Id = workTimeId1, HoursSplitteds = new List<BankHoursSplitted>()},
                new HoursToApproveModel{ Id = workTimeId2, HoursSplitteds = new List<BankHoursSplitted>()}
            };

            workTimeRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<WorkTime, bool>>>()))
                .Returns(
                    (Expression<Func<WorkTime, bool>> predicate) 
                    => new WorkTime { Id = 1, Status = WorkTimeStatus.Sent });

            userDataMock.Setup(s => s.GetCurrentUser()).Returns(new UserLiteModel { Id = 1 });

            var actual = sut.ApproveAll(workTimes);

            Assert.False(actual.HasErrors());

            var firstMessage = actual.Messages.First();
            var actualMessage = firstMessage.Text;
            var expectedMessage = Resources.WorkTimeManagement.WorkTime.ApprovedSuccess;

            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [Test]
        public void ShouldFailApproveAll()
        {
            const int workTimeId1 = 1;

            const int workTimeId2 = 2;

            var workTimes = new List<HoursToApproveModel>
            {
                new HoursToApproveModel{ Id = workTimeId1, HoursSplitteds = new List<BankHoursSplitted>()},
                new HoursToApproveModel{ Id = workTimeId2, HoursSplitteds = new List<BankHoursSplitted>()}
            };

            workTimeRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<WorkTime, bool>>>()))
                .Returns(
                    (Expression<Func<WorkTime, bool>> predicate) 
                    => new WorkTime { Id = 1, Status = WorkTimeStatus.Approved });

            userDataMock.Setup(s => s.GetCurrentUser()).Returns(new UserLiteModel { Id = 1 });

            var actual = sut.ApproveAll(workTimes);

            Assert.True(actual.HasErrors());

            var firstMessage = actual.Messages.First(x => x.Type == MessageType.Error);
            var actualMessage = firstMessage.Text;
            var expectedMessage = Resources.Common.ErrorSave;

            Assert.AreEqual(expectedMessage, actualMessage);
        }
    }
}
