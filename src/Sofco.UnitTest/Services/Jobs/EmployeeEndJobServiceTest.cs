﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Mail;
using Sofco.Domain;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Service.Implementations.Jobs;
using Sofco.Service.Settings.Jobs;

namespace Sofco.UnitTest.Services.Jobs
{
    [TestFixture]
    public class EmployeeEndJobServiceTest
    {
        const string MailTo = "mail1@mail.com";

        const string TimeZone = "Argentina Standard Time";

        const string PmoCode = "PMO";

        private EmployeeEndNotificationJobService sut;

        private Mock<IEmployeeRepository> employeeRepositoryMock;

        private Mock<IGroupRepository> groupRepositoryMock;

        private Mock<IMailBuilder> mailBuilderMock;

        private Mock<IMailSender> mailSenderMock;

        private Mock<IOptions<JobSetting>> jobSettingOptionMock;

        private Mock<IOptions<EmailConfig>> emailOptionMock;

        private Mock<IUnitOfWork> unitOfWork;

        [SetUp]
        public void Setup()
        {
            employeeRepositoryMock = new Mock<IEmployeeRepository>();
            groupRepositoryMock = new Mock<IGroupRepository>();
            mailBuilderMock = new Mock<IMailBuilder>();
            mailSenderMock = new Mock<IMailSender>();
            jobSettingOptionMock = new Mock<IOptions<JobSetting>>();
            emailOptionMock = new Mock<IOptions<EmailConfig>>();

            unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.EmployeeRepository).Returns(employeeRepositoryMock.Object);
            unitOfWork.Setup(x => x.GroupRepository).Returns(groupRepositoryMock.Object);

            jobSettingOptionMock.SetupGet(s => s.Value).Returns(
                new JobSetting
                {
                    EmployeeEndJob = new EmployeeEndJobSetting
                    {
                        DaysFromNow = 1
                    },
                    LocalTimeZoneName = TimeZone
                });

            emailOptionMock.SetupGet(s => s.Value).Returns(
                new EmailConfig
                {
                    PmoCode = PmoCode
                });

            groupRepositoryMock.Setup(s => s.GetEmail(PmoCode)).Returns(MailTo);

            mailSenderMock.Setup(s => s.Send(It.IsAny<Email>()));

            sut = new EmployeeEndNotificationJobService(unitOfWork.Object,
                mailBuilderMock.Object,
                mailSenderMock.Object,
                jobSettingOptionMock.Object,
                emailOptionMock.Object);
        }

        [Test]
        public void ShouldPassSendNotification()
        {
            employeeRepositoryMock.Setup(s => s.GetByEndDate(It.IsAny<DateTime>()))
                .Returns(new List<Employee>() { new Employee { Id = 1 } });

            sut.SendNotification();

            employeeRepositoryMock.Verify(s => s.GetByEndDate(It.IsAny<DateTime>()), Times.Once);

            groupRepositoryMock.Verify(s => s.GetEmail(PmoCode), Times.Once);

            mailSenderMock.Verify(s => s.Send(It.IsAny<Email>()), Times.Once);
        }

        [Test]
        public void ShouldFailSendNotification()
        {
            employeeRepositoryMock.Setup(s => s.GetByEndDate(It.IsAny<DateTime>()))
                .Returns(new List<Employee>());

            sut.SendNotification();

            employeeRepositoryMock.Verify(s => s.GetByEndDate(It.IsAny<DateTime>()), Times.Once);

            groupRepositoryMock.Verify(s => s.GetEmail(PmoCode), Times.Never);

            mailSenderMock.Verify(s => s.Send(It.IsAny<Email>()), Times.Never);
        }
    }
}
