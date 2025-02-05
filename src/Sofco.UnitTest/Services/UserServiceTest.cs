﻿using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Service.Implementations.Admin;

namespace Sofco.UnitTest.Services
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IUnitOfWork> unitOfWork;
        private Mock<ILogMailer<UserService>> loggerMock;
        private Mock<ISessionManager> sessionManagerMock;
        private Mock<IMapper> mapperMock;
        private Mock<IOptions<AppSetting>> appSettingMock;
        private Mock<IMailSender> mailSenderMock;
        private Mock<IMailBuilder> mailBuilderMock;
        private Mock<IOptions<EmailConfig>> emailConfigMock;
        private UserService sut;

        [SetUp]
        public void Setup()
        {
            userRepositoryMock = new Mock<IUserRepository>();

            unitOfWork = new Mock<IUnitOfWork>();

            mailSenderMock = new Mock<IMailSender>();
            mailBuilderMock = new Mock<IMailBuilder>();
            appSettingMock = new Mock<IOptions<AppSetting>>();
            emailConfigMock = new Mock<IOptions<EmailConfig>>();

            unitOfWork.Setup(x => x.UserRepository).Returns(userRepositoryMock.Object);
            loggerMock = new Mock<ILogMailer<UserService>>();

            userRepositoryMock.Setup(s => s.Update(It.IsAny<User>()));

            sessionManagerMock = new Mock<ISessionManager>();

            mapperMock = new Mock<IMapper>();

            sut = new UserService(unitOfWork.Object, loggerMock.Object, sessionManagerMock.Object, mailSenderMock.Object, mailBuilderMock.Object, appSettingMock.Object, emailConfigMock.Object, mapperMock.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldPassActive(bool active)
        {
            userRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new User
                {
                    Id = 1,
                    Email = "email@mail.com"
                });

            var responseActual = sut.Active(1, active);

            Assert.IsNotNull(responseActual);

            var userActual = responseActual.Data;

            if (active)
            {
                Assert.True(userActual.Active);
                Assert.IsNull(userActual.EndDate);
                Assert.GreaterOrEqual(DateTime.Now, userActual.StartDate);
            } else
            {
                Assert.False(userActual.Active);
                Assert.GreaterOrEqual(DateTime.Now, userActual.EndDate);
            }

            var messageActual = responseActual.Messages.FirstOrDefault();

            Assert.IsNotNull(messageActual);
            Assert.AreEqual(active ? Resources.Admin.User.Enabled : Resources.Admin.User.Disabled, $"{messageActual.Text}");
            Assert.AreEqual(MessageType.Success, messageActual.Type);

            userRepositoryMock.Verify(s => s.GetSingle(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            userRepositoryMock.Verify(s => s.Update(It.IsAny<User>()), Times.Once);
            unitOfWork.Verify(s => s.Save(), Times.Once);
        }

        [Test]
        public void ShouldFailActive()
        {
            userRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((User)null);

            var responseActual = sut.Active(1, true);

            var messageActual = responseActual.Messages.FirstOrDefault();

            Assert.IsNotNull(messageActual);
            Assert.AreEqual(Resources.Admin.User.NotFound, $"{messageActual.Text}");
            Assert.AreEqual(MessageType.Error, messageActual.Type);

            userRepositoryMock.Verify(s => s.GetSingle(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            userRepositoryMock.Verify(s => s.Update(It.IsAny<User>()), Times.Never);
            unitOfWork.Verify(s => s.Save(), Times.Never);
        }
    }
}
