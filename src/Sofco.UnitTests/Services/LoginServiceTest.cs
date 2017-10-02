using System.Net.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Sofco.Model.Users;
using Sofco.Service;
using Sofco.Service.Http.Interfaces;
using Sofco.Service.Settings;
using Sofco.Common.Domains;

namespace Sofco.UnitTests.Services
{
    [TestFixture]
    public class LoginServiceTest
    {
        const string LoginResult = "loginResult";

        private Mock<IBaseHttpClient<string>> clientMock;

        private Mock<IOptions<AzureAdConfig>> azureAdOptionsMock;

        private LoginService sut;

        [SetUp]
        public void Setup()
        {
            clientMock = new Mock<IBaseHttpClient<string>>();

            clientMock.Setup(s => s.Post(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(new Result<string>(LoginResult));

            var azureConfig = new AzureAdConfig {
                Tenant = "http://tenant.com/",
                AadInstance = "instance1",
                Audience = "audience1",
                ClientId = "client1",
                Domain = "this.com",
                GrantType = "sec"
            };
            azureAdOptionsMock = new Mock<IOptions<AzureAdConfig>>();
            azureAdOptionsMock.SetupGet<AzureAdConfig>(s => s.Value).Returns(azureConfig);

            sut = new LoginService(azureAdOptionsMock.Object, clientMock.Object);
        }

        [Test]
        public void ShouldPassLogin()
        {
            var userLogin = new UserLogin
            {
                UserName = "User1",
                Password = "pass01"
            };

            var actualLogin = sut.Login(userLogin);

            Assert.NotNull(actualLogin);
            Assert.AreEqual(LoginResult, actualLogin.ResultData);

            clientMock.Verify(s => s.Post(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()), Times.Once);
        }
    }
}
