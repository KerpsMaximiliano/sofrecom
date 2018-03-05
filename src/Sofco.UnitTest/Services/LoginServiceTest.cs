﻿using System.Net.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Model.Users;
using Sofco.Service.Http.Interfaces;
using Sofco.Service.Settings;
using Sofco.Common.Domains;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Service.Implementations;

namespace Sofco.UnitTest.Services
{
    [TestFixture]
    public class LoginServiceTest
    {
        const string LoginResult = "loginResult";

        private Mock<IBaseHttpClient> clientMock;

        private Mock<IOptions<AzureAdConfig>> azureAdOptionsMock;

        private Mock<IOptions<AppSetting>> appSettingMock;

        private Mock<IUserRepository> userRepository;

        private Mock<IUnitOfWork> unitOfWork;

        private LoginService sut;

        [SetUp]
        public void Setup()
        {
            clientMock = new Mock<IBaseHttpClient>();
            userRepository = new Mock<IUserRepository>();

            clientMock.Setup(s => s.Post<string>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new Result<string>(LoginResult));

            unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.UserRepository).Returns(userRepository.Object);

            userRepository.Setup(x => x.IsActive(It.IsAny<string>())).Returns(true);

            var azureConfig = new AzureAdConfig {
                Tenant = "http://tenant.com/",
                AadInstance = "instance1",
                Audience = "audience1",
                ClientId = "client1",
                Domain = "this.com",
                GrantType = "sec"
            };
            azureAdOptionsMock = new Mock<IOptions<AzureAdConfig>>();
            azureAdOptionsMock.SetupGet(s => s.Value).Returns(azureConfig);

            var appSetting = new AppSetting
            {
                Domain = "spawn.com.ar"
            };
            appSettingMock = new Mock<IOptions<AppSetting>>();
            appSettingMock.SetupGet(s => s.Value).Returns(appSetting);

            sut = new LoginService(azureAdOptionsMock.Object, clientMock.Object, unitOfWork.Object, appSettingMock.Object);
        }

        [Test]
        public void ShouldPassLogin()
        {
            var userLogin = new UserLogin
            {
                UserName = "User1",
                Password = "M0ldGTdMghuKIi472KelyA=="
            };

            var actualLogin = sut.Login(userLogin);

            Assert.NotNull(actualLogin);
            Assert.AreEqual(LoginResult, actualLogin.Data);

            clientMock.Verify(s => s.Post<string>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()), Times.Once);
        }
    }
}
