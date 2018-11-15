using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.Managers;

namespace Sofco.UnitTest.Framework.Managers
{
    [TestFixture]
    public class WorkTimeSendMailManagerTest
    {
        private const int ValidEmployeeId = 1;

        private const int ValidAnalyticId = 1;

        private const int ValidAnalyticId2 = 2;

        private const string ValidManagerEmail = "mail1@mail.com";

        private const string ValidUserApproverEmail = "mail2@mail.com";

        private Analytic validAnalytic;

        private Mock<IMailSender> mailSenderMock;

        private Mock<IEmployeeData> employeeDataMock;

        private Mock<IUnitOfWork> unitOfWorkMock;

        private Mock<ILogMailer<WorkTimeSendMailManager>> loggerMock;

        private Mock<ICloseDateRepository> closeDateRepositoryMock;

        private Mock<IAnalyticRepository> analyticRepositoryMock;

        private Mock<IWorkTimeRepository> workTimeRepositoryMock;

        private Mock<IUserApproverRepository> userApproverRepositoryMock;

        private IWorkTimeSendMailManager sut;

        [SetUp]
        public void SetUp()
        {
            mailSenderMock = new Mock<IMailSender>();

            employeeDataMock = new Mock<IEmployeeData>();

            unitOfWorkMock = new Mock<IUnitOfWork>();

            loggerMock = new Mock<ILogMailer<WorkTimeSendMailManager>>();

            sut = new WorkTimeSendMailManager(mailSenderMock.Object,
                employeeDataMock.Object,
                unitOfWorkMock.Object,
                loggerMock.Object);

            mailSenderMock.Setup(s => s.Send(It.IsAny<List<IMailData>>()));

            closeDateRepositoryMock = new Mock<ICloseDateRepository>();

            unitOfWorkMock.SetupGet(s => s.CloseDateRepository).Returns(closeDateRepositoryMock.Object);

            closeDateRepositoryMock.Setup(s => s.GetBeforeCurrentAndNext())
                .Returns(new CloseDatesSettings(1, 1, 1));

            analyticRepositoryMock = new Mock<IAnalyticRepository>();

            unitOfWorkMock.SetupGet(s => s.AnalyticRepository).Returns(analyticRepositoryMock.Object);

            analyticRepositoryMock.Setup(s =>
                    s.GetByAllocations(ValidEmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(GetAnalyticList());

            workTimeRepositoryMock = new Mock<IWorkTimeRepository>();

            unitOfWorkMock.SetupGet(s => s.WorkTimeRepository).Returns(workTimeRepositoryMock.Object);

            employeeDataMock.Setup(s => s.GetCurrentEmployee()).Returns(new Employee
            {
                Id = ValidEmployeeId,
                Name = "One"
            });

            userApproverRepositoryMock = new Mock<IUserApproverRepository>();

            userApproverRepositoryMock.Setup(s =>
                    s.GetApproverByEmployeeIdAndAnalyticId(ValidEmployeeId, ValidAnalyticId, UserApproverType.WorkTime))
                .Returns(new List<User>
                {
                    new User {Id = 1, Email = ValidUserApproverEmail}
                });
            userApproverRepositoryMock.Setup(s =>
                    s.GetApproverByEmployeeIdAndAnalyticId(ValidEmployeeId, ValidAnalyticId, UserApproverType.WorkTime))
                .Returns(new List<User>
                {
                    new User {Id = 1, Email = ValidUserApproverEmail}
                });

            userApproverRepositoryMock.Setup(s =>
                    s.GetApproverByEmployeeIdAndAnalyticId(ValidEmployeeId, ValidAnalyticId2, UserApproverType.WorkTime))
                .Returns(new List<User>());


            unitOfWorkMock.SetupGet(s => s.UserApproverRepository).Returns(userApproverRepositoryMock.Object);
        }

        private List<Analytic> GetAnalyticList()
        {
            return new List<Analytic>
            {
                new Analytic { Id = ValidAnalyticId, Name = "One", Manager = new User
                {
                    Email = ValidManagerEmail
                }},
                new Analytic { Id = ValidAnalyticId2, Name = "Two", Manager = new User
                {
                    Email = ValidManagerEmail
                }}
            };
        }

        private List<WorkTime> GetWorkTimes()
        {
            validAnalytic = new Analytic
            {
                Id = ValidAnalyticId,
                Name = "One"
            };

            var validTask = new Task { Id = 1, Description = "TaskOne" };

            return new List<WorkTime>
            {
                new WorkTime { Id = 1, AnalyticId = ValidAnalyticId, Date = DateTime.Now.AddDays(1), Analytic = validAnalytic, Task = validTask},
                new WorkTime { Id = 2, AnalyticId = ValidAnalyticId, Date = DateTime.Now.AddDays(2), Analytic = validAnalytic, Task = validTask}
            };
        }

        [Test]
        public void ShouldPassSendEmail()
        {
            var workTimes = GetWorkTimes();

            sut.SendEmail(workTimes);

            employeeDataMock.Verify(s => s.GetCurrentEmployee(), Times.Once);

            closeDateRepositoryMock.Verify(s => s.GetBeforeCurrentAndNext(), Times.Once);

            analyticRepositoryMock.Verify(s => s.GetByAllocations(ValidEmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>()));

            mailSenderMock.Verify(s => s.Send(It.IsAny<List<IMailData>>()), Times.Once);
        }

        [Test]
        public void ShouldFailSendEmail()
        {
            var workTimes = GetWorkTimes();

            analyticRepositoryMock.Setup(s =>
                    s.GetByAllocations(ValidEmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<Analytic>());

            sut.SendEmail(workTimes);

            employeeDataMock.Verify(s => s.GetCurrentEmployee(), Times.Once);

            closeDateRepositoryMock.Verify(s => s.GetBeforeCurrentAndNext(), Times.Once);

            analyticRepositoryMock.Verify(s => s.GetByAllocations(ValidEmployeeId, It.IsAny<DateTime>(), It.IsAny<DateTime>()));

            mailSenderMock.Verify(s => s.Send(It.IsAny<List<IMailData>>()), Times.Never);
        }

        [Test]
        public void ShouldPassGetMails()
        {
            var workTimes = GetWorkTimes();

            var testableSut = new WorkTimeSendMailManagerTestable(mailSenderMock.Object,
                employeeDataMock.Object, unitOfWorkMock.Object, loggerMock.Object);

            var actual = testableSut.GetMails(workTimes);

            Assert.IsNotEmpty(actual);

            Assert.AreEqual(1, actual.Count);
        }
    }

    internal class WorkTimeSendMailManagerTestable : WorkTimeSendMailManager
    {
        public WorkTimeSendMailManagerTestable(IMailSender mailSender, IEmployeeData employeeData, IUnitOfWork unitOfWork, ILogMailer<WorkTimeSendMailManager> logger) : base(mailSender, employeeData, unitOfWork, logger)
        {
        }

        internal new List<IMailData> GetMails(List<WorkTime> workTimes)
        {
            return base.GetMails(workTimes);
        }
    }
}
