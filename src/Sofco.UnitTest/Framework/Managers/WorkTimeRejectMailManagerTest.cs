using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.Managers;

namespace Sofco.UnitTest.Framework.Managers
{
    [TestFixture]
    public class WorkTimeRejectMailManagerTest
    {
        private Mock<IUnitOfWork> unitOfWork;

        private Mock<ILogMailer<WorkTimeRejectMailManager>> loggerMock;

        private Mock<IMailBuilder> mailBuilderMock;

        private Mock<IMailSender> mailSenderMock;

        private Mock<IEmployeeRepository> employeeRepositoryMock;

        private Mock<IAnalyticRepository> analyticRepositoryMock;

        private Mock<ITaskRepository> taskRepositoryMock;

        private Mock<IUserApproverRepository> userApproverRepositoryMock;

        private WorkTime validWorkTime;

        private IWorkTimeRejectMailManager sut;

        [SetUp]
        public void Setup()
        {
            unitOfWork = new Mock<IUnitOfWork>();

            employeeRepositoryMock = new Mock<IEmployeeRepository>();

            employeeRepositoryMock.Setup(s => s.GetById(It.IsAny<int>())).Returns(new Employee());

            analyticRepositoryMock = new Mock<IAnalyticRepository>();

            analyticRepositoryMock.Setup(s => s.GetById(It.IsAny<int>())).Returns(new Analytic
            {
                Manager = new User()
            });

            taskRepositoryMock = new Mock<ITaskRepository>();

            taskRepositoryMock.Setup(s => s.GetById(It.IsAny<int>())).Returns(new Task());

            userApproverRepositoryMock = new Mock<IUserApproverRepository>();

            userApproverRepositoryMock.Setup(s => s.GetApproverByEmployeeIdAndAnalyticId(
                    It.IsAny<int>(), It.IsAny<int>(), UserApproverType.WorkTime))
                .Returns(new List<User>());

            unitOfWork.SetupGet(s => s.EmployeeRepository).Returns(employeeRepositoryMock.Object);
            unitOfWork.SetupGet(s => s.AnalyticRepository).Returns(analyticRepositoryMock.Object);
            unitOfWork.SetupGet(s => s.TaskRepository).Returns(taskRepositoryMock.Object);
            unitOfWork.SetupGet(s => s.UserApproverRepository).Returns(userApproverRepositoryMock.Object);

            loggerMock = new Mock<ILogMailer<WorkTimeRejectMailManager>>();

            mailBuilderMock = new Mock<IMailBuilder>();

            mailSenderMock = new Mock<IMailSender>();

            validWorkTime = new WorkTime
            {
                Id = 1,
                Status = WorkTimeStatus.Sent,
                EmployeeId = 1,
                AnalyticId = 1
            };

            sut = new WorkTimeRejectMailManager(unitOfWork.Object, loggerMock.Object, 
                mailBuilderMock.Object, mailSenderMock.Object);
        }

        [Test]
        public void ShouldPassReject()
        {
            sut.SendEmail(validWorkTime);

            employeeRepositoryMock.Verify(s => s.GetById(It.IsAny<int>()), Times.Once);
            analyticRepositoryMock.Verify(s => s.GetById(It.IsAny<int>()), Times.Once);
            taskRepositoryMock.Verify(s => s.GetById(It.IsAny<int>()), Times.Once);
            userApproverRepositoryMock.Verify(s => s
                .GetApproverByEmployeeIdAndAnalyticId(It.IsAny<int>(), It.IsAny<int>(), UserApproverType.WorkTime), Times.Once);
        }
    }
}
