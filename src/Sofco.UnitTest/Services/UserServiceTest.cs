using System;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Logger;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
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

        private UserService sut;

        [SetUp]
        public void Setup()
        {
            userRepositoryMock = new Mock<IUserRepository>();

            unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.UserRepository).Returns(userRepositoryMock.Object);
            loggerMock = new Mock<ILogMailer<UserService>>();

            userRepositoryMock.Setup(s => s.Update(It.IsAny<User>()));

            sessionManagerMock = new Mock<ISessionManager>();

            sut = new UserService(unitOfWork.Object, loggerMock.Object, sessionManagerMock.Object);
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
            Assert.AreEqual(active ? Resources.Admin.User.Enabled : Resources.Admin.User.Disabled, $"{messageActual.Folder}.{messageActual.Code}");
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
            Assert.AreEqual(Resources.Admin.User.NotFound, $"{messageActual.Folder}.{messageActual.Code}");
            Assert.AreEqual(MessageType.Error, messageActual.Type);

            userRepositoryMock.Verify(s => s.GetSingle(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            userRepositoryMock.Verify(s => s.Update(It.IsAny<User>()), Times.Never);
            unitOfWork.Verify(s => s.Save(), Times.Never);
        }
    }
}
