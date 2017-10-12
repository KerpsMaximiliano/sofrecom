using System;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Service.Implementations.Admin;

namespace Sofco.UnitTest.Services
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IGroupRepository> groupRepositoryMock;
        private Mock<IUserGroupRepository> userGroupRepository;

        private UserService sut;

        [SetUp]
        public void Setup()
        {
            userRepositoryMock = new Mock<IUserRepository>();

            groupRepositoryMock = new Mock<IGroupRepository>();

            userGroupRepository = new Mock<IUserGroupRepository>();

            userRepositoryMock.Setup(s => s.Update(It.IsAny<User>()));

            sut = new UserService(userRepositoryMock.Object, groupRepositoryMock.Object, userGroupRepository.Object);
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
            Assert.AreEqual(active ? Resources.es.Admin.User.Enabled : Resources.es.Admin.User.Disabled, messageActual.Description);
            Assert.AreEqual(MessageType.Success, messageActual.Type);

            userRepositoryMock.Verify(s => s.GetSingle(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            userRepositoryMock.Verify(s => s.Update(It.IsAny<User>()), Times.Once);
            userRepositoryMock.Verify(s => s.Save(), Times.Once);
        }

        [Test]
        public void ShouldFailActive()
        {
            userRepositoryMock.Setup(s => s.GetSingle(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((User)null);

            var responseActual = sut.Active(1, true);

            var messageActual = responseActual.Messages.FirstOrDefault();

            Assert.IsNotNull(messageActual);
            Assert.AreEqual(Resources.es.Admin.User.NotFound, messageActual.Description);
            Assert.AreEqual(MessageType.Error, messageActual.Type);

            userRepositoryMock.Verify(s => s.GetSingle(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
            userRepositoryMock.Verify(s => s.Update(It.IsAny<User>()), Times.Never);
            userRepositoryMock.Verify(s => s.Save(), Times.Never);
        }
    }
}
