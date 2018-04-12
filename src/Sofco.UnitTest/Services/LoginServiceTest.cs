using System.Net.Http;
using AutoMapper;
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
using Sofco.Core.Models.Admin;
using Sofco.Model.AzureAd;
using Sofco.Service.Implementations;

namespace Sofco.UnitTest.Services
{
    [TestFixture]
    public class LoginServiceTest
    {
        private AzureAdLoginResponse loginResult;

        private Mock<IBaseHttpClient> clientMock;

        private Mock<IOptions<AzureAdConfig>> azureAdOptionsMock;

        private Mock<IOptions<AppSetting>> appSettingMock;

        private Mock<IUserRepository> userRepository;

        private Mock<IUnitOfWork> unitOfWork;

        private Mock<IMapper> mapperMock;

        private LoginService sut;

        [SetUp]
        public void Setup()
        {
            clientMock = new Mock<IBaseHttpClient>();
            userRepository = new Mock<IUserRepository>();

            loginResult = new AzureAdLoginResponse
            {
                access_token = "AccessTokenOne",
                refresh_token = "RefreshTokenOne"
            };

            clientMock.Setup(s => s.Post<AzureAdLoginResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new Result<AzureAdLoginResponse>(loginResult));

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

            mapperMock = new Mock<IMapper>();

            mapperMock.Setup(s => s.Map<UserTokenModel>(It.IsAny<AzureAdLoginResponse>()))
                .Returns(new UserTokenModel { AccessToken = "AccessTokenOne", RefreshToken = "RefreshTokenOne" });

            sut = new LoginService(azureAdOptionsMock.Object, clientMock.Object, unitOfWork.Object, appSettingMock.Object, mapperMock.Object);
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
            Assert.AreEqual(loginResult.access_token, actualLogin.Data.AccessToken);

            clientMock.Verify(s => s.Post<AzureAdLoginResponse>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()), Times.Once);
        }
    }
}
